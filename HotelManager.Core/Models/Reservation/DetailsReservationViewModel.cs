using HotelManager.Core.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Reservation
{
    public class DetailsReservationViewModel
    {
        public int ClientsPerPage { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int TotalClientsCount { get; set; }
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string UserName { get; set; }
        public virtual List<ClientViewModel> Clients { get; set; } = new List<ClientViewModel>();
        public DateTime Arrival { get; set; }
        public DateTime Leaving { get; set; }
        public bool IsBreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public decimal Total { get; set; }
    }
}
