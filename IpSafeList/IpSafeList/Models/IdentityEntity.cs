using IpSafeList.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpSafeList.Models
{
    public class IdentityEntity
    {
        [Identity]
        [AutoIncrement]
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsLocked { get; set; }

    }
}
