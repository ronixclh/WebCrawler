using Microsoft.AspNetCore.Mvc;
using WebCrawlerAPI.Models;
using WebCrawlerAPI.Services.Contracts;

namespace WebCrawlerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IScrapper _scraper;
        private readonly IFilter _filter;
        private readonly ICrawlerLogger _logger;

        public NewsController(IScrapper scraper, IFilter filter, ICrawlerLogger logger)
        {
            _scraper = scraper;
            _filter = filter;
            _logger = logger;
        }

        [HttpGet("morethan5words")]
        public ActionResult<IEnumerable<NewsEntry>> GetMoreThan5Words()
        {
            var entries = _scraper.ScrapeHackerNews();
            var filteredEntries = _filter.FilterByWordCount(entries, 5, true);

            _logger.StoreRequestLog("More than 5 words");
            return Ok(filteredEntries);
        }

        [HttpGet("lessthanorequal5words")]
        public ActionResult<IEnumerable<NewsEntry>> GetLessThanOrEqual5Words()
        {
            var entries = _scraper.ScrapeHackerNews();
            var filteredEntries = _filter.FilterByWordCount(entries, 5, false);

            _logger.StoreRequestLog("Less than or equal to 5 words");
            return Ok(filteredEntries);
        }
    }
}
