/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using RoboCoP.Plus;

namespace RoboCoP.Common.X
{
    public abstract class VideoProvider<T> : SensorProvider<T>
        where T: VideoProviderSettings, new()
    {
        public VideoProvider(ServiceApp<T> App) : base(App)
        {
            Init();
        }

        /// <summary>
        /// Previous image
        /// </summary>
        protected static Image image = new Bitmap(1, 1); //changed to "protected static" from "private" 02/07/11

        /// <summary>
        /// number of previous image
        /// </summary>
        protected static int imageNumber = 0;

        protected bool _needImage = false;
        protected bool UrgeImage()
        {
            // needimage equals true -> image busy
            // value == false -> don`t need for image^^
            // getimage == null -> we don`t know, how getting image(
            if (!_needImage && GetImage != null)
            {
                _needImage = true;
                GetImage();
                _needImage = false;
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
            App.KeyPressed += KeyPressed;
            App.RegisterKey(ConsoleKey.LeftArrow, "Choose left direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            App.RegisterKey(ConsoleKey.RightArrow, "Choose right direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            App.RegisterKey(ConsoleKey.UpArrow, "Choose top direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            App.RegisterKey(ConsoleKey.DownArrow, "Choose bottom direction to change margins. If with ctrl is pressed - decrease, else increase. If shift is pressed - enhance");
            App.RegisterKey(ConsoleKey.S, "Merge settings to config file");
            App.RegisterKey(ConsoleKey.Q, "Get image");
            App.RegisterKey(ConsoleKey.E, "Save image");
            #endregion
            #region Subscribe to signals
            if (App.Service.Com != null)
                App.Service.Com["VideoProducer"].AddSignalListener("GetImage", () => needImage = true);
            #endregion
            #region Enable timer
            if (App.Settings.EnableTimer)
            {
                var timer = new System.Timers.Timer(App.Settings.TimerInterval);
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
                    App.Debug(" reduce ");
                    step *= -1;
                    break;
                // If shift - increase step
                case ConsoleModifiers.Shift:
                    App.Debug(" increase ");
                    step *= 10;
                    break;
                // If both - reduce direction and increase step
                case ConsoleModifiers.Control | ConsoleModifiers.Shift:
                    App.Debug("increase reduce");
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
                    App.Debug("left: ");
                    direction = 0;
                    break;
                case ConsoleKey.UpArrow:
                    App.Debug("up: ");
                    direction = 1;
                    break;
                case ConsoleKey.RightArrow:
                    App.Debug("right: ");
                    direction = 2;
                    break;
                case ConsoleKey.DownArrow:
                    App.Debug("down: ");
                    direction = 3;
                    break;
                // Save config
                case ConsoleKey.S:
                    App.MergeSettings();
                    App.Debug("Save");
                    break;
                // Get image
                case ConsoleKey.Q:
                    App.Log("Signal to send new image");
                    UrgeImage();
                    break;
                case ConsoleKey.E:
                    App.Debug("Save image");
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
                App.Settings.Margins[direction] += step;
                App.Debug(App.Settings.Margins[direction]);
            }
            #endregion
        }
      
    }
}*/