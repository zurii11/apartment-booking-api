using Microsoft.Data.SqlClient;

namespace ApartmentBooking.Database;

public static class DbInitializer
{
    public static void EnsureDatabaseExists(IConfiguration config)
    {
        var booking_connection_string = new SqlConnectionStringBuilder(config.GetConnectionString("DefaultConnection"));

        try
        {
            using var conn = new SqlConnection(booking_connection_string.ConnectionString);
            conn.Open();
            Console.WriteLine("BookingDB exists!");
            return;
        }
        catch (SqlException ex) when (ex.Number == 4060)
        {
            Console.WriteLine("BookingDB doesn't exist. Creating...");
        }

        var master_connection_string = new SqlConnectionStringBuilder(config.GetConnectionString("DefaultConnection"))
        {
            InitialCatalog = "master"
        };

        using (var master_conn = new SqlConnection(master_connection_string.ConnectionString))
        {
            master_conn.Open();
            var create_db = File.ReadAllText("sql/create_db.sql");
            using var create_db_cmd = new SqlCommand(create_db, master_conn);
            create_db_cmd.ExecuteNonQuery();
            Console.WriteLine("CREATE DATABASE BookingDB issued...");
        }

        for (int i = 0; i < 10; i++)
        {
            try
            {
                using var booking_conn = new SqlConnection(booking_connection_string.ConnectionString);
                booking_conn.Open();
                string init_sql = File.ReadAllText("sql/init.sql");
                string procedures_sql = File.ReadAllText("sql/procedures.sql");

                var statements = procedures_sql.Split("-- SPLIT", StringSplitOptions.RemoveEmptyEntries);

                using var init_cmd = new SqlCommand(init_sql, booking_conn);
                init_cmd.ExecuteNonQuery();

                foreach (var stmt in statements)
                {
                    Console.WriteLine($"Executing statement:\n {stmt}");
                    using var procedures_cmd = new SqlCommand(stmt, booking_conn);
                    procedures_cmd.ExecuteNonQuery();
                }

                break;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"ERROR: {ex.Number}");
                Console.WriteLine("Retrying DB connection...");
                Thread.Sleep(2000);
            }
        }
    }
}
