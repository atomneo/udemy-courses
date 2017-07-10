using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        private List<Customer> customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Jane Doe"},
            new Customer { Id = 2, Name = "John Smith"},
            new Customer { Id = 3, Name = "Jan Kowalski"},
        };

        // GET: Customers
        public ActionResult Index()
        {
            return View(customers);
        }

        public ActionResult Details(int id)
        {
            var customer = customers[id - 1];
            return View(customer);
        }
    }
}