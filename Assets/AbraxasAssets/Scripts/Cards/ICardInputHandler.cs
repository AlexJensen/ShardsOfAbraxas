using UnityEngine.EventSystems;

namespace Abraxas.Behaviours.Cards
{
    internal interface ICardInputHandler: IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
    }
}