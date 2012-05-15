using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace wordgrid
{
    class Program
    {
        private static string dictionaryFile = @"dict.txt";

        static void Main(string[] args)
        {
            int m, n;
            if (args.Length != 2)
            {
                Console.WriteLine(String.Format("Expected 2 arguments but got {0}", args.Length));
                Environment.Exit(1);
            }

            if (!int.TryParse(args[0], out m) || m <= 1)
            {
                Console.WriteLine("Expected integer rows number >= 2 but got {0}", args[0]);
                Environment.Exit(2);
            }
            if (!int.TryParse(args[1], out n) || n <= 1)
            {
                Console.WriteLine("Expected integer columns number >= 2 but got {0}", args[1]);
                Environment.Exit(3);
            }

            if (!File.Exists(dictionaryFile))
            {
                Console.WriteLine(String.Format("Dictionary File {0} does not exist.", dictionaryFile));
                Environment.Exit(4);
            }

            DateTime startTime = DateTime.Now;
            List<string> wordList = new List<string>(5000);
            using (StreamReader s = new StreamReader(dictionaryFile))
            {
                string str;
                while (!String.IsNullOrEmpty(str = s.ReadLine())) { wordList.Add(str); }
            }
            Console.WriteLine(String.Format("{0} dictionary entries read in {1} seconds", wordList.Count, (DateTime.Now - startTime).TotalSeconds));

            Grid grid = new Grid(m, n, wordList);
            grid.Generate();
        }
    }
}
