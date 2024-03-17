using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puissance4.Business.Exceptions
{
    public class GameException(string message): Exception(message)
    {
    }
}
