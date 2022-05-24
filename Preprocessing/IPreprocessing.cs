using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.CUDA;

namespace VI.AOD.Preprocessing
{
    public interface IPreprocessing
    {
        Bitmap Process(Bitmap source); 
        Mat Process(Mat source);
    }


}

