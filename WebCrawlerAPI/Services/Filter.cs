using System.Text.RegularExpressions;
using WebCrawlerAPI.Models;
using WebCrawlerAPI.Services.Contracts;

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
        var cleanTitle = Regex.Replace(title, @"[^\p{L}\s]", "");
        return cleanTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
