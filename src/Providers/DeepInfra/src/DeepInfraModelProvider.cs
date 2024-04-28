using OpenAI.Constants;

namespace LangChain.Providers.DeepInfra;

/// <summary>
/// Contains all the DeepInfra models.
/// </summary>
public static class DeepInfraModelProvider
{
    private static Dictionary<DeepInfraModelIds, ChatModels> Models { get; set; } = new()
    {
        { DeepInfraModelIds.MetaLlama370BInstruct, new ChatModels("meta-llama/Meta-Llama-3-70B-Instruct",8192,5.9E-07,7.900000000000001E-07)},
        { DeepInfraModelIds.MetaLlama38BInstruct, new ChatModels("meta-llama/Meta-Llama-3-8B-Instruct",8192,8E-08,8E-08)},
        { DeepInfraModelIds.Mixtral8X22bInstructV01, new ChatModels("mistralai/Mixtral-8x22B-Instruct-v0.1",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.Wizardlm28X22b, new ChatModels("microsoft/WizardLM-2-8x22B",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.Wizardlm27B, new ChatModels("microsoft/WizardLM-2-7B",32768,7E-08,7E-08)},
        { DeepInfraModelIds.ZephyrOrpo141BA35bV01, new ChatModels("HuggingFaceH4/zephyr-orpo-141b-A35b-v0.1",65536,6.5E-07,6.5E-07)},
        { DeepInfraModelIds.Gemma117BIt, new ChatModels("google/gemma-1.1-7b-it",8192,7E-08,7E-08)},
        { DeepInfraModelIds.DbrxInstruct, new ChatModels("databricks/dbrx-instruct",32768,6E-07,6E-07)},
        { DeepInfraModelIds.Mixtral8X7BInstructV01, new ChatModels("mistralai/Mixtral-8x7B-Instruct-v0.1",32768,2.4E-07,2.4E-07)},
        { DeepInfraModelIds.Mistral7BInstructV02, new ChatModels("mistralai/Mistral-7B-Instruct-v0.2",32768,7E-08,7E-08)},
        { DeepInfraModelIds.Llama270BChatHf, new ChatModels("meta-llama/Llama-2-70b-chat-hf",4096,6.4E-07,8.000000000000001E-07)},
        { DeepInfraModelIds.Dolphin26Mixtral8X7B, new ChatModels("cognitivecomputations/dolphin-2.6-mixtral-8x7b",32768,2.4E-07,2.4E-07)},
        { DeepInfraModelIds.Lzlv70BFp16Hf, new ChatModels("lizpreciatior/lzlv_70b_fp16_hf",4096,5.9E-07,7.900000000000001E-07)},
        { DeepInfraModelIds.OpenChat35, new ChatModels("openchat/openchat_3.5",8192,1.0000000000000001E-07,1.0000000000000001E-07)},
        { DeepInfraModelIds.Llava157BHf, new ChatModels("llava-hf/llava-1.5-7b-hf",4096,3.4000000000000003E-07,3.4000000000000003E-07)},
        { DeepInfraModelIds.Airoboros70B, new ChatModels("deepinfra/airoboros-70b",4096,7E-07,9.000000000000001E-07)},
        { DeepInfraModelIds.Llama27BChatHf, new ChatModels("meta-llama/Llama-2-7b-chat-hf",4096,7E-08,7E-08)},
        { DeepInfraModelIds.Yi34BChat, new ChatModels("01-ai/Yi-34B-Chat",4096,6E-07,6E-07)},
        { DeepInfraModelIds.ChronosHermes13BV2, new ChatModels("Austism/chronos-hermes-13b-v2",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.MythomaxL213B, new ChatModels("Gryphe/MythoMax-L2-13b",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.PhindCodellama34BV2, new ChatModels("Phind/Phind-CodeLlama-34B-v2",4096,6E-07,6E-07)},
        { DeepInfraModelIds.Starcoder215B, new ChatModels("bigcode/starcoder2-15b",16384,4.0000000000000003E-07,4.0000000000000003E-07)},
        { DeepInfraModelIds.Codellama34BInstructHf, new ChatModels("codellama/CodeLlama-34b-Instruct-hf",4096,6E-07,6E-07)},
        { DeepInfraModelIds.Codellama70BInstructHf, new ChatModels("codellama/CodeLlama-70b-Instruct-hf",4096,7E-07,9.000000000000001E-07)},
        { DeepInfraModelIds.Llama213BChatHf, new ChatModels("meta-llama/Llama-2-13b-chat-hf",4096,1.3E-07,1.3E-07)},
        { DeepInfraModelIds.Mistral7BInstructV01, new ChatModels("mistralai/Mistral-7B-Instruct-v0.1",32768,7E-08,7E-08)},
        { DeepInfraModelIds.Mixtral8X22bV01, new ChatModels("mistralai/Mixtral-8x22B-v0.1",65536,6.5E-07,6.5E-07)},

    };

    [CLSCompliant(false)]
    public static ChatModels GetModelById(DeepInfraModelIds modelId)
    {
        if (Models.TryGetValue(modelId, out var id))
        {
            return id;
        }

        throw new ArgumentException($"Invalid Deep Infra Model {modelId}");
    }
}