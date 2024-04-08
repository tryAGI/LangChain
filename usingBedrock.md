# AWS Access
First, you need to configure your AWS access.  You can do this several way but here we're going to show you how to do that with a AWS Profile or Access Key ID and Secret Access Key.


## Installation

1. Install the AWS CLI by following the instructions for your operating system from the [official AWS documentation](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html).
Verify the installation by running the following command in your terminal:
```
aws --version
```

## Configuration

#### Option 1: Configure AWS CLI using AWS Access Key ID and Secret Access Key
1. Obtain your AWS Access Key ID and Secret Access Key:
    - Sign in to the AWS Management Console.
    - Navigate to the IAM dashboard.
    - Click on "Users" in the left sidebar and select your IAM user.
    - Click on the "Security credentials" tab.
    - Click on "Create access key" to generate a new Access Key ID and Secret Access Key.
    - Save the Access Key ID and Secret Access Key in a secure location.
1. Configure the AWS CLI with your Access Key ID and Secret Access Key by running the following command:
```
aws configure
```
Follow the prompts to enter your Access Key ID, Secret Access Key, default region, and default output format.

#### Option 2: Configure AWS CLI using AWS Profile
1. Create a new AWS Profile by running the following command:
```
aws configure --profile <profile-name>
```
Replace `<profile-name>` with your desired profile name.
1. Follow the prompts to enter your Access Key ID, Secret Access Key, default region, and default output format for the specified profile.
1. To use the AWS Profile in your commands, add the `--profile <profile-name>` flag. For example:
```
aws s3 ls --profile <profile-name>
```
Verification
To verify that your AWS CLI is configured correctly, run a simple command like:
```
aws sts get-caller-identity
```

# Configure Amazon Bedrock Provider

You can configure the Bedrock provider one of three ways.  
1. You can use declare an empty Bedrock constructor:
```
var provider = new BedrockProvider();
```
This configuration assumes you've defined an AWS Profile and have set it up on your local computer and your default Region Endpoint is USEast-1.

2. You can pass the Bedrock provider with a Region parameter.
```
var provider = new BedrockProvider(RegionEndpoint.USWest2);
```
This configuration configures your provides to use the specified AWS Region.

3. Finally, you can configure the Bedrock provider with an AWS Access Key ID and Secret Access Key parameters.
```
var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESSKEYID");
var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRETACCESSKEY");
var provider = new BedrockProvider(accessKeyId, secretAccessKey, RegionEndpoint.USWest2);
``` 

# How to use Amazon Bedrock LLMs

