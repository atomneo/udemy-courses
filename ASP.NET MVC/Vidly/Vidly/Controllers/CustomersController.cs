﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;

        private Random random = new Random();

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ViewResult Index()
        {
            //I don't know why Mosh suggested to cache genres in Customers controller, but whatever ;-)

            //if (MemoryCache.Default[CacheKeys.GENRES] == null)
            //{
            //    MemoryCache.Default[CacheKeys.GENRES] = _context.Genres.ToList();
            //}

            //var genres = MemoryCache.Default[CacheKeys.GENRES] as IEnumerable<Genre>;

            return View();
        }

        public ActionResult Details(int id)
        {
            var customer = _context.Customers.Include(c => c.MembershipType).SingleOrDefault(c => c.Id == id);

            if (customer == null)
                return HttpNotFound();

            return View(customer);
        }

        public ActionResult New()
        {
            var membershipTypes = _context.MembershipTypes.ToList();
            var viewModel = new CustomerFormViewModel
            {
                MembershipTypes = membershipTypes,
                Customer = new Customer(),
            };
            return View("CustomerForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CustomerFormViewModel
                {
                    Customer = customer,
                    MembershipTypes = _context.MembershipTypes.ToList(),
                };
                return View("CustomerForm", viewModel);
            }

            if (customer.Id == 0)
            {
                _context.Customers.Add(customer);
            }
            else
            {
                var customerInDb = _context.Customers.Single(c => c.Id == customer.Id);
                customerInDb.Name = customer.Name;
                customerInDb.Birthday = customer.Birthday;
                customerInDb.MembershipTypeId = customer.MembershipTypeId;
                customerInDb.IsSubscribedToNewsletter = customer.IsSubscribedToNewsletter;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Customers");
        }

        public ActionResult Edit(int id)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);

            if (customer == null)
                return HttpNotFound();

            var viewModel = new CustomerFormViewModel
            {
                Customer = customer,
                MembershipTypes = _context.MembershipTypes.ToList(),
            };

            return View("CustomerForm", viewModel);
        }

        public ActionResult NewRandom()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.namefake.com/");
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            request.Timeout = 5000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var content = response.GetResponseStream();
                var encoding = Encoding.ASCII;

                using (var reader = new StreamReader(content, encoding))
                {
                    var json = reader.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    var customer = new Customer
                    {
                        Name = data["name"],
                        Birthday = DateTime.ParseExact(data["birth_data"], "yyyy-MM-dd", null),
                        IsSubscribedToNewsletter = random.Next(0, 2) == 0,
                        MembershipTypeId = (byte)random.Next(1, 5),
                    };
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                }
            }
            catch (TimeoutException e)
            {

                Console.WriteLine(e);
                //todo: show message
            }

            return RedirectToAction("Index", "Customers");
        }
    }
}