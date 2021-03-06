using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using VI.AOD.Preprocessing.CUDA;
using VI.AOD.Preprocessing.ExtensionMethods;

namespace VI.AOD.Preprocessing.SleeperObjectClassification
{
    public class SecondStagePreprocessing : IPreprocessing
    {
        protected bool initialized = false;
        protected GPUFilterApplier gaussianGPUBlur;
        protected GPUFilterApplier sobelGPUFilter; 

        public SecondStagePreprocessing()
        {
            Initialize(); 
        }

        public void Initialize()
        {
            if (!initialized)
            {
                gaussianGPUBlur = new GPUFilterApplier(EFilterType.Gaussian, 3);             
            }
                
            initialized = true;
        }

        public Bitmap Process(Bitmap source)
        {
            Bitmap temp = null;
            Bitmap clone = (Bitmap)source.Clone();

            temp = clone.ConvertToGrayscale();
            temp = temp.ApplyGaussianNoise(0.3);
            temp = temp.Normalization();
            temp = gaussianGPUBlur.ApplyFilter(temp);
            temp = temp.ApplySobelFilter();
            temp = temp.Normalization();

            return temp;
        }

        public Mat Process(Mat source)
        {
            throw new NotImplementedException();
        }
    }
}
