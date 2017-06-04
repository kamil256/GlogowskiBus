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
        void Insert(BusStop busStop);
        void Update(BusStop busStop);
        void Delete(int id);
    }
}