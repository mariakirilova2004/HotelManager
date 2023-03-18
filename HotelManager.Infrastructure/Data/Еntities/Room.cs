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
        public int Id { get; set; }

        [Required]
        [Range(RoomMinNumber, RoomMaxNumber)]
        public int Number { get; set; }

        [Required]
        [Range(RoomMinCapacity, RoomMaxCapacity)]
        public int Capacity { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
  
        [ForeignKey(nameof(RoomTypeId))]
        public RoomType? RoomType { get; set; }

        [Required]
        public bool IsFree { get; set; }

        [Required]
        [Range(RoomMinPriceForAdultBed, RoomMaxPriceForAdultBed)]
        public decimal PriceForAdultBed { get; set; }

        [Required]
        [Range(RoomMinPriceForChildBed, RoomMaxPriceForChildBed)]
        public decimal PriceForChildBed { get; set; }
    }
}
