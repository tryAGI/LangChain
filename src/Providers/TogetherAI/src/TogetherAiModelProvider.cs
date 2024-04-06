using LangChain.Providers.TogetherAi;
using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter
{
    /// <summary>
    /// Contains all the OpenRouter models.
    /// </summary>
    public static class TogetherAiModelProvider
    {
        private static IReadOnlyDictionary<TogetherAiModelIds, ChatModels> Models { get; set; }
        static TogetherAiModelProvider()
        {
            var dic = new Dictionary<TogetherAiModelIds, ChatModels>
            {
          		{ TogetherAiModelIds.ChronosHermes13B, new ChatModels("Austism/chronos-hermes-13b",2048,3E-07,3E-07)},
				{ TogetherAiModelIds.MythomaxL213B, new ChatModels("Gryphe/MythoMax-L2-13b",4096,3E-07,3E-07)},
				{ TogetherAiModelIds.NousCapybaraV197B, new ChatModels("NousResearch/Nous-Capybara-7B-V1p9",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.NousHermes2MistralDpo7B, new ChatModels("NousResearch/Nous-Hermes-2-Mistral-7B-DPO",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.NousHermes2Mixtral8X7BDpo, new ChatModels("NousResearch/Nous-Hermes-2-Mixtral-8x7B-DPO",32768,6E-07,6E-07)},
				{ TogetherAiModelIds.NousHermes2Mixtral8X7BSft, new ChatModels("NousResearch/Nous-Hermes-2-Mixtral-8x7B-SFT",32768,6E-07,6E-07)},
				{ TogetherAiModelIds.NousHermes2Yi34B, new ChatModels("NousResearch/Nous-Hermes-2-Yi-34B",4096,8.000000000000001E-07,8.000000000000001E-07)},
				{ TogetherAiModelIds.NousHermesLlama213B, new ChatModels("NousResearch/Nous-Hermes-Llama2-13b",4096,3E-07,3E-07)},
				{ TogetherAiModelIds.NousHermesLlama27B, new ChatModels("NousResearch/Nous-Hermes-llama-2-7b",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.OpenOrcaMistral7B8K, new ChatModels("Open-Orca/Mistral-7B-OpenOrca",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.PhindCodeLlamaV234B, new ChatModels("Phind/Phind-CodeLlama-34B-v2",16384,8.000000000000001E-07,8.000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat05B, new ChatModels("Qwen/Qwen1.5-0.5B-Chat",32768,1.0000000000000001E-07,1.0000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat18B, new ChatModels("Qwen/Qwen1.5-1.8B-Chat",32768,1.0000000000000001E-07,1.0000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat14B, new ChatModels("Qwen/Qwen1.5-14B-Chat",32768,3E-07,3E-07)},
				{ TogetherAiModelIds.Qwen15Chat32B, new ChatModels("Qwen/Qwen1.5-32B-Chat",32768,8.000000000000001E-07,8.000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat4B, new ChatModels("Qwen/Qwen1.5-4B-Chat",32768,1.0000000000000001E-07,1.0000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat72B, new ChatModels("Qwen/Qwen1.5-72B-Chat",32768,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.Qwen15Chat7B, new ChatModels("Qwen/Qwen1.5-7B-Chat",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.RemmSlerpL213B, new ChatModels("Undi95/ReMM-SLERP-L2-13B",4096,3E-07,3E-07)},
				{ TogetherAiModelIds.ToppyM7B, new ChatModels("Undi95/Toppy-M-7B",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.WizardcoderV1015B, new ChatModels("WizardLM/WizardCoder-15B-V1.0",8192,3E-07,3E-07)},
				{ TogetherAiModelIds.WizardcoderPythonV1034B, new ChatModels("WizardLM/WizardCoder-Python-34B-V1.0",8192,8.000000000000001E-07,8.000000000000001E-07)},
				{ TogetherAiModelIds.WizardlmV1213B, new ChatModels("WizardLM/WizardLM-13B-V1.2",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.OlmoInstruct7B, new ChatModels("allenai/OLMo-7B-Instruct",2048,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.CodeLlamaInstruct13B, new ChatModels("codellama/CodeLlama-13b-Instruct-hf",16384,2.2E-07,2.2E-07)},
				{ TogetherAiModelIds.CodeLlamaPython13B, new ChatModels("codellama/CodeLlama-13b-Python-hf",16384,2.2E-07,2.2E-07)},
				{ TogetherAiModelIds.CodeLlamaInstruct34B, new ChatModels("codellama/CodeLlama-34b-Instruct-hf",16384,7.760000000000001E-07,7.760000000000001E-07)},
				{ TogetherAiModelIds.CodeLlamaPython34B, new ChatModels("codellama/CodeLlama-34b-Python-hf",16384,7.760000000000001E-07,7.760000000000001E-07)},
				{ TogetherAiModelIds.CodeLlamaInstruct70B, new ChatModels("codellama/CodeLlama-70b-Instruct-hf",4096,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.CodeLlamaPython70B, new ChatModels("codellama/CodeLlama-70b-Python-hf",4096,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.CodeLlama70B, new ChatModels("codellama/CodeLlama-70b-hf",16384,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.CodeLlamaInstruct7B, new ChatModels("codellama/CodeLlama-7b-Instruct-hf",16384,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.CodeLlamaPython7B, new ChatModels("codellama/CodeLlama-7b-Python-hf",16384,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Dolphin25Mixtral8X7B, new ChatModels("cognitivecomputations/dolphin-2.5-mixtral-8x7b",32768,6E-07,6E-07)},
				{ TogetherAiModelIds.DeepseekCoderInstruct33B, new ChatModels("deepseek-ai/deepseek-coder-33b-instruct",16384,8.000000000000001E-07,8.000000000000001E-07)},
				{ TogetherAiModelIds.DeepseekLlmChat67B, new ChatModels("deepseek-ai/deepseek-llm-67b-chat",4096,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.Platypus2Instruct70B, new ChatModels("garage-bAInd/Platypus2-70B-instruct",4096,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.GemmaInstruct2B, new ChatModels("google/gemma-2b-it",8192,1.0000000000000001E-07,1.0000000000000001E-07)},
				{ TogetherAiModelIds.GemmaInstruct7B, new ChatModels("google/gemma-7b-it",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.VicunaV1513B, new ChatModels("lmsys/vicuna-13b-v1.5",4096,3E-07,3E-07)},
				{ TogetherAiModelIds.VicunaV157B, new ChatModels("lmsys/vicuna-7b-v1.5",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Llama2Chat13B, new ChatModels("meta-llama/Llama-2-13b-chat-hf",4096,2.2E-07,2.2E-07)},
				{ TogetherAiModelIds.Llama2Chat70B, new ChatModels("meta-llama/Llama-2-70b-chat-hf",4096,9.000000000000001E-07,9.000000000000001E-07)},
				{ TogetherAiModelIds.Llama2Chat7B, new ChatModels("meta-llama/Llama-2-7b-chat-hf",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Mistral7BInstruct, new ChatModels("mistralai/Mistral-7B-Instruct-v0.1",4096,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Mistral7BInstructV02, new ChatModels("mistralai/Mistral-7B-Instruct-v0.2",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Mixtral8X7BInstructV01, new ChatModels("mistralai/Mixtral-8x7B-Instruct-v0.1",32768,6E-07,6E-07)},
				{ TogetherAiModelIds.OpenChat35, new ChatModels("openchat/openchat-3.5-1210",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.SnorkelMistralPairrmDpo7B, new ChatModels("snorkelai/Snorkel-Mistral-PairRM-DPO",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.OpenHermes2Mistral7B, new ChatModels("teknium/OpenHermes-2-Mistral-7B",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.OpenHermes25Mistral7B, new ChatModels("teknium/OpenHermes-2p5-Mistral-7B",8192,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Llama27B32KInstruct7B, new ChatModels("togethercomputer/Llama-2-7B-32K-Instruct",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.RedpajamaInciteChat7B, new ChatModels("togethercomputer/RedPajama-INCITE-7B-Chat",2048,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.RedpajamaInciteChat3B, new ChatModels("togethercomputer/RedPajama-INCITE-Chat-3B-v1",2048,1.0000000000000001E-07,1.0000000000000001E-07)},
				{ TogetherAiModelIds.StripedhyenaNous7B, new ChatModels("togethercomputer/StripedHyena-Nous-7B",32768,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.Alpaca7B, new ChatModels("togethercomputer/alpaca-7b",2048,2.0000000000000002E-07,2.0000000000000002E-07)},
				{ TogetherAiModelIds.UpstageSolarInstructV111B, new ChatModels("upstage/SOLAR-10.7B-Instruct-v1.0",4096,3E-07,3E-07)},
				{ TogetherAiModelIds._01AiYiChat34B, new ChatModels("zero-one-ai/Yi-34B-Chat",4096,8.000000000000001E-07,8.000000000000001E-07)},

            };


            Models = dic;
        }

        public static ChatModels GetModelById(TogetherAiModelIds modelId)
        {
            if (Models.ContainsKey(modelId))
            {
                return Models[modelId];
            }
            else
            {
                throw new ArgumentException($"Invalid Together.Ai Model {modelId.ToString()}");
            }
        }
    }
}
