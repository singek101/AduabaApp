using Aduaba.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aduaba.Models
{
    public class ApplicationUser:IdentityUser
    {
        [PersonalData]
        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("FirstName"), StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("LastName"), StringLength(50)]
        public string LastName { get; set; }
        
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

        [NotMapped]
        public string Address => $"{HouseNumber}, {StreetName}\n {City}, {State}\n{Country}\n Postal Code:{PostalCode}";

        public List<RefreshToken> RefreshTokens { get; set; }
       
        public virtual IEnumerable<Card> Cards { get; set; }
        public virtual IEnumerable<Cart> Cart { get; set; }
        public virtual IEnumerable<CartItem> CartItems { get; set; }
        public virtual IEnumerable<WishList> Wishlist { get; set; }
        public virtual IEnumerable<Order> Orders { get; set; }
        public virtual IEnumerable<PaymentHistory> PaymentHistories { get; set; }
    }
}