using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace RoboCoP.Plus.Common.Prototype
{
    public abstract class VideoProducer<T>
        where T: VideoProducerSettings, new()
    {
        public VideoProducer(TerminalServiceApp<T> App)
        {
            app = App;
            Init();
        }

        /// <summary>
        /// Actually, service
        /// </summary>
        protected TerminalServiceApp<T> app;

        /// <summary>
        /// Previous image
        /// </summary>
        protected static Image image = new Bitmap(1, 1); //changed to "protected static" from "private" 02/07/11

        /// <summary>
        /// number of previous image
        /// </summary>
        protected static int imageNumber = 0;

        protected bool _needImage = false;
        protected bool needImage {
            set{
                // needimage equals true -> image busy
                // value == false -> don`t need for image^^
                // getimage == null -> we don`t know, how getting image(
                if (!_needImage && value && GetImage != null)
                {
                    _needImage = true;
                    GetImage();
                    _needImage = false;
                }
            }
        }
        /// <summary>
        /// get new image
        /// </summary>
        public Action GetImage;

        /// <summary>
        /// Init standart
        /// </summary>
        private void Init()
        {
            #region Register keys
            app.KeyPressed += KeyPressed;
            app.RegisterKey(ConsoleKey.LeftArrow, "Choose left direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            app.RegisterKey(ConsoleKey.RightArrow, "Choose right direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            app.RegisterKey(ConsoleKey.UpArrow, "Choose top direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            app.RegisterKey(ConsoleKey.DownArrow, "Choose bottom direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            app.RegisterKey(ConsoleKey.S, "Merge settings to config file");
            app.RegisterKey(ConsoleKey.Q, "Get image");
            app.RegisterKey(ConsoleKey.E, "Save image");
            #endregion
            #region Subscribe to signals
            if (app.Service.Com != null)
                app.Service.Com["VideoProducer"].AddSignalListener("GetImage", () => needImage = true);
            #endregion
            #region Enable timer
            if (app.Settings.EnableTimer)
            {
                var timer = new System.Timers.Timer(app.Settings.TimerInterval);
                timer.Elapsed += (o, e) => needImage = true;
                timer.Start();
            }
            #endregion
        }
        /// <summary>
        /// Process key
        /// </summary>
        /// <param name="key">
        /// Pressed key
        /// </param>
        void KeyPressed(ConsoleKeyInfo key)
        {
            #region compute step
            var step = 1;
            switch (key.Modifiers)
            {
                // If press control - reduce direction
                case ConsoleModifiers.Control:
                    app.Debug(" reduce ");
                    step *= -1;
                    break;
                // If shift - increase step
                case ConsoleModifiers.Shift:
                    app.Debug(" increase ");
                    step *= 10;
                    break;
                // If both - reduce direction and increase step
                case ConsoleModifiers.Control | ConsoleModifiers.Shift:
                    app.Debug("increase reduce");
                    step *= -10;
                    break;
            }
            #endregion
            #region process key
            var direction = -1;
            switch (key.Key)
            {
                // select direction
                case ConsoleKey.LeftArrow:
                    app.Debug("left: ");
                    direction = 0;
                    break;
                case ConsoleKey.UpArrow:
                    app.Debug("up: ");
                    direction = 1;
                    break;
                case ConsoleKey.RightArrow:
                    app.Debug("right: ");
                    direction = 2;
                    break;
                case ConsoleKey.DownArrow:
                    app.Debug("down: ");
                    direction = 3;
                    break;
                // Save config
                case ConsoleKey.S:
                    app.MergeSettings();
                    app.Debug("Save");
                    break;
                // Get image
                case ConsoleKey.Q:
                    app.Log("Signal to send new image");
                    needImage = true;
                    break;
                case ConsoleKey.E:
                    app.Debug("Save image");
                    new Thread(() =>
                    {
                        while (image == null) Thread.Sleep(20);
                        lock (image)
                        {
                            image.Save(imageNumber + ".jpg");
                        }
                    }).Start();
                    break;
            }
            #endregion
            #region Change margin
            if (direction >= 0)
            {
                app.Settings.Margins[direction] += step;
                app.Debug(app.Settings.Margins[direction]);
            }
            #endregion
        }
        /// <summary>
        /// Resize & crop bmp then save it to image
        /// </summary>
        /// <param name="bmp">Image, which you want to crop and resize</param>
        protected Image ResizeAndCrop(Image bmp) //changed to "protected" 02/07/11
        {
            // create place for result image
            if (bmp.Width - app.Settings.Margins[0] - app.Settings.Margins[2] <= 0 || bmp.Height - app.Settings.Margins[1] - app.Settings.Margins[3] <= 0)
            {
                app.Error("Width or height <= 0. You`ll increase it;)");
                return bmp;
            }
            var tmpImg = new Bitmap((int)(app.Settings.ResizeQ * (bmp.Width - app.Settings.Margins[0] - app.Settings.Margins[2])),
                    (int)(app.Settings.ResizeQ * (bmp.Height - app.Settings.Margins[1] - app.Settings.Margins[3])));
            // Create graphics, where we will put result
            var oGraphic = Graphics.FromImage(tmpImg);
            var oRectangle = new Rectangle(
                -(int)(app.Settings.ResizeQ * app.Settings.Margins[0]),
                -(int)(app.Settings.ResizeQ * app.Settings.Margins[1]),
                (int)(app.Settings.ResizeQ * bmp.Width),
                (int)(app.Settings.ResizeQ * bmp.Height));
            // if need compress
            if (app.Settings.Compress)
            {
                // decrease quality
                oGraphic.CompositingQuality = CompositingQuality.HighSpeed;
                oGraphic.SmoothingMode = SmoothingMode.HighSpeed;
                oGraphic.InterpolationMode = InterpolationMode.HighQualityBilinear;
            }
            else
            {
                // else full quality
                oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            // rotate image
            oGraphic.TranslateTransform((float)(image.Width / 2), (float)(image.Height / 2));
            oGraphic.RotateTransform((float)app.Settings.Angle);
            oGraphic.TranslateTransform(-(float)(image.Width / 2), -(float)(image.Height / 2));

            oGraphic.DrawImage(bmp, oRectangle);
            lock (image)
            {
                image = bmp;
            }
            return bmp;
        }

        /// <summary>
        /// Send image to destanation
        /// </summary>
        protected void SendImage(Image img)//changed to protected 02/07/11
        {
            image = img;
            var date = DateTime.Now;
            app.Log("Sending image " + imageNumber);
            ResizeAndCrop(img);
            var ms = new MemoryStream();
            image.Save(ms, (app.Settings.Compress) ? ImageFormat.Jpeg : ImageFormat.Bmp);
            app.Debug("UnpackTime = " + (DateTime.Now - date).ToString());
            // if count of outs < neccesary count - we angry
            if (app.Service.Out.Count < 1)
            {
                app.Error("Must be at least 1 out");
                return;
            }
            app.Service.Out[0].SendBinary(ms.GetBuffer());
            ms.Close();
            app.Log("Success send " + imageNumber);
            app.Debug("SendTime = " + (DateTime.Now - date).ToString());
            imageNumber++;
            app.EndCycle();
        }

    }
}