namespace Abraxas.Popups
{
    public interface IPopupWindowManager
    {
        public IPopup PopupWindow<T>(bool val) where T : IPopup;
    }
}
