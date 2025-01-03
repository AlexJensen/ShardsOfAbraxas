using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Fields.Controllers;
using System;
using System.Drawing;

class BondDecorator : CardDecorator
{
    public BondDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override ICardController FindCollisionAlongPath(IFieldController field, ref Point destination, Point movement)
    {
        if (movement.X == 0) return null;

        var fieldGrid = field.FieldGrid;
        int step = Math.Sign(movement.X);

        // Start one step ahead of our current X in the direction of movement
        for (int i = Cell.FieldPosition.X + step; i != destination.X + step; i += step)
        {
            // Check if there's a card in the path
            if (fieldGrid[Cell.FieldPosition.Y][i].CardsOnCell > 0)
            {
                var collided = fieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                if (collided.Owner == Aggregator.Owner && fieldGrid[Cell.FieldPosition.Y][i].CardsOnCell == 1)
                {
                    // Collided with a card at [Y][i] that has the Plentiful status effect.
                    // Adjust the destination to the same cell as the other unit.
                    destination.X = i;
                    return collided;
                }
                // Collided with a card at [Y][i]. 
                // Adjust the destination to just before the collision.
                destination.X = i - step;
                return collided;
            }
        }

        return null; // No collision found
    }

    public override bool IsCellAvailable(ICellController cell)
    {
        if (cell.Player != Owner || cell.CardsOnCell >= 2)
            return false;

        if (cell.CardsOnCell == 0)
            return true;

        var occupant = cell.GetCardAtIndex(0);
        if (occupant.Owner == Owner)
        {
            return true;
        }

        return false;
    }


}
