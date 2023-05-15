using Abraxas.Cards.Controllers;

namespace Abraxas.Cells.Models
{
    public interface ICellModel
    {
        void AddCard(ICardController card, int index);
        void RemoveCard(ICardController card);
    }
}
