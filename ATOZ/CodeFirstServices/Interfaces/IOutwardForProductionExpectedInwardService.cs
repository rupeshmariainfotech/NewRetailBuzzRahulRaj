using CodeFirstEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstServices.Interfaces
{
    public interface IOutwardForProductionExpectedInwardService
    {
        void Create(OutwardForProductionExpectedInward data);
        void Update(OutwardForProductionExpectedInward data);
        IEnumerable<OutwardForProductionExpectedInward> GetRowsByOutwardNo(string outward);
    }
}
