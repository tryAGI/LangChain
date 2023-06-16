namespace LangChain.Providers;

// ReSharper disable InconsistentNaming
#pragma warning disable CA1707

/// <summary>
/// According https://platform.openai.com/docs/models/
/// <remarks>
/// GPT-4 is currently in a limited beta and only accessible to those who have been granted access. Please join the waitlist to get access. <br/>
/// OpenAI models are non-deterministic, meaning that identical inputs can yield different outputs.
/// Setting temperature to 0 will make the outputs mostly deterministic, but a small amount of variability may remain.
/// </remarks>
/// </summary>
public static class OpenAiModelIds
{ 
    /// <summary>
    /// More capable than any GPT-3.5 model, able to do more complex tasks, and optimized for chat. <br/>
    /// Will be updated with our latest model iteration 2 weeks after it is released. <br/>
    /// Max tokens: 8,192 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// <remarks>On June 27th, 2023, gpt-4 will be updated to point from gpt-4-0314 to gpt-4-0613, the latest model iteration.</remarks>
    /// </summary>
    public const string Gpt4 = "gpt-4";
    
    /// <summary>
    /// Snapshot of gpt-4 from March 14th 2023 with function calling data. <br/>
    /// Unlike gpt-4, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 8,192 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    [Obsolete("This model will be discontinued on 09/13/2023")]
    public const string Gpt4_0314 = "gpt-4-0314";
    
    /// <summary>
    /// Snapshot of gpt-4 from June 13th 2023 with function calling data. <br/>
    /// Unlike gpt-4, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 8,192 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    public const string Gpt4_0613 = "gpt-4-0613";
    
    /// <summary>
    /// Same capabilities as the base gpt-4 mode but with 4x the context length. <br/>
    /// Will be updated with our latest model iteration. <br/>
    /// Max tokens: 32,768 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// <remarks>On June 27th, 2023, gpt-4 will be updated to point from gpt-4-0314 to gpt-4-0613, the latest model iteration.</remarks>
    /// </summary>
    public const string Gpt4_32k = "gpt-4-32";
    
    /// <summary>
    /// Snapshot of gpt-4-32 from March 14th 2023. <br/>
    /// Unlike gpt-4-32k, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 32,768 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    [Obsolete("This model will be discontinued on 09/13/2023")]
    public const string Gpt4_32k_0314 = "gpt-4-32k-0314";
    
    /// <summary>
    /// Snapshot of gpt-4-32 from June 13th 2023. <br/>
    /// Unlike gpt-4-32k, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 32,768 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    public const string Gpt4_32k_0613 = "gpt-4-32k-0613";
    
    /// <summary>
    /// Most capable GPT-3.5 model and optimized for chat at 1/10th the cost of text-davinci-003. <br/>
    /// Will be updated with our latest model iteration 2 weeks after it is released. <br/>
    /// Max tokens: 4,096 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// <remarks>On June 27th, 2023, gpt-3.5-turbo will be updated to point from gpt-3.5-turbo-0301 to gpt-3.5-turbo-0613.</remarks>
    /// </summary>
    public const string Gpt35Turbo = "gpt-3.5-turbo";
    
    /// <summary>
    /// Snapshot of gpt-3.5-turbo from March 1th 2023 with function calling data. <br/>
    /// Unlike gpt-3.5-turbo, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 4,096 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    [Obsolete("This model will be discontinued on 09/13/2023")]
    public const string Gpt35Turbo_0301 = "gpt-3.5-turbo-0301";
    
    /// <summary>
    /// Snapshot of gpt-3.5-turbo from June 13th 2023 with function calling data. <br/>
    /// Unlike gpt-3.5-turbo, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 4,096 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    public const string Gpt35Turbo_0613 = "gpt-3.5-turbo-0613";
    
    /// <summary>
    /// Same capabilities as the standard gpt-3.5-turbo model but with 4 times the context. <br/>
    /// Max tokens: 16,384 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    public const string Gpt35Turbo_16k = "gpt-3.5-turbo-16k";
    
    /// <summary>
    /// Snapshot of gpt-3.5-turbo-16k from June 13th 2023. <br/>
    /// Unlike gpt-3.5-turbo-16k, this model will not receive updates, and will be deprecated 3 months after a new version is released. <br/>
    /// Max tokens: 16,384 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    public const string Gpt35Turbo_16k_0613 = "gpt-3.5-turbo-16k-0613";
}