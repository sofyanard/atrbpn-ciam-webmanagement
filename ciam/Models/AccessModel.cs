using System;

namespace Ciam.Models
{
    public class AccessModel
    {
        public bool manageGroupMembership { get; set; }
        public bool view { get; set; }
        public bool mapRoles { get; set; }
        public bool impersonate { get; set; }
        public bool manage { get; set; }
    }
}