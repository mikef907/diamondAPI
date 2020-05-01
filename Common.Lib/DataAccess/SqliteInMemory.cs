using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Common.Lib.DataAccess
{
    public class SqliteInMemory
    {
        public static SqliteConnection connection;

        public static DbContextOptions CreateOptions<T>()
        where T : DbContext
        {
            //This creates the SQLite connection string to in-memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();

            //This creates a SqliteConnectionwith that string
            var connection = new SqliteConnection(connectionString);

            //The connection MUST be opened here
            connection.Open();

            //Now we have the EF Core commands to create SQLite options
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connection);

            return builder.Options;
        }

        public static void ConfigBuilder<T>(DbContextOptionsBuilder builder)
            where T : DbContext
        {
            if (connection == null)
            {
                //This creates the SQLite connection string to in-memory database
                var connectionStringBuilder = new SqliteConnectionStringBuilder
                { DataSource = ":memory:" };
                var connectionString = connectionStringBuilder.ToString();

                //This creates a SqliteConnectionwith that string
                connection = new SqliteConnection(connectionString);

                //The connection MUST be opened here
                connection.Open();

            }
            builder.UseSqlite(connection);
        }
    }
}
