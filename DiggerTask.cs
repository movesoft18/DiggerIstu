using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digger.Architecture;

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
        int _imgIndex = 0;

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
            if (_imgIndex == 0)
            {
                s = "Digger.png";
                _imgIndex = 1;
            }
            else
            {
                s = "Digger1.png";
                _imgIndex = 0;
            }
            return s;
        }
    }
    // Sack
    public class Sack : ICreature
    {
        bool _isFallingDown = false;
        int _fallingDistance = 0;

        public CreatureCommand Act(int x, int y)
        {
            if (y < Game.MapHeight - 1)
            {
                var obj = Game.Map[x, y + 1];
                if (obj is null || ((obj is Player || obj is Monster) && _isFallingDown))
                {
                    _isFallingDown = true;
                    _fallingDistance++;
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1, TransformTo = this };
                }
            }
                if (_isFallingDown && _fallingDistance > 1)
                {
                    _isFallingDown = false;
                    _fallingDistance = 0;
                    return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
                }
                _isFallingDown = false;
                _fallingDistance = 0;
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
        bool _isDead = false; // признак, что монстр мертв
        protected int _pause = 4; // задержка передвижения монстра (чтобы двигался медленнее Player'a)
        protected int _imgIndex = 0; // Текущий индекс изображения монстра 0 или 1, в зависимости от индекса меняется спрайт и достигается эффект анимации
        protected int _x = 0; // координата x монстра
        protected int _y = 0; // координата x монстра
        public bool IsDead { get => _isDead; }
        Queue<CommanderCommand> _commandQueue = new Queue<CommanderCommand>(); // очередь команд, которые исполняются подчиненным
        public bool HasCommand { get => _commandQueue.Count > 0; } // свойство, отвечающее за наличие у монстра неисполненных команд
        public int X { get => _x; }
        public int Y { get => _y; }

        public void SetCommand(CommanderCommand command) 
        {
            if (!IsDead)            // если мертв то команды игнорируем
                _commandQueue.Enqueue(command);
        }

        protected bool CanMove(ICreature obj) // проверка может ли монстр переместиться в клетку с объектом obj
        {
            /*_pause--;
            if (_pause == 0)
            {
                _pause = 4;
            }
            else
                return false;*/
            return (obj is Gold) || (obj is null) || (obj is Player);
        }

        public virtual CreatureCommand Act(int x, int y)
        {
            if (_commandQueue.Count > 0) // если есть комманды, берем из очереди
            {
                var cmd = _commandQueue.Dequeue();
                if (CanMove(Game.Map[x + cmd.dx, y + cmd.dy]))
                {
                    _x = x + cmd.dx;
                    _y = y + cmd.dy;
                    return new CreatureCommand { DeltaX = cmd.dx, DeltaY = cmd.dy, TransformTo = this };
                }
            }
            _x = x;
            _y = y;
            return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = this };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            _isDead = (conflictedObject is Monster || conflictedObject is Sack || conflictedObject is CommanderMonster);
            return IsDead;
        }

        public int GetDrawingPriority()
        {
            return 7;
        }

        public virtual string GetImageFileName()
        {
            string s;
            if (_imgIndex == 0)
            {
                s = "Monster.png";
                _imgIndex = 1;
            }
            else
            {
                s = "Monster1.png";
                _imgIndex = 0;
            }
            return s;
        }
    }

    public class CommanderMonster: Monster
    {
        List<Monster> _slaveMonstersList = new List<Monster>();

        public void AddSlaveMonster(Monster monster)
        {
            _slaveMonstersList.Add(monster);
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

        public override CreatureCommand Act(int x, int y)
        {
            int playerX, playerY;
            CommanderCommand cmd = new CommanderCommand { dx = 0, dy = 0 };

            if (NeedMove(out playerX, out playerY)) // Проверяем, существует ли на карте Player и если да, то вычисляем вектор перемешения к нему
            {
                cmd = CalculateDeltaForCommander(x, y, playerX, playerY);
            }
            SendCommandsToSlaveMonsters(playerX, playerY);
            return new CreatureCommand { DeltaX = cmd.dx, DeltaY = cmd.dy, TransformTo = this };
        }

        // вычисление вектора перемещения к игроку монстра-командира
        CommanderCommand CalculateDeltaForCommander(int x, int y, int playerX, int playerY) 
        {
            int dx = 0, dy = 0;
            if (x > playerX && (CanMove(Game.Map[x - 1, y])))
            {
                dx = -1; dy = 0;
            }
            else if (x < playerX && (CanMove(Game.Map[x + 1, y])))
            {
                dx = 1; dy = 0;
            }
            else if (y > playerY && (CanMove(Game.Map[x, y - 1])))
            {
                dx = 0; dy = -1;
            }
            else if (y < playerY && (CanMove(Game.Map[x, y + 1])))
            {
                dx = 0; dy = 1;
            }
            return new CommanderCommand { dx = dx, dy = dy };
        }

        // вычисление вектора перемещения к игроку монстра-подчиненного
        // этот метод и предыдущий разделены, так как это дает возможность реализовать разную логику перемещения монстра к игроку
        // можно наделить монстров интеллектом движения по лабиринту по классическим алгоритмам обхода лабиринтов при желании
        CommanderCommand CalculateDeltaForSlave(int x, int y, int playerX, int playerY)
        {
            int dx = 0, dy = 0;
            if (x > playerX && (CanMove(Game.Map[x - 1, y])))
            {
                dx = -1; dy = 0;
            }
            else if (x < playerX && (CanMove(Game.Map[x + 1, y])))
            {
                dx = 1; dy = 0;
            }
            else if (y > playerY && (CanMove(Game.Map[x, y - 1])))
            {
                dx = 0; dy = -1;
            }
            else if (y < playerY && (CanMove(Game.Map[x, y + 1])))
            {
                dx = 0; dy = 1;
            }
            return new CommanderCommand { dx = dx, dy = dy };
        }

        void SendCommandsToSlaveMonsters(int playerX, int playerY)
        {
            foreach(var m in _slaveMonstersList)
            {
                if (m.IsDead || m.HasCommand ) continue;
                CommanderCommand cmd = CalculateDeltaForSlave(m.X, m.Y, playerX, playerY);
                m.SetCommand(cmd);
                if (Math.Abs(m.X - playerX) > 1 || Math.Abs(m.Y - playerY) > 1)
                    m.SetCommand(cmd);
            }
        }

        public override string GetImageFileName()
        {
            string s;
            if (_imgIndex == 0)
            {
                s = "MonsterCommander.png";
                _imgIndex = 1;
            }
            else
            {
                s = "MonsterCommander1.png";
                _imgIndex = 0;
            }
            return s;
        }
    }
}
