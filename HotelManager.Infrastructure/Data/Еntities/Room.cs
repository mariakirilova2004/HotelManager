using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelManager.Infrastructure.Data.DataConstants.Room;
using  HotelManager.Infrastructure.Data.Еntities;

namespace HotelManager.Infrastructure.Data.Еntities
{
    public class Room
    {
        [Key]
        [Required]
        [MinLength(RoomMinNumber), MaxLength(RoomMaxNumber)]
        public int Number { get; set; }

        [Required]
        [MinLength(RoomMinCapacity), MaxLength(RoomMaxCapacity)]
        public int Capacity { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
  
        [ForeignKey(nameof(RoomTypeId))]
        public RoomType? RoomType { get; set; }

        [Required]
        public bool IsFree { get; set; }

        [Required]
        [MinLength(RoomMinPriceForAdultBed), MaxLength(RoomMaxPriceForAdultBed)]
        public int PriceForAdultBed { get; set; }

        [Required]
        [MinLength(RoomMinPriceForChildBed), MaxLength(RoomMaxPriceForChildBed)]
        public int PriceForChildBed { get; set; }
    }
}
