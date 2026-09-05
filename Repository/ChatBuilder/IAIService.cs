using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ChatBuilder
{
    public interface IAIService
    {
        Task<string> AskAsync(string message);
    } // interface...
}
