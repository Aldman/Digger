﻿using System;
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
        => !(Game.Map[x + deltaX, y + deltaY]
            .GetImageFileName() == "Sack.png"
            || (x + deltaX < 0) || (x + deltaX >= Game.MapWidth)
            || (y + deltaY < 0) || (y + deltaY >= Game.MapHeight));
        

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
            if (conflictedObject is Sack)
                return true;
            else if (conflictedObject is Gold)
                Game.Scores += 10;
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
            return conflictedObject is Player;
        }

        public int GetDrawingPriority() => 0;

        public string GetImageFileName() => "Gold.png";
    }

    public class Sack : ICreature
    {
        int FallenSteps;
        ICreature TransformIn = null;

        public CreatureCommand Act(int x, int y)
        {
            var deltaY = 1;
            if (!String.IsNullOrEmpty(Game.Map[x, y + deltaY].GetImageFileName()))
                deltaY = 0;
            if (FallenSteps > 1 && IsSackStopDrop(x, y, deltaY))
                TransformIn = new Gold();
            FallenSteps++;
            return new CreatureCommand()
            {
                DeltaX = 0,
                DeltaY = deltaY,
                TransformTo = TransformIn
            };
        }

        private bool IsSackStopDrop(int x, int y, int deltaY)
        => Game.Map[x, y + deltaY].GetImageFileName() == "Terrain.png"
            || Game.Map[x, y + deltaY].GetImageFileName() == "Sack.png"
            || Game.Map[x, y + deltaY].GetImageFileName() == "Gold.png"
            || Game.Map[x, y + deltaY].GetImageFileName() == "Sack.png"
            || (y + deltaY >= Game.MapHeight);

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority() => 0;

        public string GetImageFileName() => "Sack.png";
    }
}
