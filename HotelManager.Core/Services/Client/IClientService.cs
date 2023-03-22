using HotelManager.Core.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Client
{
    public interface IClientService
    {
        Task Add(AddClientFormModel model);
        Task Delete(int id);
        AddClientFormModel GetById(int id);
        Task Edit(AddClientFormModel model);
        AllClientsQueryModel All(string firstNameSearch, string lastNameSearch, int currentPage, int clientsPerPage);

        public bool EmailExists(string email, int id);
        public bool PhoneNumberExists(string phoneNumber, int id);
        bool Exists(int id);
        DetailsClientViewModel CauseDetailsById(int id);
    }
}
