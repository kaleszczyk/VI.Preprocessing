using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.ExtensionMethods;



namespace VI.AOD.Preprocessing.Preprocessing.SleeperObjectClassification
{
    public class CommonPreprocessing : IPreprocessing
    {
        public Mat Process(Mat source)
        {
                      
            CvInvoke.Normalize(source, source, 0, 255, Emgu.CV.CvEnum.NormType.MinMax, Emgu.CV.CvEnum.DepthType.Cv8U, source); 
            CvInvoke.CLAHE(source, 5, new Size(20, 10), source);
           
            return source;
        }

        public Bitmap Process(Bitmap source)
        {
            throw new NotImplementedException();
        }
    }
}
