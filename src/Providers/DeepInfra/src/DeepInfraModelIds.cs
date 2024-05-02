namespace LangChain.Providers.DeepInfra;

/// <summary>
/// List of all the Predefined DeepInfra Models
/// </summary>
public enum DeepInfraModelIds
{

    /// <summary>
    /// Name: Meta-Llama-3-70B-Instruct <br/>
    /// Organization: meta-llama <br/>
    /// Context Length: 8192 <br/>
    /// Prompt Cost: $0.59/MTok <br/>
    /// Completion Cost: $0.59/MTok <br/>
    /// Description: Model Details Meta developed and released the Meta Llama 3 family of large language models (LLMs), a collection of pretrained and instruction tuned generative text models in 8 and 70B sizes. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/meta-llama/Meta-Llama-3-70B-Instruct">https://huggingface.co/meta-llama/Meta-Llama-3-70B-Instruct</a> 
    /// </summary>
    MetaLlama370BInstruct,

    /// <summary>
    /// Name: Meta-Llama-3-8B-Instruct <br/>
    /// Organization: meta-llama <br/>
    /// Context Length: 8192 <br/>
    /// Prompt Cost: $0.08/MTok <br/>
    /// Completion Cost: $0.08/MTok <br/>
    /// Description:  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/meta-llama/Meta-Llama-3-8B-Instruct">https://huggingface.co/meta-llama/Meta-Llama-3-8B-Instruct</a> 
    /// </summary>
    MetaLlama38BInstruct,

    /// <summary>
    /// Name: Mixtral-8x22B-Instruct-v0.1 <br/>
    /// Organization: mistralai <br/>
    /// Context Length: 65536 <br/>
    /// Prompt Cost: $0.65/MTok <br/>
    /// Completion Cost: $0.65/MTok <br/>
    /// Description: This is the instruction fine-tuned version of Mixtral-8x22B - the latest and largest mixture of experts large language model (LLM) from Mistral AI. This state of the art machine learning model uses a mixture 8 of experts (MoE) 22b models. During inference 2 experts are selected. This architecture allows large models to be fast and cheap at inference. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/mistralai/Mixtral-8x22B-Instruct-v0.1">https://huggingface.co/mistralai/Mixtral-8x22B-Instruct-v0.1</a> 
    /// </summary>
    Mixtral8X22bInstructV01,

    /// <summary>
    /// Name: WizardLM-2-8x22B <br/>
    /// Organization: microsoft <br/>
    /// Context Length: 65536 <br/>
    /// Prompt Cost: $0.65/MTok <br/>
    /// Completion Cost: $0.65/MTok <br/>
    /// Description: WizardLM-2 8x22B is Microsoft AI's most advanced Wizard model. It demonstrates highly competitive performance compared to those leading proprietary models. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/microsoft/WizardLM-2-8x22B">https://huggingface.co/microsoft/WizardLM-2-8x22B</a> 
    /// </summary>
    Wizardlm28X22b,

    /// <summary>
    /// Name: WizardLM-2-7B <br/>
    /// Organization: microsoft <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.07/MTok <br/>
    /// Completion Cost: $0.07/MTok <br/>
    /// Description: WizardLM-2 7B is the smaller variant of Microsoft AI's latest Wizard model. It is the fastest and achieves comparable performance with existing 10x larger open-source leading models <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/microsoft/WizardLM-2-7B">https://huggingface.co/microsoft/WizardLM-2-7B</a> 
    /// </summary>
    Wizardlm27B,

    /// <summary>
    /// Name: gemma-1.1-7b-it <br/>
    /// Organization: google <br/>
    /// Context Length: 8192 <br/>
    /// Prompt Cost: $0.07/MTok <br/>
    /// Completion Cost: $0.07/MTok <br/>
    /// Description: Gemma is an open-source model designed by Google. This is Gemma 1.1 7B (IT), an update over the original instruction-tuned Gemma release. Gemma 1.1 was trained using a novel RLHF method, leading to substantial gains on quality, coding capabilities, factuality, instruction following and multi-turn conversation quality. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/google/gemma-1.1-7b-it">https://huggingface.co/google/gemma-1.1-7b-it</a> 
    /// </summary>
    Gemma117BIt,

    /// <summary>
    /// Name: dbrx-instruct <br/>
    /// Organization: databricks <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.6/MTok <br/>
    /// Completion Cost: $0.6/MTok <br/>
    /// Description: DBRX is an open source LLM created by Databricks. It uses mixture-of-experts (MoE) architecture with 132B total parameters of which 36B parameters are active on any input. It outperforms existing open source LLMs like Llama 2 70B and Mixtral-8x7B on standard industry benchmarks for language understanding, programming, math, and logic. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/databricks/dbrx-instruct">https://huggingface.co/databricks/dbrx-instruct</a> 
    /// </summary>
    DbrxInstruct,

    /// <summary>
    /// Name: Mixtral-8x7B-Instruct-v0.1 <br/>
    /// Organization: mistralai <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.24/MTok <br/>
    /// Completion Cost: $0.24/MTok <br/>
    /// Description: Mixtral is mixture of expert large language model (LLM) from Mistral AI. This is state of the art machine learning model using a mixture 8 of experts (MoE) 7b models. During inference 2 expers are selected. This architecture allows large models to be fast and cheap at inference. The Mixtral-8x7B outperforms Llama 2 70B on most benchmarks. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/mistralai/Mixtral-8x7B-Instruct-v0.1">https://huggingface.co/mistralai/Mixtral-8x7B-Instruct-v0.1</a> 
    /// </summary>
    Mixtral8X7BInstructV01,

    /// <summary>
    /// Name: Mistral-7B-Instruct-v0.2 <br/>
    /// Organization: mistralai <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.07/MTok <br/>
    /// Completion Cost: $0.07/MTok <br/>
    /// Description: The Mistral-7B-Instruct-v0.2 Large Language Model (LLM) is a instruct fine-tuned version of the Mistral-7B-v0.2 generative text model using a variety of publicly available conversation datasets. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.2">https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.2</a> 
    /// </summary>
    Mistral7BInstructV02,

    /// <summary>
    /// Name: Llama-2-70b-chat-hf <br/>
    /// Organization: meta-llama <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.64/MTok <br/>
    /// Completion Cost: $0.64/MTok <br/>
    /// Description: LLaMa 2 is a collections of LLMs trained by Meta. This is the 70B chat optimized version. This endpoint has per token pricing. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/meta-llama/Llama-2-70b-chat-hf">https://huggingface.co/meta-llama/Llama-2-70b-chat-hf</a> 
    /// </summary>
    Llama270BChatHf,

    /// <summary>
    /// Name: dolphin-2.6-mixtral-8x7b <br/>
    /// Organization: cognitivecomputations <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.24/MTok <br/>
    /// Completion Cost: $0.24/MTok <br/>
    /// Description: The Dolphin 2.6 Mixtral 8x7b model is a finetuned version of the Mixtral-8x7b model, trained on a variety of data including coding data, for 3 days on 4 A100 GPUs. It is uncensored and requires trust_remote_code. The model is very obedient and good at coding, but not DPO tuned. The dataset has been filtered for alignment and bias. The model is compliant with user requests and can be used for various purposes such as generating code or engaging in general chat. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/cognitivecomputations/dolphin-2.6-mixtral-8x7b">https://huggingface.co/cognitivecomputations/dolphin-2.6-mixtral-8x7b</a> 
    /// </summary>
    Dolphin26Mixtral8X7B,

    /// <summary>
    /// Name: lzlv_70b_fp16_hf <br/>
    /// Organization: lizpreciatior <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.59/MTok <br/>
    /// Completion Cost: $0.59/MTok <br/>
    /// Description: A Mythomax/MLewd_13B-style merge of selected 70B models  A multi-model merge of several  LLaMA2 70B finetunes for roleplaying and creative work. The goal was to create a model that combines creativity with intelligence for an enhanced experience. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/lizpreciatior/lzlv_70b_fp16_hf">https://huggingface.co/lizpreciatior/lzlv_70b_fp16_hf</a> 
    /// </summary>
    Lzlv70BFp16Hf,

    /// <summary>
    /// Name: openchat_3.5 <br/>
    /// Organization: openchat <br/>
    /// Context Length: 8192 <br/>
    /// Prompt Cost: $0.1/MTok <br/>
    /// Completion Cost: $0.1/MTok <br/>
    /// Description: OpenChat is a library of open-source language models that have been fine-tuned with C-RLFT, a strategy inspired by offline reinforcement learning. These models can learn from mixed-quality data without preference labels and have achieved exceptional performance comparable to ChatGPT. The developers of OpenChat are dedicated to creating a high-performance, commercially viable, open-source large language model and are continuously making progress towards this goal. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/openchat/openchat_3.5">https://huggingface.co/openchat/openchat_3.5</a> 
    /// </summary>
    OpenChat35,

    /// <summary>
    /// Name: llava-1.5-7b-hf <br/>
    /// Organization: llava-hf <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.34/MTok <br/>
    /// Completion Cost: $0.34/MTok <br/>
    /// Description: LLaVa is a multimodal model that supports vision and language models combined. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/llava-hf/llava-1.5-7b-hf">https://huggingface.co/llava-hf/llava-1.5-7b-hf</a> 
    /// </summary>
    Llava157BHf,

    /// <summary>
    /// Name: airoboros-70b <br/>
    /// Organization: deepinfra <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.7/MTok <br/>
    /// Completion Cost: $0.7/MTok <br/>
    /// Description: Latest version of the Airoboros model fine-tunned version of llama-2-70b using the Airoboros dataset. This model is currently running jondurbin/airoboros-l2-70b-2.2.1  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/deepinfra/airoboros-70b">https://huggingface.co/deepinfra/airoboros-70b</a> 
    /// </summary>
    Airoboros70B,

    /// <summary>
    /// Name: Llama-2-7b-chat-hf <br/>
    /// Organization: meta-llama <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.07/MTok <br/>
    /// Completion Cost: $0.07/MTok <br/>
    /// Description: Llama 2 is a collection of pretrained and fine-tuned generative text models ranging in scale from 7 billion to 70 billion parameters. This is the repository for the 7B fine-tuned model, optimized for dialogue use cases and converted for the Hugging Face Transformers format.  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/meta-llama/Llama-2-7b-chat-hf">https://huggingface.co/meta-llama/Llama-2-7b-chat-hf</a> 
    /// </summary>
    Llama27BChatHf,

    /// <summary>
    /// Name: Yi-34B-Chat <br/>
    /// Organization: 01-ai <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.6/MTok <br/>
    /// Completion Cost: $0.6/MTok <br/>
    /// Description:  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/01-ai/Yi-34B-Chat">https://huggingface.co/01-ai/Yi-34B-Chat</a> 
    /// </summary>
    Yi34BChat,

    /// <summary>
    /// Name: chronos-hermes-13b-v2 <br/>
    /// Organization: Austism <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.13/MTok <br/>
    /// Completion Cost: $0.13/MTok <br/>
    /// Description: This offers the imaginative writing style of chronos while still retaining coherency and being capable. Outputs are long and utilize exceptional prose. Supports a maxium context length of 4096. The model follows the Alpaca prompt format. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/Austism/chronos-hermes-13b-v2">https://huggingface.co/Austism/chronos-hermes-13b-v2</a> 
    /// </summary>
    ChronosHermes13BV2,

    /// <summary>
    /// Name: MythoMax-L2-13b <br/>
    /// Organization: Gryphe <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.13/MTok <br/>
    /// Completion Cost: $0.13/MTok <br/>
    /// Description:  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/Gryphe/MythoMax-L2-13b">https://huggingface.co/Gryphe/MythoMax-L2-13b</a> 
    /// </summary>
    MythomaxL213B,

    /// <summary>
    /// Name: zephyr-orpo-141b-A35b-v0.1 <br/>
    /// Organization: HuggingFaceH4 <br/>
    /// Context Length: 65536 <br/>
    /// Prompt Cost: $0.65/MTok <br/>
    /// Completion Cost: $0.65/MTok <br/>
    /// Description: Zephyr 141B-A35B is an instruction-tuned (assistant) version of Mixtral-8x22B. It was fine-tuned on a mix of publicly available, synthetic datasets. It achieves strong performance on chat benchmarks. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/HuggingFaceH4/zephyr-orpo-141b-A35b-v0.1">https://huggingface.co/HuggingFaceH4/zephyr-orpo-141b-A35b-v0.1</a> 
    /// </summary>
    ZephyrOrpo141BA35bV01,

    /// <summary>
    /// Name: Phind-CodeLlama-34B-v2 <br/>
    /// Organization: Phind <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.6/MTok <br/>
    /// Completion Cost: $0.6/MTok <br/>
    /// Description: Phind-CodeLlama-34B-v2 is an open-source language model that has been fine-tuned on 1.5B tokens of high-quality programming-related data and achieved a pass@1 rate of 73.8% on HumanEval. It is multi-lingual and proficient in Python, C/C++, TypeScript, Java, and more. It has been trained on a proprietary dataset of instruction-answer pairs instead of code completion examples.  The model is instruction-tuned on the Alpaca/Vicuna format to be steerable and easy-to-use. It accepts the Alpaca/Vicuna instruction format and can generate one completion for each prompt. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/Phind/Phind-CodeLlama-34B-v2">https://huggingface.co/Phind/Phind-CodeLlama-34B-v2</a> 
    /// </summary>
    PhindCodellama34BV2,

    /// <summary>
    /// Name: starcoder2-15b <br/>
    /// Organization: bigcode <br/>
    /// Context Length: 16384 <br/>
    /// Prompt Cost: $0.4/MTok <br/>
    /// Completion Cost: $0.4/MTok <br/>
    /// Description: StarCoder2-15B model is a 15B parameter model trained on 600+ programming languages. It specializes in code completion. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/bigcode/starcoder2-15b">https://huggingface.co/bigcode/starcoder2-15b</a> 
    /// </summary>
    Starcoder215B,

    /// <summary>
    /// Name: CodeLlama-34b-Instruct-hf <br/>
    /// Organization: codellama <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.6/MTok <br/>
    /// Completion Cost: $0.6/MTok <br/>
    /// Description: Code Llama is a state-of-the-art LLM capable of generating code, and natural language about code, from both code and natural language prompts. This particular instance is the 34b instruct variant <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/codellama/CodeLlama-34b-Instruct-hf">https://huggingface.co/codellama/CodeLlama-34b-Instruct-hf</a> 
    /// </summary>
    Codellama34BInstructHf,

    /// <summary>
    /// Name: CodeLlama-70b-Instruct-hf <br/>
    /// Organization: codellama <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.7/MTok <br/>
    /// Completion Cost: $0.7/MTok <br/>
    /// Description: CodeLlama-70b is the largest and latest code generation from the Code Llama collection.  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/codellama/CodeLlama-70b-Instruct-hf">https://huggingface.co/codellama/CodeLlama-70b-Instruct-hf</a> 
    /// </summary>
    Codellama70BInstructHf,

    /// <summary>
    /// Name: Llama-2-13b-chat-hf <br/>
    /// Organization: meta-llama <br/>
    /// Context Length: 4096 <br/>
    /// Prompt Cost: $0.13/MTok <br/>
    /// Completion Cost: $0.13/MTok <br/>
    /// Description: Llama 2 is a collection of pretrained and fine-tuned generative text models ranging in scale from 7 billion to 70 billion parameters. This is the repository for the 7B fine-tuned model, optimized for dialogue use cases and converted for the Hugging Face Transformers format.  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/meta-llama/Llama-2-13b-chat-hf">https://huggingface.co/meta-llama/Llama-2-13b-chat-hf</a> 
    /// </summary>
    Llama213BChatHf,

    /// <summary>
    /// Name: Mistral-7B-Instruct-v0.1 <br/>
    /// Organization: mistralai <br/>
    /// Context Length: 32768 <br/>
    /// Prompt Cost: $0.07/MTok <br/>
    /// Completion Cost: $0.07/MTok <br/>
    /// Description: The Mistral-7B-Instruct-v0.1 Large Language Model (LLM) is a instruct fine-tuned version of the Mistral-7B-v0.1 generative text model using a variety of publicly available conversation datasets. <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.1">https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.1</a> 
    /// </summary>
    Mistral7BInstructV01,

    /// <summary>
    /// Name: Mixtral-8x22B-v0.1 <br/>
    /// Organization: mistralai <br/>
    /// Context Length: 65536 <br/>
    /// Prompt Cost: $0.65/MTok <br/>
    /// Completion Cost: $0.65/MTok <br/>
    /// Description: Mixtral-8x22B is the latest and largest mixture of expert large language model (LLM) from Mistral AI. This is state of the art machine learning model using a mixture 8 of experts (MoE) 22b models. During inference 2 expers are selected. This architecture allows large models to be fast and cheap at inference.  This model is not instruction tuned.  <br/>
    /// HuggingFace Url: <a href="https://huggingface.co/mistralai/Mixtral-8x22B-v0.1">https://huggingface.co/mistralai/Mixtral-8x22B-v0.1</a> 
    /// </summary>
    Mixtral8X22bV01,

}