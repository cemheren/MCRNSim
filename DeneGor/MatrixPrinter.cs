using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeneGor
{
    public static class MatrixOperations
    {
        public static double[,] SetValuesInRowOrColumn(this double[,] matrix, string dimension, int columnOrRowNumber, double[] array)
        {
            if (dimension == "row")
            {
                if (array.Length != matrix.GetLength(1))
                {
                    throw new IndexOutOfRangeException("Matrix dimension and array dimension must be the same");
                }

                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    matrix[columnOrRowNumber, i] = array[i];
                }
            }
            else
            {
                if (array.Length != matrix.GetLength(0))
                {
                    throw new IndexOutOfRangeException("Matrix dimension and array dimension must be the same");
                }

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    matrix[i, columnOrRowNumber] = array[i];
                }
            }

            return matrix;
        }

        public static void PrintMatrixToFile(this double[,] matrix, string filename = "", string seperator = " ") 
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = Guid.NewGuid().ToString();
            }

            var streamWriter = new StreamWriter(filename, false);

            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                var widthString = "";
                for (int k = 0; k < width; k++)
                {
                    widthString += matrix[i, k];
                    if (k != width - 1)
                    {
                        widthString += seperator;
                    }
                }

                streamWriter.WriteLine(widthString);
            }

            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}
