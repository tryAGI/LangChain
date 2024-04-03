using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter
{
    public static class OpenRouterModelProvider
    {
        private static IReadOnlyDictionary<OpenRouterModelIds, ChatModels> Models { get; set; }// = new Dictionary<string, ChatModels>();
        static OpenRouterModelProvider()
        {
            var dic = new Dictionary<OpenRouterModelIds, ChatModels>
            {
                { OpenRouterModelIds.Nous_Capybara_7B_Free, new ChatModels("nousresearch/nous-capybara-7b:free", 4096, 0, 0)},
                { OpenRouterModelIds.Mistral_7B_Instruct_Free, new ChatModels("mistralai/mistral-7b-instruct:free", 32768, 0, 0)},
                { OpenRouterModelIds.Mythomist_7B_Free, new ChatModels("gryphe/mythomist-7b:free", 32768, 0, 0) },
                { OpenRouterModelIds.Toppy_M_7B_Free, new ChatModels("undi95/toppy-m-7b:free", 4096, 0, 0) },
                { OpenRouterModelIds.Cinematika_7B_Alpha_Free, new ChatModels("openrouter/cinematika-7b:free", 8000, 0, 0) },
                { OpenRouterModelIds.Google_Gemma_7B_Free, new ChatModels("google/gemma-7b-it:free", 8192, 0, 0) },
                { OpenRouterModelIds.Psyfighter_13B, new ChatModels("jebcarter/psyfighter-13b", 4096, 0.001, 0.001) },
                { OpenRouterModelIds.Psyfighter_V2_13B, new ChatModels("koboldai/psyfighter-13b-2", 4096, 0.001, 0.001) },
                { OpenRouterModelIds.Nous_Hermes_13B, new ChatModels("nousresearch/nous-hermes-llama2-13b", 4096, 0.00015, 0.00015) },
                { OpenRouterModelIds.Meta_Codellama_34B_Instruct, new ChatModels("meta-llama/codellama-34b-instruct", 8192, 0.0004, 0.0004) },
                { OpenRouterModelIds.Phind_Codellama_34B_V2, new ChatModels("phind/phind-codellama-34b", 4096, 0.0004, 0.0004) },
                { OpenRouterModelIds.Neural_Chat_7B_V3_1, new ChatModels("intel/neural-chat-7b", 4096, 0.005, 0.005) },
                { OpenRouterModelIds.Nous_Hermes_2_Mixtral_8X7b_DPO, new ChatModels("nousresearch/nous-hermes-2-mixtral-8x7b-dpo", 32000, 0.0003, 0.0003) },
                { OpenRouterModelIds.Nous_Hermes_2_Mixtral_8X7b_SFT, new ChatModels("nousresearch/nous-hermes-2-mixtral-8x7b-sft", 32000, 0.0003, 0.0003) },
                { OpenRouterModelIds.Llava_13B, new ChatModels("haotian-liu/llava-13b", 2048, 0.005, 0.005) },
                { OpenRouterModelIds.Nous_Hermes_2_Vision_7B_Alpha, new ChatModels("nousresearch/nous-hermes-2-vision-7b", 4096, 0.005, 0.005) },
                { OpenRouterModelIds.Meta_Llama_V2_13B_Chat, new ChatModels("meta-llama/llama-2-13b-chat", 4096, 0.0001474, 0.0001474) },
                { OpenRouterModelIds.Synthia_70B, new ChatModels("migtissera/synthia-70b", 8192, 0.00375, 0.00375) },
                { OpenRouterModelIds.Pygmalion_Mythalion_13B, new ChatModels("pygmalionai/mythalion-13b", 8192, 0.001125, 0.001125) },
                { OpenRouterModelIds.Mythomax_13B, new ChatModels("gryphe/mythomax-l2-13b", 4096, 0.000225, 0.000225) },
                { OpenRouterModelIds.Xwin_70B, new ChatModels("xwin-lm/xwin-lm-70b", 8192, 0.00375, 0.00375) },
                { OpenRouterModelIds.Goliath_120B, new ChatModels("alpindale/goliath-120b", 6144, 0.009375, 0.009375) },
                { OpenRouterModelIds.Noromaid_20B, new ChatModels("neversleep/noromaid-20b", 8192, 0.00225, 0.00225) },
                { OpenRouterModelIds.Mythomist_7B, new ChatModels("gryphe/mythomist-7b", 32768, 0.000375, 0.000375) },
                { OpenRouterModelIds.Midnight_Rose_70B, new ChatModels("sophosympatheia/midnight-rose-70b", 4096, 0.009, 0.009) },
                { OpenRouterModelIds.Remm_SLERP_13B_Extended, new ChatModels("undi95/remm-slerp-l2-13b:extended", 6144, 0.001125, 0.001125) },
                { OpenRouterModelIds.Mythomax_13B_Extended, new ChatModels("gryphe/mythomax-l2-13b:extended", 8192, 0.001125, 0.001125) },
                { OpenRouterModelIds.Mancer_Weaver_Alpha, new ChatModels("mancer/weaver", 8000, 0.003375, 0.003375) },
                { OpenRouterModelIds.Nous_Capybara_7B, new ChatModels("nousresearch/nous-capybara-7b", 4096, 0.00018, 0.00018) },
                { OpenRouterModelIds.Meta_Codellama_70B_Instruct, new ChatModels("codellama/codellama-70b-instruct", 2048, 0.00081, 0.00081) },
                { OpenRouterModelIds.Openhermes_2_Mistral_7B, new ChatModels("teknium/openhermes-2-mistral-7b", 4096, 0.00018, 0.00018) },
                { OpenRouterModelIds.Openhermes_2_5_Mistral_7B, new ChatModels("teknium/openhermes-2.5-mistral-7b", 4096, 0.00018, 0.00018) },
                { OpenRouterModelIds.Remm_SLERP_13B, new ChatModels("undi95/remm-slerp-l2-13b", 4096, 0.00027, 0.00027) },
                { OpenRouterModelIds.Toppy_M_7B, new ChatModels("undi95/toppy-m-7b", 4096, 0.00018, 0.00018) },
                { OpenRouterModelIds.Cinematika_7B_Alpha, new ChatModels("openrouter/cinematika-7b", 8000, 0.00018, 0.00018) },
                { OpenRouterModelIds.Yi_34B_Chat, new ChatModels("01-ai/yi-34b-chat", 4096, 0.00072, 0.00072) },
                { OpenRouterModelIds.Yi_34B_Base, new ChatModels("01-ai/yi-34b", 4096, 0.00072, 0.00072) },
                { OpenRouterModelIds.Yi_6B_Base, new ChatModels("01-ai/yi-6b", 4096, 0.000126, 0.000126) },
                { OpenRouterModelIds.Stripedhyena_Nous_7B, new ChatModels("togethercomputer/stripedhyena-nous-7b", 32768, 0.00018, 0.00018) },
                { OpenRouterModelIds.Stripedhyena_Hessian_7B_Base, new ChatModels("togethercomputer/stripedhyena-hessian-7b", 32768, 0.00018, 0.00018) },
                { OpenRouterModelIds.Mixtral_8X7b_Base, new ChatModels("mistralai/mixtral-8x7b", 32768, 0.00054, 0.00054) },
                { OpenRouterModelIds.Nous_Hermes_2_Yi_34B, new ChatModels("nousresearch/nous-hermes-yi-34b", 4096, 0.00072, 0.00072) },
                { OpenRouterModelIds.Nous_Hermes_2_Mistral_7B_DPO, new ChatModels("nousresearch/nous-hermes-2-mistral-7b-dpo", 8192, 0.00018, 0.00018) },
                { OpenRouterModelIds.Mistral_Openorca_7B, new ChatModels("open-orca/mistral-7b-openorca", 8192, 0.0001425, 0.0001425) },
                { OpenRouterModelIds.Hugging_Face_Zephyr_7B, new ChatModels("huggingfaceh4/zephyr-7b-beta", 4096, 0.0001425, 0.0001425) },
                { OpenRouterModelIds.OpenAI_GPT_3_5_Turbo, new ChatModels("OpenAI/gpt-3.5-turbo", 4095, 0.001, 0.002) },
                { OpenRouterModelIds.OpenAI_GPT_3_5_Turbo_16K_0125, new ChatModels("OpenAI/gpt-3.5-turbo-0125", 16385, 0.0005, 0.0015) },
                { OpenRouterModelIds.OpenAI_GPT_3_5_Turbo_16K, new ChatModels("OpenAI/gpt-3.5-turbo-16k", 16385, 0.003, 0.004) },
                { OpenRouterModelIds.OpenAI_GPT_4_Turbo, new ChatModels("OpenAI/gpt-4-turbo-preview", 128000, 0.01, 0.03) },
                { OpenRouterModelIds.OpenAI_GPT_4, new ChatModels("OpenAI/gpt-4", 8191, 0.03, 0.06) },
                { OpenRouterModelIds.OpenAI_GPT_4_32K, new ChatModels("OpenAI/gpt-4-32k", 32767, 0.06, 0.12) },
                { OpenRouterModelIds.OpenAI_GPT_4_Vision, new ChatModels("OpenAI/gpt-4-vision-preview", 128000, 0.01, 0.03) },
                { OpenRouterModelIds.OpenAI_GPT_3_5_Turbo_Instruct, new ChatModels("OpenAI/gpt-3.5-turbo-instruct", 4095, 0.0015, 0.002) },
                { OpenRouterModelIds.Google_Palm_2_Chat, new ChatModels("google/palm-2-chat-bison", 36864, 0.00025, 0.0005) },
                { OpenRouterModelIds.Google_Palm_2_Code_Chat, new ChatModels("google/palm-2-codechat-bison", 28672, 0.00025, 0.0005) },
                { OpenRouterModelIds.Google_Palm_2_Chat_32K, new ChatModels("google/palm-2-chat-bison-32k", 131072, 0.00025, 0.0005) },
                { OpenRouterModelIds.Google_Palm_2_Code_Chat_32K, new ChatModels("google/palm-2-codechat-bison-32k", 131072, 0.00025, 0.0005) },
                { OpenRouterModelIds.Google_Gemini_Pro_1_0, new ChatModels("google/gemini-pro", 131072, 0.000125, 0.000375) },
                { OpenRouterModelIds.Google_Gemini_Pro_Vision_1_0, new ChatModels("google/gemini-pro-vision", 65536, 0.000125, 0.000375) },
                { OpenRouterModelIds.Perplexity_PPLX_70B_Online, new ChatModels("perplexity/pplx-70b-online", 4096, 0.001, 0.001) },
                { OpenRouterModelIds.Perplexity_PPLX_7B_Online, new ChatModels("perplexity/pplx-7b-online", 4096, 0.0002, 0.0002) },
                { OpenRouterModelIds.Perplexity_PPLX_7B_Chat, new ChatModels("perplexity/pplx-7b-chat", 8192, 0.0002, 0.0002) },
                { OpenRouterModelIds.Perplexity_PPLX_70B_Chat, new ChatModels("perplexity/pplx-70b-chat", 4096, 0.001, 0.001) },
                { OpenRouterModelIds.Perplexity_Sonar_7B, new ChatModels("perplexity/sonar-small-chat", 16384, 0.0002, 0.0002) },
                { OpenRouterModelIds.Perplexity_Sonar_8X7b, new ChatModels("perplexity/sonar-medium-chat", 16384, 0.0006, 0.0006) },
                { OpenRouterModelIds.Perplexity_Sonar_7B_Online, new ChatModels("perplexity/sonar-small-online", 12000, 0.0002, 0.0002) },
                { OpenRouterModelIds.Perplexity_Sonar_8X7b_Online, new ChatModels("perplexity/sonar-medium-online", 12000, 0.0006, 0.0006) },
                { OpenRouterModelIds.Anthropic_Claude_3_Opus, new ChatModels("anthropic/claude-3-opus", 200000, 0.015, 0.075) },
                { OpenRouterModelIds.Anthropic_Claude_3_Sonnet, new ChatModels("anthropic/claude-3-sonnet", 200000, 0.003, 0.015) },
                { OpenRouterModelIds.Anthropic_Claude_3_Haiku, new ChatModels("anthropic/claude-3-haiku", 200000, 0.00025, 0.00125) },
                { OpenRouterModelIds.Anthropic_Claude_3_Opus_Self_Moderated, new ChatModels("anthropic/claude-3-opus:beta", 200000, 0.015, 0.075) },
                { OpenRouterModelIds.Anthropic_Claude_3_Sonnet_Self_Moderated, new ChatModels("anthropic/claude-3-sonnet:beta", 200000, 0.003, 0.015) },
                { OpenRouterModelIds.Anthropic_Claude_3_Haiku_Self_Moderated, new ChatModels("anthropic/claude-3-haiku:beta", 200000, 0.00025, 0.00125) },
                { OpenRouterModelIds.Meta_Llama_V2_70B_Chat, new ChatModels("meta-llama/llama-2-70b-chat", 4096, 0.0007, 0.0009) },
                { OpenRouterModelIds.Nous_Capybara_34B, new ChatModels("nousresearch/nous-capybara-34b", 32768, 0.0009, 0.0009) },
                { OpenRouterModelIds.Airoboros_70B, new ChatModels("jondurbin/airoboros-l2-70b", 4096, 0.0007, 0.0009) },
                { OpenRouterModelIds.Bagel_34B_V0_2, new ChatModels("jondurbin/bagel-34b", 8000, 0.00575, 0.00575) },
                { OpenRouterModelIds.Chronos_Hermes_13B_V2, new ChatModels("austism/chronos-hermes-13b", 4096, 0.00022, 0.00022) },
                { OpenRouterModelIds.Mistral_7B_Instruct, new ChatModels("mistralai/mistral-7b-instruct", 32768, 0.00013, 0.00013) },
                { OpenRouterModelIds.OpenChat_3_5, new ChatModels("OpenChat/OpenChat-7b", 8192, 0.00013, 0.00013) },
                { OpenRouterModelIds.Lzlv_70B, new ChatModels("lizpreciatior/lzlv-70b-fp16-hf", 4096, 0.0007, 0.0009) },
                { OpenRouterModelIds.Mixtral_8X7b_Instruct, new ChatModels("mistralai/mixtral-8x7b-instruct", 32768, 0.00027, 0.00027) },
                { OpenRouterModelIds.Dolphin_2_6_Mixtral_8X7b, new ChatModels("cognitivecomputations/dolphin-mixtral-8x7b", 32000, 0.0005, 0.0005) },
                { OpenRouterModelIds.Noromaid_Mixtral_8X7b_Instruct, new ChatModels("neversleep/noromaid-mixtral-8x7b-instruct", 8000, 0.008, 0.008) },
                { OpenRouterModelIds.RWKV_V5_World_3B, new ChatModels("rwkv/rwkv-5-world-3b", 10000, 0, 0) },
                { OpenRouterModelIds.RWKV_V5_3B_AI_Town, new ChatModels("recursal/rwkv-5-3b-ai-town", 10000, 0, 0) },
                { OpenRouterModelIds.RWKV_V5_Eagle_7B, new ChatModels("recursal/eagle-7b", 10000, 0, 0) },
                { OpenRouterModelIds.Google_Gemma_7B, new ChatModels("google/gemma-7b-it", 8192, 0.00013, 0.00013) },
                { OpenRouterModelIds.Databricks_DBRX_132B_Instruct, new ChatModels("databricks/dbrx-instruct", 32768, 0.0009, 0.0009) },
                { OpenRouterModelIds.Anthropic_Claude_V2, new ChatModels("anthropic/claude-2", 200000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_V2_1, new ChatModels("anthropic/claude-2.1", 200000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_V2_0, new ChatModels("anthropic/claude-2.0", 100000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_Instant_V1, new ChatModels("anthropic/claude-instant-1", 100000, 0.0008, 0.0024) },
                { OpenRouterModelIds.Anthropic_Claude_Instant_V1_2, new ChatModels("anthropic/claude-instant-1.2", 100000, 0.0008, 0.0024) },
                { OpenRouterModelIds.Anthropic_Claude_V2_Self_Moderated, new ChatModels("anthropic/claude-2:beta", 200000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_V2_1_Self_Moderated, new ChatModels("anthropic/claude-2.1:beta", 200000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_V2_0_Self_Moderated, new ChatModels("anthropic/claude-2.0:beta", 100000, 0.008, 0.024) },
                { OpenRouterModelIds.Anthropic_Claude_Instant_V1_Self_Moderated, new ChatModels("anthropic/claude-instant-1:beta", 100000, 0.0008, 0.0024) },
                { OpenRouterModelIds.Hugging_Face_Zephyr_7B_Free, new ChatModels("huggingfaceh4/zephyr-7b-beta:free", 4096, 0, 0) },
                { OpenRouterModelIds.OpenChat_3_5_Free, new ChatModels("OpenChat/OpenChat-7b:free", 8192, 0, 0) },
                { OpenRouterModelIds.Mixtral_8X7b_Instruct_Nitro, new ChatModels("mistralai/mixtral-8x7b-instruct:nitro", 32768, 0.0005, 0.0005) },
                { OpenRouterModelIds.Meta_Llama_V2_70B_Chat_Nitro, new ChatModels("meta-llama/llama-2-70b-chat:nitro", 4096, 0.0009, 0.0009) },
                { OpenRouterModelIds.Mythomax_13B_Nitro, new ChatModels("gryphe/mythomax-l2-13b:nitro", 4096, 0.0002, 0.0002) },
                { OpenRouterModelIds.Mistral_7B_Instruct_Nitro, new ChatModels("mistralai/mistral-7b-instruct:nitro", 32768, 0.0002, 0.0002) },
                { OpenRouterModelIds.Google_Gemma_7B_Nitro, new ChatModels("google/gemma-7b-it:nitro", 8192, 0.0002, 0.0002) },
                { OpenRouterModelIds.Databricks_DBRX_132B_Instruct_Nitro, new ChatModels("databricks/dbrx-instruct:nitro", 32768, 0.0009, 0.0009) },
                { OpenRouterModelIds.Mistral_Tiny, new ChatModels("mistralai/mistral-tiny", 32000, 0.00025, 0.00025) },
                { OpenRouterModelIds.Mistral_Small, new ChatModels("mistralai/mistral-small", 32000, 0.002, 0.006) },
                { OpenRouterModelIds.Mistral_Medium, new ChatModels("mistralai/mistral-medium", 32000, 0.0027, 0.0081) },
                { OpenRouterModelIds.Mistral_Large, new ChatModels("mistralai/mistral-large", 32000, 0.008, 0.024) },
                { OpenRouterModelIds.Cohere_Command, new ChatModels("cohere/command", 4096, 0.001, 0.002) },
                { OpenRouterModelIds.Cohere_Command_R, new ChatModels("cohere/command-r", 128000, 0.0005, 0.0015) }
            };

            Models = dic;
        }

        public static ChatModels GetModelById(OpenRouterModelIds modelId)
        {
            if (Models.ContainsKey(modelId))
            {
                return Models[modelId];
            }
            else
            {
                throw new ArgumentException($"Invalid Open Router Model {modelId.ToString()}");
            }
        }
    }
}
