using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstEntities;

namespace CodeFirstServices.Interfaces
{
    public interface IReceivedReadyGoodItemService
    {
        void Create(ReceivedReadyGoodItem data);
        void Update(ReceivedReadyGoodItem data);
        IEnumerable<ReceivedReadyGoodItem> GetRowsByInwardNo(string InwardNo);
    }
}
