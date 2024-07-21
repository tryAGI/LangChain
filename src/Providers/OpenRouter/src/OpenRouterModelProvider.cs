using OpenAI;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// Contains all the OpenRouter models.
/// </summary>
public static class OpenRouterModelProvider
{
    private static Dictionary<OpenRouterModelIds, ChatModelMetadata> Models { get; set; } = new()
    {
        { OpenRouterModelIds.Dolphin292Mixtral8X22b, ToMetadata("cognitivecomputations/dolphin-mixtral-8x22b",65536,1E-06,1E-06)},
        { OpenRouterModelIds.Qwen272BInstruct, ToMetadata("qwen/qwen-2-72b-instruct",32768,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.OpenChat368B, ToMetadata("openchat/openchat-8b",8192,8E-08,8E-08)},
        { OpenRouterModelIds.MistralMistral7BInstruct, ToMetadata("mistralai/mistral-7b-instruct",32768,7E-08,7E-08)},
        { OpenRouterModelIds.MistralMistral7BInstructV03, ToMetadata("mistralai/mistral-7b-instruct-v0.3",32768,7E-08,7E-08)},
        { OpenRouterModelIds.NousresearchHermes2ProLlama38B, ToMetadata("nousresearch/hermes-2-pro-llama-3-8b",8192,1.5E-07,1.5E-07)},
        { OpenRouterModelIds.Phi3MiniInstructFree, ToMetadata("microsoft/phi-3-mini-128k-instruct:free",128000,0,0)},
        { OpenRouterModelIds.Phi3MiniInstruct, ToMetadata("microsoft/phi-3-mini-128k-instruct",128000,1.0000000000000001E-07,1.0000000000000001E-07)},
        { OpenRouterModelIds.Phi3MediumInstructFree, ToMetadata("microsoft/phi-3-medium-128k-instruct:free",128000,0,0)},
        { OpenRouterModelIds.Phi3MediumInstruct, ToMetadata("microsoft/phi-3-medium-128k-instruct",128000,1E-06,1E-06)},
        { OpenRouterModelIds.Llama3Lumimaid70B, ToMetadata("neversleep/llama-3-lumimaid-70b",8192,3.375E-06,4.5E-06)},
        { OpenRouterModelIds.GoogleGeminiFlash15Preview, ToMetadata("google/gemini-flash-1.5",2800000,2.5E-07,7.5E-07)},
        { OpenRouterModelIds.PerplexityLlama3Sonar8B, ToMetadata("perplexity/llama-3-sonar-small-32k-chat",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexityLlama3Sonar8BOnline, ToMetadata("perplexity/llama-3-sonar-small-32k-online",28000,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexityLlama3Sonar70B, ToMetadata("perplexity/llama-3-sonar-large-32k-chat",32768,1E-06,1E-06)},
        { OpenRouterModelIds.PerplexityLlama3Sonar70BOnline, ToMetadata("perplexity/llama-3-sonar-large-32k-online",28000,1E-06,1E-06)},
        { OpenRouterModelIds.DeepseekV2Chat, ToMetadata("deepseek/deepseek-chat",128000,1.4E-07,2.8E-07)},
        { OpenRouterModelIds.DeepseekCoder, ToMetadata("deepseek/deepseek-coder",16000,1.4E-07,2.8E-07)},
        { OpenRouterModelIds.MetaLlama38BBase, ToMetadata("meta-llama/llama-3-8b",8192,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.MetaLlama370BBase, ToMetadata("meta-llama/llama-3-70b",8192,8.100000000000001E-07,8.100000000000001E-07)},
        { OpenRouterModelIds.OpenAiGpt4O, ToMetadata("openai/gpt-4o",128000,5E-06,1.5E-05)},
        { OpenRouterModelIds.OpenAiGpt4O20240513, ToMetadata("openai/gpt-4o-2024-05-13",128000,5E-06,1.5E-05)},
        { OpenRouterModelIds.MetaLlamaguard28B, ToMetadata("meta-llama/llama-guard-2-8b",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.LlavaV1634B, ToMetadata("liuhaotian/llava-yi-34b",4096,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.Olmo7BInstruct, ToMetadata("allenai/olmo-7b-instruct",2048,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.Qwen15110BChat, ToMetadata("qwen/qwen-110b-chat",32768,1.6200000000000002E-06,1.6200000000000002E-06)},
        { OpenRouterModelIds.Qwen1514BChat, ToMetadata("qwen/qwen-14b-chat",32768,2.7E-07,2.7E-07)},
        { OpenRouterModelIds.Qwen157BChat, ToMetadata("qwen/qwen-7b-chat",32768,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.Qwen154BChat, ToMetadata("qwen/qwen-4b-chat",32768,9E-08,9E-08)},
        { OpenRouterModelIds.Qwen1572BChat, ToMetadata("qwen/qwen-72b-chat",32768,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.Qwen1532BChat, ToMetadata("qwen/qwen-32b-chat",32768,7.5E-07,7.5E-07)},
        { OpenRouterModelIds.MetaLlama38BInstructFree, ToMetadata("meta-llama/llama-3-8b-instruct:free",8192,0,0)},
        { OpenRouterModelIds.Llama3Lumimaid8B, ToMetadata("neversleep/llama-3-lumimaid-8b",24576,2.006E-07,1.125E-06)},
        { OpenRouterModelIds.Llama3Lumimaid8BExtended, ToMetadata("neversleep/llama-3-lumimaid-8b:extended",24576,2.006E-07,1.125E-06)},
        { OpenRouterModelIds.SnowflakeArcticInstruct, ToMetadata("snowflake/snowflake-arctic-instruct",4096,2.16E-06,2.16E-06)},
        { OpenRouterModelIds.Firellava13B, ToMetadata("fireworks/firellava-13b",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.LynnLlama3Soliloquy8BV2, ToMetadata("lynn/soliloquy-l3",24576,5.0000000000000004E-08,5.0000000000000004E-08)},
        { OpenRouterModelIds.Fimbulvetr11BV2, ToMetadata("sao10k/fimbulvetr-11b-v2",8192,3.75E-07,1.5E-06)},
        { OpenRouterModelIds.MetaLlama38BInstructExtended, ToMetadata("meta-llama/llama-3-8b-instruct:extended",16384,2.006E-07,1.125E-06)},
        { OpenRouterModelIds.MetaLlama38BInstructNitro, ToMetadata("meta-llama/llama-3-8b-instruct:nitro",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.MetaLlama370BInstructNitro, ToMetadata("meta-llama/llama-3-70b-instruct:nitro",8192,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.MetaLlama38BInstruct, ToMetadata("meta-llama/llama-3-8b-instruct",8192,7E-08,7E-08)},
        { OpenRouterModelIds.MetaLlama370BInstruct, ToMetadata("meta-llama/llama-3-70b-instruct",8192,5.9E-07,7.900000000000001E-07)},
        { OpenRouterModelIds.MistralMixtral8X22bInstruct, ToMetadata("mistralai/mixtral-8x22b-instruct",65536,6.5E-07,6.5E-07)},
        { OpenRouterModelIds.Wizardlm28X22b, ToMetadata("microsoft/wizardlm-2-8x22b",65536,6.5E-07,6.5E-07)},
        { OpenRouterModelIds.Wizardlm27B, ToMetadata("microsoft/wizardlm-2-7b",32000,7E-08,7E-08)},
        { OpenRouterModelIds.ToppyM7BNitro, ToMetadata("undi95/toppy-m-7b:nitro",4096,7E-08,7E-08)},
        { OpenRouterModelIds.MistralMixtral8X22BBase, ToMetadata("mistralai/mixtral-8x22b",65536,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.OpenAiGpt4Turbo, ToMetadata("openai/gpt-4-turbo",128000,1E-05,3E-05)},
        { OpenRouterModelIds.GoogleGeminiPro15Preview, ToMetadata("google/gemini-pro-1.5",2800000,2.5E-06,7.5E-06)},
        { OpenRouterModelIds.CohereCommandRPlus, ToMetadata("cohere/command-r-plus",128000,3E-06,1.5E-05)},
        { OpenRouterModelIds.DatabricksDbrx132BInstruct, ToMetadata("databricks/dbrx-instruct",32768,1.08E-06,1.08E-06)},
        { OpenRouterModelIds.MidnightRose70B, ToMetadata("sophosympatheia/midnight-rose-70b",4096,9E-06,9E-06)},
        { OpenRouterModelIds.CohereCommand, ToMetadata("cohere/command",4096,1E-06,2E-06)},
        { OpenRouterModelIds.CohereCommandR, ToMetadata("cohere/command-r",128000,5E-07,1.5E-06)},
        { OpenRouterModelIds.AnthropicClaude3Haiku, ToMetadata("anthropic/claude-3-haiku",200000,2.5E-07,1.25E-06)},
        { OpenRouterModelIds.AnthropicClaude3HaikuSelfModerated, ToMetadata("anthropic/claude-3-haiku:beta",200000,2.5E-07,1.25E-06)},
        { OpenRouterModelIds.GoogleGemma7BNitro, ToMetadata("google/gemma-7b-it:nitro",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.MistralMistral7BInstructNitro, ToMetadata("mistralai/mistral-7b-instruct:nitro",32768,7E-08,7E-08)},
        { OpenRouterModelIds.Mixtral8X7BInstructNitro, ToMetadata("mistralai/mixtral-8x7b-instruct:nitro",32768,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.MetaLlamaV270BChatNitro, ToMetadata("meta-llama/llama-2-70b-chat:nitro",4096,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.Mythomax13BNitro, ToMetadata("gryphe/mythomax-l2-13b:nitro",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.AnthropicClaude3Opus, ToMetadata("anthropic/claude-3-opus",200000,1.5E-05,7.5E-05)},
        { OpenRouterModelIds.AnthropicClaude3Sonnet, ToMetadata("anthropic/claude-3-sonnet",200000,3E-06,1.5E-05)},
        { OpenRouterModelIds.AnthropicClaude3OpusSelfModerated, ToMetadata("anthropic/claude-3-opus:beta",200000,1.5E-05,7.5E-05)},
        { OpenRouterModelIds.AnthropicClaude3SonnetSelfModerated, ToMetadata("anthropic/claude-3-sonnet:beta",200000,3E-06,1.5E-05)},
        { OpenRouterModelIds.MistralLarge, ToMetadata("mistralai/mistral-large",32000,8E-06,2.4E-05)},
        { OpenRouterModelIds.GoogleGemma7BFree, ToMetadata("google/gemma-7b-it:free",8192,0,0)},
        { OpenRouterModelIds.GoogleGemma7B, ToMetadata("google/gemma-7b-it",8192,7E-08,7E-08)},
        { OpenRouterModelIds.NousHermes2Mistral7BDpo, ToMetadata("nousresearch/nous-hermes-2-mistral-7b-dpo",8192,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.AnthropicClaudeV2SelfModerated, ToMetadata("anthropic/claude-2:beta",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV20SelfModerated, ToMetadata("anthropic/claude-2.0:beta",100000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV21SelfModerated, ToMetadata("anthropic/claude-2.1:beta",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeInstantV1SelfModerated, ToMetadata("anthropic/claude-instant-1:beta",100000,8.000000000000001E-07,2.4E-06)},
        { OpenRouterModelIds.OpenAiGpt35Turbo16K0125, ToMetadata("openai/gpt-3.5-turbo-0125",16385,5E-07,1.5E-06)},
        { OpenRouterModelIds.MetaCodellama70BInstruct, ToMetadata("codellama/codellama-70b-instruct",2048,8.100000000000001E-07,8.100000000000001E-07)},
        { OpenRouterModelIds.RwkvV5Eagle7B, ToMetadata("recursal/eagle-7b",10000,0,0)},
        { OpenRouterModelIds.OpenAiGpt4TurboPreview, ToMetadata("openai/gpt-4-turbo-preview",128000,1E-05,3E-05)},
        { OpenRouterModelIds.RemmSlerp13BExtended, ToMetadata("undi95/remm-slerp-l2-13b:extended",6144,1.125E-06,1.125E-06)},
        { OpenRouterModelIds.NousHermes2Mixtral8X7BSft, ToMetadata("nousresearch/nous-hermes-2-mixtral-8x7b-sft",32768,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.NousHermes2Mixtral8X7BDpo, ToMetadata("nousresearch/nous-hermes-2-mixtral-8x7b-dpo",32768,2.7E-07,2.7E-07)},
        { OpenRouterModelIds.MistralTiny, ToMetadata("mistralai/mistral-tiny",32000,2.5E-07,2.5E-07)},
        { OpenRouterModelIds.MistralSmall, ToMetadata("mistralai/mistral-small",32000,2E-06,6E-06)},
        { OpenRouterModelIds.MistralMedium, ToMetadata("mistralai/mistral-medium",32000,2.7E-06,8.1E-06)},
        { OpenRouterModelIds.ChronosHermes13BV2, ToMetadata("austism/chronos-hermes-13b",4096,1.3E-07,1.3E-07)},
        { OpenRouterModelIds.NousHermes2Yi34B, ToMetadata("nousresearch/nous-hermes-yi-34b",4096,7.2E-07,7.2E-07)},
        { OpenRouterModelIds.NoromaidMixtral8X7BInstruct, ToMetadata("neversleep/noromaid-mixtral-8x7b-instruct",8000,8E-06,8E-06)},
        { OpenRouterModelIds.MistralMistral7BInstructV02, ToMetadata("mistralai/mistral-7b-instruct-v0.2",32768,7E-08,7E-08)},
        { OpenRouterModelIds.Dolphin26Mixtral8X7B, ToMetadata("cognitivecomputations/dolphin-mixtral-8x7b",32768,5E-07,5E-07)},
        { OpenRouterModelIds.GoogleGeminiPro10, ToMetadata("google/gemini-pro",91728,1.25E-07,3.75E-07)},
        { OpenRouterModelIds.GoogleGeminiProVision10, ToMetadata("google/gemini-pro-vision",45875,1.25E-07,3.75E-07)},
        { OpenRouterModelIds.Mixtral8X7BBase, ToMetadata("mistralai/mixtral-8x7b",32768,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.Mixtral8X7BInstruct, ToMetadata("mistralai/mixtral-8x7b-instruct",32768,2.4E-07,2.4E-07)},
        { OpenRouterModelIds.RwkvV5World3B, ToMetadata("rwkv/rwkv-5-world-3b",10000,0,0)},
        { OpenRouterModelIds.RwkvV53BAiTown, ToMetadata("recursal/rwkv-5-3b-ai-town",10000,0,0)},
        { OpenRouterModelIds.StripedhyenaNous7B, ToMetadata("togethercomputer/stripedhyena-nous-7b",32768,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.StripedhyenaHessian7BBase, ToMetadata("togethercomputer/stripedhyena-hessian-7b",32768,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.PsyfighterV213B, ToMetadata("koboldai/psyfighter-13b-2",4096,1E-06,1E-06)},
        { OpenRouterModelIds.Yi34BChat, ToMetadata("01-ai/yi-34b-chat",4096,7.2E-07,7.2E-07)},
        { OpenRouterModelIds.Yi34BBase, ToMetadata("01-ai/yi-34b",4096,7.2E-07,7.2E-07)},
        { OpenRouterModelIds.Yi6BBase, ToMetadata("01-ai/yi-6b",4096,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.NousCapybara7BFree, ToMetadata("nousresearch/nous-capybara-7b:free",4096,0,0)},
        { OpenRouterModelIds.NousCapybara7B, ToMetadata("nousresearch/nous-capybara-7b",4096,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.OpenChat357BFree, ToMetadata("openchat/openchat-7b:free",8192,0,0)},
        { OpenRouterModelIds.OpenChat357B, ToMetadata("openchat/openchat-7b",8192,7E-08,7E-08)},
        { OpenRouterModelIds.Mythomist7BFree, ToMetadata("gryphe/mythomist-7b:free",32768,0,0)},
        { OpenRouterModelIds.Noromaid20B, ToMetadata("neversleep/noromaid-20b",8192,1.5E-06,2.25E-06)},
        { OpenRouterModelIds.Mythomist7B, ToMetadata("gryphe/mythomist-7b",32768,3.75E-07,3.75E-07)},
        { OpenRouterModelIds.NeuralChat7BV31, ToMetadata("intel/neural-chat-7b",4096,5E-06,5E-06)},
        { OpenRouterModelIds.AnthropicClaudeV2, ToMetadata("anthropic/claude-2",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV21, ToMetadata("anthropic/claude-2.1",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.OpenHermes25Mistral7B, ToMetadata("teknium/openhermes-2.5-mistral-7b",4096,1.7000000000000001E-07,1.7000000000000001E-07)},
        { OpenRouterModelIds.NousCapybara34B, ToMetadata("nousresearch/nous-capybara-34b",32768,9.000000000000001E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.OpenAiGpt4Vision, ToMetadata("openai/gpt-4-vision-preview",128000,1E-05,3E-05)},
        { OpenRouterModelIds.Lzlv70B, ToMetadata("lizpreciatior/lzlv-70b-fp16-hf",4096,5.9E-07,7.900000000000001E-07)},
        { OpenRouterModelIds.ToppyM7BFree, ToMetadata("undi95/toppy-m-7b:free",4096,0,0)},
        { OpenRouterModelIds.Goliath120B, ToMetadata("alpindale/goliath-120b",6144,9.375E-06,9.375E-06)},
        { OpenRouterModelIds.ToppyM7B, ToMetadata("undi95/toppy-m-7b",4096,7E-08,7E-08)},
        { OpenRouterModelIds.AutoBestForPrompt, ToMetadata("openrouter/auto",200000,0,0)},
        { OpenRouterModelIds.HuggingFaceZephyr7BFree, ToMetadata("huggingfaceh4/zephyr-7b-beta:free",4096,0,0)},
        { OpenRouterModelIds.GooglePalm2Chat32K, ToMetadata("google/palm-2-chat-bison-32k",91750,2.5E-07,5E-07)},
        { OpenRouterModelIds.GooglePalm2CodeChat32K, ToMetadata("google/palm-2-codechat-bison-32k",91750,2.5E-07,5E-07)},
        { OpenRouterModelIds.OpenHermes2Mistral7B, ToMetadata("teknium/openhermes-2-mistral-7b",4096,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.MistralOpenOrca7B, ToMetadata("open-orca/mistral-7b-openorca",8192,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.Airoboros70B, ToMetadata("jondurbin/airoboros-l2-70b",4096,7E-07,9.000000000000001E-07)},
        { OpenRouterModelIds.Mythomax13BExtended, ToMetadata("gryphe/mythomax-l2-13b:extended",8192,1.125E-06,1.125E-06)},
        { OpenRouterModelIds.Xwin70B, ToMetadata("xwin-lm/xwin-lm-70b",8192,3.75E-06,3.75E-06)},
        { OpenRouterModelIds.MistralMistral7BInstructFree, ToMetadata("mistralai/mistral-7b-instruct:free",32768,0,0)},
        { OpenRouterModelIds.OpenAiGpt35TurboInstruct, ToMetadata("openai/gpt-3.5-turbo-instruct",4095,1.5E-06,2E-06)},
        { OpenRouterModelIds.MistralMistral7BInstructV01, ToMetadata("mistralai/mistral-7b-instruct-v0.1",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PygmalionMythalion13B, ToMetadata("pygmalionai/mythalion-13b",8192,1.125E-06,1.125E-06)},
        { OpenRouterModelIds.OpenAiGpt35Turbo16K, ToMetadata("openai/gpt-3.5-turbo-16k",16385,3E-06,4E-06)},
        { OpenRouterModelIds.OpenAiGpt432K, ToMetadata("openai/gpt-4-32k",32767,6E-05,0.00012)},
        { OpenRouterModelIds.MetaCodellama34BInstruct, ToMetadata("meta-llama/codellama-34b-instruct",8192,7.2E-07,7.2E-07)},
        { OpenRouterModelIds.PhindCodellama34BV2, ToMetadata("phind/phind-codellama-34b",4096,7.2E-07,7.2E-07)},
        { OpenRouterModelIds.NousHermes13B, ToMetadata("nousresearch/nous-hermes-llama2-13b",4096,1.8E-07,1.8E-07)},
        { OpenRouterModelIds.MancerWeaverAlpha, ToMetadata("mancer/weaver",8000,1.875E-06,2.25E-06)},
        { OpenRouterModelIds.AnthropicClaudeV20, ToMetadata("anthropic/claude-2.0",100000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeInstantV1, ToMetadata("anthropic/claude-instant-1",100000,8.000000000000001E-07,2.4E-06)},
        { OpenRouterModelIds.RemmSlerp13B, ToMetadata("undi95/remm-slerp-l2-13b",4096,2.7E-07,2.7E-07)},
        { OpenRouterModelIds.GooglePalm2Chat, ToMetadata("google/palm-2-chat-bison",25804,2.5E-07,5E-07)},
        { OpenRouterModelIds.GooglePalm2CodeChat, ToMetadata("google/palm-2-codechat-bison",20070,2.5E-07,5E-07)},
        { OpenRouterModelIds.Mythomax13B, ToMetadata("gryphe/mythomax-l2-13b",4096,1.3E-07,1.3E-07)},
        { OpenRouterModelIds.MetaLlamaV213BChat, ToMetadata("meta-llama/llama-2-13b-chat",4096,1.3E-07,1.3E-07)},
        { OpenRouterModelIds.MetaLlamaV270BChat, ToMetadata("meta-llama/llama-2-70b-chat",4096,6.4E-07,8.000000000000001E-07)},
        { OpenRouterModelIds.OpenAiGpt35Turbo, ToMetadata("openai/gpt-3.5-turbo",16385,5E-07,1.5E-06)},
        { OpenRouterModelIds.OpenAiGpt4, ToMetadata("openai/gpt-4",8191,3E-05,6E-05)},

    };

    public static ChatModelMetadata ToMetadata(string id, int contextLength, double pricePerInputTokenInUsd, double pricePerOutputTokenInUsd)
    {
        return new ChatModelMetadata
        {
            Id = id,
            ContextLength = contextLength,
            PricePerInputTokenInUsd = pricePerInputTokenInUsd,
            PricePerOutputTokenInUsd = pricePerOutputTokenInUsd,
        };
    }
    
    [CLSCompliant(false)]
    public static ChatModelMetadata GetModelById(OpenRouterModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Open Router Model {modelId}");
    }

    [CLSCompliant(false)]
    public static ChatModelMetadata GetModelById(string modelId)
    {
        return Models.Values.First(s => s.Id == modelId);
    }
}