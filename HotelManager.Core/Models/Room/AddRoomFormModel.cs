﻿using HotelManager.Core.Models.RoomType;
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
        [Range(RoomMinNumber, RoomMaxNumber)]
        public int Number { get; set; }

        [Required]
        [Range(RoomMinCapacity, RoomMaxCapacity)]
        public int Capacity { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        public bool IsFree { get; set; }

        [Required]
        [Range(RoomMinPriceForAdultBed, RoomMaxPriceForAdultBed)]
        public decimal PriceForAdultBed { get; set; }

        [Required]
        [Range(RoomMinPriceForChildBed, RoomMaxPriceForChildBed)]
        public decimal PriceForChildBed { get; set; }

        public List<Infrastructure.Data.Еntities.RoomType>? RoomTypes { get; set; } = new List<Infrastructure.Data.Еntities.RoomType>();
    }
}
