using WebCrawler.Models;

public class Program
{
    static void Main(string[] args)
    {
        var scraper = new Scraper();
        var filter = new Filter();
        var logger = new Logger();

        try
        {
            var entries = scraper.ScrapeHackerNews();
            logger.StoreRequestLog("Scrape Data");

            Console.WriteLine("\nEntries with more than 5 words ordered by comments:");
            var filteredMoreThanFiveWords = filter.FilterByWordCount(entries, 5, moreThanFive: true);
            foreach (var entry in filteredMoreThanFiveWords)
            {
                Console.WriteLine($"{entry.Title} - {entry.Comments} comments");
            }

            Console.WriteLine("\nEntries with 5 or fewer words ordered by points:");
            var filteredLessOrEqualFiveWords = filter.FilterByWordCount(entries, 5, moreThanFive: false);
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
}
