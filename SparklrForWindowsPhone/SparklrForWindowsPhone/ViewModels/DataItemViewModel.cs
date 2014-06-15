
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace SparklrForWindowsPhone.ViewModels
{
    public class DataItemViewModel : ViewModelBase
    {
        private Uri imageSource;
        private Uri imageThumbnailSource;
        private string title;
        private string information;
        private string group;

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        public Uri ImageSource
        {
            get
            {
                return this.imageSource;
            }
            set
            {
                if (this.imageSource != value)
                {
                    this.imageSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image thumbnail source.
        /// </summary>
        public Uri ImageThumbnailSource
        {
            get
            {
                return this.imageThumbnailSource;
            }
            set
            {
                if (this.imageThumbnailSource != value)
                {
                    this.imageThumbnailSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        public string Information
        {
            get
            {
                return this.information;
            }
            set
            {
                if (this.information != value)
                {
                    this.information = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public string Group
        {
            get
            {
                return this.group;
            }
            set
            {
                if (this.group != value)
                {
                    this.group = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.title;
        }

        /// <summary> 
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance. 
        /// </summary> 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param> 
        /// <returns> 
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.  

        /// </returns> 
        public override bool Equals(object obj)
        {
            DataItemViewModel typedObject = obj as DataItemViewModel;
            if (typedObject == null)
            {
                return false;
            }
            return this.Title == typedObject.Title && this.Information == typedObject.Information;
        }

        /// <summary> 
        /// Returns a hash code for this instance. 
        /// </summary> 
        /// <returns> 
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.   

        /// </returns> 
        public override int GetHashCode()
        {
            return this.Title.GetHashCode() ^ this.Information.GetHashCode();
        }
    }
}
