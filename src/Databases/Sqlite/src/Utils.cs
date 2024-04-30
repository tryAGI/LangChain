namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public static class Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static float ComputeEuclideanDistance(float[] vector1, float[] vector2)
    {
        vector1 = vector1 ?? throw new ArgumentNullException(nameof(vector1));
        vector2 = vector2 ?? throw new ArgumentNullException(nameof(vector2));

        double sum = 0.0;
        for (int i = 0; i < vector1.Length; i++)
        {
            sum += Math.Pow(vector1[i] - vector2[i], 2);
        }

        return (float)Math.Sqrt(sum);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static float ComputeManhattanDistance(float[] vector1, float[] vector2)
    {
        vector1 = vector1 ?? throw new ArgumentNullException(nameof(vector1));
        vector2 = vector2 ?? throw new ArgumentNullException(nameof(vector2));

        double sum = 0.0;
        for (int i = 0; i < vector1.Length; i++)
        {
            sum += Math.Abs(vector1[i] - vector2[i]);
        }

        return (float)sum;
    }
}