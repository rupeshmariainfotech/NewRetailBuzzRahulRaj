using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstEntities;


namespace CodeFirstServices.Interfaces
{
    public interface IOutwardForProductionItemService
    {
        void Create(OutwardForProductionItem data);
        void Update(OutwardForProductionItem data);
        IEnumerable<OutwardForProductionItem> GetRowsByOutwardNo(string outward);
        IEnumerable<OutwardForProductionItem> GetRowsByOutwardAndItemNo(string outward, string ItemNo);
    }
}
