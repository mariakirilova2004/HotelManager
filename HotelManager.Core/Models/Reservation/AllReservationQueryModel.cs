using HotelManager.Core.Models.Client;
using HotelManager.Core.Models.Room;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Reservation
{
    public class AllReservationQueryModel
    {
        [DisplayName("Search by text")]
        public string SearchTerm { get; init; }

        [DisplayName("Search in field")]
        public string SearchTermOn { get; init; }

        public int ReservationsPerPage { get; init; } = 10;
        public int CurrentPage { get; init; } = 1;
        public int TotalReservationsCount { get; set; }
        public List<ReservationViewModel> Reservations { get; set; } = new List<ReservationViewModel>();
    }
}
