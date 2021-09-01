using System.Collections.Generic;

namespace Visma.IAM.Models
{
    public class ManagerRole : EmployeeRole
    {
        public ManagerRole() : base(
            "Manager",
            new List<Permission> { Permission.Create, Permission.Read, Permission.Update, Permission.Delete})
        {
        }
    }
}
