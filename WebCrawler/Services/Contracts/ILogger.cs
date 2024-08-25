

namespace WebCrawler.Services.Contracts
{
    public interface ILogger
    {
        public void StoreRequestLog(string filter);
    }
}
