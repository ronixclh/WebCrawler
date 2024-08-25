namespace WebCrawlerAPI.Services.Contracts
{
    public interface ICrawlerLogger
    {
        public void StoreRequestLog(string filter);
    }
}
