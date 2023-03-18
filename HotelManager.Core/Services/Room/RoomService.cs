using HotelManager.Core.Models.Room;
using HotelManager.Core.Services.RoomType;
using HotelManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Room
{
    public class RoomService : IRoomService
    {
        private readonly HotelManagerDbContext dbContext;
        private IRoomTypeService roomTypeService;

        public RoomService(HotelManagerDbContext _dbContext, IRoomTypeService _roomTypeService)
        {
            this.dbContext = _dbContext;
            this.roomTypeService = _roomTypeService;
        }

        public async Task Add(AddRoomFormModel model)
        {
            var room = new Infrastructure.Data.Еntities.Room()
            {
                Number = model.Number,
                Capacity = model.Capacity,
                IsFree = model.IsFree,
                PriceForAdultBed = model.PriceForAdultBed,
                PriceForChildBed = model.PriceForChildBed,
                RoomTypeId = model.RoomTypeId
            };

            await this.dbContext.Rooms.AddAsync(room);
            await this.dbContext.SaveChangesAsync();
        }

        public AllRoomsQueryModel All(int? capacity = null, string type = "", bool availability = false, int currentPage = 1, int roomsPerPage = 1)
        {
            var roomsQuery = new List<RoomViewModel>();

            roomsQuery = this.dbContext.Rooms.Select(r => new RoomViewModel
            {
                Number = r.Number,
                Capacity = r.Capacity,
                RoomType = r.RoomType.Type,
                IsFree = r.IsFree,
                PriceForAdultBed = r.PriceForAdultBed,
                PriceForChildBed = r.PriceForChildBed
            }).ToList();

            if (capacity != null && capacity != 0)
            {
                roomsQuery = roomsQuery.Where(rq => rq.Capacity == capacity).ToList();
            }

            if (type != null && type != "All")
            {
                roomsQuery = roomsQuery.Where(rq => rq.RoomType.CompareTo(type) == 0).ToList();
            }

            if (availability == false)
            {
                roomsQuery = roomsQuery.Where(rq => rq.IsFree == false).ToList();
            }
            else
            {
                roomsQuery = roomsQuery.Where(rq => rq.IsFree == true).ToList();
            }

            roomsQuery = roomsQuery.OrderByDescending(c => c.IsFree).ToList();

            var totalRooms = roomsQuery.Count();

            return new AllRoomsQueryModel()
            {
                RoomsPerPage = roomsPerPage,
                TotalRoomsCount = totalRooms,
                Rooms = roomsQuery
            };
        }

        public bool NumberExists(int number)
        {
            return this.dbContext.Rooms.Any(r => r.Number == number);
        }
    }
}
