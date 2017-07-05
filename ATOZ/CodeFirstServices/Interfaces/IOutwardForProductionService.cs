using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstEntities;


namespace CodeFirstServices.Interfaces
{
    public interface IOutwardForProductionService
    {
        void Create(OutwardForProduction data);
        void Update(OutwardForProduction data);
        OutwardForProduction GetLastRowrByFinYr(string year);
        IEnumerable<OutwardForProduction> GetActiveOutwardNos(string term);
        OutwardForProduction GetDetailsByOutwardNo(string outwardno);
        IEnumerable<OutwardForProduction> GetReportByDate(DateTime fromdate, DateTime todate);
    }
}
