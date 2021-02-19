using Discord;
using Discord.Webhook;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Helpers
{
    public static class DiscordHelper
    {
        public static DiscordWebhookClient InitWebhookClient(string webhookUrl)
        {
            var client = new DiscordWebhookClient(webhookUrl);

            return client;
        }

        public async static void SendMessage(Account acc, string text, Embed[] embeds = null)
        {
            await acc.WebhookClient.SendMessageAsync(text: text, embeds: embeds);
        }
    }
}