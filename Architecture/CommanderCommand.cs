using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Architecture
{
    public struct CommanderCommand // команда подчиненному монстру идти относительно текущей позиции точку, определенную смещениями dx и dy
    {
        public int dx;
        public int dy;
    }
}
