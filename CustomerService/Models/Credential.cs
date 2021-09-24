using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class Credential
    {
        public String UserName { get; set; }
        public String FullName { get; set; }
        public String Password { get; set; }

        public String UserRole { get; set; }
    }
}
