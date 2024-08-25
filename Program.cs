using WebCrawler.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data.SQLite;

public class Program
{
    static void Main(string[] args)
    {
        try
        {
            var entries = ScrapeHackerNews();
            StoreRequestLog("Scrape Data");

            Console.WriteLine("\nEntries with more than 5 words ordered by comments:");

            var filteredMoreThanFiveWords = FilterByWordCount(entries, 5, moreThanFive: true);
            foreach (var entry in filteredMoreThanFiveWords)
            {
                Console.WriteLine($"{entry.Title} - {entry.Comments} comments");
            }

            Console.WriteLine("\nEntries with 5 or fewer words ordered by points:");
            var filteredLessOrEqualFiveWords = FilterByWordCount(entries, 5, moreThanFive: false);
            foreach (var entry in filteredLessOrEqualFiveWords)
            {
                Console.WriteLine($"{entry.Title} - {entry.Points} points");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadLine();

    }

    public static List<NewsEntry> ScrapeHackerNews()
    {
        var entries = new List<NewsEntry>();
        string baseUrl = "https://news.ycombinator.com/";
        string currentUrl = baseUrl;
        int totalCount = 0;  

        try
        {
            while (!string.IsNullOrEmpty(currentUrl))
            {
                var web = new HtmlWeb();

                Console.WriteLine($"\nAttempting to load page: {currentUrl}");
                var doc = web.Load(currentUrl);
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
                    break;
                }

                Console.WriteLine($"Found {titleNodes.Count} title nodes and {subtextNodes.Count} subtext nodes.");

                for (int i = 0; i < Math.Min(30, titleNodes.Count); i++)
                {
                    totalCount++; 

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

                    entries.Add(new NewsEntry { Number = totalCount, Title = title, Points = points, Comments = comments });

                    Console.WriteLine($"Entry {totalCount}: {title} - {points} points - {comments} comments");
                }

                var moreLinkNode = doc.DocumentNode.SelectSingleNode("//a[@class='morelink']");
                if (moreLinkNode != null)
                {
                    var nextPageRelativeUrl = moreLinkNode.GetAttributeValue("href", string.Empty);
                    currentUrl = baseUrl + nextPageRelativeUrl;
                }
                else
                {
                    currentUrl = null;
                    Console.WriteLine("No more pages to scrape.");
                }

                Console.WriteLine("\nSleeping for 5 seconds to avoid rate-limiting...");
                Thread.Sleep(5000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ScrapeHackerNews: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        return entries;
    }

    public static List<NewsEntry> FilterByWordCount(List<NewsEntry> entries, int wordCount, bool moreThanFive)
    {
        var filteredEntries = entries
            .Where(entry => moreThanFive ? CountWords(entry.Title) > wordCount : CountWords(entry.Title) <= wordCount)
            .ToList();

        return moreThanFive
            ? filteredEntries.OrderByDescending(e => e.Comments).ToList()
            : filteredEntries.OrderByDescending(e => e.Points).ToList();
    }

    public static int CountWords(string title)
    {
        var cleanTitle = Regex.Replace(title, @"[^\w\s]", "");
        return cleanTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static void StoreRequestLog(string filter)
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string databasePath = System.IO.Path.Combine(desktopPath, "usage_logs.db");

            using (var connection = new SQLiteConnection("Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                var createTableCommand = new SQLiteCommand(
                     "CREATE TABLE IF NOT EXISTS Requests (Id INTEGER PRIMARY KEY AUTOINCREMENT, Timestamp DATETIME, FilterType TEXT)",
                     connection);
                createTableCommand.ExecuteNonQuery();

                var insertCommand = new SQLiteCommand(
                    "INSERT INTO Requests (Timestamp, FilterType) VALUES (@timestamp, @filterType)",
                connection);

                insertCommand.Parameters.AddWithValue(@"timestamp", DateTime.Now);
                insertCommand.Parameters.AddWithValue(@"filterType", filter);
                insertCommand.ExecuteNonQuery();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in StoreRequestLog: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

    }
}