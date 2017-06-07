using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            //Func<int, int> square = x => x * x;
            //Expression<Func<int, int, int>> add = (x, y) => x + y;
            //Func<int, int, int> addI = add.Compile();

            //var result = addI(3, 5);
            //Console.WriteLine(result);
            //Console.WriteLine(add);
            //

            //var cars = ProcessFile("fuel.csv");
            //var manufacturers = ProcessManufacturers("manufacturers.csv");

            #region Join Query
            //var query = from car in cars
            //            join manufacturer in manufacturers 
            //                on new { car.Manufacturer, car.Year }  
            //                equals
            //                new { Manufacturer = manufacturer.Name, manufacturer.Year }
            //            orderby car.Combiend descending, car.Name ascending
            //            select new
            //            {
            //                manufacturer.Headquarters,
            //                car.Name,
            //                car.Combiend
            //            };

            //var query2 = cars.Join(manufacturers,
            //                        c => new { c.Manufacturer, c.Year },
            //                        m => new { Manufacturer = m.Name, m.Year }, 
            //                        (c, m) => new
            //                        {
            //                            m.Headquarters,
            //                            c.Name,
            //                            c.Combiend
            //                        })
            //                .OrderByDescending(c => c.Combiend)
            //                .ThenBy(c => c.Name);

            //var result = cars.Any(c => c.Manufacturer == "Ford");
            ////var result = cars.All(c => c.Manufacturer == "Ford");

            //Console.WriteLine(result);

            //var result = cars.SelectMany(c => c.Name)
            //                 .OrderBy(c => c);

            //foreach (var character in result)
            //{
            //    Console.WriteLine(character);
            //}


            //foreach (var car in query2.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combiend}");
            //}
            #endregion

            #region Group Query
            //var query =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper() into manufacturer
            //    orderby manufacturer.Key
            //    select manufacturer;

            //var query =
            //    from manufacturer in manufacturers
            //    join car in cars on manufacturer.Name equals car.Manufacturer
            //        into carGroup
            //    orderby manufacturer.Name
            //    select new
            //    {
            //        Manufacturer = manufacturer,
            //        Cars = carGroup
            //    } into result
            //    group result by result.Manufacturer.Headquarters;

            //var query =
            //    from car in cars
            //    group car by car.Manufacturer into carGroup
            //    select new
            //    {
            //        Name = carGroup.Key,
            //        Max = carGroup.Max(c => c.Combiend),
            //        Min = carGroup.Min(c => c.Combiend),
            //        Avg = carGroup.Average(c => c.Combiend)
            //    } into result
            //    orderby result.Max descending
            //    select result;

            //var query2 =
            //    cars.GroupBy(c => c.Manufacturer.ToUpper())
            //        .OrderBy(g => g.Key);

            //var query2 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
            //        (m, carGroup) =>
            //            new
            //            {
            //                Manufacturer = m,
            //                Cars = carGroup
            //            })
            //    .GroupBy(m => m.Manufacturer.Headquarters);

            //var query2 =
            //    cars.GroupBy(c => c.Manufacturer)
            //        .Select(g => 
            //        {
            //            var results = g.Aggregate(new CarStatistics(),
            //                                (acc, c) => acc.Accumulate(c),
            //                                acc => acc.Compute());
            //            return new
            //            {
            //                Name = g.Key,
            //                Avg = results.Average,
            //                Min = results.Min,
            //                Max = results.Max
            //            };
            //        })
            //        .OrderByDescending(g => g.Max);

            //foreach (var group in query)
            //{
            //    Console.WriteLine($"{group.Key}");
            //    foreach (var car in group.SelectMany(g => g.Cars)
            //                             .OrderByDescending(c => c.Combiend).Take(2))
            //    {

            //        Console.WriteLine($"\t{car.Name} : {car.Combiend}");
            //    }
            //}

            //foreach (var result in query2)
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\t Max: {result.Max}");
            //    Console.WriteLine($"\t Min: {result.Min}");
            //    Console.WriteLine($"\t Avg: {result.Avg}");
            //}
            #endregion

            #region XML
            //CreateXml();
            //QueryXml();
            #endregion

            #region Entity Framework
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
            #endregion

            Console.ReadLine();
        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            //var query = from car in db.Cars
            //            group car by car.Manufacturer into manufacturer
            //            select new
            //            {
            //                Name = manufacturer.Key,
            //                Cars = (from car in manufacturer
            //                        orderby car.Combiend descending
            //                        select car).Take(2)
            //            };

            var query = db.Cars.GroupBy(c => c.Manufacturer)
                               .Select(g => new
                               {
                                   Name = g.Key,
                                   Cars = g.OrderByDescending(c => c.Combiend).Take(2)
                               });

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Name}");
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combiend}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessFile("fuel.csv");
            var db = new CarDb();
            //db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryXml()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");

            var query =
                from element in document.Element(ns + "Cars")?.Elements(ex + "Car") //document.Descendants("Car")
                                                            ?? Enumerable.Empty<XElement>()
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXml()
        {
            var records = ProcessFile("fuel.csv");

            var document = new XDocument();

            //var cars = new XElement("Cars");

            //foreach (var record in records)
            //{
            //    //var name = new XAttribute("Name", record.Name);
            //    //var combined = new XAttribute("Combined", record.Combiend);
            //    //var car = new XElement("Car", name, combined);

            //    var car = new XElement("Car",
            //                    new XAttribute("Name", record.Name),
            //                    new XAttribute("Combined", record.Combiend),
            //                    new XAttribute("Manufacturer", record.Manufacturer));

            //    cars.Add(car);
            //}
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var cars = new XElement(ns + "Cars",
                from record in records
                select new XElement(ex + "Car",
                            new XAttribute("Name", record.Name),
                            new XAttribute("Combined", record.Combiend),
                            new XAttribute("Manufacturer", record.Manufacturer)));

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                .Where(line => line.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });

            return query.ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            //return File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(line => Car.ParseFromCsv(line)).ToList();

            //var query = from line in File.ReadAllLines(path).Skip(1)
            //            where line.Length > 1
            //            select Car.ParseFromCsv(line);

            //return query.ToList();

            var query = File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .ToCar();

            return query.ToList();
        }
    }

    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }

        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combiend;
            Max = Math.Max(Max, car.Combiend);
            Min = Math.Min(Min, car.Combiend);
            return this;
        }

        internal CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }

        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public int Average { get; set; }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combiend = int.Parse(columns[7])
                };
            }
        }
    }
    
}
