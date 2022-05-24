using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.ExtensionMethods;

namespace VI.AOD.Preprocessing.CUDA.Kernels
{
    public abstract class CommonGPUKernel
    {
        [GpuParam]
        protected int blockColumns;
        [GpuParam]
        protected int blockRows;
        [GpuParam]
        protected int maxNeighbourDistance;
        [GpuParam]
        protected int[] filterMask;

        public CommonGPUKernel(int bc, int br, int kernelSize)
        {
            if (kernelSize < 3 || kernelSize > 101 || kernelSize % 2 == 0) throw new ArgumentException("Kernel size has to be 3x3, 5x5, 7x7 or 9x9");

            blockColumns = bc;
            blockRows = br;

            this.maxNeighbourDistance = kernelSize / 2;
            this.filterMask = CreateFilter(kernelSize).To1D();
        }

        protected abstract int[,] CreateFilter(int kernelSize);

        public void Kernel(byte[] source, byte[] result, int stride, int Image_Height, int strideOffset)
        {
            var x = blockIdx.x * blockDim.x + threadIdx.x;
            var y = blockIdx.y * blockDim.y + threadIdx.y;
            if (y < Image_Height && x < stride - strideOffset)
            {
                double sumX = 0;
                double filterWeight = 0;

                int iter = 0;

                for (int j = y - this.maxNeighbourDistance; j <= y + this.maxNeighbourDistance; j++)
                {
                    if (j >= 0 && j < Image_Height)
                        for (int i = x - this.maxNeighbourDistance; i <= x + this.maxNeighbourDistance; i++)
                        {
                            if (i >= 0 && i < stride - strideOffset)
                            {
                                sumX += (source[j * stride + i]) * (filterMask[iter]);
                                filterWeight += filterMask[iter];
                            }
                            iter++;
                        }
                }

                filterWeight = filterWeight == 0 ? 1 : filterWeight; 
                result[y * stride + x] = (byte)(sumX / filterWeight);
            }
        }

        public LaunchParam LaunchParam(int rows, int cols)
        {
            var block = new dim3(blockColumns, blockRows);
            var grid = new dim3((int)Math.Ceiling((double)cols / blockColumns), (int)Math.Ceiling((double)rows / blockRows));
            return new LaunchParam(grid, block);
        }
    }
}
