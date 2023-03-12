using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Infrastructure.Data
{
    public class DataConstants
    {
        public class User
        {
            public const int UserMinLengthFirstName = 2;
            public const int UserMaxLengthFirstName = 20;

            public const int UserMinLengthMiddleName = 2;
            public const int UserMaxLengthMiddleName = 20;

            public const int UserMinLengthLastName = 2;
            public const int UserMaxLengthLastName = 20;

            public const int UserEGNLength = 10;

            public const int UserPhoneNumberLength = 10;
        }

        public class Client
        {
            public const int ClientMinLengthFirstName = 2;
            public const int ClientMaxLengthFirstName = 20;
                             
            public const int ClientMinLengthLastName = 2;
            public const int ClientMaxLengthLastName = 20;
                             
            public const int ClientPhoneNumberLength = 10;
        }

        public class Room
        {
            public const int RoomMinCapacity = 1;
            public const int RoomMaxCapacity = 8;

            public const int RoomMinPriceForAdultBed = 0;
            public const int RoomMaxPriceForAdultBed = 100000;

            public const int RoomMinPriceForChildBed = 0;
            public const int RoomMaxPriceForChildBed = 100000;

            public const int RoomMinNumber = 0;
            public const int RoomMaxNumber = 7500;
        }

        public class Reservation
        {
            public const int RoomMinCapacity = 1;
            public const int RoomMaxCapacity = 8;

            public const int RoomMinPriceForAdultBed = 0;
            public const int RoomMaxPriceForAdultBed = 100000;

            public const int RoomMinPriceForChildBed = 0;
            public const int RoomMaxPriceForChildBed = 100000;

            public const int RoomMinNumber = 0;
            public const int RoomMaxNumber = 7500;
        }
    }
}
