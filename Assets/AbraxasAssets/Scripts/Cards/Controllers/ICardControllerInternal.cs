using Abraxas.StatusEffects;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// ICardControllerInternal is an interface for non publically accessable Card Controller fields and methods. These should only be used by Card Controllers for internal decorator patterning processes.
    /// </summary>
    internal interface ICardControllerInternal : ICardController
    {
        void ApplyStatusEffect(IStatusEffect effect);
        bool HasStatusEffect<T>() where T : IStatusEffect;
        void RemoveStatusEffect<T>() where T : IStatusEffect;
    }
}
