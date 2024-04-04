using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Providers.OpenRouter.CodeGenerator.Classes
{
    public class ModelInfo
    {
        public int Index { get; set; }
        public string? ModelId { get; set; }
        public string? ModelName { get; set;}
        public string? Description { get; set; }
        public string? DicAddCode { get; set; }
        public string? PredefinedClassCode { get; set; }
        public string? EnumMemberName { get; set; }
        public string? EnumMemberCode { get; set; }
    }
}
