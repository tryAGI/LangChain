using OpenAI;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// Contains all the DeepInfra models.
/// </summary>
public static class DeepInfraModelProvider
{
    private static Dictionary<DeepInfraModelIds, ChatModelMetadata> Models { get; set; } = new()
    {
        { DeepInfraModelIds.MetaLlama31405BInstruct, ToMetadata("meta-llama/Meta-Llama-3.1-405B-Instruct",32768,2.7E-06,2.7E-06)},
        { DeepInfraModelIds.MetaLlama3170BInstruct, ToMetadata("meta-llama/Meta-Llama-3.1-70B-Instruct",131072,5.2E-07,7.5E-07)},
        { DeepInfraModelIds.MetaLlama318BInstruct, ToMetadata("meta-llama/Meta-Llama-3.1-8B-Instruct",131072,6E-08,6E-08)},
        { DeepInfraModelIds.Gemma227BIt, ToMetadata("google/gemma-2-27b-it",4096,2.7E-07,2.7E-07)},
        { DeepInfraModelIds.Gemma29BIt, ToMetadata("google/gemma-2-9b-it",4096,9E-08,9E-08)},
        { DeepInfraModelIds.Dolphin291Llama370B, ToMetadata("cognitivecomputations/dolphin-2.9.1-llama-3-70b",8192,5.9E-07,7.900000000000001E-07)},
        { DeepInfraModelIds.L370BEuryaleV21, ToMetadata("Sao10K/L3-70B-Euryale-v2.1",8192,5.9E-07,7.900000000000001E-07)},
        { DeepInfraModelIds.MetaLlama370BInstruct, ToMetadata("meta-llama/Meta-Llama-3-70B-Instruct",8192,5.2E-07,7.5E-07)},
        { DeepInfraModelIds.Qwen272BInstruct, ToMetadata("Qwen/Qwen2-72B-Instruct",32768,5.6E-07,7.7E-07)},
        { DeepInfraModelIds.Phi3Medium4KInstruct, ToMetadata("microsoft/Phi-3-medium-4k-instruct",4096,1.4E-07,1.4E-07)},
        { DeepInfraModelIds.OpenChat368B, ToMetadata("openchat/openchat-3.6-8b",8192,6E-08,6E-08)},
        { DeepInfraModelIds.Mistral7BInstructV03, ToMetadata("mistralai/Mistral-7B-Instruct-v0.3",32768,6E-08,6E-08)},
        { DeepInfraModelIds.MetaLlama38BInstruct, ToMetadata("meta-llama/Meta-Llama-3-8B-Instruct",8192,6E-08,6E-08)},
        { DeepInfraModelIds.Mixtral8X22bInstructV01, ToMetadata("mistralai/Mixtral-8x22B-Instruct-v0.1",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.Wizardlm28X22b, ToMetadata("microsoft/WizardLM-2-8x22B",65536,6.3E-07,6.3E-07)},
        { DeepInfraModelIds.Wizardlm27B, ToMetadata("microsoft/WizardLM-2-7B",32768,7E-08,7E-08)},
        { DeepInfraModelIds.Mixtral8X7BInstructV01, ToMetadata("mistralai/Mixtral-8x7B-Instruct-v0.1",32768,2.4E-07,2.4E-07)},
        { DeepInfraModelIds.Lzlv70BFp16Hf, ToMetadata("lizpreciatior/lzlv_70b_fp16_hf",4096,5.9E-07,7.900000000000001E-07)},
        { DeepInfraModelIds.Llava157BHf, ToMetadata("llava-hf/llava-1.5-7b-hf",4096,3.4000000000000003E-07,3.4000000000000003E-07)},
        { DeepInfraModelIds.Yi34BChat, ToMetadata("01-ai/Yi-34B-Chat",4096,6E-07,6E-07)},
        { DeepInfraModelIds.ChronosHermes13BV2, ToMetadata("Austism/chronos-hermes-13b-v2",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.MythomaxL213B, ToMetadata("Gryphe/MythoMax-L2-13b",4096,1.0000000000000001E-07,1.0000000000000001E-07)},
        { DeepInfraModelIds.MythomaxL213BTurbo, ToMetadata("Gryphe/MythoMax-L2-13b-turbo",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.ZephyrOrpo141BA35bV01, ToMetadata("HuggingFaceH4/zephyr-orpo-141b-A35b-v0.1",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.PhindCodellama34BV2, ToMetadata("Phind/Phind-CodeLlama-34B-v2",4096,6E-07,6E-07)},
        { DeepInfraModelIds.Qwen27BInstruct, ToMetadata("Qwen/Qwen2-7B-Instruct",32768,7E-08,7E-08)},
        { DeepInfraModelIds.Starcoder215B, ToMetadata("bigcode/starcoder2-15b",16384,4.0000000000000003E-07,4.0000000000000003E-07)},
        { DeepInfraModelIds.Starcoder215BInstructV01, ToMetadata("bigcode/starcoder2-15b-instruct-v0.1",null,1.5E-07,1.5E-07)},
        { DeepInfraModelIds.Codellama34BInstructHf, ToMetadata("codellama/CodeLlama-34b-Instruct-hf",4096,6E-07,6E-07)},
        { DeepInfraModelIds.Codellama70BInstructHf, ToMetadata("codellama/CodeLlama-70b-Instruct-hf",4096,7E-07,9.000000000000001E-07)},
        { DeepInfraModelIds.Dolphin26Mixtral8X7B, ToMetadata("cognitivecomputations/dolphin-2.6-mixtral-8x7b",32768,2.4E-07,2.4E-07)},
        { DeepInfraModelIds.DbrxInstruct, ToMetadata("databricks/dbrx-instruct",32768,6E-07,6E-07)},
        { DeepInfraModelIds.Airoboros70B, ToMetadata("deepinfra/airoboros-70b",4096,7E-07,9.000000000000001E-07)},
        { DeepInfraModelIds.Codegemma7BIt, ToMetadata("google/codegemma-7b-it",8192,7E-08,7E-08)},
        { DeepInfraModelIds.Gemma117BIt, ToMetadata("google/gemma-1.1-7b-it",8192,7E-08,7E-08)},
        { DeepInfraModelIds.Llama213BChatHf, ToMetadata("meta-llama/Llama-2-13b-chat-hf",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.Llama270BChatHf, ToMetadata("meta-llama/Llama-2-70b-chat-hf",4096,6.4E-07,8.000000000000001E-07)},
        { DeepInfraModelIds.Llama27BChatHf, ToMetadata("meta-llama/Llama-2-7b-chat-hf",4096,7E-08,7E-08)},
        { DeepInfraModelIds.Mistral7BInstructV01, ToMetadata("mistralai/Mistral-7B-Instruct-v0.1",32768,6E-08,6E-08)},
        { DeepInfraModelIds.Mistral7BInstructV02, ToMetadata("mistralai/Mistral-7B-Instruct-v0.2",32768,6E-08,6E-08)},
        { DeepInfraModelIds.Mixtral8X22bV01, ToMetadata("mistralai/Mixtral-8x22B-v0.1",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.Nemotron4340BInstruct, ToMetadata("nvidia/Nemotron-4-340B-Instruct",4096,4.2000000000000004E-06,4.2000000000000004E-06)},
        { DeepInfraModelIds.OpenChat35, ToMetadata("openchat/openchat_3.5",8192,6E-08,6E-08)},

    };

    public static ChatModelMetadata ToMetadata(string? id, int? contextLength, double? pricePerInputTokenInUsd, double? pricePerOutputTokenInUsd)
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
    public static ChatModelMetadata GetModelById(DeepInfraModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Deep Infra Model {modelId}");
    }
}