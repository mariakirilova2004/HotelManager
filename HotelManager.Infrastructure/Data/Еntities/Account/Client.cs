using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelManager.Infrastructure.Data.DataConstants.Client;

namespace HotelManager.Infrastructure.Data.Еntities.Account
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLengthAttribute(ClientMaxLengthFirstName, MinimumLength = ClientMinLengthFirstName)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(ClientMaxLengthLastName, MinimumLength = ClientMinLengthLastName)]
        public string LastName { get; set; }

        [Required]
        [MinLength(ClientPhoneNumberLength), MaxLength(ClientPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool IsAdult { get; set; }

        [Required]
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
