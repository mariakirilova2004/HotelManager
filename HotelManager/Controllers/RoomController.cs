using Microsoft.AspNetCore.Mvc;
using HotelManager.Core.Services.Room;
using HotelManager.Core.Models.Room;
using Microsoft.Extensions.Caching.Memory;
using HotelManager.Extensions;
using HotelManager.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using HotelManager.Core.Services.RoomType;
using Ganss.Xss;

namespace HotelManager.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class RoomController : Controller
    {
        private readonly IRoomService roomService;
        private readonly IRoomTypeService roomTypeService;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;

        public RoomController(IRoomService _roomService,
                              IRoomTypeService _roomTypeService,
                              IMemoryCache _cache,
                              ILogger<RoomController> _logger)
        {
            this.roomService = _roomService;
            this.cache = _cache;
            this.logger = _logger;
            this.roomTypeService = _roomTypeService;
        }

        [Route("/AllRooms")]
        [HttpGet]
        public IActionResult All([FromQuery] AllRoomsQueryModel query)
        {
            var roomService = this.cache.Get<AllRoomsQueryModel>("RoomsCacheKey");
            if (roomService == null)
            {
                var queryResult = this.roomService.All(
                    query.Capacity,
                    query.Type,
                    query.Availability,
                    query.CurrentPage,
                    query.RoomsPerPage);

                query.TotalRoomsCount = queryResult.TotalRoomsCount;
                query.Rooms = queryResult.Rooms;
                query.RoomTypes = roomTypeService.All();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

                this.cache.Set("RoomsCacheKey", roomService, cacheOptions);
            }
            return View(query);
        }
    }
}
