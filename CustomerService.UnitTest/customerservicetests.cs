using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomerService.Controllers;
using CustomerService.Models;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CustomerService.Standard;

namespace CustomerServiceTests
{
    [TestFixture]
    public sealed class customerservicetests
    {
        private CustomerController _customercontroller;
        private Mock<IBizManager<Customer>> _custBizManagerMock;

        [SetUp]
        public void Setup()
        {
            _custBizManagerMock = new Mock<IBizManager<Customer>>();
            _customercontroller = new CustomerController(_custBizManagerMock.Object);
        }
        [Test]
        public void shouldreturn_nocontent_when_getallcustomersEmpty()
        {
            var emptycustomerlist = (IList<Customer>)null;
            _custBizManagerMock.Setup(custBizManager => custBizManager.GetAll()).Returns(emptycustomerlist);
            var apiresponse = _customercontroller.GetAllCustomer();
            apiresponse.Should().NotBeNull();
            apiresponse.Should().BeOfType<NotFoundResult>();
            ((NotFoundResult)apiresponse).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NotFound));
        }
        [Test]
        public void shouldreturnallcustomerlist_whencustomerisnotempty()
        {
            var customers = new List<Customer>()
            {
                new Customer{Id="12345",FirstName="Siran",LastName="Raj",DOB="11/05/1994",SSN="123-12-12345" },
                new Customer{Id="67890",FirstName="christina",LastName="Mary",DOB="12/02/1998",SSN="123-25-56122" }
            };
            _custBizManagerMock.Setup(custBizManager => custBizManager.GetAll()).Returns(customers);
            var apiresponse = _customercontroller.GetAllCustomer();
            apiresponse.Should().NotBeNull();
            apiresponse.Should().BeOfType<OkObjectResult>();

            var httpresponse = apiresponse as OkObjectResult;

            httpresponse.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            httpresponse.Value.Should().NotBeNull();
            httpresponse.Value.Should().BeOfType<List<Customer>>();

            var customersfromresponsebody = httpresponse.Value as List<Customer>;
            customersfromresponsebody.Count.Should().Be(2);
        }
        [Test]
        public void shouldreturnok_whenaddingcustomersuccessful()
        {
            var newcustomer = new Customer { FirstName = "Siran", LastName = "Raj", DOB = "11/05/1994", SSN = "123-12-12345" };
            var apiresponse = _customercontroller.AddCustomer(newcustomer);
            apiresponse.Should().NotBeNull();
            apiresponse.Should().BeOfType<CreatedAtRouteResult>();

            var httpresponse = apiresponse as CreatedAtRouteResult;
            httpresponse.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.Created));
         httpresponse.RouteName.Should().Be("GetCustomerByID");

        }
    
        [Test]
        public void ShouldReturnNotFound_When_GetCustomerByID_With_InvalidID()
        {
            var response = _customercontroller.GetCustomerByID("94829480");

            response.Should().NotBeNull();
            response.Should().BeOfType<NotFoundResult>();
            ((NotFoundResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NotFound));
        }
        [Test]
        public void ShouldReturnCustomer_When_GettingCustomerByID()
        {
            var mockedResponse = new Customer() { FirstName = "Siran", LastName = "Raj", DOB = "11/05/1994", SSN = "123-12-12345" };

            _custBizManagerMock.Setup(ibiz => ibiz.GetByID("12345")).Returns(mockedResponse);

            var response = _customercontroller.GetCustomerByID("12345");

            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<Customer>();
            var values = result.Value as Customer;
        }
        [Test]
        public void ShouldReturnBadRequest_When_UpdatingCustomer_With_EmptyID()
        {
            var customer = new Customer() { FirstName = "Siran", LastName = "Raj", DOB = "11/05/1994", SSN = "123-12-12345" };

            var response = _customercontroller.UpdateCustomerByID("", customer);

            //Testing code
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
            ((BadRequestResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }
        [Test]
        public void ShouldReturnBadRequest_When_UpdatingCustomer_With_InvalidID()
        {
            var customer = new Customer() { FirstName = "Siran", LastName = "Raj", DOB = "11/05/1994", SSN = "123-12-12345" };

            var response = _customercontroller.UpdateCustomerByID("94809482", customer);

            //Testing code
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
            ((BadRequestResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }
        [Test]
        public void ShouldReturnNoContentResult_When_UpdatingCustomer_Succesful()
        {
            var mockedResponse = new Customer() { Id = "12345", FirstName = "Siran", LastName = "Raj", DOB = "11/05/1994", SSN = "123-12-12345" };

            _custBizManagerMock.Setup(ibiz => ibiz.GetByID("12345")).Returns(mockedResponse);

            var response = _customercontroller.UpdateCustomerByID("12345", mockedResponse);

            //Test code
            response.Should().NotBeNull();
            response.Should().BeOfType<NoContentResult>();
            var result = response as NoContentResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NoContent));
        }

        [Test]
        public void ShouldReturnBadRequest_When_DeletingCustomerByID_With_EmptyID()
        {
            var response = _customercontroller.DeleteCustomerByID("");

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
            ((BadRequestResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Test]
        public void ShouldReturnNotFound_When_DeletingCustomerByID_With_InvalidID()
        {
            var response = _customercontroller.DeleteCustomerByID("94809482");

            response.Should().NotBeNull();
            response.Should().BeOfType<NotFoundResult>();
            ((NotFoundResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShouldReturnNoContent_When_DeletingCustomerByID_Succesful()
        {
            _custBizManagerMock.Setup(ibiz => ibiz.DeleteByID("12345")).Returns(true);

            var response = _customercontroller.DeleteCustomerByID("12345");

            response.Should().NotBeNull();
            response.Should().BeOfType<NoContentResult>();
            var result = response as NoContentResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NoContent));
        }

    }
}
