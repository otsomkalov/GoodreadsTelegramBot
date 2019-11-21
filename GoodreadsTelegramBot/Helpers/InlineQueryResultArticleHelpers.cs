using GoodreadsTelegramBot.Models;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace GoodreadsTelegramBot.Helpers
{
    public static class InlineQueryResultArticleHelpers
    {
        public static InlineQueryResultArticle GetInlineQueryResultArticleForWork(Work work)
        {
            return new InlineQueryResultArticle(work.BestBook.Id.ToString(), work.BestBook.Title,
                new InputTextMessageContent(MarkdownHelpers.GetMarkdown(work))
                {
                    ParseMode = ParseMode.Html
                })
            {
                ThumbUrl = work.BestBook.ImageUrl,
                Description = work.BestBook.Author.Name
            };
        }
    }
}
