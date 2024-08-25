using WebCrawler.Models;

namespace WebCrawler.Services.Contracts
{
    public interface IFilter
    {
        public List<NewsEntry> FilterByWordCount(List<NewsEntry> entries, int wordCount, bool moreThanFive);
        public int CountWords(string title);
    }
}
