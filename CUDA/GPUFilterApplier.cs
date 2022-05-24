using Accord.Imaging.Converters;
using Alea;
using Alea.CSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.CUDA.Kernels;

namespace VI.AOD.Preprocessing.CUDA
{
    public class GPUFilterApplier
    {
        private const int TileDim = 16;
        private const int BlockRows = 32;

        private readonly Gpu gpu = Gpu.Default;

        GPUKernelFactory kernelFactory; 
        CommonGPUKernel gpuKernel;
        private object syncObjc = new object();

        public GPUFilterApplier(EFilterType filterType, int kernelSize)
        {
            kernelFactory = new GPUKernelFactory();
            gpuKernel = kernelFactory.CreateGPUKernel(filterType, TileDim, BlockRows, kernelSize); 
        }

        [GpuManaged]
        public Bitmap ApplyFilter(Bitmap source)
        {
            lock (syncObjc)
            {
                var format = source.PixelFormat; //8 bit per pixel grayscale format

                //result
                Bitmap output;// = new Bitmap(sizex, sizey, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);           

                int width = source.Size.Width;
                int height = source.Size.Height;
                int freeStride = 0;

                if (format == PixelFormat.Format8bppIndexed)
                    //create the result as grayscale image with the same width and height
                    output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
                else
                {
                    throw new ArgumentException("only greyscale images allowed");
                }

                //specifies the attributes of bitmap                
                BitmapData outputBitmapData =
                //locks a output bitmap into system memory     
                output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, output.PixelFormat);
                // rozmiar piksela    liczba bitów na piksel (w formacie 8bpp) / 8 ??????
                int outputPixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8; //1
                                                                                           //szerokość pojedynczego wiersza pikseli 
                int outputOffset = outputBitmapData.Stride - width * outputPixelSize; //820-820=0
                int outputImageSize = outputBitmapData.Height * outputBitmapData.Stride;

                if (height % 32 == 0) freeStride = outputBitmapData.Stride;

                byte[] dst = new byte[outputImageSize + freeStride];

                BitmapData inputBitmapData =
                source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
                int inputPixelSize = Bitmap.GetPixelFormatSize(inputBitmapData.PixelFormat) / 8;
                int inputOffset = inputBitmapData.Stride - inputBitmapData.Width * outputPixelSize;
                int inputImageSize = inputBitmapData.Height * inputBitmapData.Stride;

                byte[] src = new byte[inputImageSize + freeStride];


                Marshal.Copy(inputBitmapData.Scan0, src, 0, inputImageSize);


                //uruchomienie kernela

                gpu.Launch(this.gpuKernel.Kernel, this.gpuKernel.LaunchParam(height, width), src, dst, outputBitmapData.Stride, height, outputOffset);


                Marshal.Copy(dst, 0, outputBitmapData.Scan0, inputImageSize);

                output.UnlockBits(outputBitmapData);
                source.UnlockBits(inputBitmapData);
                Gpu.Free(src);
                Gpu.Free(dst);
                source.Dispose(); 
                return output;
            }
        }

    }
}

