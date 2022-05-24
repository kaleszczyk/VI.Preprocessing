using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VI.AOD.Preprocessing.ExtensionMethods
{
    public static class ArrayExtensions
    {
        public static T[] To1D<T>(this T[,] source)
        {
            var totalLength = source.Length;
            var edgeLength = Math.Sqrt(totalLength);

            T[] result = new T[totalLength];

            int index = 0;
            for (int i = 0; i < edgeLength; i++)
            {
                for (int j = 0; j < edgeLength; j++)
                {
                    result[index] = source[i, j];
                    index++;
                }
            }
            return result;
        }
    }
}
