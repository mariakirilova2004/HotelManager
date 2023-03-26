using Ganss.Xss;
using HotelManager.Core.Models.Client;
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

            var room = roomService.GetById(model.RoomNumberId);

            reservation.Total = AnalyzeTotal(reservation.Clients.Where(c => c.IsAdult).Count(),
                                             reservation.Clients.Where(c => !c.IsAdult).Count(),
                                             room.PriceForAdultBed,
                                             room.PriceForChildBed,
                                             room.Capacity,
                                             (reservation.Leaving - reservation.Arrival).Days);

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
                        reservationsQuery = reservationsQuery.Where(u => u.Clients.Any(c => c.PhoneNumber.CompareTo(searchTerm) == 0)).ToList();
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

        public decimal AnalyzeTotal(int numberAdults, int numberChildren, decimal prizeForAdult, decimal prizeForChildren, int capacity, int days)
        {
            decimal total = numberAdults * prizeForAdult + numberChildren * prizeForChildren;

            if (numberAdults + numberChildren < capacity)
            {
                total += (capacity - (numberAdults + numberChildren)) * prizeForChildren;
            }

            return total * days;
        }

        public async Task Delete(int id)
        {
            var reservation = this.dbContext.Reservations.Where(r => r.Id == id).FirstOrDefault();
            this.dbContext.Reservations.Remove(reservation);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task Edit(AddReservationFormModel model, string UserId)
        {
            var sanitalizer = new HtmlSanitizer();

            var reservation = this.dbContext.Reservations.Where(r => r.Id == model.Id).Include(r => r.Clients).FirstOrDefault();

            reservation.UserId = UserId;
            reservation.RoomNumberId = model.RoomNumberId;
            reservation.Arrival = model.Arrival;
            reservation.Leaving = model.Leaving;
            reservation.IsBreakfastIncluded = model.IsBreakfastIncluded;
            reservation.IsAllInclusive = model.IsAllInclusive;

            dbContext.Reservations.Update(reservation);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AddReservationFormModel GetById(int id)
        {
            return this.dbContext.Reservations.Where(r => r.Id == id)?.Select(r => new AddReservationFormModel
            {
                Id = r.Id,
                RoomNumberId = r.Room.Id,
                Arrival = r.Arrival,
                Leaving = r.Leaving,
                IsAllInclusive = r.IsAllInclusive,
                IsBreakfastIncluded = r.IsBreakfastIncluded
            })?.FirstOrDefault();
        }

        public bool Exists(int id)
        {
            return this.dbContext.Reservations.Where(r => r.Id == id).FirstOrDefault() != null;
        }

        public DetailsReservationViewModel ReservationDetails(int id, int currentPage, int clientsPerPage)
        {
            var reservation = this.dbContext.Reservations
                                            .Where(r => r.Id == id)
                                            .Include(c => c.Clients)
                                            .Include(r => r.Room)
                                            .ThenInclude(r => r.RoomType)
                                            .Include(r => r.User)
                                            .FirstOrDefault();

            var clientsQuery = reservation.Clients
                                     .Skip((currentPage - 1) * clientsPerPage)
                                     .Take(clientsPerPage)
                                     .Select(c => new ClientViewModel
                                     {
                                         Id = c.Id,
                                         FirstName = c.FirstName,
                                         LastName = c.LastName,
                                         PhoneNumber = c.PhoneNumber,
                                         IsAdult = c.IsAdult,
                                         Email = c.Email
                                     }).ToList();

            var newReservation = new DetailsReservationViewModel()
            {
                Id = reservation.Id,
                RoomNumber = reservation.Room.Number,
                RoomType = reservation.Room.RoomType.Type,
                UserName = reservation.User.UserName,
                Arrival = reservation.Arrival,
                Leaving = reservation.Leaving,
                IsBreakfastIncluded = reservation.IsBreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
                Total = reservation.Total
            };

            var totalReservations = reservation.Clients.Count();

            newReservation.ClientsPerPage = clientsPerPage;
            newReservation.TotalClientsCount = totalReservations;
            newReservation.CurrentPage = currentPage;
            newReservation.Clients = clientsQuery;

            return newReservation;
        }

        public async Task AddClient(AddClientReservationFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            var reservation = this.dbContext.Reservations.Where(r => r.Id == model.Id).Include(r => r.Clients).Include(r => r.Room).FirstOrDefault();

            reservation.Clients.Add(clientService.GetClientById(model.ClientId));

            this.dbContext.Reservations.Update(reservation);

            reservation.Total = AnalyzeTotal(reservation.Clients.Where(c => c.IsAdult).Count(),
                                             reservation.Clients.Where(c => !c.IsAdult).Count(),
                                             reservation.Room.PriceForAdultBed,
                                             reservation.Room.PriceForChildBed,
                                             reservation.Room.Capacity,
                                             (reservation.Leaving - reservation.Arrival).Days);

            this.dbContext.Reservations.Update(reservation);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteClient(int id, int clientId)
        {
            var sanitalizer = new HtmlSanitizer();

            var reservation = this.dbContext.Reservations.Where(r => r.Id == id).Include(r => r.Clients).Include(r => r.Room).FirstOrDefault();
            var client = clientService.GetClientById(clientId);

            reservation.Clients.Remove(client);

            this.dbContext.Reservations.Update(reservation);

            reservation.Total = AnalyzeTotal(reservation.Clients.Where(c => c.IsAdult).Count(),
                                             reservation.Clients.Where(c => !c.IsAdult).Count(),
                                             reservation.Room.PriceForAdultBed,
                                             reservation.Room.PriceForChildBed,
                                             reservation.Room.Capacity,
                                             (reservation.Leaving - reservation.Arrival).Days);

            this.dbContext.Reservations.Update(reservation);

            await this.dbContext.SaveChangesAsync();
        }

        public List<ReservationClientModel> ClientsForReservationDetails(int Id)
        {
            var reservation = this.dbContext.Reservations.Where(r => r.Id == Id).Include(r => r.Clients).FirstOrDefault();
            return this.dbContext.Clients
            .Where(c => !reservation.Clients.Contains(c))
            .Select(c => new ReservationClientModel()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                IsAdult = c.IsAdult
            })
            .ToList();
        }

        public bool IsFreeThatTime(DateTime arrival, DateTime leaving, int roomId)
        {
            var reservations = this.dbContext.Reservations.Where(r => r.RoomNumberId == roomId);

            return reservations.Any(r => (r.Arrival < arrival && r.Leaving < arrival) || (r.Arrival > leaving && r.Leaving > leaving));
        }
    }
}
