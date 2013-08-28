using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;

namespace SparklrWP.Pages
{
    //Most stuff taken from http://www.frenk.com/2011/03/windows-phone-7-correct-pinch-zoom-in-silverlight/
    /// <summary>
    /// Provides pinch to zoom for an image. Can either be naviagted to with ?image=URLENCODED-LOCATION or instanciated with a location
    /// </summary>
    public partial class PinchToZoom : PhoneApplicationPage
    {
        // these two fields fully define the zoom state:
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);


        private const double MAX_IMAGE_ZOOM = 5;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;

        public PinchToZoom()
        {
            InitializeComponent();
        }

        public PinchToZoom(string location)
        {
            InitializeComponent();
            loadImage(location);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string location;

            if (NavigationContext.QueryString.TryGetValue("image", out location))
            {
                loadImage(location);
            }
        }

        private async void loadImage(string location)
        {
            ZoomableImage.Source = await Utils.Helpers.LoadImageFromUrlAsync(location);
            LoadingFinished.Begin();
        }

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ZoomableImage, 0);
            _oldFinger2 = e.GetPosition(ZoomableImage, 1);
            _oldScaleFactor = 1;
        }

        private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(ZoomableImage, 0);
            var currentFinger2 = e.GetPosition(ZoomableImage, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #region Utils

        /// <summary>
        /// Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        /// Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            TotalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        /// Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform)ZoomableImage.RenderTransform).ScaleX = TotalImageScale;
            ((CompositeTransform)ZoomableImage.RenderTransform).ScaleY = TotalImageScale;
        }

        /// <summary>
        /// Updates the image position by applying the delta.
        /// Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((ZoomableImage.ActualWidth * TotalImageScale) + newPosition.X < ZoomableImage.ActualWidth)
                newPosition.X = ZoomableImage.ActualWidth - (ZoomableImage.ActualWidth * TotalImageScale);

            if ((ZoomableImage.ActualHeight * TotalImageScale) + newPosition.Y < ZoomableImage.ActualHeight)
                newPosition.Y = ZoomableImage.ActualHeight - (ZoomableImage.ActualHeight * TotalImageScale);

            ImagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        /// Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform)ZoomableImage.RenderTransform).TranslateX = ImagePosition.X;
            ((CompositeTransform)ZoomableImage.RenderTransform).TranslateY = ImagePosition.Y;
        }

        /// <summary>
        /// Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            TotalImageScale = 1;
            ImagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        /// Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((ZoomableImage.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < ZoomableImage.ActualWidth)
                return false;

            if ((ZoomableImage.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < ZoomableImage.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
        }

        #endregion

    }
}