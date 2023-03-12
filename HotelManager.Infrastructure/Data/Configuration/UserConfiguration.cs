using HotelManager.Infrastructure.Data.Еntities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Infrastructure.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(CreateUser());
        }

        private User CreateUser()
        {
            var hasher = new PasswordHasher<User>();

            var user = new User()
            {
                FirstName = "User",
                MiddleName = "Userov",
                LastName = "Userov",
                EGN = "0888888888",
                Email = "admin@mail.com",
                NormalizedEmail = "ADMIN@MAIL.COM",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "0888888888",
                HiringDate = new DateTime(01/01/2015),
                IsActive = true,
                DismissionDate = null
            };

            user.PasswordHash = hasher.HashPassword(user, "user123");
            return user;
        }
    }
}
