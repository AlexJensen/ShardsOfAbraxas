using Abraxas.Core;
using Abraxas.Behaviours.Cards;

namespace Abraxas.Behaviours.Events
{
    public class EventManager : Singleton<EventManager>
    {
        public delegate void GameEvent(params object[] vals);

        public event GameEvent OnCardMoved;
        public event GameEvent OnCardDestroyed;
        public event GameEvent OnCardEnteredField;
        public event GameEvent OnBeginningStateStarted;
        public event GameEvent OnBeforeCombatStarted;

        public void CardMove(Card card)
        {
            if (OnCardMoved != null) OnCardMoved(card);
        }
        public void CardDestroyed(Card card)
        {
            if (OnCardDestroyed != null) OnCardDestroyed(card);
        }
        public void CardEnteredField(Card card)
        {
            if (OnCardEnteredField != null) OnCardEnteredField(card);
        }
        public void BeginningStateStarted()
        {
            if (OnBeginningStateStarted != null) OnBeginningStateStarted();
        }
        public void BeforeCombatStarted()
        {
            if (OnBeforeCombatStarted != null) OnBeforeCombatStarted();
        }
    }
}