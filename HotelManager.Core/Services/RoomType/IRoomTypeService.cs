using HotelManager.Core.Models.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.RoomType
{
    public interface IRoomTypeService
    {
        List<RoomTypeViewModel> All();
        List<Infrastructure.Data.Еntities.RoomType> AllAdd();

        Infrastructure.Data.Еntities.RoomType FindById(int Id);
    }
}
