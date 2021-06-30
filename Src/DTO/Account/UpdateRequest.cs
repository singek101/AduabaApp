using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aduaba.DTO
{
    public class UpdateRequest
    {
      
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string HouseNumber { get; set; }
        
        public string StreetName { get; set; }
        
        public string City { get; set; }

       
        public string State { get; set; }

        
        public string PostalCode { get; set; }

       
        public string Country { get; set; }

       
        public string Address => $"{HouseNumber}, {StreetName}\n {City}, {State}\n{Country}\n Postal Code:{PostalCode}";


    }
}
