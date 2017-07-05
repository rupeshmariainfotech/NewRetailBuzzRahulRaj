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
    public class OutwardForProductionItemService:IOutwardForProductionItemService
    {
        private readonly IOutwardForProductionItemRepository _OutwardForProductionItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public OutwardForProductionItemService(IOutwardForProductionItemRepository OutwardForProductionItemRepository, IUnitOfWork unitOfWork)
        {
            this._OutwardForProductionItemRepository = OutwardForProductionItemRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Create(OutwardForProductionItem data)
        {
            _OutwardForProductionItemRepository.Add(data);
            _unitOfWork.Commit();
        }

        public void Update(OutwardForProductionItem data)
        {
            _OutwardForProductionItemRepository.Update(data);
            _unitOfWork.Commit();
        }

        public IEnumerable<OutwardForProductionItem> GetRowsByOutwardNo(string outward)
        {
            var data = _OutwardForProductionItemRepository.GetMany(d => d.OutwardCode == outward);
            return data;
        }

        public IEnumerable<OutwardForProductionItem> GetRowsByOutwardAndItemNo(string ItemNo, string outward)
        {
            var data = _OutwardForProductionItemRepository.GetMany(d => d.OutwardCode == outward && d.ProductionItem == ItemNo);
            return data;
        }
    }
}
