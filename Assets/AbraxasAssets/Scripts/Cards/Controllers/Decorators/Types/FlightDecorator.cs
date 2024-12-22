using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Zones.Fields.Controllers;
using System;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;

class FlightDecorator : DefaultBehaviorDecorator
{
    public FlightDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator MoveAndHandleCollisions(IFieldController field)
    {
        var movement = new Point(Owner == Player.Player1 ? StatBlock.Stats.SPD : -StatBlock.Stats.SPD, 0);
        Point destination = new(
            Math.Clamp(Cell.FieldPosition.X + movement.X, 0, field.FieldGrid[0].Count - 1),
            Cell.FieldPosition.Y);
        var fieldGrid = field.FieldGrid;

        // Flight can move through any number of cards, so we don't need to check for collisions.
        if (fieldGrid[destination.Y][destination.X].CardsOnCell == 0)
        {
            // If the position is open, move the card there directly.
            yield return field.MoveCardToCell(Aggregator, fieldGrid[destination.Y][destination.X]);
            if (fieldGrid[destination.Y][destination.X].Player != Owner && fieldGrid[destination.Y][destination.X].Player != Player.Neutral)
            {
                yield return PassHomeRow();
            }
            yield break;
        }
        yield return base.MoveAndHandleCollisions(field);
    }
}
