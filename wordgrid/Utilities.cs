using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wordgrid
{
    public static class Utilities
    {
        public static List<int[]> ShiftStrings(IEnumerable<string> input)
        {
            List<int[]> ret = new List<int[]>();
            foreach (string s in input)
            {
                int[] t = new int[s.Length];
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] > 'z' || s[i] < 'a')
                    {
                        continue;
                    }
                    t[i] = s[i] - 97;
                }
                ret.Add(t);
            }
            return ret;
        }

        public static bool StartsWith(int[] s, int[] start, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (s[i] != start[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
