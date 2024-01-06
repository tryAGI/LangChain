namespace StableDiffusion;

public class Automatic1111ModelOptions
{
    public string NegativePrompt { get; set; }="";
    public int Seed { get; set; } = -1;
    public int Steps { get; set; } = 20;
    public float CFGScale { get; set; } = 6.0f;
    public int Width { get; set; }= 512;
    public int Height { get; set; }= 512;
    public string Sampler { get; set; } = "Euler a";

}