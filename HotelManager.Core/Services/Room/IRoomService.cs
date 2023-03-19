using HotelManager.Core.Models.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.Room
{
    public interface IRoomService
    {
        AllRoomsQueryModel All(int? capacity = null, 
                               string type = "", 
                               bool availability = false,
                               int currentPage = 1,
                               int roomsPerPage = 1);
        bool NumberExists(int number, int id);

        Task Add(AddRoomFormModel model);
        Task Delete(int id);
        AddRoomFormModel GetById(int id);
        Task Edit(AddRoomFormModel model);
    }
}
