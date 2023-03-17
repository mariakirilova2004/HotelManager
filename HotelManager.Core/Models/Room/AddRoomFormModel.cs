using HotelManager.Core.Models.RoomType;
using HotelManager.Infrastructure.Data.Еntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelManager.Infrastructure.Data.DataConstants.Room;

namespace HotelManager.Core.Models.Room
{
    public class AddRoomFormModel
    {
        [Required]
        [MinLength(RoomMinNumber), MaxLength(RoomMaxNumber)]
        public int Number { get; set; }

        [Required]
        [MinLength(RoomMinCapacity), MaxLength(RoomMaxCapacity)]
        public int Capacity { get; set; }
        public string RoomType { get; set; }

        [Required]
        public bool IsFree { get; set; }

        [Required]
        [MinLength(RoomMinPriceForAdultBed), MaxLength(RoomMaxPriceForAdultBed)]
        public int PriceForAdultBed { get; set; }

        [Required]
        [MinLength(RoomMinPriceForChildBed), MaxLength(RoomMaxPriceForChildBed)]
        public int PriceForChildBed { get; set; }

        public List<RoomTypeViewModel> RoomTypes { get; set; } = new List<RoomTypeViewModel>();
    }
}
