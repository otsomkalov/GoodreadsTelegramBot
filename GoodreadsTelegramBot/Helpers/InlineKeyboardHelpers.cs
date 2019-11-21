using Telegram.Bot.Types.ReplyMarkups;

namespace GoodreadsTelegramBot.Helpers
{
    public static class InlineKeyboardHelpers
    {
        public static InlineKeyboardMarkup GetStartInlineKeyboardMarkup()
        {
            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("🔍 Search books"),
                InlineKeyboardButton.WithSwitchInlineQuery("🔗 Find & share book")
            });
        }
    }
}
