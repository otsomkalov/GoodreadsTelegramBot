using System;
using System.Linq;
using System.Threading.Tasks;
using GoodreadsTelegramBot.Helpers;
using GoodreadsTelegramBot.Models;
using RestSharp;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace GoodreadsTelegramBot
{
    internal static class Program
    {
        private static TelegramBotClient _bot;
        private static IRestClient _restClient;
        private static ILogger _logger;

        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("You need to supply bot token and API code");

                return;
            }

            _logger = Configuration.ConfigureLogger();

            _bot = new TelegramBotClient(args[0]);

            _restClient = new RestClient("https://www.goodreads.com/search/index.xml")
                .AddDefaultQueryParameter("key", args[1])
                .AddDefaultQueryParameter("search", "title");

            _bot.OnInlineQuery += OnInlineQueryAsync;
            _bot.OnMessage += OnMessageAsync;

            _bot.StartReceiving();

            _logger.Information("Bot started!");

            await Task.Delay(-1);
        }

        private static async void OnMessageAsync(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;

                _logger.Information("Got message: {@Message}", message);

                if (message.Text.StartsWith("/start"))
                    await _bot.SendTextMessageAsync(new ChatId(message.From.Id),
                        "This bot allows you search & share books from Goodreads. It works on every dialog, " +
                        "just type @GoodreadsBooksBot in message input",
                        replyMarkup: InlineKeyboardHelpers.GetStartInlineKeyboardMarkup());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error during processing message");
            }
        }

        private static async void OnInlineQueryAsync(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            var inlineQuery = inlineQueryEventArgs.InlineQuery;

            if (string.IsNullOrWhiteSpace(inlineQuery.Query)) return;

            var searchRequest = new RestRequest()
                .AddQueryParameter("q", inlineQuery.Query);

            var searchResponse = await _restClient.ExecuteTaskAsync<GoodreadsResponse>(searchRequest);

            var response =
                searchResponse.Data.Search.Results.Select(InlineQueryResultArticleHelpers
                    .GetInlineQueryResultArticleForWork);

            await _bot.AnswerInlineQueryAsync(inlineQuery.Id, response);
        }
    }
}
