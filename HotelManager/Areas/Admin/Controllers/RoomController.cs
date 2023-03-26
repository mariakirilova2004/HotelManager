using Microsoft.AspNetCore.Mvc;
using HotelManager.Core.Services.Room;
using HotelManager.Core.Models.Room;
using Microsoft.Extensions.Caching.Memory;
using HotelManager.Extensions;
using HotelManager.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using HotelManager.Core.Services.RoomType;
using Ganss.Xss;

namespace HotelManager.Areas.Admin.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class RoomController : AdminController
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

        [Route("Admin/Rooms")]
        [HttpGet]
        public IActionResult All([FromQuery] AllRoomsQueryModel query)
        {
            //await roomService.UpdateRooms();

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

        [HttpGet]
        public IActionResult Add()
        {
            if (!User.IsAdmin())
            {
                TempData[MessageConstant.WarningMessage] = "You cannot add Rooms!";
                this.logger.LogInformation("User {0} tried to add room, but they are not Admin!", this.User.Id());
                return RedirectToAction("All", "Room");
            }
            var model = new AddRoomFormModel
            {
                RoomTypes = roomTypeService.AllAdd()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRoomFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid add";
                model.RoomTypes = roomTypeService.AllAdd();
                return View(model);
            }

            if (roomService.NumberExists(model.Number, model.Id))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a room with this Room Number";
                model.RoomTypes = roomTypeService.AllAdd();
                return View(model);
            }

            try
            {         
                await roomService.Add(model);
            }
            catch (Exception e)
            {
                TempData[MessageConstant.ErrorMessage] = $"Unsuccessfully added room";
                model.RoomTypes = roomTypeService.AllAdd();
                return View(model);
            }

            TempData[MessageConstant.SuccessMessage] = $"Room Number {model.Number} has been successfully added";
            return RedirectToAction(nameof(All));
        }

        //Delete the room

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (!User.IsAdmin())
            {
                TempData[MessageConstant.WarningMessage] = "You cannot delete Rooms!";
                this.logger.LogInformation("User {0} tried to delete room, but they are not Admin!", this.User.Id());
                return RedirectToAction("Index", "Home");
            }

            try
            {
                await this.roomService.Delete(Id);
                TempData[MessageConstant.SuccessMessage] = "Successfully deleted room";
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted room";
                this.logger.LogInformation("Room {0} could not be deleted!", Id);
            }
            return RedirectToAction(nameof(All));
        }

        // Update data of a room

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            if (!User.IsAdmin())
            {
                TempData[MessageConstant.WarningMessage] = "You cannot edit Rooms!";
                this.logger.LogInformation("User {0} tried to edit room, but they are not Admin!", this.User.Id());
                return RedirectToAction(nameof(All));
            }

            var model = roomService.GetById(Id);

            if (model == null)
            {
                TempData[MessageConstant.WarningMessage] = "No such Room!";
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddRoomFormModel model)
        {

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid edit";
                return View(model);
            }

            if (roomService.NumberExists(model.Number, model.Id))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a room with this Room Number";
                model.RoomTypes = roomTypeService.AllAdd();
                return View(model);
            }

            try
            {
                await this.roomService.Edit(model);
            }
            catch (Exception)
            {
                this.logger.LogInformation("Room {0} did not manage to be edited!", model.Number);
                TempData[MessageConstant.ErrorMessage] = "Unsuccessful editing of a room";
                model.RoomTypes = roomTypeService.AllAdd();
                return View(model);
            }

            return RedirectToAction("All", "Room");
        }
    }
}
