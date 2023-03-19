using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.User
{
    public class AllUsersQueryModel
    {
        [DisplayName("Search by text")]
        public string SearchTerm { get; init; }

        [DisplayName("Search in field")]
        public string SearchTermOn { get; init; }

        public int UsersPerPage { get; init; } = 10;
        public int CurrentPage { get; init; } = 1;
        public int TotalUsersCount { get; set; }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}
