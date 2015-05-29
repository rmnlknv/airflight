using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DataAccess;

namespace Tickets.Models
{
    public class TicketsModel
    {
        public int From { get; set; }
        public int To { get; set; }

        [Required(ErrorMessage = "This field is required!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required!")]
        public string LastName { get; set; }
        public decimal Price { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public TimeSpan Time { get; set; }
        public int Location { get; set; }
        public int Destination { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Destinations { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public TimeSpan TotalTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Airport { get; set; }
        public Dictionary<int, string> Airports { get; set; }
        public List<Flight> Flights { get; set; }
        public List<Flight> Path { get; set; }
        public List<List<Flight>> Way {get; set; }
    }
}