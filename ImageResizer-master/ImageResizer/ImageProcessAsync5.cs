using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcessAsync5 : ImageProcess
    {
        public override string DestinationPath { get { return Path.Combine(Environment.CurrentDirectory, "ASyncOutput5"); } }

        public override string Name { get { return "非同步5(亂七八糟版本)"; } }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        protected override void ResizeImages(string sourcePath, string destPath, double scale)
        {
            var allFiles = this.FindImages(sourcePath);
            var resizeImageTasks = new List<Task>();
            allFiles.ForEach(filePath =>
            {
                //var resizeImageTask = Task.Factory.StartNew(() => this.ResizeImageWork(filePath, destPath, scale));
                var resizeImageTask = this.ResizeImageWork(filePath, destPath, scale);
                resizeImageTasks.Add(resizeImageTask);
            });
            Task.WaitAll(resizeImageTasks.ToArray());
        }

        /// <summary>
        /// 進行圖片的非同步縮放作業
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destPath"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private Task ResizeImageWork(string filePath, string destPath, double scale)
        {
            return Task.Run(async () =>
            {
                Image imgPhoto = await Task.Run<Image>(() => Image.FromFile(filePath));
                //Image imgPhoto = await imgPhotoTask;
                string imgName = await Task.Run<string>(() => Path.GetFileNameWithoutExtension(filePath));

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = await Task.Run<int>(() => (int)(sourceWidth * scale));
                int destionatonHeight = await Task.Run<int>(() => (int)(sourceHeight * scale));

                Bitmap processedImage = await Task.Run<Bitmap>(() => processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight));

                string destFile = await Task.Run<string>(() => Path.Combine(destPath, imgName + ".jpg"));
                await Task.Run(() => processedImage.Save(destFile, ImageFormat.Jpeg));
            });
        }

        protected async Task<Bitmap> processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            return await Task.Run<Bitmap>(async () =>
            {
                Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
                Graphics g = await Task.Run<Graphics>(() => Graphics.FromImage(resizedbitmap));
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;

                await Task.Run(() => g.Clear(Color.Transparent));
                await Task.Run(() => g.DrawImage(img,
                    new Rectangle(0, 0, newWidth, newHeight),
                    new Rectangle(0, 0, srcWidth, srcHeight),
                    GraphicsUnit.Pixel));
                return resizedbitmap;
            });
        }
    }
}
