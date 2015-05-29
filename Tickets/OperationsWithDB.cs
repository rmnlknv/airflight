using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OperationsWithDB
    {
        public static int AddNewAirport(string name)
        {
            var result = -1;
            using (var context = new TicketEntities())
            {
                var airport = new Airport();
                airport.Name = name;
                context.Airports.Add(airport);
                context.SaveChanges();
                return (result = airport.Id);
            }
        }
        public static int AddNewFlight(int to, int from, decimal price, TimeSpan time)
        {
            using (var context = new TicketEntities())
            {
                var flight = new Flight();
                flight.To = to;
                flight.From = from;
                flight.Price = price;
                flight.Time = time;
                context.Flights.Add(flight);
                context.SaveChanges();
                return (flight.Id);
            }
        }
        public static Dictionary<int, string> Airports()
        {
            var result = new Dictionary<int, string>();
            using (var context = new TicketEntities())
            {
                var value = context.Airports;
                if (value.Count() != 0)
                {
                    foreach (var item in value)
                    {
                        result.Add(item.Id, item.Name);
                    }
                    return result;
                }
                else return result;
            }
        }
        public static List<Voyage> Flights()
        {
            var result = new List<Voyage>();
            using (var context = new TicketEntities())
            {
                var value = context.Flights;
                if (value.Count() != 0)
                {
                    foreach (var item in value)
                    {
                        result.Add(new Voyage()
                        {
                            Id = item.Id,
                            To = item.To,
                            From = item.From,
                            Price = item.Price,
                            Time = item.Time
                        });
                    }
                    return result;
                }
                else
                    return null;
            }
        }
        public class Voyage
        {
            public int Id { get; set; }
            public int To { get; set; }
            public int From { get; set; }
            public decimal Price { get; set; }
            public TimeSpan Time { get; set; }
        }
    }
}
