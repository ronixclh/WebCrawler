

using WebCrawlerAPI.Models;

namespace WebCrawlerAPI.Services.Contracts
{
    public interface IScrapper
    {
        public List<NewsEntry> ScrapeHackerNews();
    }
}
