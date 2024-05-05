using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// Contains all the OpenRouter models.
/// </summary>
public static class OpenRouterModelProvider
{
    private static Dictionary<OpenRouterModelIds, ChatModels> Models { get; set; } = new()
    {
        { OpenRouterModelIds.NousCapybara7BFree, new ChatModels("nousresearch/nous-capybara-7b:free",4096,0,0)},
        { OpenRouterModelIds.Mistral7BInstructFree, new ChatModels("mistralai/mistral-7b-instruct:free",32768,0,0)},
        { OpenRouterModelIds.OpenChat35Free, new ChatModels("openchat/openchat-7b:free",8192,0,0)},
        { OpenRouterModelIds.Mythomist7BFree, new ChatModels("gryphe/mythomist-7b:free",32768,0,0)},
        { OpenRouterModelIds.ToppyM7BFree, new ChatModels("undi95/toppy-m-7b:free",4096,0,0)},
        { OpenRouterModelIds.Cinematika7BAlphaFree, new ChatModels("openrouter/cinematika-7b:free",8000,0,0)},
        { OpenRouterModelIds.GoogleGemma7BFree, new ChatModels("google/gemma-7b-it:free",8192,0,0)},
        { OpenRouterModelIds.PsyfighterV213B, new ChatModels("koboldai/psyfighter-13b-2",4096,1E-06,1E-06)},
        { OpenRouterModelIds.NeuralChat7BV31, new ChatModels("intel/neural-chat-7b",4096,5E-06,5E-06)},
        { OpenRouterModelIds.Mythomax13B, new ChatModels("gryphe/mythomax-l2-13b",4096,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.PygmalionMythalion13B, new ChatModels("pygmalionai/mythalion-13b",8192,1.1249999999999998E-06,1.1249999999999998E-06)},
        { OpenRouterModelIds.Xwin70B, new ChatModels("xwin-lm/xwin-lm-70b",8192,3.7499999999999997E-06,3.7499999999999997E-06)},
        { OpenRouterModelIds.Goliath120B, new ChatModels("alpindale/goliath-120b",6144,9.375E-06,9.375E-06)},
        { OpenRouterModelIds.Noromaid20B, new ChatModels("neversleep/noromaid-20b",8192,2.2499999999999996E-06,2.2499999999999996E-06)},
        { OpenRouterModelIds.Mythomist7B, new ChatModels("gryphe/mythomist-7b",32768,3.75E-07,3.75E-07)},
        { OpenRouterModelIds.MidnightRose70B, new ChatModels("sophosympatheia/midnight-rose-70b",4096,8.999999999999999E-06,8.999999999999999E-06)},
        { OpenRouterModelIds.Fimbulvetr11BV2, new ChatModels("sao10k/fimbulvetr-11b-v2",8192,5.499E-07,2.826E-06)},
        { OpenRouterModelIds.Llama3Lumimaid8B, new ChatModels("neversleep/llama-3-lumimaid-8b",24576,2.25E-07,2.2499999999999996E-06)},
        { OpenRouterModelIds.RemmSlerp13BExtended, new ChatModels("undi95/remm-slerp-l2-13b:extended",6144,1.1249999999999998E-06,1.1249999999999998E-06)},
        { OpenRouterModelIds.Mythomax13BExtended, new ChatModels("gryphe/mythomax-l2-13b:extended",8192,1.1249999999999998E-06,1.1249999999999998E-06)},
        { OpenRouterModelIds.MetaLlama38BInstructExtended, new ChatModels("meta-llama/llama-3-8b-instruct:extended",16384,2.25E-07,2.2499999999999996E-06)},
        { OpenRouterModelIds.Llama3Lumimaid8BExtended, new ChatModels("neversleep/llama-3-lumimaid-8b:extended",24576,2.25E-07,2.2499999999999996E-06)},
        { OpenRouterModelIds.MancerWeaverAlpha, new ChatModels("mancer/weaver",8000,3.375E-06,3.375E-06)},
        { OpenRouterModelIds.NousCapybara7B, new ChatModels("nousresearch/nous-capybara-7b",4096,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.MetaCodellama34BInstruct, new ChatModels("meta-llama/codellama-34b-instruct",8192,7.200000000000001E-07,7.200000000000001E-07)},
        { OpenRouterModelIds.MetaCodellama70BInstruct, new ChatModels("codellama/codellama-70b-instruct",2048,8.1E-07,8.1E-07)},
        { OpenRouterModelIds.PhindCodellama34BV2, new ChatModels("phind/phind-codellama-34b",4096,7.200000000000001E-07,7.200000000000001E-07)},
        { OpenRouterModelIds.OpenHermes2Mistral7B, new ChatModels("teknium/openhermes-2-mistral-7b",4096,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.RemmSlerp13B, new ChatModels("undi95/remm-slerp-l2-13b",4096,2.7E-07,2.7E-07)},
        { OpenRouterModelIds.Cinematika7BAlpha, new ChatModels("openrouter/cinematika-7b",8000,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.Yi34BChat, new ChatModels("01-ai/yi-34b-chat",4096,7.200000000000001E-07,7.200000000000001E-07)},
        { OpenRouterModelIds.Yi34BBase, new ChatModels("01-ai/yi-34b",4096,7.200000000000001E-07,7.200000000000001E-07)},
        { OpenRouterModelIds.Yi6BBase, new ChatModels("01-ai/yi-6b",4096,1.26E-07,1.26E-07)},
        { OpenRouterModelIds.StripedhyenaNous7B, new ChatModels("togethercomputer/stripedhyena-nous-7b",32768,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.StripedhyenaHessian7BBase, new ChatModels("togethercomputer/stripedhyena-hessian-7b",32768,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.Mixtral8X7BBase, new ChatModels("mistralai/mixtral-8x7b",32768,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.NousHermes2Yi34B, new ChatModels("nousresearch/nous-hermes-yi-34b",4096,7.200000000000001E-07,7.200000000000001E-07)},
        { OpenRouterModelIds.NousHermes2Mixtral8X7BSft, new ChatModels("nousresearch/nous-hermes-2-mixtral-8x7b-sft",32769,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.NousHermes2Mistral7BDpo, new ChatModels("nousresearch/nous-hermes-2-mistral-7b-dpo",8192,1.8000000000000002E-07,1.8000000000000002E-07)},
        { OpenRouterModelIds.MetaLlama370BInstruct, new ChatModels("meta-llama/llama-3-70b-instruct",8192,8.1E-07,8.1E-07)},
        { OpenRouterModelIds.SnowflakeArcticInstruct, new ChatModels("snowflake/snowflake-arctic-instruct",4096,2.16E-06,2.16E-06)},
        { OpenRouterModelIds.Mixtral8X7BInstructNitro, new ChatModels("mistralai/mixtral-8x7b-instruct:nitro",32768,5.4E-07,5.4E-07)},
        { OpenRouterModelIds.MistralOpenOrca7B, new ChatModels("open-orca/mistral-7b-openorca",8192,1.4249999999999999E-07,1.4249999999999999E-07)},
        { OpenRouterModelIds.OpenAiGpt35Turbo, new ChatModels("openai/gpt-3.5-turbo",16385,5E-07,1.5E-06)},
        { OpenRouterModelIds.OpenAiGpt35Turbo16K0125, new ChatModels("openai/gpt-3.5-turbo-0125",16385,5E-07,1.5E-06)},
        { OpenRouterModelIds.OpenAiGpt35Turbo16K, new ChatModels("openai/gpt-3.5-turbo-16k",16385,3E-06,4E-06)},
        { OpenRouterModelIds.OpenAiGpt4Turbo, new ChatModels("openai/gpt-4-turbo",128000,1E-05,2.9999999999999997E-05)},
        { OpenRouterModelIds.OpenAiGpt4TurboPreview, new ChatModels("openai/gpt-4-turbo-preview",128000,1E-05,2.9999999999999997E-05)},
        { OpenRouterModelIds.OpenAiGpt4, new ChatModels("openai/gpt-4",8191,2.9999999999999997E-05,5.9999999999999995E-05)},
        { OpenRouterModelIds.OpenAiGpt432K, new ChatModels("openai/gpt-4-32k",32767,5.9999999999999995E-05,0.00011999999999999999)},
        { OpenRouterModelIds.OpenAiGpt4Vision, new ChatModels("openai/gpt-4-vision-preview",128000,1E-05,2.9999999999999997E-05)},
        { OpenRouterModelIds.OpenAiGpt35TurboInstruct, new ChatModels("openai/gpt-3.5-turbo-instruct",4095,1.5E-06,2E-06)},
        { OpenRouterModelIds.GooglePalm2Chat, new ChatModels("google/palm-2-chat-bison",25804,2.5E-07,5E-07)},
        { OpenRouterModelIds.GooglePalm2CodeChat, new ChatModels("google/palm-2-codechat-bison",20070,2.5E-07,5E-07)},
        { OpenRouterModelIds.GooglePalm2Chat32K, new ChatModels("google/palm-2-chat-bison-32k",91750,2.5E-07,5E-07)},
        { OpenRouterModelIds.GooglePalm2CodeChat32K, new ChatModels("google/palm-2-codechat-bison-32k",91750,2.5E-07,5E-07)},
        { OpenRouterModelIds.GoogleGeminiPro10, new ChatModels("google/gemini-pro",91728,1.25E-07,3.75E-07)},
        { OpenRouterModelIds.GoogleGeminiProVision10, new ChatModels("google/gemini-pro-vision",45875,1.25E-07,3.75E-07)},
        { OpenRouterModelIds.GoogleGeminiPro15Preview, new ChatModels("google/gemini-pro-1.5",2800000,2.5E-06,7.499999999999999E-06)},
        { OpenRouterModelIds.PerplexityPplx70BOnline, new ChatModels("perplexity/pplx-70b-online",4096,1E-06,1E-06)},
        { OpenRouterModelIds.PerplexityPplx7BOnline, new ChatModels("perplexity/pplx-7b-online",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexityPplx7BChat, new ChatModels("perplexity/pplx-7b-chat",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexityPplx70BChat, new ChatModels("perplexity/pplx-70b-chat",4096,1E-06,1E-06)},
        { OpenRouterModelIds.PerplexitySonar7B, new ChatModels("perplexity/sonar-small-chat",16384,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexitySonar8X7B, new ChatModels("perplexity/sonar-medium-chat",16384,6E-07,6E-07)},
        { OpenRouterModelIds.PerplexitySonar7BOnline, new ChatModels("perplexity/sonar-small-online",12000,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.PerplexitySonar8X7BOnline, new ChatModels("perplexity/sonar-medium-online",12000,6E-07,6E-07)},
        { OpenRouterModelIds.Firellava13B, new ChatModels("fireworks/firellava-13b",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.AnthropicClaude3Opus, new ChatModels("anthropic/claude-3-opus",200000,1.4999999999999999E-05,7.5E-05)},
        { OpenRouterModelIds.AnthropicClaude3Sonnet, new ChatModels("anthropic/claude-3-sonnet",200000,3E-06,1.4999999999999999E-05)},
        { OpenRouterModelIds.AnthropicClaude3Haiku, new ChatModels("anthropic/claude-3-haiku",200000,2.5E-07,1.25E-06)},
        { OpenRouterModelIds.AnthropicClaudeV2, new ChatModels("anthropic/claude-2",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV20, new ChatModels("anthropic/claude-2.0",100000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV21, new ChatModels("anthropic/claude-2.1",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeInstantV1, new ChatModels("anthropic/claude-instant-1",100000,8.000000000000001E-07,2.4E-06)},
        { OpenRouterModelIds.AnthropicClaude3OpusSelfModerated, new ChatModels("anthropic/claude-3-opus:beta",200000,1.4999999999999999E-05,7.5E-05)},
        { OpenRouterModelIds.AnthropicClaude3SonnetSelfModerated, new ChatModels("anthropic/claude-3-sonnet:beta",200000,3E-06,1.4999999999999999E-05)},
        { OpenRouterModelIds.AnthropicClaude3HaikuSelfModerated, new ChatModels("anthropic/claude-3-haiku:beta",200000,2.5E-07,1.25E-06)},
        { OpenRouterModelIds.AnthropicClaudeV2SelfModerated, new ChatModels("anthropic/claude-2:beta",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV20SelfModerated, new ChatModels("anthropic/claude-2.0:beta",100000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeV21SelfModerated, new ChatModels("anthropic/claude-2.1:beta",200000,8E-06,2.4E-05)},
        { OpenRouterModelIds.AnthropicClaudeInstantV1SelfModerated, new ChatModels("anthropic/claude-instant-1:beta",100000,8.000000000000001E-07,2.4E-06)},
        { OpenRouterModelIds.MetaLlamaV213BChat, new ChatModels("meta-llama/llama-2-13b-chat",4096,1.3E-07,1.3E-07)},
        { OpenRouterModelIds.MetaLlamaV270BChat, new ChatModels("meta-llama/llama-2-70b-chat",4096,6.4E-07,8.000000000000001E-07)},
        { OpenRouterModelIds.NousHermes13B, new ChatModels("nousresearch/nous-hermes-llama2-13b",4096,2.6E-07,2.6E-07)},
        { OpenRouterModelIds.NousCapybara34B, new ChatModels("nousresearch/nous-capybara-34b",32768,9E-07,9E-07)},
        { OpenRouterModelIds.Airoboros70B, new ChatModels("jondurbin/airoboros-l2-70b",4096,7E-07,9E-07)},
        { OpenRouterModelIds.ChronosHermes13BV2, new ChatModels("austism/chronos-hermes-13b",4096,1.3E-07,1.3E-07)},
        { OpenRouterModelIds.Mistral7BInstruct, new ChatModels("mistralai/mistral-7b-instruct",32768,1.0000000000000001E-07,2.5E-07)},
        { OpenRouterModelIds.OpenHermes25Mistral7B, new ChatModels("teknium/openhermes-2.5-mistral-7b",4096,1.7000000000000001E-07,1.7000000000000001E-07)},
        { OpenRouterModelIds.OpenChat35, new ChatModels("openchat/openchat-7b",8192,1.0000000000000001E-07,1.0000000000000001E-07)},
        { OpenRouterModelIds.ToppyM7B, new ChatModels("undi95/toppy-m-7b",4096,1.5E-07,1.5E-07)},
        { OpenRouterModelIds.Lzlv70B, new ChatModels("lizpreciatior/lzlv-70b-fp16-hf",4096,7E-07,8.000000000000001E-07)},
        { OpenRouterModelIds.Mixtral8X7BInstruct, new ChatModels("mistralai/mixtral-8x7b-instruct",32768,2.4000000000000003E-07,2.4000000000000003E-07)},
        { OpenRouterModelIds.Dolphin26Mixtral8X7B, new ChatModels("cognitivecomputations/dolphin-mixtral-8x7b",32769,5E-07,5E-07)},
        { OpenRouterModelIds.NoromaidMixtral8X7BInstruct, new ChatModels("neversleep/noromaid-mixtral-8x7b-instruct",8000,8E-06,8E-06)},
        { OpenRouterModelIds.NousHermes2Mixtral8X7BDpo, new ChatModels("nousresearch/nous-hermes-2-mixtral-8x7b-dpo",32769,2.7E-07,2.7E-07)},
        { OpenRouterModelIds.RwkvV5World3B, new ChatModels("rwkv/rwkv-5-world-3b",10000,0,0)},
        { OpenRouterModelIds.RwkvV53BAiTown, new ChatModels("recursal/rwkv-5-3b-ai-town",10000,0,0)},
        { OpenRouterModelIds.RwkvV5Eagle7B, new ChatModels("recursal/eagle-7b",10000,0,0)},
        { OpenRouterModelIds.GoogleGemma7B, new ChatModels("google/gemma-7b-it",8192,1.0000000000000001E-07,1.0000000000000001E-07)},
        { OpenRouterModelIds.DatabricksDbrx132BInstruct, new ChatModels("databricks/dbrx-instruct",32768,6E-07,6E-07)},
        { OpenRouterModelIds.MetaLlama38BInstruct, new ChatModels("meta-llama/llama-3-8b-instruct",8192,1.0000000000000001E-07,1.0000000000000001E-07)},
        { OpenRouterModelIds.Wizardlm28X22b, new ChatModels("microsoft/wizardlm-2-8x22b",65536,6.499999999999999E-07,6.499999999999999E-07)},
        { OpenRouterModelIds.Wizardlm27B, new ChatModels("microsoft/wizardlm-2-7b",32000,6.999999999999999E-08,6.999999999999999E-08)},
        { OpenRouterModelIds.MistralMixtral8X22BBase, new ChatModels("mistralai/mixtral-8x22b",65536,9E-07,9E-07)},
        { OpenRouterModelIds.MistralMixtral8X22bInstruct, new ChatModels("mistralai/mixtral-8x22b-instruct",65536,6.499999999999999E-07,6.499999999999999E-07)},
        { OpenRouterModelIds.LynnLlama3Soliloquy8BV2, new ChatModels("lynn/soliloquy-l3",24576,1.5E-07,1.5E-07)},
        { OpenRouterModelIds.HuggingFaceZephyr7BFree, new ChatModels("huggingfaceh4/zephyr-7b-beta:free",4096,0,0)},
        { OpenRouterModelIds.MetaLlamaV270BChatNitro, new ChatModels("meta-llama/llama-2-70b-chat:nitro",4096,9E-07,9E-07)},
        { OpenRouterModelIds.Mythomax13BNitro, new ChatModels("gryphe/mythomax-l2-13b:nitro",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.Mistral7BInstructNitro, new ChatModels("mistralai/mistral-7b-instruct:nitro",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.GoogleGemma7BNitro, new ChatModels("google/gemma-7b-it:nitro",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.ToppyM7BNitro, new ChatModels("undi95/toppy-m-7b:nitro",4096,1.5E-07,1.5E-07)},
        { OpenRouterModelIds.Wizardlm28X22bNitro, new ChatModels("microsoft/wizardlm-2-8x22b:nitro",65536,1E-06,1E-06)},
        { OpenRouterModelIds.MetaLlama38BInstructNitro, new ChatModels("meta-llama/llama-3-8b-instruct:nitro",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
        { OpenRouterModelIds.MetaLlama370BInstructNitro, new ChatModels("meta-llama/llama-3-70b-instruct:nitro",8192,9E-07,9E-07)},
        { OpenRouterModelIds.Llava13B, new ChatModels("haotian-liu/llava-13b",2048,1E-05,1E-05)},
        { OpenRouterModelIds.NousHermes2Vision7BAlpha, new ChatModels("nousresearch/nous-hermes-2-vision-7b",4096,1E-05,1E-05)},
        { OpenRouterModelIds.MistralTiny, new ChatModels("mistralai/mistral-tiny",32000,2.5E-07,2.5E-07)},
        { OpenRouterModelIds.MistralSmall, new ChatModels("mistralai/mistral-small",32000,2E-06,6E-06)},
        { OpenRouterModelIds.MistralMedium, new ChatModels("mistralai/mistral-medium",32000,2.7E-06,8.1E-06)},
        { OpenRouterModelIds.MistralLarge, new ChatModels("mistralai/mistral-large",32000,8E-06,2.4E-05)},
        { OpenRouterModelIds.CohereCommand, new ChatModels("cohere/command",4096,1E-06,2E-06)},
        { OpenRouterModelIds.CohereCommandR, new ChatModels("cohere/command-r",128000,5E-07,1.5E-06)},
        { OpenRouterModelIds.CohereCommandRPlus, new ChatModels("cohere/command-r-plus",128000,3E-06,1.4999999999999999E-05)},

    };

    [CLSCompliant(false)]
    public static ChatModels GetModelById(OpenRouterModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Open Router Model {modelId}");
    }

    [CLSCompliant(false)]
    public static ChatModels GetModelById(string modelId)
    {
        var model = Models.Values.FirstOrDefault(s => s.Id == modelId);
        if (model == null)
            throw new KeyNotFoundException($"Model with ID {modelId} not found.");
        return model;
    }
}