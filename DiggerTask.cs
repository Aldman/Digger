using System;
using System.Drawing;
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
            if (!CanPlayerPassWithoutObstacle(x, y, deltaX, deltaY))
            { deltaX = 0; deltaY = 0; }

            return new CreatureCommand()
            {
                DeltaX = deltaX,
                DeltaY = deltaY,
                TransformTo = this.TransformTo
            };
        }

        private bool CanPlayerPassWithoutObstacle(int x, int y
            , int deltaX, int deltaY)
        {
            var isOutside = IsCreatureTryGoOutsideFromMap(
                x, y, deltaX, deltaY);
            if (!isOutside && Game.Map[x + deltaX, y + deltaY] != null)
                return !(Game.Map[x + deltaX, y + deltaY]
                 .GetImageFileName() == "Sack.png"
                 || isOutside);
            else return !isOutside;
        }

        public static bool IsCreatureTryGoOutsideFromMap(int x, int y
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
            if (conflictedObject is Gold)
                Game.Scores += 10;
            return (conflictedObject is Sack) || (conflictedObject is Monster);
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

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand()
            {
                DeltaX = 0,
                DeltaY = 0
            };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return (conflictedObject is Player)
                    || (conflictedObject is Monster);
        }

        public int GetDrawingPriority() => 0;

        public string GetImageFileName() => "Gold.png";
    }

    public class Sack : ICreature
    {
        int fallenSteps;
        ICreature transformIn = null;

        public CreatureCommand Act(int x, int y)
        {
            var deltaY = 0;
            if ((!Player.IsCreatureTryGoOutsideFromMap(x, y, 0, 1))
                && Game.Map[x, y + 1] == null)
            {
                deltaY = 1;
                fallenSteps++;
            }
            else if ((!Player.IsCreatureTryGoOutsideFromMap(x, y, 0, 1))
                && fallenSteps > 0 && (!IsSackStopDrop(x, y, 1)))
            { deltaY = 1; fallenSteps++; }
            if (fallenSteps > 1 && IsSackStopDrop(x, y, deltaY))
                transformIn = new Gold();
            else if (IsSackStopDrop(x, y, deltaY))
                fallenSteps = 0;

            return new CreatureCommand()
            {
                DeltaX = 0,
                DeltaY = deltaY,
                TransformTo = transformIn
            };
        }

        private bool IsSackStopDrop(int x, int y, int deltaY)
        {
            if (Player.IsCreatureTryGoOutsideFromMap(x, y, 0, deltaY)) return true;
            else if (Game.Map[x, y + deltaY] == null) return false;
            else if (Game.Map[x, y + deltaY].GetImageFileName() == "Digger.png"
                  || Game.Map[x, y + deltaY].GetImageFileName() == "Monster.png")
                return false;
            return Game.Map[x, y + deltaY].GetImageFileName() == "Terrain.png"
            || Game.Map[x, y + deltaY].GetImageFileName() == "Sack.png"
            || Game.Map[x, y + deltaY].GetImageFileName() == "Gold.png"
            || (y + deltaY >= Game.MapHeight);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority() => 0;

        public string GetImageFileName() => "Sack.png";
    }

    public class Monster : ICreature
    {
        bool wasCheckEnvironment = false;
        bool isDiggerExist = false;
        Point diggerCoordinates;

        public CreatureCommand Act(int x, int y)
        {
            int deltaX = 0, deltaY = 0;
            if (!wasCheckEnvironment || GetKeyDirection(Game.KeyPressed) != Direction.Other)
                isDiggerExist = HasMapDigger(diggerCoordinates);
            wasCheckEnvironment = true;
            if (isDiggerExist)
            {
                deltaX = (diggerCoordinates.X - x);
                if (deltaX != 0) deltaX /= Math.Abs(deltaX);

                deltaY = diggerCoordinates.Y - y;
                if (deltaY != 0) deltaY /= Math.Abs(deltaY);
            }
            UpdateMonsterMoving(x, y, ref deltaX, ref deltaY);

            return new CreatureCommand
            {
                DeltaX = deltaX,
                DeltaY = deltaY,
            };
        }

        private Direction GetKeyDirection(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.Left: return Direction.Left;
                case Keys.Right: return Direction.Right;
                case Keys.Up: return Direction.Up;
                case Keys.Down: return Direction.Down;
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

        private void UpdateMonsterMoving(int x, int y, ref int deltaX, ref int deltaY)
        {
            if (Game.Map[x + deltaX, y] == null) { deltaY = 0; return; }
            else if (Game.Map[x, y + deltaY] == null) { deltaX = 0; return; }
            else if (Game.Map[x + deltaX, y].GetImageFileName() == "Digger.png")
            { deltaY = 0; return; }
            else if (Game.Map[x, y + deltaY].GetImageFileName() == "Digger.png")
            { deltaX = 0; return; }
            else if (Game.Map[x + deltaX, y].GetImageFileName() == "Sack.png"
                  || Game.Map[x + deltaX, y].GetImageFileName() == "Monster.png"
                  || Game.Map[x + deltaX, y].GetImageFileName() == "Terrain.png")
            { deltaX = 0; return; }
            else if (Game.Map[x, y + deltaY].GetImageFileName() == "Monster.png"
                  || Game.Map[x, y + deltaY].GetImageFileName() == "Terrain.png")
            { deltaY = 0; return; }
            else return;
        }

        private bool HasMapDigger(Point diggerCoordinates)
        {
            for (int i = 0; i < Game.MapHeight; i++)
                for (int j = 0; j < Game.MapWidth; j++)
                    if (Game.Map[j, i] != null
                                  && Game.Map[j, i]
                                  .GetImageFileName()
                                  == "Digger.png")
                    {
                        diggerCoordinates = new Point(j, i);
                        return true;
                    }
            return false;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority() => 1;

        public string GetImageFileName() => "Monster.png";
    }
}
