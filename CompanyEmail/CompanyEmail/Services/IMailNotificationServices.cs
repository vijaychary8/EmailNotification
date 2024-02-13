using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmail.Services
{
    public interface IMailNotificationServices
    {
        public void GetTableDetails(NpgsqlConnection connection);

    }
}
