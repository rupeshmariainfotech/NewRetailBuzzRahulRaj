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

        private readonly IClientLeadRepository _clientleadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClientLeadService(IClientLeadRepository clientleadRepository, IUnitOfWork unitOfWork)
        {
            this._clientleadRepository = clientleadRepository;
            this._unitOfWork = unitOfWork;
        }



        public void CreateClientLead(ClientLead ClientLead)
        {
            _clientleadRepository.Add(ClientLead);
            _unitOfWork.Commit();
        }

        public void DeleteClientLead(ClientLead ClientLead)
        {
            _clientleadRepository.Delete(ClientLead);
            _unitOfWork.Commit();
        }

        public void UpdateClientLead(ClientLead ClientLead)
        {
            _clientleadRepository.Update(ClientLead);
            _unitOfWork.Commit();
        }
    }
}




