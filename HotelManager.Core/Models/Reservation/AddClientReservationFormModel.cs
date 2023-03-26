using HotelManager.Core.Models.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Reservation
{
    public class AddClientReservationFormModel
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public List<ReservationClientModel> Clients { get; set; } = new List<ReservationClientModel>();
    }
}
