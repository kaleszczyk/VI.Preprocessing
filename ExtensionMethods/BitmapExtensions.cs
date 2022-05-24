using System;

using Accord.Statistics.Filters;
using Accord.Statistics.Distributions.Univariate;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Random;
using Accord;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace VI.AOD.Preprocessing.ExtensionMethods
{
    public static class BitmapExtensions
    {
        internal static Bitmap ConvertToGrayscale(this Bitmap source)
        {
            Bitmap output = null;

            // output = Grayscale.CommonAlgorithms.BT709.Apply(source);
            output = Grayscale.CommonAlgorithms.RMY.Apply(source); //most similar to cv.imread(mode=grayscale) 
            // output = Grayscale.CommonAlgorithms.Y.Apply(source);
            //Grayscale filter = new Grayscale(0.299, 0.587, 1 - (0.299 + 0.587));
            //Grayscale filter = new Grayscale(0.32768, 0.32768, 1 - (0.32768 + 0.32768));
            //Grayscale filter = new Grayscale(0.06968, 0.23434, 1 - (0.06968 + 0.23434));
            // output = filter.Apply(source); 

            //source.Dispose(); 

            return output; 
        }

        internal static Bitmap ApplyGaussianNoise(this Bitmap source, double variance)
        {
            double stdDev = Math.Pow(256 * variance, 0.5); //variance = 0.22, stdDev = (256 * 0.22)^0.5
            AdditiveNoise filter = new AdditiveNoise(new NormalDistribution(0, stdDev));
            filter.ApplyInPlace(source);

            return source;
        }
    
        public static Bitmap Normalization(this Bitmap source)
        {
            Bitmap output;

            byte newMax = 255;
            byte newMin = 0;

            byte sourceMax;
            byte sourceMin;

            var pixelFormat = source.PixelFormat;
            int width = source.Size.Width;
            int height = source.Size.Height;

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else return new Bitmap(width, height, pixelFormat);

            ImageStatistics stats = new ImageStatistics(source);

            sourceMax = (byte)stats.Gray.Max;
            sourceMin = (byte)stats.Gray.Min; 
            
            var normalizationFactor = (double)(newMax - newMin) / (sourceMax - sourceMin);

            BitmapData outputData = output.LockBits(new Rectangle(0, 0, width, height),
                         ImageLockMode.WriteOnly, output.PixelFormat);

            int outputPixelSize = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
            int outputOffset = outputData.Stride - width * outputPixelSize;

            BitmapData inputData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
               ImageLockMode.ReadOnly, source.PixelFormat);

            int inputPixelSize = Bitmap.GetPixelFormatSize(inputData.PixelFormat) / 8;
            int inputOffset = inputData.Stride - inputData.Width * outputPixelSize;

            unsafe
            {
                byte* dst = (byte*)outputData.Scan0.ToPointer();
                byte* src = (byte*)inputData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double v = (*src - sourceMin) * normalizationFactor + newMin; 

                        byte value = unchecked((byte)(v));
                        for (int c = 0; c < outputPixelSize; c++, dst++)
                            *dst = value;

                        src += inputPixelSize;
                    }
                    src += inputOffset;
                    dst += outputOffset;
                }
            }

            output.UnlockBits(outputData);
            source.UnlockBits(inputData);
            source.Dispose(); 
            return output;

        }

        public static Bitmap Resize(this Bitmap source, int newWidth, int newHeight)
        {
            Bitmap output = null;
            
            ResizeBicubic filter = new ResizeBicubic(newWidth, newHeight);
           
            output = filter.Apply(source);
            source.Dispose(); 
            return output; 
        }

        public static Bitmap ApplySobelFilter(this Bitmap source)
        {
           
            Bitmap output = null;
            int[,] intKernel = new int[7, 7] {
                {-1, -1, -1, -2, -1, -1, -1 },
                {-1, -1, -1, -2, -1, -1, -1 },
                {-1, -1, -1, -2, -1, -1, -1 },
                {0, 0, 0, 0, 0, 0, 0 },
                {1, 1, 1, 2, 1, 1, 1 },
                {1, 1, 1, 2, 1, 1, 1 },
                {1, 1, 1, 2, 1, 1, 1} };


            Convolution filter = new Convolution(intKernel);
            output = filter.Apply(source); 
            source.Dispose();
            return output;
        }

        public static float[] Rescale(this Bitmap source)
        {
            float[] output = new float[source.Width*source.Height*3]; 

            var pixelFormat = source.PixelFormat;
            int width = source.Size.Width;
            int height = source.Size.Height;



            BitmapData inputData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
               ImageLockMode.ReadOnly, source.PixelFormat);

            int inputPixelSize = Bitmap.GetPixelFormatSize(inputData.PixelFormat) / 8;
            int inputOffset = inputData.Stride - inputData.Width * inputPixelSize;
            int idx = 0; 
            unsafe
            {
                byte* src = (byte*)inputData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        output[idx++] = ((*src / 255.0f) - 0.485f) / 0.229f;
                        output[idx++] = ((*src / 255.0f) - 0.456f) / 0.224f;
                        output[idx++] = ((*src / 255.0f) - 0.406f) / 0.225f;
                        src += inputPixelSize;
                    }
                    src += inputOffset;
                }
            }

            source.UnlockBits(inputData);
            source.Dispose();

            return output;
        }

       
    }

}
