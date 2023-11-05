using System;

namespace LangChain.Databases.InMemory
{

    public class Utils
    {
        public static float ComputeEuclideanDistance(float[] vector1, float[] vector2)
        {
            double sum = 0.0;
            for (int i = 0; i < vector1.Length; i++)
            {
                sum += Math.Pow(vector1[i] - vector2[i], 2);
            }

            return (float)Math.Sqrt(sum);
        }

        public static float ComputeManhattanDistance(float[] vector1, float[] vector2)
        {
            double sum = 0.0;
            for (int i = 0; i < vector1.Length; i++)
            {
                sum += Math.Abs(vector1[i] - vector2[i]);
            }

            return (float)sum;
        }
    }
}