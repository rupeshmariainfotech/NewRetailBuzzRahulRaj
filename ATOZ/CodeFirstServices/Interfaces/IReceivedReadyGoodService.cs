using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstEntities;

namespace CodeFirstServices.Interfaces
{
    public interface IReceivedReadyGoodService
    {
        void Create(ReceivedReadyGood data);
        void Update(ReceivedReadyGood data);
        ReceivedReadyGood GetLastRowrByFinYr(string year);
        ReceivedReadyGood GetDetailsByInwardNo(string outwardno);
        IEnumerable<ReceivedReadyGood> GetReceivedGoodsByStatus(string no);
        ReceivedReadyGood GetDataByInwardNo(string inwardno);
    }
}
