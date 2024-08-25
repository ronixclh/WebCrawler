using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerAPI.Services.Contracts
{
    public interface IHtmlLoader
    {
        HtmlDocument Load(string url);
    }

}
