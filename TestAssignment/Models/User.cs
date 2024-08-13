using Microsoft.AspNetCore.Identity;

namespace TestAssignment.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string FullName { get; set; }

        public string MobileNo { get; set; }

        public string EmailID { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public int RoleID { get; set; }

        public int? ReportingPersonID { get; set; }

        public int? ReportingPersonRoleID { get; set; }

        public bool isActive { get; set; } = true;

        public DateTime AccountCreatedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime AccountUpdatedDateTime { get; set; } = DateTime.UtcNow;

        public User()
        {
            FullName = string.Empty;
            MobileNo = string.Empty;
            EmailID = string.Empty;
            Username = string.Empty;
            PasswordHash = string.Empty;
            ReportingPersonID = null;
            ReportingPersonRoleID = null;
            isActive = true;
            AccountCreatedDateTime = DateTime.UtcNow;
            AccountUpdatedDateTime = DateTime.UtcNow;
        }
    }
}
