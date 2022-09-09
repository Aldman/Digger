using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Digger
{
    public class Monster2 : ICreature
    {
        bool wasCheckEnvironment = false;
        bool isDiggerExist = false;
        Point diggerCoordinates;
        ICreature transformCreature;

        public CreatureCommand Act(int x, int y)
        {
            int deltaX = 0, deltaY = 0;
            if (!wasCheckEnvironment)
                isDiggerExist = HasMapDigger(diggerCoordinates);
            wasCheckEnvironment = true;
            if (isDiggerExist)
            {
                deltaX = (diggerCoordinates.X - x);
                deltaX = deltaX / Math.Abs(deltaX);
                
                deltaY = diggerCoordinates.Y - y;
                deltaY /= Math.Abs(deltaY);
            }
            UpdateMonsterMoving(x, y, ref deltaX, ref deltaY);

            return new CreatureCommand
            {
                DeltaX = deltaX,
                DeltaY = deltaY,
                TransformTo = transformCreature
            };
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
                  || Game.Map[x + deltaX, y].GetImageFileName() == "Monster.png")
            { deltaX = 0; return; }
            else if (Game.Map[x, y + deltaY].GetImageFileName() == "Monster.png")
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
                        return true ;
                    }
            return false;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            throw new NotImplementedException();
        }

        public string GetImageFileName()
        {
            throw new NotImplementedException();
        }
    }
}
