using HotelManager.Core.Models.RoomType;
using HotelManager.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Core.Services.RoomType
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly HotelManagerDbContext dbContext;

        public RoomTypeService(HotelManagerDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public List<RoomTypeViewModel> All()
        {
            return this.dbContext.RoomTypes.Select(rt => new RoomTypeViewModel()
            {
                Type = rt.Type,
                Description = rt.Description
            })
            .ToList();
        }

        public List<Infrastructure.Data.Еntities.RoomType> AllAdd()
        {
            return this.dbContext.RoomTypes.ToList();
        }

        public Infrastructure.Data.Еntities.RoomType FindById(int Id)
        {
            return this.dbContext.RoomTypes.Where(rt => rt.Id == Id).FirstOrDefault();
        }
    }
}
