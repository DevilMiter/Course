using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcessAsync3 : ImageProcess
    {
        public override string DestinationPath { get { return Path.Combine(Environment.CurrentDirectory, "ASyncOutput3"); } }

        public override string Name { get { return "非同步3(Parallel版本)"; } }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        protected override void ResizeImages(string sourcePath, string destPath, double scale)
        {
            var allFiles = this.FindImages(sourcePath);
            Parallel.ForEach(allFiles, filePath =>
            {
                this.ResizeImageWork(filePath, destPath, scale);
            });
        }

        private void ResizeImageWork(string filePath, string destPath, double scale)
        {
            Image imgPhoto = Image.FromFile(filePath);
            string imgName = Path.GetFileNameWithoutExtension(filePath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destionatonWidth = (int)(sourceWidth * scale);
            int destionatonHeight = (int)(sourceHeight * scale);

            Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                sourceWidth, sourceHeight,
                destionatonWidth, destionatonHeight);

            string destFile = Path.Combine(destPath, imgName + ".jpg");
            processedImage.Save(destFile, ImageFormat.Jpeg);
        }
    }
}
