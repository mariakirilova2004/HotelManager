using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Reservation
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Leaving { get; set; }
        public bool IsBreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public decimal Total { get; set; }
    }
}
