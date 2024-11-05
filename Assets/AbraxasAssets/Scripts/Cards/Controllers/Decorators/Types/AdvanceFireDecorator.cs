using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Zones.Fields.Controllers;
using System;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;

class AdvanceFireDecorator : CardControllerDecorator
{
    public AdvanceFireDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    protected override bool CanPerformPreMovementAction() => true;
    protected override bool CanPerformPostMovementAction() => false;
    private bool _hasAttacked = false;

    protected override IEnumerator PreMovementAction(IFieldController field)
    {
        if (StatBlock.Stats.RNG > 0)
        {
            _hasAttacked = false;
            var target = CheckRangedAttack(field, new Point(Owner == Player.Player1 ? StatBlock.Stats.RNG : -StatBlock.Stats.RNG, 0));
            if (target != null)
            {
                yield return Attack(target);
                _hasAttacked = true;
            }
        }
        yield break;
    }

    protected override IEnumerator MoveAndHandleCollisions(IFieldController field)
    {
        var movement = new Point(Owner == Player.Player1 ? StatBlock.Stats.SPD : -StatBlock.Stats.SPD, 0);
        Point destination = new(
            Math.Clamp(Cell.FieldPosition.X + movement.X, 0, field.FieldGrid[0].Count - 1),
            Cell.FieldPosition.Y);

        ICardController collided = null;
        var fieldGrid = field.FieldGrid;

        // Movement and collision detection
        for (int i = Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
        {
            if (fieldGrid[Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
            destination.X = i - Math.Sign(movement.X);
            collided = fieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0);
            break;
        }

        // Move the card if necessary
        if (destination != Cell.FieldPosition)
        {
            yield return field.MoveCardToCell(GetBaseCard(), fieldGrid[destination.Y][destination.X]);
        }

        // Handle collision
        if (collided != null)
        {
            if (!_hasAttacked)
            {
                yield return Fight(collided);
            }
            else
            {
                // Only the collided card deals damage
                yield return collided.Attack(GetBaseCard());
            }
        }
        else if (fieldGrid[destination.Y][destination.X].Player != Owner && fieldGrid[destination.Y][destination.X].Player != Player.Neutral)
        {
            yield return PassHomeRow();
        }

        yield break;
    }
}
