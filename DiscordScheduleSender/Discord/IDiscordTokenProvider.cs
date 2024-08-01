using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordScheduleSender.Discord
{
    public interface IDiscordTokenProvider
    {
        public string Token { get; }
        public Dictionary<long, string> WebhookUrls { get; }
    }
}
