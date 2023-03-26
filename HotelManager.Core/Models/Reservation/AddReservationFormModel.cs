using HotelManager.Core.Attributes;
using HotelManager.Core.Models.Client;
using HotelManager.Core.Models.Room;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Reservation
{
    public class AddReservationFormModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomNumberId { get; set; }

        [Required]
        [CustomArrivalDateAttribute]
        public DateTime Arrival { get; set; }

        [Required]
        [CustomLeavingDateAttribute]
        public DateTime Leaving { get; set; }

        [Required]
        public bool IsBreakfastIncluded { get; set; }

        [Required]
        public bool IsAllInclusive { get; set; }

        public List<ReservationRoomModel> Rooms { get; set; } = new List<ReservationRoomModel>();
        public List<ReservationClientModel> Clients { get; set; } = new List<ReservationClientModel>();

    }
}
