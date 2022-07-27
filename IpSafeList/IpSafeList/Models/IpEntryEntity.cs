using IpSafeList.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpSafeList.Models
{
    public class IpEntryEntity
    {
        [Identity]
        public int Id { get; set; }
        public int IdentityId { get; set; }
        public string IpAddress { get; set; } = "";
    }
}
