using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OpenAI
{
    public class OpenAIOptions
    {
        public const string SectionName = "OpenAI";
        public string ApiKey { get; set; } = string.Empty;
        public string Model {  get; set; } = string.Empty;
    } // class...
}
