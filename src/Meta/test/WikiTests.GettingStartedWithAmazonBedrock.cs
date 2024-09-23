using Amazon;
using LangChain.Providers;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;

namespace LangChain.IntegrationTests;

public partial class WikiTests
{
    [Test]
    public async Task GettingStartedWithAmazonBedrock()
    {
        //// # AWS Access
        //// First, you need to configure your AWS access.
        //// You can do this several ways, but here we're going to show you how to do that
        //// with a AWS Profile or Access Key ID and Secret Access Key.
        //// 
        //// ## Installation
        //// 
        //// 1. Install the AWS CLI by following the instructions for your operating system from
        //// the [official AWS documentation](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html).  
        //// Verify the installation by running the following command in your terminal:
        //// ```
        //// aws --version
        //// ```
        //// 
        //// ## Configuration
        //// 
        //// #### Option 1: Configure AWS CLI using AWS Access Key ID and Secret Access Key
        //// 1. Obtain your AWS Access Key ID and Secret Access Key:
        ////     - Sign in to the AWS Management Console.
        ////     - Navigate to the IAM dashboard.
        ////     - Click on "Users" in the left sidebar and select your IAM user.
        ////     - Click on the "Security credentials" tab.
        ////     - Click on "Create access key" to generate a new Access Key ID and Secret Access Key.
        ////     - Save the Access Key ID and Secret Access Key in a secure location.
        //// 1. Configure the AWS CLI with your Access Key ID and Secret Access Key by running the following command:
        //// ```
        //// aws configure
        //// ```
        //// Follow the prompts to enter your Access Key ID, Secret Access Key, default region, and default output format.
        //// 
        //// #### Option 2: Configure AWS CLI using AWS Profile
        //// 1. Create a new AWS Profile by running the following command:
        //// ```
        //// aws configure --profile <profile-name>
        //// ```
        //// Replace `<profile-name>` with your desired profile name.
        //// 1. Follow the prompts to enter your Access Key ID, Secret Access Key, default region, and default output format for the specified profile.
        //// 1. To use the AWS Profile in your commands, add the `--profile <profile-name>` flag. For example:
        //// ```
        //// aws s3 ls --profile <profile-name>
        //// ```
        //// Verification
        //// To verify that your AWS CLI is configured correctly, run a simple command like:
        //// ```
        //// aws sts get-caller-identity
        //// ```
        //// 
        //// # Configure Amazon Bedrock Provider
        //// You can configure the Bedrock provider one of three ways.  
        //// #### Option 1. You can use declare an empty Bedrock constructor:
        //// ```
        //// var provider = new BedrockProvider();
        //// ```
        //// This configuration assumes you've defined an AWS Profile and have set it up on your local computer and you're default Region Endpoint is USEast-1.
        //// 
        //// #### Option 2. You can pass the Bedrock provider with a Region parameter.
        //// ```
        //// var provider = new BedrockProvider(RegionEndpoint.USWest2);
        //// ```
        //// This configuration configures your provider to use the specified AWS Region.
        //// 
        //// #### Option 3. Finally, you can configure the Bedrock provider with a AWS Access Key ID and Secret Access Key parameters.
        //// ```
        //// var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESSKEYID");
        //// var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRETACCESSKEY");
        //// var provider = new BedrockProvider(accessKeyId, secretAccessKey, RegionEndpoint.USWest2);
        //// ``` 
        //// 
        //// # Using Amazon Bedrock LLMs
        //// Amazon Bedrock supports foundation models (FMs) from various providers including, [Amazon](https://docs.aws.amazon.com/bedrock/latest/userguide/titan-models.html), [Anthropic](https://console.anthropic.com/docs), [AI21 Labs](https://docs.ai21.com/), [Cohere](https://docs.cohere.com/docs), [Meta](https://ai.meta.com/llama/get-started), [Mistral](https://docs.mistral.ai/), and [Stability AI](https://platform.stability.ai/docs/getting-started) with more to come.
        //// You must [request access to a model](https://docs.aws.amazon.com/bedrock/latest/userguide/model-access.html) before you can use it. After doing so, you can then use the model class in your code:
        ////
        //// ### Override default model settings
        //// You can also override the default model settings.  For example, suppose the model that you're using supports Max Tokens greater than the default tokens or you want to change the Temperature, or you want to enable streaming, etc, you can do this by passing a new Settings:

        var provider = new BedrockProvider(RegionEndpoint.USWest2);
        var llm = new Claude35SonnetModel(provider)
        {
            Settings = new BedrockChatSettings
            {
                MaxTokens = 200_000,
                Temperature = 0,
                UseStreaming = true
            }
        };

        llm.RequestSent += (_, request) => Console.WriteLine($"Prompt: {request.Messages.AsHistory()}");
        llm.DeltaReceived += (_, delta) => Console.Write(delta.Content);
        llm.ResponseReceived += (_, response) => Console.WriteLine($"Completed response: {response}");

        var prompt = @"
            you are a comic book writer.  you will be given a question and you will answer it. 
            question: who are 10 of the most popular superheros and what are their powers?";

        string response = await llm.GenerateAsync(prompt);

        Console.WriteLine(response);

        //// In conclusion, by following these steps, you can set up the AWS CLI,
        //// configure the Amazon Bedrock provider, and start using the supported foundation models in your code.
        //// With the AWS CLI and Bedrock provider properly configured,
        //// you'll be ready to leverage the power of Amazon Bedrock LLMs in your applications.
    }
}