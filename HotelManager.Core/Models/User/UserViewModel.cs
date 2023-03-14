using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime HiringDate { get; set; }
        public DateTime? DismissionDate { get; set; }

        public bool IsActive { get; set; }
    }
}
