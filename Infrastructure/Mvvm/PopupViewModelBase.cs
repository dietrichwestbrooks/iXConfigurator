using System.Windows;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public abstract class PopupViewModelBase : ViewModelBase
    {
        private SizeToContent _popupSizeToContent;
        private ResizeMode _popupResizeMode;

        protected PopupViewModelBase()
        {
            PopupSizeToContent = SizeToContent.WidthAndHeight;
            PopupResizeMode = ResizeMode.NoResize;
        }

        public ResizeMode PopupResizeMode
        {
            get { return _popupResizeMode; }
            set { SetProperty(ref _popupResizeMode, value); }
        }

        public SizeToContent PopupSizeToContent
        {
            get { return _popupSizeToContent; }
            set { SetProperty(ref _popupSizeToContent, value); }
        }
    }
}
