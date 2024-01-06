namespace StableDiffusion;

public class Automatic1111ModelOptions
{
    public string NegativePrompt { get; set; }="";
    public int Seed { get; set; } = -1;
    public int Steps { get; set; } = 20;
    public double CFGScale { get; set; } = 7;
    public int Width { get; set; }= 512;
    public int Height { get; set; }= 512;


}