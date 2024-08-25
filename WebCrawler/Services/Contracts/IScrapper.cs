

using WebCrawler.Models;

namespace WebCrawler.Services.Contracts
{
    public interface IScrapper
    {
        public List<NewsEntry> ScrapeHackerNews();
    }
}
