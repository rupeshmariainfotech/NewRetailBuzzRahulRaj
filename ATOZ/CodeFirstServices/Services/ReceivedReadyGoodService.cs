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
    public class ReceivedReadyGoodService : IReceivedReadyGoodService
    {
        private readonly IReceivedReadyGoodRepository _ReceivedReadyGoodRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReceivedReadyGoodService(IReceivedReadyGoodRepository ReceivedReadyGoodRepository, IUnitOfWork unitOfWork)
        {
            this._ReceivedReadyGoodRepository = ReceivedReadyGoodRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Create(ReceivedReadyGood data)
        {
            _ReceivedReadyGoodRepository.Add(data);
            _unitOfWork.Commit();
        }

        public void Update(ReceivedReadyGood data)
        {
            _ReceivedReadyGoodRepository.Update(data);
            _unitOfWork.Commit();
        }

        public ReceivedReadyGood GetLastRowrByFinYr(string year)
        {
            var data = _ReceivedReadyGoodRepository.GetMany(p => p.InwardCode.Contains(year)).OrderBy(p => p.InwardCode).LastOrDefault();
            return data;
        }

        public ReceivedReadyGood GetDetailsByInwardNo(string outwardno)
        {
            var data = _ReceivedReadyGoodRepository.Get(d => d.OutwardForProductionCode == outwardno);
            return data;
        }

        public IEnumerable<ReceivedReadyGood> GetReceivedGoodsByStatus(string no)
        {
            var list = _ReceivedReadyGoodRepository.GetMany(l1 => l1.InwardCode.ToLower().Contains(no.ToString().ToLower()) && l1.Status=="Active").OrderBy(s => s.InwardCode);
            return list;
        }
        
        public ReceivedReadyGood GetDataByInwardNo(string inwardno)
        {
            var data = _ReceivedReadyGoodRepository.Get(d => d.InwardCode == inwardno);
            return data;
        }
    }
}
