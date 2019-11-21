using System;
using System.Text;
using GoodreadsTelegramBot.Models;

namespace GoodreadsTelegramBot.Helpers
{
    public static class MarkdownHelpers
    {
        public static string GetMarkdown(Work book)
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
