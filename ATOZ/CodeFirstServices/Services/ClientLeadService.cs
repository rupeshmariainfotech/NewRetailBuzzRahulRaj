using System.Collections.Generic;
using CodeFirstData.DBInteractions;
using CodeFirstData.EntityRepositories;
using CodeFirstEntities;
using CodeFirstServices.Interfaces;
using System.Linq;
using System;

namespace CodeFirstServices.Services
{
    public class ClientLeadService : IClientLeadService
    {

        private readonly IClientLeadRepository _ClientLeadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClientLeadService(IClientLeadRepository _ClientLeadRepository, IUnitOfWork unitOfWork)
        {
            this._ClientLeadRepository = _ClientLeadRepository;
            this._unitOfWork = unitOfWork;
        }



        public void CreateClientLead(ClientLead ClientLead)
        {
            _ClientLeadRepository.Add(ClientLead);
            _unitOfWork.Commit();
        }

        public void DeleteClientLead(ClientLead ClientLead)
        {
            _ClientLeadRepository.Delete(ClientLead);
            _unitOfWork.Commit();
        }

        public void UpdateClientLead(ClientLead ClientLead)
        {
            _ClientLeadRepository.Update(ClientLead);
            _unitOfWork.Commit();
        }

        public IEnumerable<ClientLead> GetActiveClientLead()
        {
            var details = _ClientLeadRepository.GetAll();
            return details;
        }

        public ClientLead GetClientLeadById(int id)
        {
            var details = _ClientLeadRepository.GetById(id);
            return details;
        }
    }
}




