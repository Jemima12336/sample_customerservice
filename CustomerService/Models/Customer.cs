using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class Customer
    {
        public String Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String DOB { get; set; }
        

        [Required (ErrorMessage ="SSN is Required")]
        [RegularExpression (@"^\d{10}|\d{3}-\d{2}-\d{5}$",ErrorMessage ="Invalid Social Security Number")]
        public String SSN { get; set; }
        
    }
}
