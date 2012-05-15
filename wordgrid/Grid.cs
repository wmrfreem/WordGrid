using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wordgrid
{
    /// <summary>
    /// Class for finding and printing an m-by-n WordGrid where all columns and rows are valid words
    /// </summary>
    public class Grid
    {
        // p is the primary dimension -- chosen to be the longer of m and n
        // s is the secondary dimension

        private int m;
        private int n;
        private List<int[]> pWords;
        private List<int[]> sWords;
        private BSTree pLib;
        private BSTree sLib;
        private int pWordCount;
        private int sWordCount;
        private int[] pIdx;
        private int[] sIdx;
        /// <summary>
        /// true => primary dimension is a row, false otherwise. Used when loading the grid.
        /// </summary>
        private bool row;

        public Grid(int m, int n, List<string> wordList) 
        {
            this.m = m;
            this.n = n;

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

        private static int[] GetSearch(ref int[,] grid, int idx, int end, bool row)
        {
            int[] search = new int[end + 1];
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

        // Currently unused. Expected use is to find the first word after the current
        // search word not matching a specific prefix
        // Ex. the prefix "ea" does not match any valid grids so we can skip ahead and instead
        // of naively incrementing to the next word in the list, instead search for the next word
        // not matching the given prefix, say "ebb"
        public static int NextSearchWord(ref List<int[]> source, int start, int length, int[] search, int slen)
        {
            if (start == length - 1)
            {
                return -1;
            }
            int cur = start + 1;
            int gallop = 1;
            do
            {
                if (Utilities.StartsWith(source[cur - 1], search, slen))
                {
                    if (!Utilities.StartsWith(source[cur], search, slen))
                    {
                        return cur;
                    }
                    if (cur + gallop >= length)
                    {
                        gallop = 1;
                        cur++;
                    }
                    else
                    {
                        cur += gallop;
                        gallop *= 2;
                    }
                }
                else
                {
                    cur -= gallop / 2;
                    gallop = 1;
                    cur++;
                }
            } while (cur < length);
            return -1;
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
        
        public void Generate()
        {
            DateTime startTime = DateTime.Now;
            int pCur = 0;
            int sCur = 0;

            int count = 0;
            long rejected = 0;
            bool primary = true;
            int[,] grid = new int[m, n];

            for (; ; )
            {
#if DEBUG
                //Console.WriteLine(String.Format("pCur:{0}, sCur:{1} primary:{2}", pCur, sCur, primary));
                //PrintGrid(ref grid, m, n);
#endif
                if (primary)
                {
                    if (pCur == 0)
                    {
                        pIdx[0]++;
                        if (pIdx[0] >= pWordCount)
                        {
                            break;
                        }
                        LoadWord(ref grid, pWords[pIdx[0]], 0, row);
                        pCur++;
                    }
                    else
                    {
                        int[] search = GetSearch(ref grid, pCur, sCur, row);
                        int slen = sCur;
                        if (pIdx[pCur] == -1)
                        {
                            pIdx[pCur] = pLib.Search(search, slen);
                        }
                        else
                        {
                            pIdx[pCur]++;
                        }

                        if (pIdx[pCur] == -1 || pIdx[pCur] >= pWordCount || !Utilities.StartsWith(pWords[pIdx[pCur]], search, slen))
                        {
                            pIdx[pCur] = -1;
                            rejected++;
                            sCur--;
                        }
                        else
                        {
                            LoadWord(ref grid, pWords[pIdx[pCur]], pCur, row);
                            pCur++;

                            if (pCur == pIdx.Length)
                            {
                                bool allWord = true;
                                for (int i = sCur; i < sIdx.Length; i++)
                                {
                                    search = GetSearch(ref grid, i, pIdx.Length, !row);
                                    int tempidx = sLib.Search(search, pIdx.Length);
                                    if (tempidx == -1 || !Utilities.StartsWith(sWords[tempidx], search, pIdx.Length))
                                    {
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
                }
                else
                {
                    int[] search = GetSearch(ref grid, sCur, pCur, !row);
                    int slen = pCur;
                    if (sIdx[sCur] == -1)
                    {
                        sIdx[sCur] = sLib.Search(search, slen);
                    }
                    else
                    {
                        sIdx[sCur]++;
                    }
                    if (sIdx[sCur] == -1 || sIdx[sCur] >= sWordCount || !Utilities.StartsWith(sWords[sIdx[sCur]], search, slen))
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
