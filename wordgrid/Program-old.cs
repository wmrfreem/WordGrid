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

        private static string GetSearch(ref char[,] grid, int idx, int end, bool row)
        {
            string search = String.Empty;
            if (row)
            {
                for (int i = 0; i < end; i++)
                {
                    search += grid[idx, i];
                }
            }
            else
            {
                for (int i = 0; i < end; i++)
                {
                    search += grid[i, idx];
                }
            }
            return search;
        }

        private static int[] GetSearch(ref int[,] grid, int idx, int end, bool row)
        {
            int[] search = new int[end];
            if (row)
            {
                for (int i = 0; i < end; i++)
                {
                    search[i] = grid[idx, i];
                }
            }
            else
            {
                for (int i = 0; i < end; i++)
                {
                    search[i] = grid[i, idx];
                }
            }
            return search;
        }

        private static int SearchWord(ref List<string> words, int wordCount, string search)
        {
            int idx = words.BinarySearch(search);
            if (~idx == wordCount)
            {
                return -1;
            }
            if (idx < 0)
            {
                if (words[~idx].StartsWith(search))
                {
                    return ~idx;
                }
                return -1;
            }
            return idx;
        }

        private static void LoadWord(ref char[,] grid, string word, int idx, bool row)
        {
            int len = word.Length;
            if (row)
            {
                for (int i = 0; i < len; i++)
                {
                    grid[idx, i] = word[i];
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    grid[i, idx] = word[i];
                }
            }
        }

        private static void LoadWord(ref int[,] grid, int[] word, int idx, bool row)
        {
            int len = word.Length;
            if (row)
            {
                for (int i = 0; i < len; i++)
                {
                    grid[idx, i] = word[i];
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    grid[i, idx] = word[i];
                }
            }
        }

        private static void PrintGrid(ref char[,] grid, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(grid[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void PrintGrid(ref int[,] grid, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(((char)(grid[i, j] + 97)));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

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

            List<int[]> mWords = Utilities.ShiftStrings(from w in wordList where w.Length == m select w);
            List<int[]> nWords;
            BSTree mLib = new BSTree();
            for (int i = 0; i < mWords.Count; i++)
            {
                mLib.Add(ref mWords, i);
            }
            BSTree nLib = new BSTree();
            if (m == n)
            {
                nWords = mWords;
                nLib = mLib;
            }
            else
            {
                nWords = Utilities.ShiftStrings(from w in wordList where w.Length == n select w);
                for (int i = 0; i < nWords.Count; i++)
                {
                    nLib.Add(ref nWords, i);
                }
            }


            if (mWords.Count == 0 || nWords.Count == 0)
            {
                Console.WriteLine(String.Format("No words exist for m={0} or n={1}", m, n));
                Environment.Exit(5);
            }


            List<int[]> pWords;
            List<int[]> sWords;
            BSTree pLib;
            BSTree sLib;
            int pWordCount;
            int sWordCount;
            int[] pIdx;
            int[] sIdx;
            bool row;
            if (m > n)
            {
                pWords = mWords;
                sWords = nWords;
                pLib = mLib;
                sLib = nLib;
                pWordCount = mWords.Count;
                sWordCount = nWords.Count;
                pIdx = new int[n];
                sIdx = new int[m];
                row = false;
            }
            else
            {
                pWords = nWords;
                sWords = mWords;
                pLib = nLib;
                sLib = mLib;
                pWordCount = nWords.Count;
                sWordCount = mWords.Count;
                pIdx = new int[m];
                sIdx = new int[n];
                row = true;
            }
            for (int i = 0; i < pIdx.Length; i++)
            {
                pIdx[i] = -1;
            }
            for (int i = 0; i < sIdx.Length; i++)
            {
                sIdx[i] = -1;
            }

            int pCur = 0;
            int sCur = 0;

            int count = 0;
            long rejected = 0;
            bool primary = true;
            int[,] grid = new int[m,n];
            for (;;)
            {
                //Console.WriteLine(String.Format("pCur:{0}, sCur:{1}", pCur, sCur));
                //PrintGrid(ref grid, m, n);
                if (primary)
                {
                    if (pCur == 0)
                    {
                        pIdx[pCur]++;
                        if (pIdx[pCur] >= pWordCount)
                        {
                            break;
                        }
                        LoadWord(ref grid, pWords[pIdx[pCur]], pCur, row);
                        pCur++;
                    }
                    else
                    {
                        int[] search = GetSearch(ref grid, pCur, sCur, row);
                        if (pIdx[pCur] == -1)
                        {
                            pIdx[pCur] = pLib.Search(search);
                        }
                        else
                        {
                            pIdx[pCur]++;
                        }

                        if (pIdx[pCur] == -1 || pIdx[pCur] >= pWordCount || !Utilities.StartsWith(pWords[pIdx[pCur]],search))
                        {
                            pIdx[pCur] = -1;
                            rejected++;
                            sCur--;
                        }
                        else
                        {
                            LoadWord(ref grid, pWords[pIdx[pCur]], pCur, row);
                            pCur++;
                        }

                        if (pCur == pIdx.Length)
                        {
                            bool allWord = true;
                            for (int i = sCur; i < sIdx.Length; i++)
                            {
                                search = GetSearch(ref grid, i, pIdx.Length, !row);
                                int tempidx = sLib.Search(search);
                                if (tempidx == -1 || !Utilities.StartsWith(sWords[tempidx], search))
                                {
                                    int[] q = GetSearch(ref grid, i, pIdx.Length, !row);
                                    allWord = false;
                                    rejected++;
                                    break;
                                }
                            }
                            if (allWord)
                            {
                                count++;
                                PrintGrid(ref grid, m, n);
                            }
                            pCur--;
                            continue;
                        }
                    }
                }
                else
                {
                    int[] search = GetSearch(ref grid, sCur, pCur, !row);
                    if (sIdx[sCur] == -1)
                    {
                        sIdx[sCur] = sLib.Search(search);
                    }
                    else
                    {
                        sIdx[sCur]++;
                    }
                    if (sIdx[sCur] == -1 || sIdx[sCur] >= sWordCount || !Utilities.StartsWith(sWords[sIdx[sCur]],search))
                    {
                        sIdx[sCur] = -1;
                        rejected++;
                        pCur--;
                    }
                    else
                    {
                        LoadWord(ref grid, sWords[sIdx[sCur]], sCur, !row);
                        sCur++;
                    }
                }
                primary = !primary;
            }
            Console.WriteLine(String.Format("Found {0} {1}x{2} solutions. Rejected {3} possibilities.", count, m, n, rejected));
            Console.WriteLine(String.Format("Total Time: {0} seconds", (DateTime.Now - startTime).TotalSeconds));
        }
    }
}
