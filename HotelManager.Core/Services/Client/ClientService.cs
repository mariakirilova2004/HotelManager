using Ganss.Xss;
using HotelManager.Core.Models.Client;
using HotelManager.Core.Models.Reservation;
using HotelManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Client
{
    public class ClientService : IClientService
    {
        private readonly HotelManagerDbContext dbContext;

        public ClientService(HotelManagerDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public async Task Add(AddClientFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            var client = new Infrastructure.Data.Еntities.Client();

            client.FirstName = sanitalizer.Sanitize(model.FirstName);
            client.LastName = sanitalizer.Sanitize(model.LastName);
            client.PhoneNumber = model.PhoneNumber;
            client.Email = sanitalizer.Sanitize(model.Email);
            client.IsAdult = model.IsAdult;

            await this.dbContext.Clients.AddAsync(client);
            await this.dbContext.SaveChangesAsync();
        }


        public AllClientsQueryModel All(string firstNameSearch, string lastNameSearch, int currentPage, int clientsPerPage)
        {
            var clientsQuery = this.dbContext.Clients.ToList();

            if (firstNameSearch != null && firstNameSearch != "")
            {
                clientsQuery = clientsQuery.Where(cq => cq.FirstName == firstNameSearch).ToList();
            }

            if (lastNameSearch != null && lastNameSearch != "")
            {
                clientsQuery = clientsQuery.Where(cq => cq.LastName == lastNameSearch).ToList();
            }

            var clients = clientsQuery
                .Skip((currentPage - 1) * clientsPerPage)
                .Take(clientsPerPage)
                .Select(c => new ClientViewModel
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    IsAdult = c.IsAdult,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email
                }).ToList();

            clients = clients.OrderBy(c => c.FirstName).ToList();

            var totalClients = clientsQuery.Count();

            return new AllClientsQueryModel()
            {
                ClientsPerPage = clientsPerPage,
                TotalClientsCount = totalClients,
                Clients = clients
            };
        }

        public async Task Delete(int id)
        {
            var client = this.dbContext.Clients.Where(c => c.Id == id).FirstOrDefault();
            this.dbContext.Clients.Remove(client);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task Edit(AddClientFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            var client = this.dbContext.Clients.Where(c => c.Id == model.Id).FirstOrDefault();

            client.FirstName = sanitalizer.Sanitize(model.FirstName);
            client.LastName = sanitalizer.Sanitize(model.LastName);
            client.PhoneNumber = model.PhoneNumber;
            client.Email = sanitalizer.Sanitize(model.Email);
            client.IsAdult = model.IsAdult;

            dbContext.Clients.Update(client);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AddClientFormModel GetById(int id)
        {
            return this.dbContext.Clients.Where(c => c.Id == id)?.Select(c => new AddClientFormModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                IsAdult = c.IsAdult
            })?.FirstOrDefault();
        }
        public bool EmailExists(string email, int id)
        {
            return this.dbContext.Clients.Where(c => c.Email.CompareTo(email) == 0 && c.Id != id).FirstOrDefault() != null;
        }

        public bool PhoneNumberExists(string phoneNumber, int id)
        {
            return this.dbContext.Clients.Where(c => c.Email.CompareTo(phoneNumber) == 0 && c.Id != id).FirstOrDefault() != null;
        }

        public bool Exists(int id)
        {
            return this.dbContext.Clients.Where(c => c.Id == id).FirstOrDefault() != null;
        }

        public DetailsClientViewModel ReservationDetails(int id, int currentPage, int reservationsPerPage)
        {            
            var client = this.dbContext.Clients.Where(c => c.Id == id).Include(c => c.Reservations).FirstOrDefault();
            var reservations = client.Reservations
                                     .Skip((currentPage - 1) * reservationsPerPage)
                                     .Take(reservationsPerPage)
                                     .Select(r => new ReservationViewModel 
                                     { 
                                         RoomNumber = r.Room.Number,
                                         Arrival = r.Arrival,
                                         Leaving = r.Leaving,
                                         IsBreakfastIncluded = r.IsBreakfastIncluded,
                                         IsAllInclusive = r.IsAllInclusive,
                                         Total = r.Total
                                     }).ToList();

            var newClient =  new DetailsClientViewModel()
                    {
                        Id = client.Id,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Email = client.Email,
                        PhoneNumber = client.PhoneNumber,
                        IsAdult = client.IsAdult,
                        Reservations = reservations
                    };

            reservations = reservations.OrderByDescending(c => c.Leaving).ToList();

            var totalReservations = reservations.Count();

            newClient.ReservationsPerPage = reservationsPerPage;
            newClient.TotalReservationsCount = totalReservations;
            newClient.Reservations = reservations;

            return newClient;
        }
    }
}
