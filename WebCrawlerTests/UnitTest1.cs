namespace WebCrawlerTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            string input = "Test string";

            int wordCount = Program.CountWords(input);

            Assert.That(wordCount, Is.EqualTo(2));
        }
    }
}