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
        //TO DO: Implement All() function to interface
        AllClientsQueryModel All();

        Task Add(AddClientFormModel model);
        Task Delete(int id);
        AddClientFormModel GetById(int id);
        Task Edit(AddClientFormModel model);
    }
}
