using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordScheduleSender.Discord
{
    public class DiscordBot
    {
        public IDiscordTokenProvider? DiscordToken { get; private set; }
        public DiscordSocketClient? Client { get; protected set; }

        public async Task MainAsync()
        {
            Client = new DiscordSocketClient();

            Client.Log += Log;
            Client.MessageReceived += MessageReceivedAsync;
            Client.InteractionCreated += InteractionCreatedAsync;

            DiscordToken = new SecretDiscordToken();

            await Client.LoginAsync(TokenType.Bot, DiscordToken.Token);
            await Client.StartAsync();
        }

        private static Task Log(LogMessage msg)
        {
            Debug.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            try
            {
                // The bot should never respond to itself.
                if (message.Author.Id == Client.CurrentUser.Id)
                    return;

                Debug.WriteLine(message.Content);

                if (message.Content == "!ping")
                {
                    //// Create a new ComponentBuilder, in which dropdowns & buttons can be created.
                    //var cb = new ComponentBuilder()
                    //    .WithButton("Click me!", "unique-id", ButtonStyle.Primary);

                    //// Send a message with content 'pong', including a button.
                    //// This button needs to be build by calling .Build() before being passed into the call.
                    //await message.Channel.SendMessageAsync("pong!", components: cb.Build());

                    Debug.WriteLine($"{message.Channel.Id}");
                    await message.Channel.SendMessageAsync($"{message.Channel.Id}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private async Task InteractionCreatedAsync(SocketInteraction interaction)
        {
            // safety-casting is the best way to prevent something being cast from being null.
            // If this check does not pass, it could not be cast to said type.
            if (interaction is SocketMessageComponent component)
            {
                // Check for the ID created in the button mentioned above.
                if (component.Data.CustomId == "unique-id")
                    await interaction.RespondAsync("Thank you for clicking my button!");

                else
                    Console.WriteLine("An ID has been received that has no handler!");
            }
        }

        public async Task<ulong> SendDiscordWebhook(string url, string message)
        {
            var webhook = new DiscordWebhookClient(url);
            var messageID = await webhook.SendMessageAsync(message);

            return messageID;
        }

        public async Task ModifyDiscordWebhook(string url, string message, ulong messageId)
        {
            var webhook = new DiscordWebhookClient(url);
            await webhook.ModifyMessageAsync(messageId, msg => msg.Content = message);
        }
    }
}
