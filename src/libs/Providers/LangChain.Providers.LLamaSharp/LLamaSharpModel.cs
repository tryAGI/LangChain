using System.Diagnostics;
using LLama;
using System.Reflection;

namespace LangChain.Providers.LLamaSharp
{
    public class LLamaSharpModel : IChatModel
    {
        private readonly LLamaSharpConfiguration _configuration;
        private readonly LLamaModel _model;
        public string Id { get; }
        public Usage TotalUsage { get; private set; }
        public int ContextLength =>_configuration.ContextSize;

        public LLamaSharpModel(LLamaSharpConfiguration configuration)
        {
            _configuration = configuration;
            Id = Path.GetFileNameWithoutExtension(configuration.PathToModelFile);

            _model = new LLamaModel(
                new LLamaParams(model: configuration.PathToModelFile,
                    interactive: true,
                    instruct: configuration.Mode==ELLamaSharpModelMode.Instruction,
                    temp: configuration.Temperature,
                    n_ctx: configuration.ContextSize, repeat_penalty: 1.0f));
        }

        string ConvertRole(MessageRole role)
        {
            return role switch
            {
                MessageRole.Human => "Human: ",
                MessageRole.Ai => "Assistant: ",
                MessageRole.System => "",
                _ => throw new NotSupportedException($"the role {role} is not supported")
            };
        }

        string ConvertMessage(Message message)
        {
            return $"{ConvertRole(message.Role)}{message.Content}";
        }

        
        
        

        public Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default)
        {
            // take all messages except the last one
            // and make them a prompt
            var messagesArray = request.Messages.ToArray();
            var prePromptMessages = messagesArray.Take(messagesArray.Length-1).ToList();
            
            
            
            // use the last message as input
            string input;
            if(prePromptMessages.Count>0)
                input = request.Messages.Last().Content;
            else
                input = ConvertMessage(request.Messages.Last());

            var watch = Stopwatch.StartNew();

            IEnumerable<string> response;


            var session = CreateSession(prePromptMessages);
            response = session.Chat(input + "\n");
            string buf = "";
            foreach (var message in response)
            {
                buf += message;
                if (_configuration.Mode == ELLamaSharpModelMode.Instruction)
                {
                    if (buf.EndsWith("###"))
                    {
                        buf = buf.Substring(0, buf.Length - 3);
                        break;
                    }
                        
                }
            }
            
            var output = SanitizeOutput(buf);

            var result = request.Messages.ToList();

            switch (_configuration.Mode)
            {
                case ELLamaSharpModelMode.Chat:
                    result.Add(output.AsAiMessage());
                    break;
                case ELLamaSharpModelMode.Instruction:
                    result.Add(output.AsSystemMessage());
                    break;
            }


            watch.Stop();

            // Unsupported
            var usage = Usage.Empty with
            {
                Time = watch.Elapsed,
            };
            TotalUsage += usage;

            return Task.FromResult(new ChatResponse(
                Messages: result,
                Usage: usage));
        }

        private static string SanitizeOutput(string output)
        {
            output = output.Replace("\nHuman:", "");
            output = output.Replace("Assistant:", "");
            output = output.Trim();
            return output;
        }

        private ChatSession<LLamaModel> CreateSession(List<Message> preprompt)
        {
            var res = new ChatSession<LLamaModel>(_model);

            if (_configuration.Mode == ELLamaSharpModelMode.Chat)
                res = res.WithAntiprompt(new[] { "Human:" });

            if (preprompt.Count > 0)
            {
                preprompt.Add("".AsHumanMessage());

                var prompt = string.Join(
                    "\n", preprompt.Select(ConvertMessage).ToArray());

                res = res
                    .WithPrompt(prompt);
            }


            return res;
        }
    }
}