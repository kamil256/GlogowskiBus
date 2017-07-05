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
        IList<BusStopBL> Get();
        BusStopBL GetById(int id);
        BusStopBL Insert(BusStopBL busStop);
        BusStopBL Update(BusStopBL busStop);
        int? Delete(int id);
    }
}