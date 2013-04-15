using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using RoboCoP.Plus;
using System.IO;
using System.Drawing.Imaging;

namespace RoboCoP.Common
{
    public class VideoProvider<T> : SensorProvider<T>
        where T : VideoProviderSettings, new()
    {

        bool GetImageFlag;
        public Func<Image> GetImage;

        public VideoProvider(ServiceApp<T> app)
            : base(app, "VideoProvider", "GetImage")
        {
            InitCustomSerializer(BitmapSerializer);
            //TODO: здесь зарегистрировать управляющие клавиши и п
            //app.RegisterKey(...)
            //и подписаться на событие
            //app.KeyPressed+=
            //это нужно для изменения настроек камеры онлайн
        }

        public override object GetSensorData()
        {
            if (GetImage==null) return null;
            while (GetImageFlag) System.Threading.Thread.Sleep(1);
            GetImageFlag = true;
            var img=GetImage();
            //здесь выполнить обрезание картинки в соответствие с настройками
            GetImageFlag = false;
            return img;
        }


        /// <summary>
        /// Send image to destanation
        /// </summary>
        protected byte[] BitmapSerializer(object _img)//changed to protected 02/07/11
        {
            Image img = (Image)_img;
            var ms = new MemoryStream();
            img.Save(ms, (App.Settings.Compress) ? ImageFormat.Jpeg : ImageFormat.Bmp);
            var buffer=ms.GetBuffer();
            ms.Close();
            return buffer;
        }
    }
}
