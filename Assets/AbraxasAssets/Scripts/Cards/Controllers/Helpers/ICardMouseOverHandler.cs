namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// ICardMouseOverHandler is an interface for handling card mouse over events.
    /// </summary>
    public interface ICardMouseOverHandler
    {
        void OnPointerEnter();
        void OnPointerExit();
    }
}
