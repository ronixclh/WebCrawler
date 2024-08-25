
using System.Data.SQLite;
using WebCrawler.Services.Contracts;

public class Logger: ILogger
{
    public void StoreRequestLog(string filter)
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string databasePath = System.IO.Path.Combine(desktopPath, "usage_logs.db");

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
