using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => 
            {
                int temp = x + y;
                return temp;
            };
            Action<int> write = x => Console.WriteLine(x);

            write(square(add(3, 5)));

            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee { Id = 1, Name="Scott" },
                new Employee { Id = 2, Name ="Chris" }
            };

            IEnumerable<Employee> sales = new List<Employee>
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            //Console.WriteLine(developers.Count());
            //IEnumerator<Employee> enumerator = developers.GetEnumerator();

            //while (enumerator.MoveNext())
            //{
            //    Console.WriteLine(enumerator.Current.Name);
            //}

            //foreach (var employee in developers.Where(NameStartsWiths))
            //{
            //    Console.WriteLine(employee.Name);
            //}

            //foreach (var employee in developers.Where(
            //    delegate (Employee employee) 
            //    {
            //        return employee.Name.StartsWith("S");
            //    }))
            //{
            //    Console.WriteLine(employee.Name);
            //}

            var query = developers.Where(e => e.Name.Length == 5)
                                  .OrderByDescending(o => o.Name)
                                  .Select(e => e);

            var query2 = from developer in developers
                         where developer.Name.Length == 5
                         orderby developer.Name descending
                         select developer;

            foreach (var employee in query2)
            {
                Console.WriteLine(employee.Name);
            }

            Console.ReadLine();
        }

        private static bool NameStartsWiths(Employee employee)
        {
            return employee.Name.StartsWith("S");
        }
    }
    
}
