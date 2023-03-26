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

            if(!reservationService.IsFreeThatTime(model.Arrival, model.Leaving, model.RoomNumberId))
            {
                TempData[MessageConstant.ErrorMessage] = "These dates are not available";
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

        //Update data of a room

       [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot edit Reservations!";
                this.logger.LogInformation("User {0} tried to edit reservation, but they are not User!", this.User.Id());
                return RedirectToAction("Index", "Home");
            };

            var model = reservationService.GetById(Id);

            if (model == null)
            {
                TempData[MessageConstant.WarningMessage] = "No such Reservation!";
                return RedirectToAction(nameof(All));
            }

            model.Clients = clientService.ClientsForReservationDetails();
            model.Rooms = roomService.RoomsForReservationDetails();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddReservationFormModel model)
        {

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid edit";
                model.Clients = clientService.ClientsForReservationDetails();
                model.Rooms = roomService.RoomsForReservationDetails();
                return View(model);
            }

            if (!reservationService.Exists(model.Id))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is no such reservation";
                return RedirectToAction(nameof(All));
            }

            try
            {
                await this.reservationService.Edit(model, User.Id());
                TempData[MessageConstant.SuccessMessage] = "Successfully edited reservation";
            }
            catch (Exception)
            {
                this.logger.LogInformation("Reservation {0} did not manage to be edited!", model.Id);
                TempData[MessageConstant.ErrorMessage] = "Unsuccessful editing of a reservation";
                model.Clients = clientService.ClientsForReservationDetails();
                model.Rooms = roomService.RoomsForReservationDetails();
                return View(model);
            }

            return RedirectToAction("All", "Reservation");
        }

        [HttpGet]
        public async Task<IActionResult> Details(DetailsReservationViewModel query)
        {
            if (!this.reservationService.Exists(query.Id))
            {
                TempData[MessageConstant.WarningMessage] = "There is no such reservation!";
                this.logger.LogInformation("User {0} tried to access invalid reservation!", this.User.Id());
                return RedirectToAction(nameof(All));
            }

            var queryResult = this.reservationService.ReservationDetails(
                query.Id,
                query.CurrentPage,
                query.ClientsPerPage);


            return View(queryResult);
        }

        [HttpGet]
        public IActionResult AddClient(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot add clients to Reservation!";
                this.logger.LogInformation("User {0} tried to add client to reservation, but they are not User!", this.User.Id());
                return RedirectToAction("Index", "Home");
            };

            var model = new AddClientReservationFormModel()
            {
                Id = id,
                Clients = reservationService.ClientsForReservationDetails(id)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient(AddClientReservationFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid add";
                model.Clients = clientService.ClientsForReservationDetails();
                return View(model);
            }

            try
            {
                await reservationService.AddClient(model);
            }
            catch (Exception e)
            {
                TempData[MessageConstant.ErrorMessage] = $"Unsuccessfully added client to reservation";
                model.Clients = clientService.ClientsForReservationDetails();
                return View(model);
            }

            TempData[MessageConstant.SuccessMessage] = $"Reservation Number {model.Id} has been successfully edited";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        //Delete the room

        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id, int clientId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot delete Reservation's Client!";
                this.logger.LogInformation("User {0} tried to delete reservation's client, but they are not User!", this.User.Id());
                return RedirectToAction("Index", "Home");
            };

            try
            {
                await this.reservationService.DeleteClient(id, clientId);
                TempData[MessageConstant.SuccessMessage] = "Successfully deleted reservation's client";
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted reservation's client";
                this.logger.LogInformation("Reservation's client could not be deleted!");
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
