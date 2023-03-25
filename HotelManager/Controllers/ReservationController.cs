using HotelManager.Core.Constants;
using HotelManager.Core.Models.Reservation;
using HotelManager.Core.Services.Client;
using HotelManager.Core.Services.Reservation;
using HotelManager.Core.Services.Room;
using HotelManager.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManager.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ReservationController : Controller
    {
        private readonly IRoomService roomService;
        private readonly IClientService clientService;
        private readonly IReservationService reservationService;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;

        public ReservationController(IRoomService _roomService,
                              IClientService _clientService,
                              IReservationService _reservationService,
                              IMemoryCache _cache,
                              ILogger<RoomController> _logger)
        {
            this.roomService = _roomService;
            this.reservationService = _reservationService;
            this.clientService = _clientService;
            this.cache = _cache;
            this.logger = _logger;
        }

        [HttpGet]
        public IActionResult All([FromQuery] AllReservationQueryModel query)
        {
            if (reservationService != null)
            {
                var queryResult = this.reservationService.All(
                    query.SearchTerm,
                    query.SearchTermOn,
                    query.CurrentPage,
                    query.ReservationsPerPage);

                query.TotalReservationsCount = queryResult.TotalReservationsCount;
                query.Reservations = queryResult.Reservations;
            }
            return View(query);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot make Reservations!";
                this.logger.LogInformation("User {0} tried to make reservation, but they are not User!", this.User.Id());
                return RedirectToAction("Index", "Home");
            };
            var model = new AddReservationFormModel()
            {
                Clients = clientService.ClientsForReservationDetails(),
                Rooms = roomService.RoomsForReservationDetails()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddReservationFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid add";
                model.Clients = clientService.ClientsForReservationDetails();
                model.Rooms = roomService.RoomsForReservationDetails();
                return View(model);
            }

            if(model.Leaving.CompareTo(model.Arrival) <= 0)
            {
                TempData[MessageConstant.ErrorMessage] = "Arrival date should be before leaving date!";
                model.Clients = clientService.ClientsForReservationDetails();
                model.Rooms = roomService.RoomsForReservationDetails();
                return View(model);
            }

            try
            {
                await reservationService.Add(model, User.Id());
            }
            catch (Exception e)
            {
                TempData[MessageConstant.ErrorMessage] = $"Unsuccessfully made reservation";
                model.Clients = clientService.ClientsForReservationDetails();
                model.Rooms = roomService.RoomsForReservationDetails();
                return View(model);
            }

            TempData[MessageConstant.SuccessMessage] = $"Reservation Number {model.Id} has been successfully made";
            return RedirectToAction(nameof(All));
        }

        //Delete the room

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot delete Reservations!";
                this.logger.LogInformation("User {0} tried to delete reservation, but they are not User!", this.User.Id());
                return RedirectToAction("Index", "Home");
            };

            try
            {
                await this.reservationService.Delete(Id);
                TempData[MessageConstant.SuccessMessage] = "Successfully deleted reservation";
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted reservation";
                this.logger.LogInformation("Reservation {0} could not be deleted!", Id);
            }
            return RedirectToAction(nameof(All));
        }

        // Update data of a room

        //[HttpGet]
        //public async Task<IActionResult> Edit(int Id)
        //{
        //    if (!User.IsAdmin())
        //    {
        //        TempData[MessageConstant.WarningMessage] = "You cannot edit Rooms!";
        //        this.logger.LogInformation("User {0} tried to edit room, but they are not Admin!", this.User.Id());
        //        return RedirectToAction(nameof(All));
        //    }

        //    var model = roomService.GetById(Id);

        //    if (model == null)
        //    {
        //        TempData[MessageConstant.WarningMessage] = "No such Room!";
        //        return RedirectToAction(nameof(All));
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(AddRoomFormModel model)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Invalid edit";
        //        return View(model);
        //    }

        //    if (roomService.NumberExists(model.Number, model.Id))
        //    {
        //        TempData[MessageConstant.ErrorMessage] = "Тhere is a room with this Room Number";
        //        model.RoomTypes = roomTypeService.AllAdd();
        //        return View(model);
        //    }

        //    try
        //    {
        //        await this.roomService.Edit(model);
        //    }
        //    catch (Exception)
        //    {
        //        this.logger.LogInformation("Room {0} did not manage to be edited!", model.Number);
        //        TempData[MessageConstant.ErrorMessage] = "Unsuccessful editing of a room";
        //        model.RoomTypes = roomTypeService.AllAdd();
        //        return View(model);
        //    }

        //    return RedirectToAction("All", "Room");
        //}

        //[HttpGet]
        //public async Task<IActionResult> Details(DetailsClientViewModel query)
        //{
        //    if (!this.clientService.Exists(query.Id))
        //    {
        //        TempData[MessageConstant.WarningMessage] = "There is no such client!";
        //        this.logger.LogInformation("User {0} tried to access invalid client!", this.User.Id());
        //        return RedirectToAction(nameof(All));
        //    }

        //    var queryResult = this.clientService.ReservationDetails(
        //        query.Id,
        //        query.CurrentPage,
        //        query.ReservationsPerPage);


        //    return View(queryResult);
        //}
    }
}
