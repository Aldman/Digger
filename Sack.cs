using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger
{
    public class SackS : ICreature
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
