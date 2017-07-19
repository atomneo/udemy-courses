using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Vidly.DTOs;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    public class MoviesController : ApiController
    {
        private ApplicationDbContext _context;

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/movies
        public IHttpActionResult GetMovies()
        {
            return Ok(_context.Movies.Include(m => m.Genre).ToList().Select(Mapper.Map<Movie, MovieDto>));
        }

        // GET /api/movies/1
        public IHttpActionResult GetMovie(int id)
        {
            var movie = _context.Movies.SingleOrDefault(c => c.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<Movie, MovieDto>(movie));
        }

        // POST /api/movies
        [HttpPost]
        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public IHttpActionResult CreateMovie(MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var movie = Mapper.Map<MovieDto, Movie>(movieDto);
            _context.Movies.Add(movie);
            _context.SaveChanges();

            return Created(new Uri(Request.RequestUri + "/" + movie.Id), Mapper.Map<Movie, MovieDto>(movie));
        }

        // PUT /api/movies/1
        [HttpPut]
        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public IHttpActionResult UpdateMovie(int id, MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != movieDto.Id)
            {
                return BadRequest("Id is key parameter and cannot be changed");
            }

            var movieInDb = _context.Movies.SingleOrDefault(c => c.Id == id);

            if (movieInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(movieDto, movieInDb);

            _context.SaveChanges();

            return Ok();
        }

        // DELETE /api/movies/1
        [HttpDelete]
        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public IHttpActionResult DeleteMovie(int id)
        {
            var moviesInDb = _context.Movies.SingleOrDefault(c => c.Id == id);

            if (moviesInDb == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(moviesInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}
