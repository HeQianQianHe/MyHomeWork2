using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> test = new List<int>();
            test.Add(1);
            test.Add(2);
            test.Remove(1);
            Console.WriteLine(test[0]);
            Console.ReadKey();
        }
    }
}
