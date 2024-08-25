using HtmlAgilityPack;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using WebCrawler.Models;
using WebCrawler.Services;
using WebCrawler.Services.Contracts;

namespace WebCrawlerTests
{
    public class FilterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CountWords_ValidTitle_ReturnsCorrectCount()
        {
            string input = "Test string";
            var filter = new Filter();

            int wordCount = filter.CountWords(input);

            Assert.That(wordCount, Is.EqualTo(2));
        }

        [Test]
        public void CountWords_TitleWithNumbers_ReturnsCorrectCount()
        {
            string input = "Test 12345 string";
            var filter = new Filter();

            int wordCount = filter.CountWords(input);

            Assert.That(wordCount, Is.EqualTo(2));
        }

        [Test]
        public void CountWords_TitleWithSpecialCharacters_ReturnsCorrectCount()
        {
            string input = "Test -@!# string";
            var filter = new Filter();

            int wordCount = filter.CountWords(input);

            Assert.That(wordCount, Is.EqualTo(2));
        }

        [Test]
        public void FilterByWordCount_MoreThanFiveWords_ReturnsCorrectEntries()
        {
            var entries = new List<NewsEntry>
            {
                new NewsEntry { Title = "Test with more than five words", Comments = 50 },
                new NewsEntry { Title = "Is Only five words here", Comments = 20 },
            };

            var filter = new Filter();
            var result = filter.FilterByWordCount(entries, 5, true);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test with more than five words"));
        }
    }

    [TestFixture]
    public class ScraperTests
    {
        private Mock<IHtmlLoader> _htmlLoaderMock;
        private Scraper _scraper;

        [SetUp]
        public void Setup()
        {
            _htmlLoaderMock = new Mock<IHtmlLoader>();
            _scraper = new Scraper(_htmlLoaderMock.Object);
        }

        [Test]
        public void ScrapeHackerNews_ValidHtml_ReturnsListOfNewsEntries()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(@"
                <html>
                    <body>
                        <tr class='athing'>
                            <td class='title'><a href='link'>Test Title</a></td>
                        </tr>
                        <td class='subtext'>
                            <span class='score'>100 points</span>
                            <a>50 comments</a>
                        </td>
                    </body>
                </html>");

            _htmlLoaderMock.Setup(loader => loader.Load(It.IsAny<string>())).Returns(htmlDoc);

            var result = _scraper.ScrapeHackerNews();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Test Title"));
            Assert.That(result[0].Points, Is.EqualTo(100));
            Assert.That(result[0].Comments, Is.EqualTo(50));
        }

        [Test]
        public void ScrapeHackerNews_InvalidHtml_ReturnsEmptyList()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml("<html><body></body></html>");

            _htmlLoaderMock.Setup(loader => loader.Load(It.IsAny<string>())).Returns(htmlDoc);

            var result = _scraper.ScrapeHackerNews();

            Assert.That(result.Count, Is.EqualTo(0)); 
        }

        [Test]
        public void ScrapeHackerNews_WhenHtmlLoaderLoadThrowsException_ReturnsEmptyList()
        {
            _htmlLoaderMock.Setup(loader => loader.Load(It.IsAny<string>())).Throws(new System.Exception("Network error"));

            var result = _scraper.ScrapeHackerNews();

            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
