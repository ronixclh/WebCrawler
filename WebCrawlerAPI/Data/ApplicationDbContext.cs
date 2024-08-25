using Microsoft.EntityFrameworkCore;

namespace WebCrawlerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RequestLog> Requests { get; set; }
    }

    public class RequestLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string FilterType { get; set; }
    }
}
