using Domain.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ChatBuilder
{
    public class AIServiceClass : IAIService
    {
        private readonly OpenAIOptions _options;
        private readonly OpenAIClient _client;

        public AIServiceClass(IOptions<OpenAIOptions> options)
        {            
            _options = options.Value;
            _client = new OpenAIClient(_options.ApiKey);
        } // constructor...

        public async Task<string> AskAsync(string message)
        {
            var chatClient = _client.GetChatClient(_options.Model);

            var response = await chatClient.CompleteChatAsync(message);

            string answer = response.Value.Content[0].Text;
            return answer;
        } // AskAsync...
    } // class...
}
