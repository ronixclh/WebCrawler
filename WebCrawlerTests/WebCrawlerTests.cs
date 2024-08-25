using WebCrawler.Services;

namespace WebCrawlerTests
{
    public class WebCrawlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            string input = "Test string";
            var filter = new Filter();

            int wordCount = filter.CountWords(input);

            Assert.That(wordCount, Is.EqualTo(2));
        }
    }
}