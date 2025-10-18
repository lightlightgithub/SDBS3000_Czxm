using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SDBS3000.Ucs.Controls
{
    public class CareTextBox : TextBox
    {
        private UIElement _flowDocumentView;
        private AdornerLayer _adornerLayer;
        private UIElement _caretSubElement;
        private ScaleTransform _scaleTransform;
        private bool _needNew = true;
        private Viewbox _mainView;

        public CareTextBox()
        {
            Loaded += (a, b) =>
            {
                _mainView = GetParentObject<Viewbox>(this, "");
                if (_mainView == null) return;
                PreviewGotKeyboardFocus -= HandleGetFocus;
                this.LayoutUpdated -= OnResetCaret;

                PreviewGotKeyboardFocus += HandleGetFocus;
                this.LayoutUpdated += OnResetCaret;
            };
        }

        private void OnResetCaret(object s, EventArgs args)
        {
            //_mainView = BLHelper.GetParentObject<Viewbox>(this, "MainViewBox");
            if (_mainView == null) return;
            var child = VisualTreeHelper.GetChild(_mainView, 0) as ContainerVisual;
            if (child == null)
                return;
            var scale = child.Transform as ScaleTransform;
            this.ScaleX = scale.ScaleX;

            if (!IsKeyboardFocused) return;
            if (_adornerLayer == null)
            {
                if (_flowDocumentView == null) return;
                _adornerLayer = AdornerLayer.GetAdornerLayer(_flowDocumentView);
            }
            if (_adornerLayer == null || _flowDocumentView == null) return;
            if (_scaleTransform != null && _caretSubElement != null && !_needNew)
            {
                _scaleTransform.ScaleX = 1 / ScaleX;
            }
            else
            {
                var adorners = _adornerLayer.GetAdorners(_flowDocumentView);
                if (adorners == null || adorners.Length < 1) return;
                var caret = adorners[0];
                _caretSubElement = (UIElement)VisualTreeHelper.GetChild(caret, 0);
                if (_caretSubElement == null) return;
                if (!(_caretSubElement.RenderTransform is ScaleTransform))
                {
                    _scaleTransform = new ScaleTransform(1 / ScaleX, 1);
                    _caretSubElement.RenderTransform = _scaleTransform;
                }
                _needNew = false;
            }
        }

        private void HandleGetFocus(object s, KeyboardFocusChangedEventArgs args)
        {
            _needNew = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var cthost = GetTemplateChild("PART_ContentHost") as FrameworkElement;
            if (cthost == null)
                return;
            _flowDocumentView = cthost is ScrollViewer ? (UIElement)((ScrollViewer)cthost).Content : ((Decorator)cthost).Child;
        }

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(CareTextBox), new UIPropertyMetadata(1.0));

        public T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
