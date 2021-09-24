using System;
using System.Collections.Generic;
using System.Linq;

using CustomerService.Models;
using CustomerService.Standard;

namespace CustomerService.Manager
{
    public class custBizManager
        :IBizManager<Customer>
    {
        private static readonly IList<Customer> _repository 
            = new List<Customer>();        

        public IList<Customer> GetAll()
        {
            return _repository;
        }

        public void Add(Customer customer)
        {
            var rnd = new Random();
            customer.Id = rnd.Next(1000000, 9999999).ToString();                            //Changes
            _repository.Add(customer);
        }

        public Customer GetByID(string id)
        {
            return _repository
                .Where(cust => cust.Id == id)
                .FirstOrDefault();
        }

        public void UpdateByID(string id, Customer customer)
        {
            var targetCustomer 
                = _repository
                .FirstOrDefault(cust => cust.Id == id);

            if (targetCustomer != null)
            {
                targetCustomer.FirstName = customer.FirstName;
                targetCustomer.LastName = customer.LastName;
                targetCustomer.DOB = customer.DOB;
                targetCustomer.SSN = customer.SSN;
            }
        }

        public bool DeleteByID(string id)
        {
            var customer 
                = _repository
                .Where(cust => cust.Id == id)
                .FirstOrDefault();

            if (customer == null)                                                           //Pattern
            {
                return false;
            }
            _repository.Remove(customer);
            return true;
        }
    }
}