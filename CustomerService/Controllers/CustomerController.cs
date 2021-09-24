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

        [HttpGet]                                                                   //GET:api/customer
        public IActionResult GetAllCustomer() 
        {
            var response = custbizManager.GetAll();
            if(response == null)                                                    
            {
                return base.NotFound();                                                   //404      
            }
            return base.Ok(response);                                                    //200 - OK  
        }

        [HttpPost]                                                                  //POST:api/customer
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName) || string.IsNullOrWhiteSpace(customer.DOB))
            {
                return UnprocessableEntity("All the details are required");          //422 - UnprocessableEntity
            }
            custbizManager.Add(customer);
            return CreatedAtRoute("GetCustomerByID", new { id = customer.Id }, customer);     //Attaching ID to new customer
        }

        [HttpGet("{id}",Name = "GetCustomerByID")]                                   //GET:api/customer/1234567
        public IActionResult GetCustomerByID(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid customer ID.");                          // 400 - BAD REQUEST
            }

            var response = custbizManager.GetByID(id);
            if(response == null)
            {
                return base.NotFound();                                                  // 404 - NOT FOUND
            }
            return base.Ok(response);                                                    //200 - OK
        }

        [HttpPut("{id}")]                                                           //PUT:api/customer/1234567
        public IActionResult UpdateCustomerByID(string id, [FromBody] Customer customer)
        {
            if(string.IsNullOrWhiteSpace(id) || id != customer.Id || string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName) || string.IsNullOrWhiteSpace(customer.DOB))
            {
                return base.BadRequest();                                                //400 - BAD REQUEST
            }

            var customerById = custbizManager.GetByID(id);
            if(customerById == null)
            {
                return base.NotFound();                                                  //404 - NOT FOUND
            }
            custbizManager.UpdateByID(id, customer);
            return new NoContentResult();                                           //204 - NO CONTENT
        }

        [Authorize(Policy = Policies.Admin)]                                        //Role based opertion 
        [HttpDelete("{id}")]                                                        //DELETE:api/customer/123456
        public IActionResult DeleteCustomerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return base.BadRequest();                                                //400 - BAD REQUEST
            }

            var response = custbizManager.DeleteByID(id);
            if(!response)
            {
                return base.NotFound();                                                  //404 - NOT FOUND
            }
            return base.NoContent();                                                     //204 - NO CONTENT
        }
    }
}