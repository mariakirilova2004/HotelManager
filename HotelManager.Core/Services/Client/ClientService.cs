using Ganss.Xss;
using HotelManager.Core.Models.Client;
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
            var client = new Infrastructure.Data.Еntities.Client()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                IsAdult = model.IsAdult
            };

            await this.dbContext.Clients.AddAsync(client);
            await this.dbContext.SaveChangesAsync();
        }

        public AllClientsQueryModel All()
        {
            // TO DO: Implement All() function
            return new AllClientsQueryModel()
            {
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

            client.FirstName = model.FirstName;
            client.LastName = model.LastName;
            client.PhoneNumber = model.PhoneNumber;
            client.Email = model.Email;
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
    }
}
