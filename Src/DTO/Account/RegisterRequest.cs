using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aduaba.DTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("FirstName"), StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("LastName"), StringLength(50)]
        public string LastName { get; set; }
        [NotMapped]
       
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required, StringLength(11)]
        public string PhoneNumber { get; set; }
        public string HouseNumber { get; set; }
        [Required(ErrorMessage = "Street name is required")]
        [StringLength(50)]
        public string StreetName { get; set; }
        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("PostalCode")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is reqiuired")]
        [StringLength(40)]
        public string Country { get; set; }

    }
}
