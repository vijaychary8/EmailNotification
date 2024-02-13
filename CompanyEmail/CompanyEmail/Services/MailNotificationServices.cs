using CompanyEmail.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NLog;
using Npgsql;


namespace CompanyEmail.Services
{
    public class MailNotificationServices: IMailNotificationServices
    {
        private static List<NotificationModel> _notifications = new List<NotificationModel>();
        private readonly MailSettings _mailSettings;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MailNotificationServices(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public void GetTableDetails(NpgsqlConnection connection)
        {
           

            using (var cmd = new NpgsqlCommand("SELECT * FROM tblnotifications where processstart=false and issend=false and companyid='172'", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NotificationModel column = new NotificationModel
                        {
                            CompanyId = Convert.ToInt32(reader["companyid"]),
                            ProcessStart = Convert.ToBoolean(reader["processstart"]),
                            IsSend= Convert.ToBoolean(reader["processstart"]),
                            Company = Convert.ToString(reader["company"]),

                            FromEmail = Convert.ToString(reader["fromemail"]),
                            ToEmail= Convert.ToString(reader["toemail"]),
                            Subject= Convert.ToString(reader["subject"]),
                            EmailBody= Convert.ToString(reader["emailbody"]),
                            ServiceType= Convert.ToString(reader["servicetype"]),
         
                        };

                        _notifications.Add(column);

                    }
                }
            }
            using (var cmd = new NpgsqlCommand("SELECT * FROM tblnotifications where processstart=true and issend=false", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NotificationModel column = new NotificationModel
                        {
                            CompanyId = Convert.ToInt32(reader["companyid"]),
                            ProcessStart = Convert.ToBoolean(reader["processstart"]),
                            IsSend = Convert.ToBoolean(reader["processstart"]),
                            Company = Convert.ToString(reader["company"]),

                            FromEmail = Convert.ToString(reader["fromemail"]),
                            ToEmail = Convert.ToString(reader["toemail"]),
                            Subject = Convert.ToString(reader["subject"]),
                            EmailBody = Convert.ToString(reader["emailbody"]),
                            ServiceType = Convert.ToString(reader["servicetype"]),
                        };

                        _notifications.Add(column);

                    }
                }
            }


           
            UpdateData(connection, "1");

        }
        async void UpdateData(NpgsqlConnection connection, String companyId)
        {
            foreach (var column in _notifications)
            {
                var updateQuery = "UPDATE tblnotifications SET processstart = True,modifiedby=1,modifiedname='Admin',modifiedon=@ModifiedOn WHERE companyid = @CompanyId";
                using (var updateCmd = new NpgsqlCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("CompanyId", Convert.ToInt32(column.CompanyId)); // Person's name to update
                    updateCmd.Parameters.AddWithValue("ModifiedOn",DateTime.Now ); // Person's name to update

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    await SendEmailAsync(connection, Convert.ToInt32(column.CompanyId),column);

                }
            }



        }



        public async Task SendEmailAsync(NpgsqlConnection connection, Int32 companyId,NotificationModel company)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Gaido", "shashidharreddydakuri@gmail.com"));
            email.To.Add(new MailboxAddress("Gaido", "shashidharreddydakuri@gmail.com"));

            email.Subject = "Testing out email sending";

            Logger.Info("heh");



            /*   string FilePath = AppContext.BaseDirectory.Split("\\bin\\")[0] + "\\Templates\\WelcomeTemplate.html";
               StreamReader str = new StreamReader(FilePath);
               string MailText = str.ReadToEnd();*/
            string? MailText = company.EmailBody;
           
            /*            str.Close();
            */
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
           

            email.Body = builder.ToMessageBody();

            try
            {
                using (var smtp = new SmtpClient())
                {

                    smtp.Connect(_mailSettings.Host, Convert.ToInt32(_mailSettings.Port), SecureSocketOptions.StartTls);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(_mailSettings.Username, _mailSettings.Password);

                    var response = smtp.Send(email);
                    if (response.Split(" ")[0] == "Ok")
                    {
                        var updateQuery = "UPDATE tblnotifications SET issend = True ,modifiedby=1,modifiedname='Admin',modifiedon=@ModifiedOn WHERE companyid = @CompanyId";
                        using (var updateCmd = new NpgsqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("CompanyId", companyId); // Person's name to update
                            updateCmd.Parameters.AddWithValue("ModifiedOn", DateTime.Now); // Person's name to update

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                        }
                    }
                    Logger.Info(response);
                    smtp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

    }

}
