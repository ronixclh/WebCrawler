using HtmlAgilityPack;
using System.Text.RegularExpressions;
using WebCrawler.Models;
using WebCrawler.Services.Contracts;

public class Scraper: IScrapper
{
    private readonly IHtmlLoader _htmlLoader;  // Declare a private field for HtmlWeb

    // Constructor to inject the HtmlWeb dependency
    public Scraper(IHtmlLoader htmlLoader)
    {
        _htmlLoader = htmlLoader;
    }
    public List<NewsEntry> ScrapeHackerNews()
    {
        var entries = new List<NewsEntry>();
        string baseUrl = "https://news.ycombinator.com/";

        try
        {
            
            Console.WriteLine($"\nAttempting to load page: {baseUrl}");
            var doc = _htmlLoader.Load(baseUrl);
            Console.WriteLine("Page loaded successfully.");

            if (doc == null)
            {
                Console.WriteLine("Error: Failed to load the page.");
                return entries;
            }

            var titleNodes = doc.DocumentNode.SelectNodes("//tr[@class='athing']");
            var subtextNodes = doc.DocumentNode.SelectNodes("//td[@class='subtext']");

            if (titleNodes == null || subtextNodes == null)
            {
                Console.WriteLine("Error: Unable to fetch title or subtext nodes from Hacker News.");
                return entries;
            }

            Console.WriteLine($"Found {titleNodes.Count} title nodes and {subtextNodes.Count} subtext nodes.");

            for (int i = 0; i < Math.Min(30, titleNodes.Count); i++)
            {
                var number = i + 1;
                var titleNode = titleNodes[i].SelectSingleNode(".//td[@class='title']//a");
                var title = titleNode?.InnerText ?? "No title";

                var pointsText = subtextNodes[i].SelectSingleNode(".//span[@class='score']")?.InnerText ?? "0 points";
                var points = 0;
                if (!string.IsNullOrEmpty(pointsText))
                {
                    var pointsMatch = Regex.Match(pointsText, @"\d+");
                    if (pointsMatch.Success)
                    {
                        points = int.Parse(pointsMatch.Value);
                    }
                }

                var commentsText = subtextNodes[i].SelectNodes(".//a")?.Last()?.InnerText ?? "0 comments";
                var comments = 0;
                if (!string.IsNullOrEmpty(commentsText))
                {
                    var commentsMatch = Regex.Match(commentsText, @"\d+");
                    if (commentsMatch.Success)
                    {
                        comments = int.Parse(commentsMatch.Value);
                    }
                }

                entries.Add(new NewsEntry { Number = number, Title = title, Points = points, Comments = comments });

                Console.WriteLine($"Entry {number}: {title} - {points} points - {comments} comments");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ScrapeHackerNews: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        return entries;
    }
}
