using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodreadsTelegramBot.Models;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace GoodreadsTelegramBot
{
    internal static class Program
    {
        private static TelegramBotClient _bot;
        private static IRestClient _restClient;

        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("You need to supply bot token and API code");

                return;
            }

            _bot = new TelegramBotClient(args[0]);

            _restClient = new RestClient("https://www.goodreads.com/search/index.xml")
                .AddDefaultQueryParameter("key", args[1])
                .AddDefaultQueryParameter("search", "title");

            _bot.OnInlineQuery += OnInlineQueryAsync;
            _bot.OnMessage += OnMessageAsync;

            _bot.StartReceiving();

            Console.WriteLine("Bot started!");

            while (true)
            {
                await Task.Delay(int.MaxValue);
            }
        }

        private static async void OnMessageAsync(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message.Text.StartsWith("/start"))
                await _bot.SendTextMessageAsync(new ChatId(message.From.Id),
                    "This bot allows you search & share books from Goodreads. It works on every dialog, " +
                    "just type @GoodreadsBooksBot in message input",
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("🔍 Search books"),
                        InlineKeyboardButton.WithSwitchInlineQuery("🔗 Find & share book")
                    }));
        }

        private static async void OnInlineQueryAsync(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            var inlineQuery = inlineQueryEventArgs.InlineQuery;

            if (string.IsNullOrWhiteSpace(inlineQuery.Query)) return;

            var searchRequest = new RestRequest()
                .AddQueryParameter("q", inlineQuery.Query);

            var searchResponse = await _restClient.ExecuteTaskAsync<GoodreadsResponse>(searchRequest);

            var response = searchResponse.Data.Search.Results.Select(work =>
            {
                var resultArticle =
                    new InlineQueryResultArticle(work.BestBook.Id.ToString(), work.BestBook.Title,
                        new InputTextMessageContent(GetMarkdown(work))
                        {
                            ParseMode = ParseMode.Html
                        })
                    {
                        ThumbUrl = work.BestBook.ImageUrl,
                        Description = work.BestBook.Author.Name
                    };

                return resultArticle;
            });

            await _bot.AnswerInlineQueryAsync(inlineQuery.Id, response);
        }

        private static string GetMarkdown(Work book)
        {
            return new StringBuilder()
                .AppendLine(
                    $"<a href=\"https://www.goodreads.com/book/show/{book.BestBook.Id}\">{book.BestBook.Title}</a>")
                .AppendLine(
                    $"Author: <a href=\"https://www.goodreads.com/author/show/{book.BestBook.Author.Id}\">{book.BestBook.Author.Name}</a>")
                .AppendLine($"Rating: {new string('⭐', Convert.ToInt32(book.AverageRating))}")
                .ToString();
        }
    }
}
