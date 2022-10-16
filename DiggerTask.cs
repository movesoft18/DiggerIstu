using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger
{
    //Напишите здесь классы Player, Terrain и другие.
    //Terrain
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this};
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Player;
        }

        public int GetDrawingPriority()
        {
            return 10;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }
    // Player
    public class Player : ICreature
    {
        int imgState = 0;

        bool CanMove(ICreature obj)
        {
            return (obj is Terrain) || (obj is Gold) || (obj is null);
        }

        public CreatureCommand Act(int x, int y)
        {
            switch (Game.KeyPressed)
            {
                case System.Windows.Forms.Keys.Up:
                    if (y > 0 && CanMove(Game.Map[x, y-1]))
                    {
                        return new CreatureCommand { DeltaX = 0, DeltaY = -1, TransformTo = this };
                    }
                    break;
                case System.Windows.Forms.Keys.Down:
                    if (y < Game.MapHeight - 1 && CanMove(Game.Map[x, y + 1]))
                    {
                        return new CreatureCommand { DeltaX = 0, DeltaY = 1, TransformTo = this };
                    }
                    break;
                case System.Windows.Forms.Keys.Right:
                    if (x < Game.MapWidth - 1 && CanMove(Game.Map[x + 1, y]))
                    {
                        return new CreatureCommand { DeltaX = 1, DeltaY = 0, TransformTo = this };
                    }
                    break;
                case System.Windows.Forms.Keys.Left:
                    if (x > 0 && CanMove(Game.Map[x - 1, y]))
                    {
                        return new CreatureCommand { DeltaX = -1, DeltaY = 0, TransformTo = this };
                    }
                    break;
            }
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Gold)
                Game.Scores += 10;
            return (conflictedObject is Sack || conflictedObject is Monster);
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            string s;
            if (imgState == 0)
            {
                s = "Digger.png";
                imgState = 1;
            }
            else
            {
                s = "Digger1.png";
                imgState = 0;
            }
            return s;
        }
    }
    // Sack
    public class Sack : ICreature
    {
        bool isFallingDown = false;
        int fallingDistance = 0;

        public CreatureCommand Act(int x, int y)
        {
            if (y < Game.MapHeight - 1)
            {
                var obj = Game.Map[x, y + 1];
                if (obj is null || ((obj is Player || obj is Monster) && isFallingDown))
                {
                    isFallingDown = true;
                    fallingDistance++;
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1, TransformTo = this };
                }
            }
                if (isFallingDown && fallingDistance > 1)
                {
                    isFallingDown = false;
                    fallingDistance = 0;
                    return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
                }
                isFallingDown = false;
                fallingDistance = 0;
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return (conflictedObject is Terrain);
        }

        public int GetDrawingPriority()
        {
            return 9;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    //Gold
    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return (conflictedObject is Player || conflictedObject is Monster);
        }

        public int GetDrawingPriority()
        {
            return 8;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    //Monster
    public class Monster : ICreature
    {
        private int pause = 4;
        int imgState = 0;

        bool CanMove(ICreature obj)
        {
            pause--;
            if (pause == 0)
            {
                pause = 4;
            }
            else
                return false;
            return (obj is Gold) || (obj is null) || (obj is Player);
        }

        bool NeedMove(out int px, out int py)
        {
            for (int i = 0; i < Game.MapWidth; i++)
            {
                for (int j = 0; j < Game.MapHeight; j++)
                {
                    if (Game.Map[i, j] is Player)
                    {
                        px = i;
                        py = j;
                        return true;
                    }
                }
            }
            px = py = -1;
            return false;
        }

        public CreatureCommand Act(int x, int y)
        {
            int playerX, playerY;
            if (NeedMove(out playerX, out playerY))
            {
                if (x > playerX && (CanMove(Game.Map[x-1,y])))
                {
                    return new CreatureCommand { DeltaX = -1, DeltaY = 0, TransformTo = this };
                }
                if (x < playerX && (CanMove(Game.Map[x + 1, y])))
                {
                    return new CreatureCommand { DeltaX = 1, DeltaY = 0, TransformTo = this };
                }
                if (y > playerY && (CanMove(Game.Map[x, y - 1])))
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = -1, TransformTo = this };
                }
                if (y < playerY && (CanMove(Game.Map[x, y + 1])))
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1, TransformTo = this };
                }

            }
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return (conflictedObject is Monster || conflictedObject is Sack);
        }

        public int GetDrawingPriority()
        {
            return 7;
        }

        public string GetImageFileName()
        {
            string s;
            if (imgState == 0)
            {
                s = "Monster.png";
                imgState = 1;
            }
            else
            {
                s = "Monster1.png";
                imgState = 0;
            }
            return s;
        }
    }
}
