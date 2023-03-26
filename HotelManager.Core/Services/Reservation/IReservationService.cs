using HotelManager.Core.Models.Client;
using HotelManager.Core.Models.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Reservation
{
    public interface IReservationService
    {
        Task Add(AddReservationFormModel model, string UserId);
        Task Delete(int id);
        AddReservationFormModel GetById(int id);
        Task Edit(AddReservationFormModel model, string UserId);
        AllReservationQueryModel All(string searchTerm, string searchTermOn, int currentPage, int clientsPerPage);
        bool Exists(int id);
        DetailsReservationViewModel ReservationDetails(int id, int currentPage, int clientsPerPage);
        decimal AnalyzeTotal(int numberAdults, int numberChildren, decimal prizeForAdult, decimal prizeForChildren, int capacity, int days);
        Task AddClient(AddClientReservationFormModel model);
        List<ReservationClientModel> ClientsForReservationDetails(int Id);
        Task DeleteClient(int id, int clientId);
        bool IsFreeThatTime(DateTime arrival, DateTime leaving, int roomId);
    }
}
