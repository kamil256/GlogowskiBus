using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Abstract
{
    public interface IBusLineService
    {
        IList<BusLine> Get();
        BusLine GetById(int id);
        BusLine Insert(BusLine busStop);
        BusLine Update(BusLine busStop);
        int? Delete(int id);
    }
}
