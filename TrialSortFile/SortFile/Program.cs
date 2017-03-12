using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sort file path:");
            var pathFile = Console.ReadLine();
            var doc = new Sheet();
            var users = doc.SaveFile(pathFile);
            if (doc.IsValidFile)
            {
                Console.WriteLine();
                users.ForEach(i => Console.WriteLine("{0}, {1}\t\r", i.LastName, i.FirstName));
                Console.WriteLine();
                Console.WriteLine("Finished: created {0}", doc.PathSavedFile);
            }
            else
            {
                Console.WriteLine(doc.ErrorMessege);
            }
            
            Console.ReadLine();
        }
    }
}
