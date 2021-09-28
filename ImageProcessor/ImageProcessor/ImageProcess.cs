using System;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace ImageProcessor
{
    static class ImageProcess
    {
        public enum Function
        {
            IdxRgrToGray = 0,
            IdxBlur = 1,
            IdxGaussianBlur = 2,
            IdxMedianBlur = 3,
            IdxAdaptiveThreshold = 4,
            IdxBilateralFilter = 5,
            IdxErode = 6,
            IdxDilate = 7,
            IdxClose = 8,
            IdxOpen = 9,
            IdxGradient = 10,
            IdxTophat = 11,
            IdxBlackhat = 12,
        }

        #region function in combobox

        // Bgr to Gray
        static public Bitmap RgbToGray(Bitmap src)
        {
            Image<Bgr, Byte> bgrImage = src.ToImage<Bgr, Byte>();
            Image<Gray, Byte> grayImage = bgrImage.Convert<Gray, Byte>();
            return grayImage.ToBitmap();
        }

        // Blur
        static public Bitmap Blur(Bitmap src, int maskSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            CvInvoke.Blur(image, image, new Size(maskSize, maskSize), new Point(-1, -1));
            return image.ToBitmap();
        }

        // Gaussian Blur
        static public Bitmap GaussianBlur(Bitmap src, int maskSize, double sigma)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            CvInvoke.GaussianBlur(image, image, new Size(maskSize, maskSize), sigma);
            return image.ToBitmap();
        }

        // Median Blur
        static public Bitmap MedianBlur(Bitmap src, int kSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            CvInvoke.MedianBlur(image, image, kSize);
            return image.ToBitmap();
        }

        // Adaptive Threshold
        static public Bitmap AdaptiveThreshold(Bitmap src, int bSize)
        {
            Image<Bgr, Byte> BgrImage = src.ToImage<Bgr, Byte>();
            Image<Gray, Byte> GrayImage = BgrImage.Convert<Gray, Byte>();
            CvInvoke.AdaptiveThreshold(
                GrayImage,
                GrayImage,
                255,
                AdaptiveThresholdType.GaussianC,
                ThresholdType.Binary,
                bSize,
                5
            );

            return GrayImage.ToBitmap();
        }

        // Bilateral Filter
        static public Bitmap BilateralFilter(Bitmap src, int wSize)
        {
            Image<Bgr, Byte> srcImage = src.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> dstImage = new Image<Bgr, Byte>(src.Width, src.Height);
            CvInvoke.BilateralFilter(
                srcImage,
                dstImage,
                -1,
                wSize ^ 2,
                Math.Sqrt(srcImage.Width^2 + srcImage.Height^2)
            );
            return dstImage.ToBitmap();
        }

        // Erode
        static public Bitmap Erode(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.Erode(
                image, 
                image, 
                element, 
                new Point(-1, -1), 
                1, 
                BorderType.Default, 
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Dilate
        static public Bitmap Dilate(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.Dilate(
                image,
                image,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Close
        static public Bitmap Close(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.MorphologyEx(
                image,
                image,
                MorphOp.Close,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Open
        static public Bitmap Open(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.MorphologyEx(
                image,
                image,
                MorphOp.Open,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Gradient
        static public Bitmap Gradient(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.MorphologyEx(
                image,
                image,
                MorphOp.Gradient,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Top Hat
        static public Bitmap Tophat(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.MorphologyEx(
                image,
                image,
                MorphOp.Tophat,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        // Black Hat
        static public Bitmap Blackhat(Bitmap src, int mSize)
        {
            Image<Bgr, Byte> image = src.ToImage<Bgr, Byte>();
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(mSize, mSize), new Point(-1, -1));
            CvInvoke.MorphologyEx(
                image,
                image,
                MorphOp.Blackhat,
                element,
                new Point(-1, -1),
                1,
                BorderType.Default,
                new MCvScalar(0, 0, 0)
            );
            return image.ToBitmap();
        }

        #endregion

        // red channel
        static public Bitmap getRedChannel(Bitmap src)
        {
            Image<Bgr, Byte> srcImage = src.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> dstImage = srcImage.Sub(new Bgr(255, 255, 0));
            return dstImage.ToBitmap();
        }

        // green channel
        static public Bitmap getGreenChannel(Bitmap src)
        {
            Image<Bgr, Byte> srcImage = src.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> dstImage = srcImage.Sub(new Bgr(255, 0, 255));
            return dstImage.ToBitmap();
        }

        // blue channel
        static public Bitmap getBlueChannel(Bitmap src)
        {
            Image<Bgr, Byte> srcImage = src.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> dstImage = srcImage.Sub(new Bgr(0, 255, 255));
            return dstImage.ToBitmap();
        }

        // rotate
        static public Bitmap Rotate(Bitmap src)
        {
            Image<Bgr, Byte> srcImage = src.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> dstImage = new Image<Bgr, Byte>(srcImage.Height, srcImage.Width);
            CvInvoke.Rotate(srcImage, dstImage, RotateFlags.Rotate90CounterClockwise);
            return dstImage.ToBitmap();
        }
    }
}
