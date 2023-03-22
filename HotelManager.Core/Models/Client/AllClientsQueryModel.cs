using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Client
{
    public class AllClientsQueryModel
    {
        [DisplayName("Search by First Name")]
        public string FirstNameSearch { get; init; }

        [DisplayName("Search by Last Name")]
        public string LastNameSearch { get; init; }
        public int ClientsPerPage { get; init; } = 10;
        public int CurrentPage { get; init; } = 1;
        public int TotalClientsCount { get; set; }
        public List<ClientViewModel> Clients { get; set; } = new List<ClientViewModel>();
    }
}
