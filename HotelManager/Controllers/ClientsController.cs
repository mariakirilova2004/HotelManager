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

namespace HotelManager.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ClientsController : Controller
    {
        private readonly IClientService clientService;
        private readonly ILogger logger;

        public ClientsController(IClientService clientService, ILogger logger)
        {
            this.clientService = clientService;
            this.logger = logger;
        }

        [ResponseCache(Duration = 60)]
        public IActionResult All([FromQuery] AllClientsQueryModel query)
        {
            // TO DO: Do the fucking query and return all clients view :)
            var queryResult = this.clientService.All();
            return View(query);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int? id)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Login", "User");

            if (id == null)
            {
                return NotFound();
            }

            AddClientFormModel model = new AddClientFormModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddClientFormModel model)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                this.logger.LogInformation($"Unauthenticated user tried adding a client.");
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                await clientService.Add(model);
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }
    }
}
