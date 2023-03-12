using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelManager.Infrastructure.Data.DataConstants.User;
using HotelManager.Infrastructure.Attributes;

namespace HotelManager.Infrastructure.Data.Еntities.Account
{
    public class User : IdentityUser
    {

        [Required]
        [StringLengthAttribute(UserMaxLengthFirstName, MinimumLength =UserMinLengthFirstName)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(UserMaxLengthMiddleName, MinimumLength = UserMinLengthMiddleName)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(UserMaxLengthLastName, MinimumLength = UserMinLengthLastName)]
        public string LastName { get; set; }

        [Required]
        [StringLength(UserEGNLength, MinimumLength = UserEGNLength)]
        public string EGN { get; set; }

        [Required]
        [StringLength(UserPhoneNumberLength, MinimumLength = UserPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [Required]
        [CustomHiringDate(ErrorMessage = "Enter valid Hiring date")]
        public DateTime HiringDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [CustomDismissionDate(ErrorMessage = "Enter valid Dismission date")]
        public DateTime? DismissionDate { get; set; }
    }
}
