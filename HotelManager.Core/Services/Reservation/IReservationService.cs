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
        //AddReservationFormModel GetById(int id);
        //Task Edit(AddReservationFormModel model);
        AllReservationQueryModel All(string searchTerm, string searchTermOn, int currentPage, int clientsPerPage);
        //public bool EmailExists(string email, int id);
        //public bool PhoneNumberExists(string phoneNumber, int id);
        //bool Exists(int id);
        //DetailsReservationModel ReservationDetails(int id, int currentPage, int reservationsPerPage);
        decimal AnalyzeTotal(int numberAdults, int numberChildren, decimal prizeForAdult, decimal prizeForChildren, int capacity);
    }
}
