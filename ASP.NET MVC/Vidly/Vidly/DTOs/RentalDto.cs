using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vidly.DTOs
{
    public class RentalDto
    {
        public int CustomerId { get; set; }
        
        public List<int> MoviesIds { get; set; }
    }
}