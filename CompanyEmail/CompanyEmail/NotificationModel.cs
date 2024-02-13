using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmail
{
    public class NotificationModel
    {
        public int CompanyId { get; set; }
        public bool ProcessStart { get; set; } 
        public bool IsSend { get; set; }
        public string? Company { get; set; }

        public string? FromEmail { get; set; }
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? EmailBody { get; set; }
        public string? ServiceType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string? ModifiedName { get; set; }
        public string? CreatedName { get; set; }
    }
}
