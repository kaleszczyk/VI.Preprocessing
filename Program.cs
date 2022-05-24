using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.Preprocessing.SleeperObjectClassification;
using VI.AOD.Preprocessing.Services;
using VI.AOD.Preprocessing.Services.SleeperObjectClassification;

namespace VI.AOD.Preprocessing
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Mat img = CvInvoke.Imread(@"D:\sleepers\TEST\probki\image_2.png", Emgu.CV.CvEnum.ImreadModes.Grayscale);
            SleeperObjectClassificationFactory sleeperObjectClassificationFactory = new SleeperObjectClassificationFactory();
            //first stage
            IPreprocessingService firstStage = sleeperObjectClassificationFactory.CreatePreprocessingService(EPreprocessingStage.Common); 
            float[] data = firstStage.Perform(img);

            using (StreamWriter sw = new StreamWriter(@"D:\sleepers\TEST\probki\image2C#.csv"))
            {
                foreach (var line in data)
                    sw.WriteLine(line.ToString());
            }
            firstStage.ImageResult.Save(@"D:\sleepers\TEST\probki\image2c#.png");
        }
    }
}
