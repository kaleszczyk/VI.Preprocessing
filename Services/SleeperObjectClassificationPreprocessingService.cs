using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.ExtensionMethods;
using VI.AOD.Preprocessing.CUDA; 

namespace VI.AOD.Preprocessing.Services
{
    public class SleeperObjectClassificationPreprocessingService : IPreprocessingService
    {
        bool initialized = false;
        GaussianGPUBlur gaussianGPUBlur;

        int exceptedWidth = 224;
        int exceptedHeight = 224; 

        public SleeperObjectClassificationPreprocessingService()
        {
            Initialize(); 
        }
        public void Initialize()
        {
            if(!initialized)
            gaussianGPUBlur = new GaussianGPUBlur(3, 0);
            initialized = true; 
        }

        public float[] Perform(Bitmap source)
        {
            float[] output = null;
            Bitmap temp = null;
            
            temp = source.ConvertToGrayscale(); 
            temp = temp.ApplyGaussianNoise();
            temp = temp.Normalization();
            temp = gaussianGPUBlur.ApplyFilter(temp);
            temp = temp.Resize(exceptedWidth, exceptedHeight);
            output = temp.Rescale(); 
            return output; 
        }
    }
}
