using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
namespace SparklrWP.Utils
{
    public class Group<T> : IEnumerable<T> where T : class
    {
        public string KeyName { get; set; }
        public IList<T> Items { get; set; }

        public Group(string name, IEnumerable<T> items)
        {
            KeyName = name;
            if (items == null)
            {
                items = new T[] { };
            }
            Items = new List<T>(items);
        }

        public bool HasItems
        {
            get { return this.FirstOrDefault() != null; }
        }

        public Visibility HasItemsVisibility
        {
            get { return HasItems ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility HasNoItemsVisibility
        {
            get { return HasItems ? Visibility.Collapsed : Visibility.Visible; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
