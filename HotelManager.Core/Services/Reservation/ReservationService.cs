using Ganss.Xss;
using HotelManager.Core.Models.Reservation;
using HotelManager.Core.Services.Client;
using HotelManager.Core.Services.Room;
using HotelManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Reservation
{
    public class ReservationService : IReservationService
    {
        private readonly HotelManagerDbContext dbContext;
        private IRoomService roomService;
        private IClientService clientService;

        public ReservationService(HotelManagerDbContext _dbContext,
                                  IRoomService _roomService,
                                  IClientService _clientService)
        {
            this.dbContext = _dbContext;
            this.roomService = _roomService;
            this.clientService = _clientService;
        }

        public async Task Add(AddReservationFormModel model, string UserId)
        {
            var sanitalizer = new HtmlSanitizer();

            var reservation = new Infrastructure.Data.Еntities.Reservation();

            reservation.UserId = UserId;
            reservation.RoomNumberId = model.RoomNumberId;
            reservation.Arrival = model.Arrival;
            reservation.Leaving = model.Leaving;
            reservation.IsBreakfastIncluded = model.IsBreakfastIncluded;
            reservation.IsAllInclusive = model.IsAllInclusive;
            reservation.Clients.Add(clientService.GetClientById(model.ClientId));

            var room = roomService.GetById(model.RoomNumberId);

            reservation.Total = AnalyzeTotal(reservation.Clients.Where(c => c.IsAdult).Count(),
                                             reservation.Clients.Where(c => !c.IsAdult).Count(),
                                             room.PriceForAdultBed,
                                             room.PriceForChildBed,
                                             room.Capacity);

            await this.dbContext.Reservations.AddAsync(reservation);
            await this.dbContext.SaveChangesAsync();
        }


        public AllReservationQueryModel All(string searchTerm, string searchTermOn, int currentPage, int reservationsPerPage)
        {
            var reservationsQuery = this.dbContext.Reservations.Include(r => r.User).Include(r => r.Room).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                switch (searchTermOn)
                {
                    case "RoomNumber":
                        reservationsQuery = reservationsQuery.Where(r => r.Room.Number.ToString().Contains(searchTerm)).ToList();
                        break;
                    case "UserName":
                        reservationsQuery = reservationsQuery.Where(u => u.User.UserName.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                    case "PhoneNumber":
                        reservationsQuery = reservationsQuery.Where(u => u.Clients?.FirstOrDefault()?.PhoneNumber.CompareTo(searchTerm) == 0 ).ToList();
                        break;
                }
            }

            var reservations = reservationsQuery
                .Skip((currentPage - 1) * reservationsPerPage)
                .Take(reservationsPerPage)
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    RoomNumber = r.Room.Number,
                    Arrival = r.Arrival,
                    Leaving = r.Leaving,
                    IsAllInclusive = r.IsAllInclusive,
                    IsBreakfastIncluded = r.IsBreakfastIncluded,
                    Total = r.Total
                }).ToList();

            reservations = reservations.OrderBy(r => r.Arrival).ToList();

            var totalReservations = reservationsQuery.Count();

            return new AllReservationQueryModel()
            {
                ReservationsPerPage = reservationsPerPage,
                TotalReservationsCount = totalReservations,
                Reservations = reservations
            };
        }

        public decimal AnalyzeTotal(int numberAdults, int numberChildren, decimal prizeForAdult, decimal prizeForChildren, int capacity)
        {
            decimal total = numberAdults * prizeForAdult + numberChildren * prizeForChildren;

            if(numberAdults + numberChildren < capacity)
            {
                total += (capacity - (numberAdults + numberChildren)) * prizeForChildren;
            }

            return total;
        }

        public async Task Delete(int id)
        {
            var reservation = this.dbContext.Reservations.Where(r => r.Id == id).FirstOrDefault();
            this.dbContext.Reservations.Remove(reservation);
            await this.dbContext.SaveChangesAsync();
        }

        //public async Task Edit(AddClientFormModel model)
        //{
        //    var sanitalizer = new HtmlSanitizer();

        //    var client = this.dbContext.Clients.Where(c => c.Id == model.Id).FirstOrDefault();

        //    client.FirstName = sanitalizer.Sanitize(model.FirstName);
        //    client.LastName = sanitalizer.Sanitize(model.LastName);
        //    client.PhoneNumber = model.PhoneNumber;
        //    client.Email = sanitalizer.Sanitize(model.Email);
        //    client.IsAdult = model.IsAdult;

        //    dbContext.Clients.Update(client);

        //    try
        //    {
        //        await dbContext.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public AddClientFormModel GetById(int id)
        //{
        //    return this.dbContext.Clients.Where(c => c.Id == id)?.Select(c => new AddClientFormModel
        //    {
        //        Id = c.Id,
        //        FirstName = c.FirstName,
        //        LastName = c.LastName,
        //        PhoneNumber = c.PhoneNumber,
        //        Email = c.Email,
        //        IsAdult = c.IsAdult
        //    })?.FirstOrDefault();
        //}
        //public bool EmailExists(string email, int id)
        //{
        //    return this.dbContext.Clients.Where(c => c.Email.CompareTo(email) == 0 && c.Id != id).FirstOrDefault() != null;
        //}

        //public bool PhoneNumberExists(string phoneNumber, int id)
        //{
        //    return this.dbContext.Clients.Where(c => c.Email.CompareTo(phoneNumber) == 0 && c.Id != id).FirstOrDefault() != null;
        //}

        //public bool Exists(int id)
        //{
        //    return this.dbContext.Clients.Where(c => c.Id == id).FirstOrDefault() != null;
        //}

        //public DetailsClientViewModel ReservationDetails(int id, int currentPage, int reservationsPerPage)
        //{
        //    var client = this.dbContext.Clients.Where(c => c.Id == id).Include(c => c.Reservations).FirstOrDefault();
        //    var reservations = client.Reservations
        //                             .Skip((currentPage - 1) * reservationsPerPage)
        //                             .Take(reservationsPerPage)
        //                             .Select(r => new ReservationViewModel
        //                             {
        //                                 RoomNumber = r.Room.Number,
        //                                 Arrival = r.Arrival,
        //                                 Leaving = r.Leaving,
        //                                 IsBreakfastIncluded = r.IsBreakfastIncluded,
        //                                 IsAllInclusive = r.IsAllInclusive,
        //                                 Total = r.Total
        //                             }).ToList();

        //    var newClient = new DetailsClientViewModel()
        //    {
        //        Id = client.Id,
        //        FirstName = client.FirstName,
        //        LastName = client.LastName,
        //        Email = client.Email,
        //        PhoneNumber = client.PhoneNumber,
        //        IsAdult = client.IsAdult,
        //        Reservations = reservations
        //    };

        //    reservations = reservations.OrderByDescending(c => c.Leaving).ToList();

        //    var totalReservations = reservations.Count();

        //    newClient.ReservationsPerPage = reservationsPerPage;
        //    newClient.TotalReservationsCount = totalReservations;
        //    newClient.Reservations = reservations;

        //    return newClient;
        //}
    }
}
