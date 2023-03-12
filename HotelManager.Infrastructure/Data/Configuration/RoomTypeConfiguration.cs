using HotelManager.Infrastructure.Data.Еntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Infrastructure.Data.Configuration
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.HasData(CreateRoomTypes());
        }

        private List<RoomType> CreateRoomTypes()
        {
            var roomType = new List<RoomType>();

            var roomType1 = new RoomType()
            {
                Id = 1,
                Type = "Single Room",
                Description = "A room with a single bed that is intended for one person to sleep in.",
               
            };

            roomType.Add(roomType1);

            var roomType2 = new RoomType()
            {
                Id = 2,
                Type = "Double Room",
                Description = "A double room is a room intended for two people, usually a couple, to stay in. One person occupying a double room has to pay a supplement."
            };

            roomType.Add(roomType2);

            var roomType3 = new RoomType()
            {
                Id = 3,
                Type = "Apartment",
                Description = "A room or set of rooms fitted especially with housekeeping facilities."
            };

            roomType.Add(roomType3);

            var roomType4 = new RoomType()
            {
                Id = 4,
                Type = "Penthouse",
                Description = "A flat on the top floor of a tall building, that is luxuriously fitted."
            };

            roomType.Add(roomType4);

            var roomType5 = new RoomType()
            {
                Id = 5,
                Type = "Maisonette",
                Description = "A maisonette has the living space split over two floors and is luxuriously fitted."
            };

            roomType.Add(roomType5);

            return roomType;
        }
    }
}
