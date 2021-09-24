using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CustomerService.Models;
using CustomerService.Manager;
using CustomerService.Standard;

namespace CustomerService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController 
        : ControllerBase
    {
        private readonly IBizManager<Customer> custbizManager;                        

        public CustomerController(IBizManager<Customer> custbizmanager)                
        {
            custbizManager = custbizmanager;
        }

        [HttpGet]                                                                   
        public IActionResult GetAllCustomer() 
        {
            var response = custbizManager.GetAll();
            if(response == null)                                                    
            {
                return base.NotFound();                                               
            }
            return base.Ok(response);                                                  
        }

        [HttpPost]                                                                  
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName) || string.IsNullOrWhiteSpace(customer.DOB))
            {
                return UnprocessableEntity("All the details are required");          
            }
            custbizManager.Add(customer);
            return CreatedAtRoute("GetCustomerByID", new { id = customer.Id }, customer);     
        }

        [HttpGet("{id}",Name = "GetCustomerByID")]                                   
        public IActionResult GetCustomerByID(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid customer ID.");                         
            }

            var response = custbizManager.GetByID(id);
            if(response == null)
            {
                return base.NotFound();                                                  
            }
            return base.Ok(response);                                                    
        }

        [HttpPut("{id}")]                                                           
        public IActionResult UpdateCustomerByID(string id, [FromBody] Customer customer)
        {
            if(string.IsNullOrWhiteSpace(id) || id != customer.Id || string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName) || string.IsNullOrWhiteSpace(customer.DOB))
            {
                return base.BadRequest();                                                
            }

            var customerById = custbizManager.GetByID(id);
            if(customerById == null)
            {
                return base.NotFound();                                                
            }
            custbizManager.UpdateByID(id, customer);
            return new NoContentResult();                                        
        }

        [Authorize(Policy = Policies.Admin)]                                        
        [HttpDelete("{id}")]                                                        
        public IActionResult DeleteCustomerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return base.BadRequest();                                                
            }

            var response = custbizManager.DeleteByID(id);
            if(!response)
            {
                return base.NotFound();                                                  
            }
            return base.NoContent();                                                     
        }
    }
}