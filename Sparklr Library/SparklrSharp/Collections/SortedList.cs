using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Collections
{
    /// <summary>
    /// Contains a List of sorted items. Note: Only the Add function will currently insert items correctly
    /// </summary>
    /// <typeparam name="T">The type of items to store</typeparam>
    public class SortedList<T> : List<T> where T : IComparable<T>
    {
        //TODO: Make other insert methods sorted.
        /// <summary>
        /// Adds an item to the List. This is an O(n) operation.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            if (this.Count == 0)
            {
                base.Insert(0, item);
            }
            else
            {
                bool added = false;

                for (int i = 0; i < this.Count; i++)
                {
                    if (item.CompareTo(this[i]) < 0)
                    {
                        added = true;
                        this.Insert(i, item);
                        break;
                    }
                }

                if (!added)
                    this.Insert(this.Count, item);
            }
        }
    }
}
