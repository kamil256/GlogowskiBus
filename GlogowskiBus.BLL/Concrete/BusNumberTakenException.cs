using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusNumberTakenException : Exception
    {
        public BusNumberTakenException(string message) : base(message)
        {
        }
    }
}
