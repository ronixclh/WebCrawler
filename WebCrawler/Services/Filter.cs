using System.Text.RegularExpressions;
using WebCrawler.Models;
using WebCrawler.Services.Contracts;

public class Filter:IFilter
{
    public List<NewsEntry> FilterByWordCount(List<NewsEntry> entries, int wordCount, bool moreThanFive)
    {
        var filteredEntries = entries
            .Where(entry => moreThanFive ? CountWords(entry.Title) > wordCount : CountWords(entry.Title) <= wordCount)
            .ToList();

        return moreThanFive
            ? filteredEntries.OrderByDescending(e => e.Comments).ToList()
            : filteredEntries.OrderByDescending(e => e.Points).ToList();
    }

    public int CountWords(string title)
    {
        var cleanTitle = Regex.Replace(title, @"[^\w\s]", "");
        return cleanTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
