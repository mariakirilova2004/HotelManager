using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManager.Infrastructure.Data.Еntities.Account;
using static HotelManager.Infrastructure.Data.DataConstants.Reservation;
using HotelManager.Infrastructure.Attributes;

namespace HotelManager.Infrastructure.Data.Еntities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomNumberId { get; set; }

        [ForeignKey(nameof(RoomNumberId))]
        public Room? Room { get; set; }

        [Required]
        public User? User { get; set; }      
        
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        public virtual List<Client> Clients { get; set; } = new List<Client>();

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

        [Required]
        public decimal Total { get; set; }
    }
}
