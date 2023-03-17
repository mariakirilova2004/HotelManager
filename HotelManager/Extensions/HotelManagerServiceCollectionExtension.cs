﻿using Microsoft.EntityFrameworkCore;
using HotelManager.Controllers;
using HotelManager.Core.Services.User;
using HotelManager.Core.Services.Room;
using HotelManager.Core.Services.RoomType;

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

            return services;
        }
    }
}
