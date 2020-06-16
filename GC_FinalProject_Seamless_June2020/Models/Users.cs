using System;
using System.Collections.Generic;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string UserType { get; set; }
        public string Theme { get; set; }
        public string Name { get; set; }
        public string Technology { get; set; }
        public string Landscape { get; set; }
        public string Summary { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string ProfilePicture { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string UserId { get; set; }
        public string Roles { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
