using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VI.AOD.Preprocessing.CUDA.Kernels
{
    public class SobelFilter : CommonGPUKernel
    {
        public SobelFilter(int bc, int br, int gaussianKernelSize) : base(bc, br, gaussianKernelSize)
        {

        }

        protected override int[,] CreateFilter(int kernelSize = 7)
        {


            int[,] intKernel = new int[7,7] {
                {-1, -1, -1, -2, -1, -1, -1 },
                {-1, -1, -1, -2, -1, -1, -1 },
                {-1, -1, -1, -2, -1, -1, -1 },
                {0, 0, 0, 0, 0, 0, 0 },
                {1, 1, 1, 2, 1, 1, 1 },
                {1, 1, 1, 2, 1, 1, 1 },
                {1, 1, 1, 2, 1, 1, 1} };



            return intKernel;
        }

    }
}
