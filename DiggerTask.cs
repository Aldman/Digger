using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Digger
{
    //Напишите здесь классы Player, Terrain и другие.
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {
        public int X, Y;
        public ICreature TransformTo;
        public CreatureCommand Act(int x, int y)
        {
            var deltaX = 0; var deltaY = 0;
            GetKeyDirection(Game.KeyPressed, ref deltaX, ref deltaY);
            if (IsPlayerTryGoOutsideFromMap(x, y, deltaX, deltaY))
            { deltaX = 0; deltaY = 0; }

            return new CreatureCommand()
            {
                DeltaX = deltaX,
                DeltaY = deltaY,
                TransformTo = this.TransformTo    
            };
        }

        private bool IsPlayerTryGoOutsideFromMap(int x, int y
            , int deltaX, int deltaY)
            => (x + deltaX < 0) || (x + deltaX >= Game.MapWidth)
            || (y + deltaY < 0) || (y + deltaY >= Game.MapHeight);

        private Direction GetKeyDirection(
            Keys keyPressed, ref int deltaX, ref int deltaY)
        {
            switch (keyPressed)
            {
                case Keys.Left: deltaX = -1; return Direction.Left;
                case Keys.Right: deltaX = 1; return Direction.Right;
                case Keys.Up: deltaY = -1; return Direction.Up;
                case Keys.Down: deltaY = 1; return Direction.Down;
                default: return Direction.Other;
            }
        }

        enum Direction
        {
            Right,
            Left,
            Up,
            Down,
            Other
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }
}
