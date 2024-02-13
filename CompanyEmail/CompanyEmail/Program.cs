using CompanyEmail.Services;
using CompanyEmail.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Npgsql;
using System.ComponentModel.Design;
using System.Net.Mail;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace CompanyEmail
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // Initialize NLog
            IConfiguration configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json")
             .Build();

            // Configure services
            var serviceProvider = new ServiceCollection()
                .Configure<MailSettings>(configuration.GetSection("MailSettings"))
                .AddTransient<IMailNotificationServices, MailNotificationServices>()
                .AddSingleton<MailSettings>()
                .BuildServiceProvider();

            // Get the service
            var myService = serviceProvider.GetRequiredService<IMailNotificationServices>();




            Logger.Info("Application started");
            string connectionString = "Host=localhost;Username=postgres;Password=admin;Database=dbgaidogate";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    
                        connection.Open();

                        myService.GetTableDetails(connection);

                        connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            // Example PostgreSQL connection
            

        }

       

    }
}
