using Microsoft.EntityFrameworkCore;
using HotelManager.Controllers;
using HotelManager.Core.Services.User;
using HotelManager.Core.Services.Room;
using HotelManager.Core.Services.RoomType;
using HotelManager.Core.Services.Client;
using HotelManager.Core.Services.Reservation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HotelManagerServiceCollectionExtension
    {
        public static IServiceCollection AddHotelManagerServices(this IServiceCollection services)
        {
            services.AddScoped<ILogger, Logger<UserController>>();
            services.AddScoped<ILogger, Logger<UserService>>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IReservationService, ReservationService>();

            return services;
        }
    }
}
