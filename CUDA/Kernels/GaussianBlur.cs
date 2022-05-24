using aforge = AForge.Math;
using Alea;
using System;
using VI.AOD.Preprocessing.ExtensionMethods;

namespace VI.AOD.Preprocessing.CUDA.Kernels
{

        class GaussianBlur : CommonGPUKernel
        {
            [GpuParam]
            private double sigma = 1;
            [GpuParam]
            private double gaussKernelFunctionFactor;


            public GaussianBlur(int bc, int br, int gaussianKernelSize) : base(bc, br, gaussianKernelSize)
            {
            }


            protected override int[,] CreateFilter(int kernelSize)
            {

                //this.gaussKernelFunctionFactor = 2 * sigma * sigma;
                //double[,] kernel = Kernel2D(kernelSize);
                //double min = kernel[0, 0];
                //int[,] intKernel = new int[kernelSize, kernelSize];

                //for (int i = 0; i < kernelSize; i++)
                //{
                //    for (int j = 0; j < kernelSize; j++)
                //    {
                //        double v = kernel[i, j] / min;

                //        if (v > ushort.MaxValue)
                //        {
                //            v = ushort.MaxValue;
                //        }
                //        intKernel[i, j] = (int)v;
                //    }
                //}

                int[,] intKernel = new int[3, 3] {
                    {1, 1, 1},
                    {0, 2, 0},
                    {1, 1, 1} };


                return intKernel;
            }




            private double[,] Kernel2D(int size)
            {
                int r = size / 2;
                double[,] kernel = new double[size, size];

                for (int y = -r, i = 0; i < size; y++, i++)
                {
                    for (int x = -r, j = 0; j < size; x++, j++)
                    {
                        kernel[i, j] = Function2D(x, y);
                    }
                }

                return kernel;
            }

            private double Function2D(double x, double y)
            {
                // (-2 * sigma * sigma)) / (2 * Math.PI * sigma * sigma);
                return Math.Exp((x * x + y * y) / (-gaussKernelFunctionFactor)) / (gaussKernelFunctionFactor * Math.PI);
            }






    }
    

}
