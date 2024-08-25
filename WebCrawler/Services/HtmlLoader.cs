
using HtmlAgilityPack;
using WebCrawler.Services.Contracts;

namespace WebCrawler.Services
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
