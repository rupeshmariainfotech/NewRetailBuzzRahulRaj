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
    public class OutwardForProductionExpectedInwardService:IOutwardForProductionExpectedInwardService
    {
        private readonly IOutwardForProductionExpectedInwardRepository _OutwardForProductionExpectedInwardRepository;
        private readonly IUnitOfWork _unitOfWork;
        public OutwardForProductionExpectedInwardService(IOutwardForProductionExpectedInwardRepository OutwardForProductionExpectedInwardRepository, IUnitOfWork unitOfWork)
        {
            this._OutwardForProductionExpectedInwardRepository = OutwardForProductionExpectedInwardRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Create(OutwardForProductionExpectedInward data)
        {
            _OutwardForProductionExpectedInwardRepository.Add(data);
            _unitOfWork.Commit();
        }

        public void Update(OutwardForProductionExpectedInward data)
        {
            _OutwardForProductionExpectedInwardRepository.Update(data);
            _unitOfWork.Commit();
        }

        public IEnumerable<OutwardForProductionExpectedInward> GetRowsByOutwardNo(string outward)
        {
            var data = _OutwardForProductionExpectedInwardRepository.GetMany(d => d.OutwardCode == outward);
            return data;
        }
    }
}
