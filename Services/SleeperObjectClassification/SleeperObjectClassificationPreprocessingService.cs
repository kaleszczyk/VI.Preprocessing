using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.ExtensionMethods;
using VI.AOD.Preprocessing.CUDA;
using System.IO;
using VI.AOD.Preprocessing.Services.SleeperObjectClassification;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VI.AOD.Preprocessing.Services
{
    public class SleeperObjectClassificationPreprocessingService : IPreprocessingService
    {


        private static int exceptedWidth = 224;
        private static int exceptedHeight = 224;

        private IPreprocessing preprocessing;

        public Bitmap ImageResult
        {
            get;
            set;
        }
        public SleeperObjectClassificationPreprocessingService(IPreprocessing preprocessing) 
        {
            this.ImageResult = null;
            this.preprocessing = preprocessing;

        }
        public float[] Perform(Mat source)
        {
            
            var result = preprocessing.Process(source);
            
            CvInvoke.Resize(result, result, new Size(exceptedWidth, exceptedHeight),0,0, Emgu.CV.CvEnum.Inter.Cubic); 
            ImageResult = result.Bitmap; 
            return AdjustImageToNeuralNetwork(ImageResult);
           
        }

        public float[] Perform(Bitmap source)
        {
            Image<Gray, Byte> imageCV = new Image<Gray, byte>(source);
            Mat mat = imageCV.Mat;
            var result = preprocessing.Process(mat);
            CvInvoke.Resize(result, result, new Size(exceptedWidth, exceptedHeight), 0, 0, Emgu.CV.CvEnum.Inter.Cubic);
            ImageResult = result.Bitmap;
            return AdjustImageToNeuralNetwork(ImageResult); 
        }

        private float[] AdjustImageToNeuralNetwork(Bitmap source)
        {
            Bitmap clone = (Bitmap)source.Clone();

            float[] result = clone.Rescale();
            clone.Dispose();
            source.Dispose();
            return result;

        }
    }
}
