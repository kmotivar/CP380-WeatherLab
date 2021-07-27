using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            var total_2020_precipitation = measurements
                                        .Where(val => val.year == 2020)
                                        .Select(s => s.precipitation)
                                        .Sum();
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            //
            // Heating Degree days have a mean temp of < 18C
            //   see: https://en.wikipedia.org/wiki/Heating_degree_day
            //
            // Cooling degree days have a mean temp of >=18C
            //
            var mean = measurements
                    .GroupBy(y => y.year)
                    .Select(s => new
                    {
                        year = s.Key,
                        hdd = s.Where(val => val.meantemp < 18).Count(),
                        cdd = s.Where(val => val.meantemp >= 18).Count()
                    });

            //
            // Most Variable days are the days with the biggest temperature
            // range. That is, the largest difference between the maximum and
            // minimum temperature
            //
            // Oh: and number formatting to zero pad.
            // 
            // For example, if you want:
            //      var x = 2;
            // To display as "0002" then:
            //      $"{x:d4}"
            //
            Console.WriteLine("Year\tHDD\tCDD");

            foreach (var i in mean)
            {
                Console.WriteLine($"{ i.year }\t{ i.hdd }\t{ i.cdd }");
            }

            
            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");

            var mvdays = measurements
                    .Select(s => new
                    {
                        date = $"{s.year}-{s.month:d2}-{s.day:d2}",
                        delta = (s.maxtemp - s.mintemp)
                    })
                    .OrderByDescending(r => r.delta);

            int count = 0;
            foreach (var i in mvdays)
            {
                if (count < 5)
                {
                    Console.WriteLine($"{i.date}\t{i.delta}");
                    count++;
                }
                else
                    break;
            }
        }
    }
}
