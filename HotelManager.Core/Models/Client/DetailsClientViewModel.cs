using HotelManager.Core.Models.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Client
{
    public class DetailsClientViewModel : ClientViewModel
    {
        public List<ReservationViewModel> Reservations = new List<ReservationViewModel>();
    }
}
