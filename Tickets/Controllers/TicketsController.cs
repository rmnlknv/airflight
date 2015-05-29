using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Models;
using DataAccess;

namespace Tickets.Controllers
{
    public class TicketsController : Controller
    {
        //
        // GET: /Tickets/
        private static List<TicketsModel> _listOfAirports = new List<TicketsModel>();
        private static List<TicketsModel> _listOfFlights = new List<TicketsModel>();
        private static List<List<Flight>> _ways;
        private static Ticket _ticket;

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult AddAirport()
        {
            return View();
        }

        public ActionResult cleardb()
        {
            using (var context = new TicketEntities())
            {
                foreach (var item in context.Flights)
                {
                    context.Flights.Remove(item);
                }
                foreach (var item in context.Airports)
                {
                    context.Airports.Remove(item);
                }
                context.SaveChanges();
                return Content("ok");
            }
        }

        [HttpPost]
        public ActionResult AddAirport(TicketsModel model)
        {
            OperationsWithDB.AddNewAirport(model.Airport);
            return RedirectToAction("Airports");
        }

        public ActionResult AddFlight()
        {
            var model = new TicketsModel();
            model.Locations = new List<SelectListItem>();
            model.Destinations = new List<SelectListItem>();
            foreach (var item in OperationsWithDB.Airports())
            {
                model.Locations.Add(new SelectListItem()
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                });
                model.Destinations.Add(new SelectListItem()
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddFlight(TicketsModel model)
        {
            OperationsWithDB.AddNewFlight(model.Location, model.Destination, model.Price, model.Time);
            return RedirectToAction("Flights");
        }
        public ActionResult Airports()
        {
            var model = new TicketsModel();
            var result = OperationsWithDB.Airports();
            model.Airports = new Dictionary<int, string>();
            _listOfAirports.Clear();
            foreach (var item in result)
            {
                model.Airports.Add(item.Key, item.Value.Trim());
            }
            _listOfAirports.Add(model);
            return View(_listOfAirports);
        }
        public ActionResult Flights()
        {
            var model = new TicketsModel();
            var airports = OperationsWithDB.Airports();
            model.Airports = new Dictionary<int, string>();
            _listOfAirports.Clear();
            foreach (var item in airports)
            {
                model.Airports.Add(item.Key, item.Value.Trim());
            }
            _listOfAirports.Add(model);
            var flights = OperationsWithDB.Flights();
            var flight = new Flight();
            model.Flights = new List<Flight>();
            _listOfFlights.Clear();
            foreach (var item in flights)
            {
                model.Flights.Add(new Flight()
                {
                    To = item.To,
                    From = item.From,
                    Price = item.Price,
                    Time = item.Time
                });
            }
            _listOfFlights.Add(model);
            return View(_listOfFlights);
        }
        public ActionResult MakeTicket()
        {
            var model = new TicketsModel();
            model.Locations = new List<SelectListItem>();
            model.Destinations = new List<SelectListItem>();
            foreach (var item in OperationsWithDB.Airports())
            {
                model.Locations.Add(new SelectListItem()
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                });
                model.Destinations.Add(new SelectListItem()
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult MakeTicket(TicketsModel model)
        {
            var location = model.Location;
            var destination = model.Destination;
            var airports = OperationsWithDB.Airports();
            var flights = OperationsWithDB.Flights();
            var path = new List<Flight>();
            _ways = new List<List<Flight>>();
            var possibleBegin = new List<Flight>();
            var possibleEnd = new List<Flight>();
            model.Flights = new List<Flight>();
            model.Airports = new Dictionary<int, string>();
            foreach (var item in flights)
            {
                model.Flights.Add(new Flight()
                {
                    Id = item.Id,
                    Price = item.Price,
                    To = item.To,
                    From = item.From,
                    Time = item.Time
                });
            }
            foreach (var item in model.Flights)
            {
                if (item.To == location && item.From == destination)
                {
                    path.Add(item);
                    _ways.Add(new List<Flight>(path));
                    path.Clear();
                }
                if (item.To == location && item.From != destination)
                {
                    possibleBegin.Add(item);
                }
                if (item.From == destination && item.To != location)
                {
                    possibleEnd.Add(item);
                }
            }
            if (possibleBegin.Count > 0 && possibleEnd.Count > 0)
            {
                for (int i = 0; i < possibleBegin.Count; i++)
                {
                    for (int j = 0; j < possibleEnd.Count; j++)
                    {
                        if (possibleBegin[i].From == possibleEnd[j].To)
                        {
                            foreach (var item in flights)
                            {
                                if (item.Id == possibleBegin[i].Id || item.Id == possibleEnd[j].Id)
                                {
                                    path.Add(new Flight()
                                    {
                                        Id = item.Id,
                                        From = item.From,
                                        To = item.To,
                                        Price = item.Price,
                                        Time = item.Time
                                    });
                                }
                            }
                            _ways.Add(new List<Flight>(path));
                            path.Clear();
                        }
                    }
                }
            }
            model.Airports = OperationsWithDB.Airports();
            var temp = OperationsWithDB.Flights();
            foreach (var item in temp)
            {
                model.Flights.Add(new Flight()
                {
                    Id = item.Id,
                    From = item.From,
                    To = item.To,
                    Price = item.Price,
                    Time = item.Time
                });
            }
            _ticket = new Ticket()
            {
                From = model.From,
                To = model.To,
                price = model.Price,
                time = model.Time,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            if (_ways.Count != 0)
            {
                if (_ways.First().Count != 0 && _ways.Last().Count != 0)
                {
                    return RedirectToAction("ShowWays");
                }
            }
            return RedirectToAction("TicketError");
        }
        public ActionResult ShowWays()
        {
            var ticketsModel = new TicketsModel();
            ticketsModel.Location = _ways.First()[0].From;
            ticketsModel.Destination = _ways.Last()[(_ways.Last().Count - 1)].To;
            ticketsModel.Way = _ways;
            TimeSpan tempTime = TimeSpan.Parse("00:00:00");
            decimal tempPrice = 0;
            TimeSpan totalTime = TimeSpan.MaxValue;
            decimal totalPrice = decimal.MaxValue;
            foreach (var item in _ways)
            {
                foreach (var value in item)
                {
                    tempTime += value.Time;
                    tempPrice += value.Price;
                }
                if (tempPrice < totalPrice)
                    totalPrice = tempPrice;
                if (tempTime < totalTime)
                    totalTime = tempTime;
            }
            ticketsModel.TotalPrice = totalPrice;
            ticketsModel.TotalTime = totalTime;
            ticketsModel.FirstName = _ticket.FirstName;
            ticketsModel.LastName = _ticket.LastName;
            ticketsModel.Airports = OperationsWithDB.Airports();
            return View(ticketsModel);
        }
        [HttpPost]
        public ActionResult ShowWays(TicketsModel model)
        {
            return View();
        }
        public ActionResult TicketError()
        {
            return View();
        }
    }
}
