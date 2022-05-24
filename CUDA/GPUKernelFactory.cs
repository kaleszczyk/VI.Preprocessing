using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.CUDA.Kernels;

namespace VI.AOD.Preprocessing.CUDA
{
    public class GPUKernelFactory
    {
        public virtual CommonGPUKernel CreateGPUKernel(EFilterType filterType, int bc, int br, int kernelSize)
        {
            CommonGPUKernel commonGPUKernel = null; 
            switch(filterType)
            {
                case EFilterType.Gaussian:
                    commonGPUKernel = new GaussianBlur(bc, br, kernelSize);
                    break;
                case EFilterType.Sobel:
                    commonGPUKernel = new SobelFilter(bc, br, kernelSize);
                    break; 
                    
            }
            return commonGPUKernel; 
        }
    }
}
