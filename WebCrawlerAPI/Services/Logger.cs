
using System.Data.SQLite;
using WebCrawlerAPI.Services.Contracts;

public class Logger: ICrawlerLogger
{
    public void StoreRequestLog(string filter)
    {
        try
        {
             
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string databasePath = System.IO.Path.Combine(appPath, "WebCrawler.db");
            Console.WriteLine($"Database path: {databasePath}");

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                var createTableCommand = new SQLiteCommand(
                     "CREATE TABLE IF NOT EXISTS Requests (Id INTEGER PRIMARY KEY AUTOINCREMENT, Timestamp DATETIME, FilterType TEXT)",
                     connection);
                createTableCommand.ExecuteNonQuery();

                var insertCommand = new SQLiteCommand(
                    "INSERT INTO Requests (Timestamp, FilterType) VALUES (@timestamp, @filterType)",
                connection);

                insertCommand.Parameters.AddWithValue(@"timestamp", DateTime.Now);
                insertCommand.Parameters.AddWithValue(@"filterType", filter);
                insertCommand.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in StoreRequestLog: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
