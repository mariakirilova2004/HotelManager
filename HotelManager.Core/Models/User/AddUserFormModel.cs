using HotelManager.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelManager.Infrastructure.Data.DataConstants.User;

namespace HotelManager.Core.Models.User
{
    public class AddUserFormModel
    {

        [Required]
        [StringLengthAttribute(UserMaxLengthUserName, MinimumLength = UserMinLengthUserName)]
        public string UserName { get; set; }

        [Required]
        [StringLengthAttribute(UserMaxLengthFirstName, MinimumLength = UserMinLengthFirstName)]
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

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required]
        [CustomHiringDate(ErrorMessage = "Enter valid Hiring date")]
        public DateTime HiringDate { get; set; }

        [Required]
        [StringLength(UserMaxLengthPassword, ErrorMessage = "Must be between {2} and {1} characters long.", MinimumLength = UserMinLengthPassword)]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } 
    }
}
