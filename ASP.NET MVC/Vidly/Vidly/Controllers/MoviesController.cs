using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext _context;

        private Random random = new Random();

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: /movies
        //public ViewResult Index(int? pageIndex, string sortBy)
        public ViewResult Index()
        {
            //if (!pageIndex.HasValue)
            //{
            //    pageIndex = 1;
            //}

            //if (String.IsNullOrEmpty(sortBy))
            //{
            //    sortBy = "Name";
            //}

            //var movies = _context.Movies.Include(m => m.Genre).ToList();
            //return View(movies);
            if (User.IsInRole("CanManageMovies"))
                return View("List");

            return View("ReadOnlyList");
        }

        public ActionResult Details(int id)
        {
            var movie = _context.Movies.Include(m => m.Genre).SingleOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return HttpNotFound();
            }

            return View(movie);
        }

        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public ActionResult New()
        {
            var genres = _context.Genres.ToList();
            var viewModel = new MovieFormViewModel()
            {
                Genres = genres,
                Movie = new Movie(),
            };
            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public ActionResult Save(Movie movie)
        {
            if (movie.Id == 0)
            {
                _context.Movies.Add(movie);
            }
            else
            {
                var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);
                movieInDb.Name = movie.Name;
                movieInDb.GenreId = movie.GenreId;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.DateAdded = movie.DateAdded;
                movieInDb.NumberInStock = movie.NumberInStock;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Movies");
        }

        // GET: /Movies/Random
        public ActionResult Random()
        {
            var movie = new Movie()
            {
                Name = "Tylko mnie kochaj"
            };

            var customers = new List<Customer>
            {
                new Customer {Name = "Customer 1"},
                new Customer {Name = "Customer 2"},
            };

            var viewModel = new RandomMovieViewModel
            {
                Movie = movie,
                Customers = customers,
            };

            return View(viewModel);
        }

        // GET: /Movies/Edit/id
        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public ActionResult Edit(int id)
        {
            var movie = _context.Movies.SingleOrDefault(m => m.Id == id);

            if (movie == null)
                return HttpNotFound();

            var viewModel = new MovieFormViewModel
            {
                Movie = movie,
                Genres = _context.Genres.ToList(),
            };

            return View("MovieForm", viewModel);
        }

        [Route("movies/released/{year:regex(\\d{4}):range(1800, 2100)}/{month:regex(\\d{2}):range(1, 12)}")]
        public ActionResult ByReleaseDate(int year, int month)
        {
            return Content($"{year}-{month}");
        }

        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
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

                    var movie = new Movie
                    {
                        Name = data["company"],
                        ReleaseDate = DateTime.ParseExact(data["birth_data"], "yyyy-MM-dd", null),
                        GenreId = (short)random.Next(1, 9),
                        NumberInStock = (short)random.Next(1, 15),
                    };
                    _context.Movies.Add(movie);
                    _context.SaveChanges();
                }
            }
            catch (TimeoutException e)
            {

                Console.WriteLine(e);
                //todo: show message
            }

            return RedirectToAction("Index", "Movies");
        }
    }
}