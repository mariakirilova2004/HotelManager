using HotelManager.Core.Models.RoomType;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Models.Room
{
    public class AllRoomsQueryModel
    {
        [DisplayName("Search by capacity")]
        public int Capacity { get; init; }

        [DisplayName("Search by Type")]
        public string Type { get; init; }

        [DisplayName("Search by availability")]
        public bool Availability { get; init; }
        public int RoomsPerPage { get; init; }
        public int CurrentPage { get; init; } = 1;
        public int TotalRoomsCount { get; set; }
        public List<RoomViewModel> Rooms { get; set; } = new List<RoomViewModel>();
        public List<RoomTypeViewModel> RoomTypes { get; set; } = new List<RoomTypeViewModel>();
    }
}
