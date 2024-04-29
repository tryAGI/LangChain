namespace LangChain.Providers.OpenRouter;

/// <summary>
/// List of all the Predefined OpenRouter Models
/// </summary>
public enum OpenRouterModelIds
{

        /// <summary>
        /// The Capybara series is a collection of datasets and models made by fine-tuning on data created by Nous, mostly in-house.  <br/>
        /// V1.9 uses unalignment techniques for more consistent and dynamic control. It also leverages a significantly better foundation model, Mistral 7B.  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        NousCapybara7BFree,
        
        /// <summary>
        /// A 7.3B parameter model that outperforms Llama 2 13B on all benchmarks, with optimizations for speed and context length.  <br/>
        /// This is v0.1 of Mistral 7B Instruct. For v0.2, use this model.  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        Mistral7BInstructFree,
        
        /// <summary>
        /// OpenChat is a library of open-source language models, fine-tuned with "C-RLFT (Conditioned Reinforcement Learning Fine-Tuning)" - a strategy inspired by offline reinforcement learning. It has been trained on mixed-quality data without preference labels.  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        OpenChat35Free,
        
        /// <summary>
        /// From the creator of MythoMax, merges a suite of models to reduce word anticipation, ministrations, and other undesirable words in ChatGPT roleplaying data.  <br/>
        /// It combines Neural Chat 7B, Airoboros 7b, Toppy M 7B, Zepher 7b beta, Nous Capybara 34B, OpenHeremes 2.5, and many others.  <br/>
        /// #merge  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        Mythomist7BFree,
        
        /// <summary>
        /// A wild 7B parameter model that merges several models using the new task_arithmetic merge method from mergekit.  <br/>
        /// List of merged models:  <br/>
        /// NousResearch/Nous-Capybara-7B-V1.9  <br/>
        /// HuggingFaceH4/zephyr-7b-beta  <br/>
        /// lemonilia/AshhLimaRP-Mistral-7B  <br/>
        /// Vulkane/120-Days-of-Sodom-LoRA-Mistral-7b  <br/>
        /// Undi95/Mistral-pippa-sharegpt-7b-qlora  <br/>
        /// #merge #uncensored  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        ToppyM7BFree,
        
        /// <summary>
        /// This model is under development. Check the OpenRouter Discord for updates.  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        Cinematika7BAlphaFree,
        
        /// <summary>
        /// Gemma by Google is an advanced, open-source language model family, leveraging the latest in decoder-only, text-to-text technology. It offers English language capabilities across text generation tasks like question answering, summarization, and reasoning. The Gemma 7B variant is comparable in performance to leading open source models.  <br/>
        /// Usage of Gemma is subject to Google's Gemma Terms of Use.  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        GoogleGemma7BFree,
        
        /// <summary>
        /// The v2 of Psyfighter - a merged model created by the KoboldAI community members Jeb Carter and TwistedShadows, made possible thanks to the KoboldAI merge request service.  <br/>
        /// The intent was to add medical data to supplement the model's fictional ability with more details on anatomy and mental states. This model should not be used for medical advice or therapy because of its high likelihood of pulling in fictional data.  <br/>
        /// It's a merge between:  <br/>
        /// KoboldAI/LLaMA2-13B-Tiefighter  <br/>
        /// Doctor-Shotgun/cat-v1.0-13b  <br/>
        /// Doctor-Shotgun/llama-2-13b-chat-limarp-v2-merged.  <br/>
        /// #merge  <br/>
        /// </summary>
        PsyfighterV213B,
        
        /// <summary>
        /// A fine-tuned model based on mistralai/Mistral-7B-v0.1 on the open source dataset Open-Orca/SlimOrca, aligned with DPO algorithm. For more details, refer to the blog: The Practice of Supervised Fine-tuning and Direct Preference Optimization on Habana Gaudi2.  <br/>
        /// </summary>
        NeuralChat7BV31,
        
        /// <summary>
        /// One of the highest performing and most popular fine-tunes of Llama 2 13B, with rich descriptions and roleplay. #merge  <br/>
        /// </summary>
        Mythomax13B,
        
        /// <summary>
        /// A blend of the new Pygmalion-13b and MythoMax. #merge  <br/>
        /// </summary>
        PygmalionMythalion13B,
        
        /// <summary>
        /// Xwin-LM aims to develop and open-source alignment tech for LLMs. Our first release, built-upon on the Llama2 base models, ranked TOP-1 on AlpacaEval. Notably, it's the first to surpass GPT-4 on this benchmark. The project will be continuously updated.  <br/>
        /// </summary>
        Xwin70B,
        
        /// <summary>
        /// A large LLM created by combining two fine-tuned Llama 70B models into one 120B model. Combines Xwin and Euryale.  <br/>
        /// Credits to  <br/>
        /// @chargoddard for developing the framework used to merge the model - mergekit.  <br/>
        /// @Undi95 for helping with the merge ratios.  <br/>
        /// #merge  <br/>
        /// </summary>
        Goliath120B,
        
        /// <summary>
        /// A collab between IkariDev and Undi. This merge is suitable for RP, ERP, and general knowledge.  <br/>
        /// #merge #uncensored  <br/>
        /// </summary>
        Noromaid20B,
        
        /// <summary>
        /// From the creator of MythoMax, merges a suite of models to reduce word anticipation, ministrations, and other undesirable words in ChatGPT roleplaying data.  <br/>
        /// It combines Neural Chat 7B, Airoboros 7b, Toppy M 7B, Zepher 7b beta, Nous Capybara 34B, OpenHeremes 2.5, and many others.  <br/>
        /// #merge  <br/>
        /// </summary>
        Mythomist7B,
        
        /// <summary>
        /// A merge with a complex family tree, this model was crafted for roleplaying and storytelling. Midnight Rose is a successor to Rogue Rose and Aurora Nights and improves upon them both. It wants to produce lengthy output by default and is the best creative writing merge produced so far by sophosympatheia.  <br/>
        /// Descending from earlier versions of Midnight Rose and Wizard Tulu Dolphin 70B, it inherits the best qualities of each.  <br/>
        /// </summary>
        MidnightRose70B,
        
        /// <summary>
        /// Creative writing model, routed with permission. It's fast, it keeps the conversation going, and it stays in character.  <br/>
        /// If you submit a raw prompt, you can use Alpaca or Vicuna formats.  <br/>
        /// </summary>
        Fimbulvetr11BV2,
        
        /// <summary>
        /// A recreation trial of the original MythoMax-L2-B13 but with updated models. #merge  <br/>
        /// Note: this is an extended-context version of this model. It may have higher prices and different outputs.  <br/>
        /// </summary>
        RemmSlerp13BExtended,
        
        /// <summary>
        /// One of the highest performing and most popular fine-tunes of Llama 2 13B, with rich descriptions and roleplay. #merge  <br/>
        /// Note: this is an extended-context version of this model. It may have higher prices and different outputs.  <br/>
        /// </summary>
        Mythomax13BExtended,
        
        /// <summary>
        /// Meta's latest class of model (Llama 3) launched with a variety of sizes and flavors. This 8B instruct-tuned version was optimized for high quality dialogue usecases.  <br/>
        /// It has demonstrated strong performance compared to leading closed-source models in human evaluations.  <br/>
        /// To read more about the model release, click here. Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// Note: this is an extended-context version of this model. It may have higher prices and different outputs.  <br/>
        /// </summary>
        MetaLlama38BInstructExtended,
        
        /// <summary>
        /// An attempt to recreate Claude-style verbosity, but don't expect the same level of coherence or memory. Meant for use in roleplay/narrative situations.  <br/>
        /// </summary>
        MancerWeaverAlpha,
        
        /// <summary>
        /// The Capybara series is a collection of datasets and models made by fine-tuning on data created by Nous, mostly in-house.  <br/>
        /// V1.9 uses unalignment techniques for more consistent and dynamic control. It also leverages a significantly better foundation model, Mistral 7B.  <br/>
        /// </summary>
        NousCapybara7B,
        
        /// <summary>
        /// Code Llama is built upon Llama 2 and excels at filling in code, handling extensive input contexts, and folling programming instructions without prior training for various programming tasks.  <br/>
        /// </summary>
        MetaCodellama34BInstruct,
        
        /// <summary>
        /// Code Llama is a family of large language models for code. This one is based on Llama 2 70B and provides zero-shot instruction-following ability for programming tasks.  <br/>
        /// </summary>
        MetaCodellama70BInstruct,
        
        /// <summary>
        /// A fine-tune of CodeLlama-34B on an internal dataset that helps it exceed GPT-4 on some benchmarks, including HumanEval.  <br/>
        /// </summary>
        PhindCodellama34BV2,
        
        /// <summary>
        /// Trained on 900k instructions, surpasses all previous versions of Hermes 13B and below, and matches 70B on some benchmarks. Hermes 2 has strong multiturn chat skills and system prompt capabilities.  <br/>
        /// </summary>
        OpenHermes2Mistral7B,
        
        /// <summary>
        /// A recreation trial of the original MythoMax-L2-B13 but with updated models. #merge  <br/>
        /// </summary>
        RemmSlerp13B,
        
        /// <summary>
        /// This model is under development. Check the OpenRouter Discord for updates.  <br/>
        /// </summary>
        Cinematika7BAlpha,
        
        /// <summary>
        /// The Yi series models are large language models trained from scratch by developers at 01.AI. This version is instruct-tuned to work better for chat.  <br/>
        /// </summary>
        Yi34BChat,
        
        /// <summary>
        /// The Yi series models are large language models trained from scratch by developers at 01.AI.  <br/>
        /// </summary>
        Yi34BBase,
        
        /// <summary>
        /// The Yi series models are large language models trained from scratch by developers at 01.AI.  <br/>
        /// </summary>
        Yi6BBase,
        
        /// <summary>
        /// This is the chat model variant of the StripedHyena series developed by Together in collaboration with Nous Research.  <br/>
        /// StripedHyena uses a new architecture that competes with traditional Transformers, particularly in long-context data processing. It combines attention mechanisms with gated convolutions for improved speed, efficiency, and scaling. This model marks a significant advancement in AI architecture for sequence modeling tasks.  <br/>
        /// </summary>
        StripedhyenaNous7B,
        
        /// <summary>
        /// This is the base model variant of the StripedHyena series, developed by Together.  <br/>
        /// StripedHyena uses a new architecture that competes with traditional Transformers, particularly in long-context data processing. It combines attention mechanisms with gated convolutions for improved speed, efficiency, and scaling. This model marks an advancement in AI architecture for sequence modeling tasks.  <br/>
        /// </summary>
        StripedhyenaHessian7BBase,
        
        /// <summary>
        /// A pretrained generative Sparse Mixture of Experts, by Mistral AI. Incorporates 8 experts (feed-forward networks) for a total of 47B parameters. Base model (not fine-tuned for instructions) - see Mixtral 8x7B Instruct for an instruct-tuned model.  <br/>
        /// #moe  <br/>
        /// </summary>
        Mixtral8X7BBase,
        
        /// <summary>
        /// Nous Hermes 2 Yi 34B was trained on 1,000,000 entries of primarily GPT-4 generated data, as well as other high quality data from open datasets across the AI landscape.  <br/>
        /// Nous-Hermes 2 on Yi 34B outperforms all Nous-Hermes and Open-Hermes models of the past, achieving new heights in all benchmarks for a Nous Research LLM as well as surpassing many popular finetunes.  <br/>
        /// </summary>
        NousHermes2Yi34B,
        
        /// <summary>
        /// Nous Hermes 2 Mixtral 8x7B SFT is the supervised finetune only version of the Nous Research model trained over the Mixtral 8x7B MoE LLM.  <br/>
        /// The model was trained on over 1,000,000 entries of primarily GPT-4 generated data, as well as other high quality data from open datasets across the AI landscape, achieving state of the art performance on a variety of tasks.  <br/>
        /// #moe  <br/>
        /// </summary>
        NousHermes2Mixtral8X7BSft,
        
        /// <summary>
        /// This is the flagship 7B Hermes model, a Direct Preference Optimization (DPO) of Teknium/OpenHermes-2.5-Mistral-7B. It shows improvement across the board on all benchmarks tested - AGIEval, BigBench Reasoning, GPT4All, and TruthfulQA.  <br/>
        /// The model prior to DPO was trained on 1,000,000 instructions/chats of GPT-4 quality or better, primarily synthetic data as well as other high quality datasets.  <br/>
        /// </summary>
        NousHermes2Mistral7BDpo,
        
        /// <summary>
        /// Meta's latest class of model (Llama 3) launched with a variety of sizes and flavors. This 70B instruct-tuned version was optimized for high quality dialogue usecases.  <br/>
        /// It has demonstrated strong performance compared to leading closed-source models in human evaluations.  <br/>
        /// To read more about the model release, click here. Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// </summary>
        MetaLlama370BInstruct,
        
        /// <summary>
        /// A pretrained generative Sparse Mixture of Experts, by Mistral AI, for chat and instruction use. Incorporates 8 experts (feed-forward networks) for a total of 47 billion parameters.  <br/>
        /// Instruct model fine-tuned by Mistral. #moe  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        Mixtral8X7BInstructNitro,
        
        /// <summary>
        /// A fine-tune of Mistral using the OpenOrca dataset. First 7B model to beat all other models less 30B.  <br/>
        /// </summary>
        MistralOpenOrca7B,
        
        /// <summary>
        /// Zephyr is a series of language models that are trained to act as helpful assistants. Zephyr-7B-β is the second model in the series, and is a fine-tuned version of mistralai/Mistral-7B-v0.1 that was trained on a mix of publicly available, synthetic datasets using Direct Preference Optimization (DPO).  <br/>
        /// </summary>
        HuggingFaceZephyr7B,
        
        /// <summary>
        /// GPT-3.5 Turbo is OpenAI's fastest model. It can understand and generate natural language or code, and is optimized for chat and traditional completion tasks.  <br/>
        /// Updated by OpenAI to point to the latest version of GPT-3.5. Training data up to Sep 2021.  <br/>
        /// </summary>
        OpenAiGpt35Turbo,
        
        /// <summary>
        /// The latest GPT-3.5 Turbo model with improved instruction following, JSON mode, reproducible outputs, parallel function calling, and more. Training data: up to Sep 2021.  <br/>
        /// This version has a higher accuracy at responding in requested formats and a fix for a bug which caused a text encoding issue for non-English language function calls.  <br/>
        /// </summary>
        OpenAiGpt35Turbo16K0125,
        
        /// <summary>
        /// This model offers four times the context length of gpt-3.5-turbo, allowing it to support approximately 20 pages of text in a single request at a higher cost. Training data: up to Sep 2021.  <br/>
        /// </summary>
        OpenAiGpt35Turbo16K,
        
        /// <summary>
        /// The latest GPT-4 Turbo model with vision capabilities. Vision requests can now use JSON mode and function calling. Training data: up to Dec 2023.  <br/>
        /// This model is updated by OpenAI to point to the latest version of GPT-4 Turbo, currently gpt-4-turbo-2024-04-09 (as of April 2024).  <br/>
        /// </summary>
        OpenAiGpt4Turbo,
        
        /// <summary>
        /// The latest GPT-4 model with improved instruction following, JSON mode, reproducible outputs, parallel function calling, and more. Training data: up to Dec 2023.  <br/>
        /// Note: heavily rate limited by OpenAI while in preview.  <br/>
        /// </summary>
        OpenAiGpt4TurboPreview,
        
        /// <summary>
        /// OpenAI's flagship model, GPT-4 is a large-scale multimodal language model capable of solving difficult problems with greater accuracy than previous models due to its broader general knowledge and advanced reasoning capabilities. Training data: up to Sep 2021.  <br/>
        /// </summary>
        OpenAiGpt4,
        
        /// <summary>
        /// GPT-4-32k is an extended version of GPT-4, with the same capabilities but quadrupled context length, allowing for processing up to 40 pages of text in a single pass. This is particularly beneficial for handling longer content like interacting with PDFs without an external vector database. Training data: up to Sep 2021.  <br/>
        /// </summary>
        OpenAiGpt432K,
        
        /// <summary>
        /// Ability to understand images, in addition to all other GPT-4 Turbo capabilties. Training data: up to Apr 2023.  <br/>
        /// Note: heavily rate limited by OpenAI while in preview.  <br/>
        /// #multimodal  <br/>
        /// </summary>
        OpenAiGpt4Vision,
        
        /// <summary>
        /// This model is a variant of GPT-3.5 Turbo tuned for instructional prompts and omitting chat-related optimizations. Training data: up to Sep 2021.  <br/>
        /// </summary>
        OpenAiGpt35TurboInstruct,
        
        /// <summary>
        /// PaLM 2 is a language model by Google with improved multilingual, reasoning and coding capabilities.  <br/>
        /// </summary>
        GooglePalm2Chat,
        
        /// <summary>
        /// PaLM 2 fine-tuned for chatbot conversations that help with code-related questions.  <br/>
        /// </summary>
        GooglePalm2CodeChat,
        
        /// <summary>
        /// PaLM 2 is a language model by Google with improved multilingual, reasoning and coding capabilities.  <br/>
        /// </summary>
        GooglePalm2Chat32K,
        
        /// <summary>
        /// PaLM 2 fine-tuned for chatbot conversations that help with code-related questions.  <br/>
        /// </summary>
        GooglePalm2CodeChat32K,
        
        /// <summary>
        /// Google's flagship text generation model. Designed to handle natural language tasks, multiturn text and code chat, and code generation.  <br/>
        /// See the benchmarks and prompting guidelines from Deepmind.  <br/>
        /// Usage of Gemini is subject to Google's Gemini Terms of Use.  <br/>
        /// </summary>
        GoogleGeminiPro10,
        
        /// <summary>
        /// Google's flagship multimodal model, supporting image and video in text or chat prompts for a text or code response.  <br/>
        /// See the benchmarks and prompting guidelines from Deepmind.  <br/>
        /// Usage of Gemini is subject to Google's Gemini Terms of Use.  <br/>
        /// #multimodal  <br/>
        /// </summary>
        GoogleGeminiProVision10,
        
        /// <summary>
        /// Google's latest multimodal model, supporting image and video in text or chat prompts.  <br/>
        /// Optimized for language tasks including:  <br/>
        /// Code generation  <br/>
        /// Text generation  <br/>
        /// Text editing  <br/>
        /// Problem solving  <br/>
        /// Recommendations  <br/>
        /// Information extraction  <br/>
        /// Data extraction or generation  <br/>
        /// AI agents  <br/>
        /// Usage of Gemini is subject to Google's Gemini Terms of Use.  <br/>
        /// Note: Preview models are offered for testing purposes and should not be used in production apps. This model is heavily rate limited.  <br/>
        /// #multimodal  <br/>
        /// </summary>
        GoogleGeminiPro15Preview,
        
        /// <summary>
        /// The larger, internet-connected chat model by Perplexity Labs, based on Llama 2 70B. The online models are focused on delivering helpful, up-to-date, and factual responses. #online  <br/>
        /// </summary>
        PerplexityPplx70BOnline,
        
        /// <summary>
        /// The smaller, internet-connected chat model by Perplexity Labs, based on Mistral 7B. The online models are focused on delivering helpful, up-to-date, and factual responses. #online  <br/>
        /// </summary>
        PerplexityPplx7BOnline,
        
        /// <summary>
        /// The smaller chat model by Perplexity Labs, with 7 billion parameters. Based on Mistral 7B.  <br/>
        /// </summary>
        PerplexityPplx7BChat,
        
        /// <summary>
        /// The larger chat model by Perplexity Labs, with 70 billion parameters. Based on Llama 2 70B.  <br/>
        /// </summary>
        PerplexityPplx70BChat,
        
        /// <summary>
        /// Sonar is Perplexity's latest model family. It surpasses their earlier models in cost-efficiency, speed, and performance.  <br/>
        /// The version of this model with Internet access is Sonar 7B Online.  <br/>
        /// </summary>
        PerplexitySonar7B,
        
        /// <summary>
        /// Sonar is Perplexity's latest model family. It surpasses their earlier models in cost-efficiency, speed, and performance.  <br/>
        /// The version of this model with Internet access is Sonar 8x7B Online.  <br/>
        /// </summary>
        PerplexitySonar8X7B,
        
        /// <summary>
        /// Sonar is Perplexity's latest model family. It surpasses their earlier models in cost-efficiency, speed, and performance.  <br/>
        /// This is the online version of Sonar 7B. It is focused on delivering helpful, up-to-date, and factual responses. #online  <br/>
        /// </summary>
        PerplexitySonar7BOnline,
        
        /// <summary>
        /// Sonar is Perplexity's latest model family. It surpasses their earlier models in cost-efficiency, speed, and performance.  <br/>
        /// This is the online version of Sonar 8x7B. It is focused on delivering helpful, up-to-date, and factual responses. #online  <br/>
        /// </summary>
        PerplexitySonar8X7BOnline,
        
        /// <summary>
        /// The first commercially permissive OSS LLaVA model.  <br/>
        /// This vision-language model was trained entirely on OSS LLM generated instruction following data.  <br/>
        /// </summary>
        Firellava13B,
        
        /// <summary>
        /// Claude 3 Opus is Anthropic's most powerful model for highly complex tasks. It boasts top-level performance, intelligence, fluency, and understanding.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3Opus,
        
        /// <summary>
        /// Claude 3 Sonnet is an ideal balance of intelligence and speed for enterprise workloads. Maximum utility at a lower price, dependable, balanced for scaled deployments.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3Sonnet,
        
        /// <summary>
        /// Claude 3 Haiku is Anthropic's fastest and most compact model for  <br/>
        /// near-instant responsiveness. Quick and accurate targeted performance.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3Haiku,
        
        /// <summary>
        /// Claude 2 delivers advancements in key capabilities for enterprises—including an industry-leading 200K token context window, significant reductions in rates of model hallucination, system prompts and a new beta feature: tool use.  <br/>
        /// </summary>
        AnthropicClaudeV2,
        
        /// <summary>
        /// Anthropic's flagship model. Superior performance on tasks that require complex reasoning. Supports hundreds of pages of text.  <br/>
        /// </summary>
        AnthropicClaudeV20,
        
        /// <summary>
        /// Claude 2 delivers advancements in key capabilities for enterprises—including an industry-leading 200K token context window, significant reductions in rates of model hallucination, system prompts and a new beta feature: tool use.  <br/>
        /// </summary>
        AnthropicClaudeV21,
        
        /// <summary>
        /// Anthropic's model for low-latency, high throughput text generation. Supports hundreds of pages of text.  <br/>
        /// </summary>
        AnthropicClaudeInstantV1,
        
        /// <summary>
        /// This is a lower-latency version of Claude 3 Opus, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Claude 3 Opus is Anthropic's most powerful model for highly complex tasks. It boasts top-level performance, intelligence, fluency, and understanding.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3OpusSelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude 3 Sonnet, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Claude 3 Sonnet is an ideal balance of intelligence and speed for enterprise workloads. Maximum utility at a lower price, dependable, balanced for scaled deployments.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3SonnetSelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude 3 Haiku, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Claude 3 Haiku is Anthropic's fastest and most compact model for  <br/>
        /// near-instant responsiveness. Quick and accurate targeted performance.  <br/>
        /// See the launch announcement and benchmark results here  <br/>
        /// #multimodal  <br/>
        /// </summary>
        AnthropicClaude3HaikuSelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude v2, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Claude 2 delivers advancements in key capabilities for enterprises—including an industry-leading 200K token context window, significant reductions in rates of model hallucination, system prompts and a new beta feature: tool use.  <br/>
        /// </summary>
        AnthropicClaudeV2SelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude v2.0, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Anthropic's flagship model. Superior performance on tasks that require complex reasoning. Supports hundreds of pages of text.  <br/>
        /// </summary>
        AnthropicClaudeV20SelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude v2.1, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Claude 2 delivers advancements in key capabilities for enterprises—including an industry-leading 200K token context window, significant reductions in rates of model hallucination, system prompts and a new beta feature: tool use.  <br/>
        /// </summary>
        AnthropicClaudeV21SelfModerated,
        
        /// <summary>
        /// This is a lower-latency version of Claude Instant v1, made available in collaboration with Anthropic, that is self-moderated: response moderation happens on the model's side instead of OpenRouter's. It's in beta, and may change in the future.  <br/>
        /// Anthropic's model for low-latency, high throughput text generation. Supports hundreds of pages of text.  <br/>
        /// </summary>
        AnthropicClaudeInstantV1SelfModerated,
        
        /// <summary>
        /// A 13 billion parameter language model from Meta, fine tuned for chat completions  <br/>
        /// </summary>
        MetaLlamaV213BChat,
        
        /// <summary>
        /// The flagship, 70 billion parameter language model from Meta, fine tuned for chat completions. Llama 2 is an auto-regressive language model that uses an optimized transformer architecture. The tuned versions use supervised fine-tuning (SFT) and reinforcement learning with human feedback (RLHF) to align to human preferences for helpfulness and safety.  <br/>
        /// </summary>
        MetaLlamaV270BChat,
        
        /// <summary>
        /// A state-of-the-art language model fine-tuned on over 300k instructions by Nous Research, with Teknium and Emozilla leading the fine tuning process.  <br/>
        /// </summary>
        NousHermes13B,
        
        /// <summary>
        /// This model is trained on the Yi-34B model for 3 epochs on the Capybara dataset. It's the first 34B Nous model and first 200K context length Nous model.  <br/>
        /// Note: This endpoint currently supports 32k context.  <br/>
        /// </summary>
        NousCapybara34B,
        
        /// <summary>
        /// A Llama 2 70B fine-tune using synthetic data (the Airoboros dataset).  <br/>
        /// Currently based on jondurbin/airoboros-l2-70b-2.2.1, but might get updated in the future.  <br/>
        /// </summary>
        Airoboros70B,
        
        /// <summary>
        /// A 75/25 merge of Chronos 13b v2 and Nous Hermes Llama2 13b. This offers the imaginative writing style of Chronos while retaining coherency. Outputs are long and use exceptional prose. #merge  <br/>
        /// </summary>
        ChronosHermes13BV2,
        
        /// <summary>
        /// A 7.3B parameter model that outperforms Llama 2 13B on all benchmarks, with optimizations for speed and context length.  <br/>
        /// This is v0.1 of Mistral 7B Instruct. For v0.2, use this model.  <br/>
        /// </summary>
        Mistral7BInstruct,
        
        /// <summary>
        /// A continuation of OpenHermes 2 model, trained on additional code datasets.  <br/>
        /// Potentially the most interesting finding from training on a good ratio (est. of around 7-14% of the total dataset) of code instruction was that it has boosted several non-code benchmarks, including TruthfulQA, AGIEval, and GPT4All suite. It did however reduce BigBench benchmark score, but the net gain overall is significant.  <br/>
        /// </summary>
        OpenHermes25Mistral7B,
        
        /// <summary>
        /// OpenChat is a library of open-source language models, fine-tuned with "C-RLFT (Conditioned Reinforcement Learning Fine-Tuning)" - a strategy inspired by offline reinforcement learning. It has been trained on mixed-quality data without preference labels.  <br/>
        /// </summary>
        OpenChat35,
        
        /// <summary>
        /// A wild 7B parameter model that merges several models using the new task_arithmetic merge method from mergekit.  <br/>
        /// List of merged models:  <br/>
        /// NousResearch/Nous-Capybara-7B-V1.9  <br/>
        /// HuggingFaceH4/zephyr-7b-beta  <br/>
        /// lemonilia/AshhLimaRP-Mistral-7B  <br/>
        /// Vulkane/120-Days-of-Sodom-LoRA-Mistral-7b  <br/>
        /// Undi95/Mistral-pippa-sharegpt-7b-qlora  <br/>
        /// #merge #uncensored  <br/>
        /// </summary>
        ToppyM7B,
        
        /// <summary>
        /// A Mythomax/MLewd_13B-style merge of selected 70B models.  <br/>
        /// A multi-model merge of several LLaMA2 70B finetunes for roleplaying and creative work. The goal was to create a model that combines creativity with intelligence for an enhanced experience.  <br/>
        /// #merge #uncensored  <br/>
        /// </summary>
        Lzlv70B,
        
        /// <summary>
        /// A pretrained generative Sparse Mixture of Experts, by Mistral AI, for chat and instruction use. Incorporates 8 experts (feed-forward networks) for a total of 47 billion parameters.  <br/>
        /// Instruct model fine-tuned by Mistral. #moe  <br/>
        /// </summary>
        Mixtral8X7BInstruct,
        
        /// <summary>
        /// This is a 16k context fine-tune of Mixtral-8x7b. It excels in coding tasks due to extensive training with coding data and is known for its obedience, although it lacks DPO tuning.  <br/>
        /// The model is uncensored and is stripped of alignment and bias. It requires an external alignment layer for ethical use. Users are cautioned to use this highly compliant model responsibly, as detailed in a blog post about uncensored models at erichartford.com/uncensored-models.  <br/>
        /// #moe #uncensored  <br/>
        /// </summary>
        Dolphin26Mixtral8X7B,
        
        /// <summary>
        /// This model was trained for 8h(v1) + 8h(v2) + 12h(v3) on customized modified datasets, focusing on RP, uncensoring, and a modified version of the Alpaca prompting (that was already used in LimaRP), which should be at the same conversational level as ChatLM or Llama2-Chat without adding any additional special tokens.  <br/>
        /// </summary>
        NoromaidMixtral8X7BInstruct,
        
        /// <summary>
        /// Nous Hermes 2 Mixtral 8x7B DPO is the new flagship Nous Research model trained over the Mixtral 8x7B MoE LLM.  <br/>
        /// The model was trained on over 1,000,000 entries of primarily GPT-4 generated data, as well as other high quality data from open datasets across the AI landscape, achieving state of the art performance on a variety of tasks.  <br/>
        /// #moe  <br/>
        /// </summary>
        NousHermes2Mixtral8X7BDpo,
        
        /// <summary>
        /// RWKV is an RNN (recurrent neural network) with transformer-level performance. It aims to combine the best of RNNs and transformers - great performance, fast inference, low VRAM, fast training, "infinite" context length, and free sentence embedding.  <br/>
        /// RWKV-5 is trained on 100+ world languages (70% English, 15% multilang, 15% code).  <br/>
        /// RWKV 3B models are provided for free, by Recursal.AI, for the beta period. More details here.  <br/>
        /// #rnn  <br/>
        /// </summary>
        RwkvV5World3B,
        
        /// <summary>
        /// This is an RWKV 3B model finetuned specifically for the AI Town project.  <br/>
        /// RWKV is an RNN (recurrent neural network) with transformer-level performance. It aims to combine the best of RNNs and transformers - great performance, fast inference, low VRAM, fast training, "infinite" context length, and free sentence embedding.  <br/>
        /// RWKV 3B models are provided for free, by Recursal.AI, for the beta period. More details here.  <br/>
        /// #rnn  <br/>
        /// </summary>
        RwkvV53BAiTown,
        
        /// <summary>
        /// Eagle 7B is trained on 1.1 Trillion Tokens across 100+ world languages (70% English, 15% multilang, 15% code).  <br/>
        /// Built on the RWKV-v5 architecture (a linear transformer with 10-100x+ lower inference cost)  <br/>
        /// Ranks as the world's greenest 7B model (per token)  <br/>
        /// Outperforms all 7B class models in multi-lingual benchmarks  <br/>
        /// Approaches Falcon (1.5T), LLaMA2 (2T), Mistral (>2T?) level of performance in English evals  <br/>
        /// Trade blows with MPT-7B (1T) in English evals  <br/>
        /// All while being an "Attention-Free Transformer"  <br/>
        /// Eagle 7B models are provided for free, by Recursal.AI, for the beta period till end of March 2024  <br/>
        /// Find out more here  <br/>
        /// rnn  <br/>
        /// </summary>
        RwkvV5Eagle7B,
        
        /// <summary>
        /// Gemma by Google is an advanced, open-source language model family, leveraging the latest in decoder-only, text-to-text technology. It offers English language capabilities across text generation tasks like question answering, summarization, and reasoning. The Gemma 7B variant is comparable in performance to leading open source models.  <br/>
        /// Usage of Gemma is subject to Google's Gemma Terms of Use.  <br/>
        /// </summary>
        GoogleGemma7B,
        
        /// <summary>
        /// DBRX is a new open source large language model developed by Databricks. At 132B, it outperforms existing open source LLMs like Llama 2 70B and Mixtral-8x7B on standard industry benchmarks for language understanding, programming, math, and logic.  <br/>
        /// It uses a fine-grained mixture-of-experts (MoE) architecture. 36B parameters are active on any input. It was pre-trained on 12T tokens of text and code data. Compared to other open MoE models like Mixtral-8x7B and Grok-1, DBRX is fine-grained, meaning it uses a larger number of smaller experts.  <br/>
        /// See the launch announcement and benchmark results here.  <br/>
        /// #moe  <br/>
        /// </summary>
        DatabricksDbrx132BInstruct,
        
        /// <summary>
        /// Zephyr 141B-A35B is A Mixture of Experts (MoE) model with 141B total parameters and 35B active parameters. Fine-tuned on a mix of publicly available, synthetic datasets.  <br/>
        /// It is an instruct finetune of Mixtral 8x22B.  <br/>
        /// #moe  <br/>
        /// </summary>
        Zephyr141BA35b,
        
        /// <summary>
        /// Meta's latest class of model (Llama 3) launched with a variety of sizes and flavors. This 8B instruct-tuned version was optimized for high quality dialogue usecases.  <br/>
        /// It has demonstrated strong performance compared to leading closed-source models in human evaluations.  <br/>
        /// To read more about the model release, click here. Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// </summary>
        MetaLlama38BInstruct,
        
        /// <summary>
        /// WizardLM-2 8x22B is Microsoft AI's most advanced Wizard model. It demonstrates highly competitive performance compared to leading proprietary models, and it consistently outperforms all existing state-of-the-art opensource models.  <br/>
        /// It is an instruct finetune of Mixtral 8x22B.  <br/>
        /// To read more about the model release, click here.  <br/>
        /// #moe  <br/>
        /// </summary>
        Wizardlm28X22b,
        
        /// <summary>
        /// WizardLM-2 7B is the smaller variant of Microsoft AI's latest Wizard model. It is the fastest and achieves comparable performance with existing 10x larger opensource leading models  <br/>
        /// It is a finetune of Mistral 7B Instruct, using the same technique as WizardLM-2 8x22B.  <br/>
        /// To read more about the model release, click here.  <br/>
        /// #moe  <br/>
        /// </summary>
        Wizardlm27B,
        
        /// <summary>
        /// Mixtral 8x22B is a large-scale language model from Mistral AI. It consists of 8 experts, each 22 billion parameters, with each token using 2 experts at a time.  <br/>
        /// It was released via X.  <br/>
        /// #moe  <br/>
        /// </summary>
        MistralMixtral8X22BBase,
        
        /// <summary>
        /// Mistral's official instruct fine-tuned version of Mixtral 8x22B. It uses 39B active parameters out of 141B, offering unparalleled cost efficiency for its size. Its strengths include:  <br/>
        /// strong math, coding, and reasoning  <br/>
        /// large context length (64k)  <br/>
        /// fluency in English, French, Italian, German, and Spanish  <br/>
        /// See benchmarks on the launch announcement here.  <br/>
        /// #moe  <br/>
        /// </summary>
        MistralMixtral8X22bInstruct,
        
        /// <summary>
        /// Soliloquy-L3 is a fast, highly capable roleplaying model designed for immersive, dynamic experiences. Trained on over 250 million tokens of roleplaying data, Soliloquy-L3 has a vast knowledge base, rich literary expression, and support for up to 24k context length. It outperforms existing ~13B models, delivering enhanced roleplaying capabilities.  <br/>
        /// Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// </summary>
        LynnLlama3Soliloquy8B,
        
        /// <summary>
        /// Zephyr is a series of language models that are trained to act as helpful assistants. Zephyr-7B-β is the second model in the series, and is a fine-tuned version of mistralai/Mistral-7B-v0.1 that was trained on a mix of publicly available, synthetic datasets using Direct Preference Optimization (DPO).  <br/>
        /// Note: this is a free, rate-limited version of this model. Outputs may be cached. Read about rate limits here.  <br/>
        /// </summary>
        HuggingFaceZephyr7BFree,
        
        /// <summary>
        /// The flagship, 70 billion parameter language model from Meta, fine tuned for chat completions. Llama 2 is an auto-regressive language model that uses an optimized transformer architecture. The tuned versions use supervised fine-tuning (SFT) and reinforcement learning with human feedback (RLHF) to align to human preferences for helpfulness and safety.  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        MetaLlamaV270BChatNitro,
        
        /// <summary>
        /// One of the highest performing and most popular fine-tunes of Llama 2 13B, with rich descriptions and roleplay. #merge  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        Mythomax13BNitro,
        
        /// <summary>
        /// A 7.3B parameter model that outperforms Llama 2 13B on all benchmarks, with optimizations for speed and context length.  <br/>
        /// This is v0.2 of Mistral 7B Instruct. For v0.1, use this model.  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        Mistral7BInstructNitro,
        
        /// <summary>
        /// Gemma by Google is an advanced, open-source language model family, leveraging the latest in decoder-only, text-to-text technology. It offers English language capabilities across text generation tasks like question answering, summarization, and reasoning. The Gemma 7B variant is comparable in performance to leading open source models.  <br/>
        /// Usage of Gemma is subject to Google's Gemma Terms of Use.  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        GoogleGemma7BNitro,
        
        /// <summary>
        /// A wild 7B parameter model that merges several models using the new task_arithmetic merge method from mergekit.  <br/>
        /// List of merged models:  <br/>
        /// NousResearch/Nous-Capybara-7B-V1.9  <br/>
        /// HuggingFaceH4/zephyr-7b-beta  <br/>
        /// lemonilia/AshhLimaRP-Mistral-7B  <br/>
        /// Vulkane/120-Days-of-Sodom-LoRA-Mistral-7b  <br/>
        /// Undi95/Mistral-pippa-sharegpt-7b-qlora  <br/>
        /// #merge #uncensored  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        ToppyM7BNitro,
        
        /// <summary>
        /// WizardLM-2 8x22B is Microsoft AI's most advanced Wizard model. It demonstrates highly competitive performance compared to leading proprietary models, and it consistently outperforms all existing state-of-the-art opensource models.  <br/>
        /// It is an instruct finetune of Mixtral 8x22B.  <br/>
        /// To read more about the model release, click here.  <br/>
        /// #moe  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        Wizardlm28X22bNitro,
        
        /// <summary>
        /// Meta's latest class of model (Llama 3) launched with a variety of sizes and flavors. This 8B instruct-tuned version was optimized for high quality dialogue usecases.  <br/>
        /// It has demonstrated strong performance compared to leading closed-source models in human evaluations.  <br/>
        /// To read more about the model release, click here. Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        MetaLlama38BInstructNitro,
        
        /// <summary>
        /// Meta's latest class of model (Llama 3) launched with a variety of sizes and flavors. This 70B instruct-tuned version was optimized for high quality dialogue usecases.  <br/>
        /// It has demonstrated strong performance compared to leading closed-source models in human evaluations.  <br/>
        /// To read more about the model release, click here. Usage of this model is subject to Meta's Acceptable Use Policy.  <br/>
        /// Note: this is a higher-throughput version of this model, and may have higher prices and slightly different outputs.  <br/>
        /// </summary>
        MetaLlama370BInstructNitro,
        
        /// <summary>
        /// LLaVA is a large multimodal model that combines a vision encoder and Vicuna for general-purpose visual and language understanding, achieving impressive chat capabilities mimicking GPT-4 and setting a new state-of-the-art accuracy on Science QA  <br/>
        /// #multimodal  <br/>
        /// </summary>
        Llava13B,
        
        /// <summary>
        /// This vision-language model builds on innovations from the popular OpenHermes-2.5 model, by Teknium. It adds vision support, and is trained on a custom dataset enriched with function calling  <br/>
        /// This project is led by qnguyen3 and teknium.  <br/>
        /// #multimodal  <br/>
        /// </summary>
        NousHermes2Vision7BAlpha,
        
        /// <summary>
        /// This model is currently powered by Mistral-7B-v0.2, and incorporates a "better" fine-tuning than Mistral 7B, inspired by community work. It's best used for large batch processing tasks where cost is a significant factor but reasoning capabilities are not crucial.  <br/>
        /// </summary>
        MistralTiny,
        
        /// <summary>
        /// This model is currently powered by Mixtral-8X7B-v0.1, a sparse mixture of experts model with 12B active parameters. It has better reasoning, exhibits more capabilities, can produce and reason about code, and is multiligual, supporting English, French, German, Italian, and Spanish.  <br/>
        /// #moe  <br/>
        /// </summary>
        MistralSmall,
        
        /// <summary>
        /// This is Mistral AI's closed-source, medium-sided model. It's powered by a closed-source prototype and excels at reasoning, code, JSON, chat, and more. In benchmarks, it compares with many of the flagship models of other companies.  <br/>
        /// </summary>
        MistralMedium,
        
        /// <summary>
        /// This is Mistral AI's closed-source, flagship model. It's powered by a closed-source prototype and excels at reasoning, code, JSON, chat, and more. Read the launch announcement here.  <br/>
        /// It is fluent in English, French, Spanish, German, and Italian, with high grammatical accuracy, and its 32K tokens context window allows precise information recall from large documents.  <br/>
        /// </summary>
        MistralLarge,
        
        /// <summary>
        /// Command is an instruction-following conversational model that performs language tasks with high quality, more reliably and with a longer context than our base generative models.  <br/>
        /// Use of this model is subject to Cohere's Acceptable Use Policy.  <br/>
        /// </summary>
        CohereCommand,
        
        /// <summary>
        /// Command-R is a 35B parameter model that performs conversational language tasks at a higher quality, more reliably, and with a longer context than previous models. It can be used for complex workflows like code generation, retrieval augmented generation (RAG), tool use, and agents.  <br/>
        /// Read the launch post here.  <br/>
        /// Use of this model is subject to Cohere's Acceptable Use Policy.  <br/>
        /// </summary>
        CohereCommandR,
        
        /// <summary>
        /// Command R+ is a new, 104B-parameter LLM from Cohere. It's useful for roleplay, general consumer usecases, and Retrieval Augmented Generation (RAG).  <br/>
        /// It offers multilingual support for ten key languages to facilitate global business operations. See benchmarks and the launch post here.  <br/>
        /// Use of this model is subject to Cohere's Acceptable Use Policy.  <br/>
        /// </summary>
        CohereCommandRPlus,
        
}