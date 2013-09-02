using System.Windows;
using System.Windows.Controls;

namespace SparklrWP.Controls
{
    public partial class LoadingOverlay : UserControl
    {
        public new DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(object), new PropertyMetadata(visibilityChanged));

        private static void visibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LoadingOverlay).Visibility = (Visibility)e.NewValue;
        }

        Visibility visibility = Visibility.Visible;
        public new Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    loadingContainer.Visibility = value;
                }
            }
        }

        public LoadingOverlay()
        {
            InitializeComponent();
        }

        public void StartLoading()
        {
            LoadingStarted.Begin();
        }

        public void FinishLoading()
        {
            LoadingFinished.Begin();
        }
    }
}
