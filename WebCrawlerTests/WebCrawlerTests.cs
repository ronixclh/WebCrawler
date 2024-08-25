using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebCrawlerAPI.Controllers;
using WebCrawlerAPI.Models;
using WebCrawlerAPI.Services.Contracts;

namespace WebCrawlerAPITests
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

    [TestFixture]
    public class NewsControllerTests
    {
        private Mock<IScrapper> _scraperMock;
        private Mock<IFilter> _filterMock;
        private Mock<ICrawlerLogger> _loggerMock;
        private NewsController _controller;

        [SetUp]
        public void SetUp()
        {
            _scraperMock = new Mock<IScrapper>();
            _filterMock = new Mock<IFilter>();
            _loggerMock = new Mock<ICrawlerLogger>();
            _controller = new NewsController(_scraperMock.Object, _filterMock.Object, _loggerMock.Object);
        }

        [Test]
        public void GetMoreThan5Words_ReturnsOkWithFilteredEntries()
        {
            var mockEntries = new List<NewsEntry>
            {
                new NewsEntry { Number = 1, Title = "Test Title One", Points = 100, Comments = 50 },
                new NewsEntry { Number = 2, Title = "Another Test Title", Points = 80, Comments = 30 },
                new NewsEntry { Number = 2, Title = "Another Test Title One Two Three", Points = 80, Comments = 30 }
            };

            _scraperMock.Setup(s => s.ScrapeHackerNews()).Returns(mockEntries);
            var filteredEntries = mockEntries.Where(e => e.Title.Split(' ').Length > 5).ToList();
            _filterMock.Setup(f => f.FilterByWordCount(mockEntries, 5, true)).Returns(filteredEntries);

            var result = _controller.GetMoreThan5Words();

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnEntries = okResult.Value as IEnumerable<NewsEntry>;
            Assert.That(returnEntries, Is.Not.Null);
            Assert.That((returnEntries as List<NewsEntry>).Count, Is.EqualTo(1));

            _loggerMock.Verify(l => l.StoreRequestLog("More than 5 words"), Times.Once);
        }

        [Test]
        public void GetLessThanOrEqual5Words_ReturnsOkWithFilteredEntries()
        {
            // Arrange
            var mockEntries = new List<NewsEntry>
            {
                new NewsEntry { Number = 1, Title = "Short Title", Points = 50, Comments = 20 }
            };

            _scraperMock.Setup(s => s.ScrapeHackerNews()).Returns(mockEntries);
            _filterMock.Setup(f => f.FilterByWordCount(mockEntries, 5, false)).Returns(mockEntries);

            // Act
            var result = _controller.GetLessThanOrEqual5Words();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnEntries = okResult.Value as IEnumerable<NewsEntry>;
            Assert.That(returnEntries, Is.Not.Null);
            Assert.That((returnEntries as List<NewsEntry>).Count, Is.EqualTo(1));

            _loggerMock.Verify(l => l.StoreRequestLog("Less than or equal to 5 words"), Times.Once);
        }
    }
}