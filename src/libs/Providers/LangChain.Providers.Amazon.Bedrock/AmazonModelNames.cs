namespace LangChain.Providers.Amazon.Bedrock
{
    public static class AmazonModelIds
    {
        public static string AI21LabsJurassic2UltraV1 = "ai21.j2-ultra-v1";
        public static string AI21LabsJurassic2MidV1 = "ai21.j2-mid-v1";

        public static string AmazonTitanEmbeddingsG1TextV1 = "amazon.titan-embed-text-v1";
        public static string AmazonTitanTextG1LiteV1 = "amazon.titan-text-lite-v1";
        public static string AmazonTitanTextG1ExpressV1 = "amazon.titan-text-express-v1";
        public static string AmazonTitanImageGeneratorG1V1 = "amazon.titan-image-generator-v1";
        public static string AmazonTitanMultiModalEmbeddingsG1V1 = "amazon.titan-embed-image-v1";

        public static string AnthropicClaude2_1 = "anthropic.claude-v2:1";
        public static string AnthropicClaude2 = "anthropic.claude-v2";
        public static string AnthropicClaude1_3 = "anthropic.claude-v1";
        public static string AnthropicClaudeInstant1_2 = "anthropic.claude-instant-v1";

        public static string CohereCommand = "cohere.command-text-v14";
        public static string CohereCommandLight = "cohere.command-light-text-v14";
        public static string CohereEmbedEnglish= "cohere.embed-english-v3";
        public static string CohereEmbedMultilingual = "cohere.embed-multilingual-v3";

        public static string MetaLlama2Chat13B = "meta.llama2-13b-chat-v1";
        public static string MetaLlama2Chat70B = "meta.llama2-70b-chat-v1";
        public static string MetaLlama213B = "meta.llama2-13b-v1";  //TODO i'm guessing the model id
        public static string MetaLlama270B = "meta.llama2-70b-v1";  //TODO i'm guessing the model id

        public static string StabilityAISDXL0_8 = "stability.stable-diffusion-xl-v0";
        public static string StabilityAISDXL1_0 = "stability.stable-diffusion-xl-v1";
    }
}
