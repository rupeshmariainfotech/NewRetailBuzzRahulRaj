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
    public class ReceivedReadyGoodItemService : IReceivedReadyGoodItemService
    {
        private readonly IReceivedReadyGoodItemRepository _ReceivedReadyGoodItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReceivedReadyGoodItemService(IReceivedReadyGoodItemRepository ReceivedReadyGoodItemRepository, IUnitOfWork unitOfWork)
        {
            this._ReceivedReadyGoodItemRepository = ReceivedReadyGoodItemRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Create(ReceivedReadyGoodItem data)
        {
            _ReceivedReadyGoodItemRepository.Add(data);
            _unitOfWork.Commit();
        }

        public void Update(ReceivedReadyGoodItem data)
        {
            _ReceivedReadyGoodItemRepository.Update(data);
            _unitOfWork.Commit();
        }

        public IEnumerable<ReceivedReadyGoodItem> GetRowsByInwardNo(string InwardNo)
        {
            var data = _ReceivedReadyGoodItemRepository.GetMany(d => d.InwardCode == InwardNo);
            return data;
        }
    }
}
