using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcessAsync2 : ImageProcess
    {
        public override string DestinationPath { get { return Path.Combine(Environment.CurrentDirectory, "ASyncOutput2"); } }

        public override string Name { get { return "非同步2(ProcessBitmap Await版本)"; } }

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
        private async Task ResizeImageWork(string filePath, string destPath, double scale)
        {
            await Task.Run(async () =>
            {
                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                Bitmap processedImage = await processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                string destFile = Path.Combine(destPath, imgName + ".jpg");
                processedImage.Save(destFile, ImageFormat.Jpeg);
            });
        }

        protected async Task<Bitmap> processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {   
            return await Task.Run<Bitmap>(() =>
            {
                Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(resizedbitmap);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(img,
                    new Rectangle(0, 0, newWidth, newHeight),
                    new Rectangle(0, 0, srcWidth, srcHeight),
                    GraphicsUnit.Pixel);
                return resizedbitmap;
            });
        }
    }
}
