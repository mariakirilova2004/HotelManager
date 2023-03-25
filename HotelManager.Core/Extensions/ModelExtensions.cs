using HotelManager.Core.Models.Client;
using HotelManager.Core.Models.Reservation;
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

        public static string GetInformation(this ClientViewModel client)
        {
            return client.Email.Replace("@", "_") + "_" + GetName(client.FirstName, client.PhoneNumber, client.LastName);
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

        public static string GetInformation(this ReservationViewModel reservation)
        {
            return reservation.RoomNumber + "_" + reservation.Id + "_" + reservation.Leaving;
        }
    }
}
