using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SparklrWP.Utils
{
    public class GroupedObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The title of the group
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the group
        /// </summary>
        /// <param name="title">The group title</param>
        public GroupedObservableCollection(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Returns true if the group contains items
        /// </summary>
        public bool HasItems
        {
            get
            {
                return this.Count > 0;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }
    }

    static class CollectionExtensions
    {
        public static ObservableCollectionWithItemNotification<GroupedObservableCollection<UserItemViewModel>> GroupFriends(this ObservableCollectionWithItemNotification<UserItemViewModel> initialCollection)
        {
            ObservableCollectionWithItemNotification<GroupedObservableCollection<UserItemViewModel>> grouped = new ObservableCollectionWithItemNotification<GroupedObservableCollection<UserItemViewModel>>();

            // sort the input
            List<UserItemViewModel> sorted = (from friend in initialCollection orderby friend.Name select friend).ToList<UserItemViewModel>();

            string alphabet = "#abcdefghijklmnopqrstuvwxyz";
            GroupedObservableCollection<UserItemViewModel> tmp;


            foreach (char c in alphabet)
            {
                tmp = new GroupedObservableCollection<UserItemViewModel>(c.ToString());

                List<UserItemViewModel> friendStartingWithLetter = (from friend in sorted where friend.Name.StartsWith(c.ToString(), System.StringComparison.InvariantCultureIgnoreCase) select friend).ToList<UserItemViewModel>();

                foreach (UserItemViewModel f in friendStartingWithLetter)
                    tmp.Add(f);

                grouped.Add(tmp);
            }

            return grouped;
        }

        public static void AddFriend(this ObservableCollection<GroupedObservableCollection<UserItemViewModel>> collection, UserItemViewModel f)
        {
            string firstLetter = f.Name[0].ToString();

            foreach (GroupedObservableCollection<UserItemViewModel> c in collection)
            {
                if (string.Compare(firstLetter, c.Title, System.StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    c.Add(f);
#if DEBUG
                    App.logger.log("Added friend {0} to group {1}", f.Name, c.Title);
#endif
                    break;
                }
            }
        }
    }
}
