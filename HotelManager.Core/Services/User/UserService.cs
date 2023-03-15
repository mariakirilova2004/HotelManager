﻿using HotelManager.Core.Models.User;
using HotelManager.Infrastructure.Data;
using HotelManager.Infrastructure.Data.Еntities.Account;

namespace HotelManager.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly HotelManagerDbContext dbContext;

        public UserService(HotelManagerDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public AllUsersQueryModel All(string searchTerm = "",
                                      string searchTermOn = "FirstName",
                                          int currentPage = 1,
                                          int usersPerPage = 1)
        {
            var usersQuery = new List<UserViewModel>();

            usersQuery = this.dbContext.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                HiringDate = u.HiringDate,
                DismissionDate = (DateTime) u.DismissionDate,
                IsActive = u.IsActive
            }).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                switch (searchTermOn)
                {
                    case "UserName": usersQuery = usersQuery.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                    case "FirstName": usersQuery = usersQuery.Where(u => u.FirstName.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                    case "MiddleName": usersQuery = usersQuery.Where(u => u.MiddleName.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                    case "LastName": usersQuery = usersQuery.Where(u => u.LastName.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                    case "Email": usersQuery = usersQuery.Where(u => u.Email.ToLower().Contains(searchTerm.ToLower())).ToList();
                        break;
                }

                usersQuery.OrderByDescending(c => c.Id).ToList();
            }

            var totalUsers = usersQuery.Count();

            return new AllUsersQueryModel()
            {
                UsersPerPage = usersPerPage,
                TotalUsersCount = totalUsers,
                Users = usersQuery
            };
        }

        //public bool EmailExists(string email)
        //{
        //    return this.repository.All<Infrastructure.Data.Entities.Account.User>(u => u.Email == email).ToList().Count > 0;
        //}

        public async Task Forget(string Id)
        {
            try
            {
                Infrastructure.Data.Еntities.Account.User user = this.dbContext.Users.Where(u => u.Id == Id).FirstOrDefault();

                if(user != null)
                {
                    user.IsActive = false;
                    user.DismissionDate = DateTime.Now;
                    this.dbContext.Update(user);
                    await this.dbContext.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        //public bool IdExists(string userId)
        //{
        //    return this.repository.All<Infrastructure.Data.Entities.Account.User>(u => u.Id == userId).ToList().Count > 0;
        //}

        //public bool NameExists(string name)
        //{
        //    return this.repository.All<Infrastructure.Data.Entities.Account.User>(u => u.UserName == name).ToList().Count > 0;
        //}
    }
}
