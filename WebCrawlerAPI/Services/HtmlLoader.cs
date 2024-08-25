using HtmlAgilityPack;
using WebCrawlerAPI.Services.Contracts;

namespace WebCrawlerAPI.Services
{

    public class HtmlLoader : IHtmlLoader
    {
        private HtmlWeb _htmlWeb = new HtmlWeb();

        public HtmlDocument Load(string url)
        {
            return _htmlWeb.Load(url);
        }
    }

}
