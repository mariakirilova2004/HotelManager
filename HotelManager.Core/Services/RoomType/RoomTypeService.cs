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
    }
}
