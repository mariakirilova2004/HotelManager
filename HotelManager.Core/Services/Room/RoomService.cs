using Ganss.Xss;
using HotelManager.Core.Models.Room;
using HotelManager.Core.Services.RoomType;
using HotelManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AllRoomsQueryModel All(int? capacity = null, string type = "", bool availability = false, int currentPage = 1, int roomsPerPage = 10)
        {
            var roomsQuery = this.dbContext.Rooms.Include(r => r.RoomType).ToList();

            if (capacity != null && capacity != 0)
            {
                roomsQuery = roomsQuery.Where(rq => rq.Capacity == capacity).ToList();
            }

            if (type != null && type != "All")
            {
                roomsQuery = roomsQuery.Where(rq => rq.RoomType.Type.CompareTo(type) == 0).ToList();
            }

            if (availability == true)
            {
                roomsQuery = roomsQuery.Where(rq => rq.IsFree == true).ToList();
            }

            var rooms = roomsQuery
                .Skip((currentPage - 1) * roomsPerPage)
                .Take(roomsPerPage)
                .Select(r => new RoomViewModel
            {
                Id = r.Id,
                Number = r.Number,
                Capacity = r.Capacity,
                RoomType = r.RoomType.Type,
                IsFree = r.IsFree,
                PriceForAdultBed = r.PriceForAdultBed,
                PriceForChildBed = r.PriceForChildBed
            }).ToList();

            rooms = rooms.OrderBy(c => c.Number).ToList();

            var totalRooms = roomsQuery.Count();

            return new AllRoomsQueryModel()
            {
                RoomsPerPage = roomsPerPage,
                TotalRoomsCount = totalRooms,
                Rooms = rooms
            };
        }

        public async Task Delete(int id)
        {
            var room = this.dbContext.Rooms.Where(r => r.Id == id).FirstOrDefault();
            this.dbContext.Rooms.Remove(room);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task Edit(AddRoomFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            var room = this.dbContext.Rooms.Where(r => r.Number == model.Number).FirstOrDefault();

            room.Number = model.Number;
            room.IsFree = model.IsFree;
            room.Capacity = model.Capacity;
            room.PriceForAdultBed = model.PriceForAdultBed;
            room.PriceForChildBed = model.PriceForChildBed;
            room.RoomTypeId = model.RoomTypeId;

            dbContext.Rooms.Update(room);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AddRoomFormModel GetById(int id)
        {
            return this.dbContext.Rooms.Where(r => r.Id == id)?.Select(r => new AddRoomFormModel {
                Id = r.Id,
                Number = r.Number,
                Capacity = r.Capacity,
                RoomTypeId = r.RoomTypeId,
                IsFree = r.IsFree,
                PriceForAdultBed = r.PriceForAdultBed,
                PriceForChildBed = r.PriceForChildBed,
                RoomTypes = roomTypeService.AllAdd()
            })?.FirstOrDefault();
        }

        public bool NumberExists(int number, int id)
        {
            return this.dbContext.Rooms.Any(r => r.Number == number && r.Id != id);
        }

        List<ReservationRoomModel> IRoomService.RoomsForReservationDetails()
        {
            return this.dbContext.Rooms.Select(r => new ReservationRoomModel()
            {
                Id = r.Id,
                Number = r.Number,
                RoomType = r.RoomType.Type
            })
            .ToList();
        }
    }
}
