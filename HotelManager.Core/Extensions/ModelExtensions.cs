using HotelManager.Core.Models.Room;
using HotelManager.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HotelManager.Core.Extensions
{
    public static class ModelExtensions
    {
        public static string GetInformation(this UserViewModel user)
        {
            return user.Email.Replace("@", "_") + "_" + GetName(user.FirstName, user.MiddleName, user.LastName);
        }

        private static string GetName(string fn, string mn, string ln)
        {
            var name = string.Join("-", new List<string> { fn, mn, ln });
            return name;
        }

        public static string GetInformation(this RoomViewModel room)
        {
            return room.Capacity + "_" + room.Number + "_" + room.RoomType;
        }
    }
}
