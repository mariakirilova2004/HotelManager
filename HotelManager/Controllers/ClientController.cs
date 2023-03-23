using HotelManager.Core.Models.Client;
using HotelManager.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using HotelManager.Infrastructure.Data.Еntities;
using HotelManager.Core.Services.Client;
using HotelManager.Core.Constants;
using Ganss.Xss;
using HotelManager.Core.Extensions;

namespace HotelManager.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ClientController : Controller
    {
        private readonly IClientService clientService;
        private readonly ILogger logger;

        public ClientController(IClientService clientService,
                                ILogger logger)
        {
            this.clientService = clientService;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult All([FromQuery] AllClientsQueryModel query)
        {
            var queryResult = this.clientService.All(
                query.FirstNameSearch,
                query.LastNameSearch,
                query.CurrentPage,
                query.ClientsPerPage);

            query.TotalClientsCount = queryResult.TotalClientsCount;
            query.Clients = queryResult.Clients;

            return View(query);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot add Clients!";
                this.logger.LogInformation("User {0} tried to add client, but they are not athenticated!", this.User.Id());
                return RedirectToAction("Login", "User");
            }

            AddClientFormModel model = new AddClientFormModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddClientFormModel model)
        {
            var sanitalizer = new HtmlSanitizer();

            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot add Clients!";
                this.logger.LogInformation($"Unauthenticated user tried add a client.");
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid add";
                return View(model);
            }

            if (clientService.EmailExists(model.Email, 0))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this email";
                return View(model);
            }

            if (clientService.PhoneNumberExists(model.PhoneNumber, 0))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this Phone Number";
                return View(model);
            }

            try
            {
                await clientService.Add(model);
                TempData[MessageConstant.SuccessMessage] = $"{model.FirstName} profile has been successfully added";
                return RedirectToAction("All", "Client");
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Something went wrong!";

                return View(model);
            }

            TempData[MessageConstant.ErrorMessage] = "Something went wrong!";

            return View(model);
        }

        //Delete the client

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot delete Clients!";
                this.logger.LogInformation("User {0} tried to delete client, but they are not authenticated!", this.User.Id());
                return RedirectToAction("Index", "Home");
            }

            try
            {
                await this.clientService.Delete(Id);
                TempData[MessageConstant.SuccessMessage] = "Successfully deleted client";
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = "Unsuccessfully deleted client";
                this.logger.LogInformation("Client {0} could not be deleted!", Id);
            }
            return RedirectToAction(nameof(All));
        }

        // Update data of a client

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData[MessageConstant.WarningMessage] = "You cannot delete Clients!";
                this.logger.LogInformation("User {0} tried to delete client, but they are not authenticated!", this.User.Id());
                return RedirectToAction("Index", "Home");
            }

            var model = clientService.GetById(Id);

            if (model == null)
            {
                TempData[MessageConstant.WarningMessage] = "No such Client!";
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddClientFormModel model)
        {

            if (!ModelState.IsValid)
            {
                TempData[MessageConstant.ErrorMessage] = "Invalid edit";
                return View(model);
            }

            if (clientService.EmailExists(model.Email, model.Id))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this email";
                return View(model);
            }

            if (clientService.PhoneNumberExists(model.PhoneNumber, model.Id))
            {
                TempData[MessageConstant.ErrorMessage] = "Тhere is a user with this Phone Number";
                return View(model);
            }

            try
            {
                await this.clientService.Edit(model);
            }
            catch (Exception)
            {
                this.logger.LogInformation("Client {0} did not manage to be edited!", model.FirstName);
                TempData[MessageConstant.ErrorMessage] = "Unsuccessful editing of a client";
                return View(model);
            }

            return RedirectToAction("All", "Client");
        }

        [HttpGet]
        public async Task<IActionResult> Details(DetailsClientViewModel query)
        {
            if (! this.clientService.Exists(query.Id))
            {
                TempData[MessageConstant.WarningMessage] = "There is no such client!";
                this.logger.LogInformation("User {0} tried to access invalid client!", this.User.Id());
                return RedirectToAction(nameof(All));
            }

            var queryResult = this.clientService.ReservationDetails(
                query.Id,
                query.CurrentPage,
                query.ReservationsPerPage);


            return View(queryResult);
        }
    }
}
