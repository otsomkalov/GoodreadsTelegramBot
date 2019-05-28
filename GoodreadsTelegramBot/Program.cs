using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodreads;
using Goodreads.Models.Request;
using Goodreads.Models.Response;
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
        private static IGoodreadsClient _goodreadsClient;

        private static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("You need to supply bot token and API code & secret");

                return;
            }

            _bot = new TelegramBotClient(args[0]);
            _goodreadsClient = GoodreadsClient.Create(args[1], args[2]);

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

            var books = await _goodreadsClient.Books.Search(inlineQuery.Query, 1, BookSearchField.Title);

            var response = books.List.Select(book =>
            {
                var resultArticle =
                    new InlineQueryResultArticle(book.Id.ToString(), book.BestBook.Title,
                        new InputTextMessageContent(GetMarkdown(book))
                        {
                            ParseMode = ParseMode.Html
                        })
                    {
                        ThumbUrl = book.BestBook.ImageUrl,
                        Description = book.BestBook.AuthorName
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
                .AppendLine($"Author: {book.BestBook.AuthorName}")
                .AppendLine($"Rating: {new string('⭐', Convert.ToInt32(book.RatingsSum))}")
                .ToString();
        }
    }
}