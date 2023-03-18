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

            if (roomService.NumberExists(model.Number))
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

        ////Forget the data of the user but the id remains

        //[HttpPost]
        //public async Task<IActionResult> Forget(string Id)
        //{
        //    if (!User.IsAdmin())
        //    {
        //        TempData[MessageConstant.WarningMessage] = "You cannot delete Users!";
        //        this.logger.LogInformation("User {0} tried to delete user, but they are not Admin!", this.User.Id());
        //        return RedirectToAction("Index", "Home");
        //    }

        //    try
        //    {
        //        await this.users.Forget(Id);
        //        TempData[MessageConstant.SuccessMessage] = "Successfully deleted user";
        //    }
        //    catch (Exception)
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted user";
        //        this.logger.LogInformation("User {0} could not be deleted!", Id);
        //    }
        //    return RedirectToAction(nameof(All));
        //}

        //// Update data of a user

        //[HttpGet]
        //public async Task<IActionResult> Edit(string Id)
        //{
        //    if (!User.IsAdmin())
        //    {
        //        TempData[MessageConstant.WarningMessage] = "You cannot edit Users!";
        //        this.logger.LogInformation("User {0} tried to edit user, but they are not Admin!", this.User.Id());
        //        return RedirectToAction("All", "User");
        //    }

        //    var model = users.GetUserById(Id);

        //    if (model == null)
        //    {
        //        TempData[MessageConstant.WarningMessage] = "No such User!";
        //        return RedirectToAction(nameof(All));
        //    }

        //    var token = await userManager.GeneratePasswordResetTokenAsync(model);

        //    EditUserFormModel user = new EditUserFormModel()
        //    {
        //        Id = model.Id,
        //        UserName = model.UserName,
        //        FirstName = model.FirstName,
        //        MiddleName = model.MiddleName,
        //        LastName = model.LastName,
        //        PhoneNumber = model.PhoneNumber,
        //        EGN = model.EGN,
        //        Email = model.Email,
        //        HiringDate = model.HiringDate,
        //        DismissionDate = model.DismissionDate,
        //        IsActive = model.IsActive,
        //        Token = token
        //    };

        //    return View(user);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(EditUserFormModel model)
        //{
        //    var sanitalizer = new HtmlSanitizer();

        //    if (!ModelState.IsValid)
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Invalid edit";
        //        return View(model);
        //    }

        //    if (userManager.Users.Any(u => u.Email == model.Email && u.Id != model.Id))
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this email";
        //        return View(model);
        //    }

        //    if (userManager.Users.Any(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.Id))
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this Phone Number";
        //        return View(model);
        //    }

        //    try
        //    {
        //        await this.users.Edit(model);
        //        if (model.Password != null)
        //        {
        //            var resetPassResult = await userManager.ResetPasswordAsync(users.GetUserById(model.Id), model.Token, model.Password);
        //            if (!resetPassResult.Succeeded)
        //            {
        //                foreach (var error in resetPassResult.Errors)
        //                {
        //                    ModelState.TryAddModelError(error.Code, error.Description);
        //                    TempData[MessageConstant.ErrorMessage] = error.Description;
        //                }
        //                return View(model);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        this.logger.LogInformation("User {0} did not manage to be edited!", model.Id);
        //        TempData[MessageConstant.ErrorMessage] = "Unsuccessful editing of a user";
        //        return View(model);
        //    }

        //    return RedirectToAction("All", "User");
        //}
    }
}
