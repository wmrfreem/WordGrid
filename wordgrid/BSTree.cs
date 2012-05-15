using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wordgrid
{
    /// <summary>
    /// A B-tree with a fixed number (26) of children for purely alphabetic data
    /// </summary>
    public class BSTree
    {
        public BSTree()
        {
            next = new BSTree[26];
            index = -1;
            first = -1;
        }

        BSTree[] next;
        int index;
        int first;

        public void Add(ref List<int[]> source, int idx)
        {
            int[] add = source[idx];
            int displace = -1;
            int depth = -1;
            BSTree cur = this;
            for (int i = 0; i < add.Length; i++)
            {
                int sidx = add[i];
                if (displace == -1)
                {
                    if (cur.first < 0)
                    {
                        cur.index = idx;
                        cur.first = idx;
                        return;
                    }
                    else if (cur.index >= 0)
                    {
                        displace = cur.index;
                        cur.index = -1;
                        depth = i;
                        for (; depth < add.Length && add[depth] == source[displace][depth]; depth++) ;
                    }
                }
                if (i <= depth)
                {
                    cur.first = displace;
                }
                if (i == depth)
                {
                    int nidx = source[displace][i];
                    cur.next[nidx] = new BSTree();
                    cur.next[nidx].index = displace;
                    cur.next[nidx].first = displace;

                    cur.next[sidx] = new BSTree();
                    cur.next[sidx].index = idx;
                    cur.next[sidx].first = idx;
                    return;
                }
                if (cur.next[sidx] == null)
                {
                    cur.next[sidx] = new BSTree();
                }
                cur = cur.next[sidx];
                if (i == add.Length - 1)
                {
                    cur.index = idx;
                    cur.first = idx;
                }
            }
        }

        public int Search(int[] search, int length)
        {
            BSTree cur = this;
            for (int i = 0; i < length; i++)
            {
                if (cur.index >= 0)
                {
                    return cur.index;
                }
                int sidx = search[i];
                if (cur.next[sidx] == null)
                {
                    return -1;
                }
                cur = cur.next[sidx];
            }
            return cur.first;
        }
    }
}