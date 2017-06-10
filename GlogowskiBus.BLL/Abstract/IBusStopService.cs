using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Abstract
{
    public interface IBusStopService
    {
        IList<BusStop> Get();
        BusStop GetById(int id);
        BusStop Insert(BusStop busStop);
        BusStop Update(BusStop busStop);
        int? Delete(int id);
    }
}