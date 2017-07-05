using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstEntities;
using CodeFirstData.EntityRepositories;
using CodeFirstData.DBInteractions;
using CodeFirstServices.Interfaces;

namespace CodeFirstServices.Services
{
    public class OutwardForProductionService : IOutwardForProductionService
    {
        private readonly IOutwardForProductionRepository _OutwardForProductionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public OutwardForProductionService(IOutwardForProductionRepository OutwardForProductionRepository, IUnitOfWork unitOfWork)
        {
            this._OutwardForProductionRepository = OutwardForProductionRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Create(OutwardForProduction data)
        {
            _OutwardForProductionRepository.Add(data);
            _unitOfWork.Commit();
        }

        public void Update(OutwardForProduction data)
        {
            _OutwardForProductionRepository.Update(data);
            _unitOfWork.Commit();
        }

        public OutwardForProduction GetLastRowrByFinYr(string year)
        {
            var data = _OutwardForProductionRepository.GetMany(p => p.OutwardCode.Contains(year)).OrderBy(p => p.OutwardCode).LastOrDefault();
            return data;
        }

        public IEnumerable<OutwardForProduction> GetActiveOutwardNos(string term)
        {
            var details = _OutwardForProductionRepository.GetMany(m => m.OutwardCode.ToLower().StartsWith(term.ToLower()) && m.Status == "Active").OrderBy(m => m.OutwardCode);
            return details;
        }

        public OutwardForProduction GetDetailsByOutwardNo(string outwardno)
        {
            var details = _OutwardForProductionRepository.Get(d => d.OutwardCode == outwardno);
            return details;
        }

        public IEnumerable<OutwardForProduction> GetReportByDate(DateTime fromdate, DateTime todate)
        {
            var data = _OutwardForProductionRepository.GetMany(d => d.Date.Date >= fromdate.Date && d.Date.Date <= todate.Date);
            return data;
        }
    }
}
