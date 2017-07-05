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
        IList<BusLineBL> Get();
        BusLineBL GetById(int id);
        BusLineBL Insert(BusLineBL busStop);
        BusLineBL Update(BusLineBL busStop);
        int? Delete(int id);
    }
}
