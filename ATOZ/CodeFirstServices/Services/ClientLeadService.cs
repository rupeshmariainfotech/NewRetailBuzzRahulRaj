﻿using System.Collections.Generic;
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
        private readonly IClientMasterRepository _clientMasterRepository;
        private readonly IClientLeadRepository _clientleadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClientLeadService(IClientLeadRepository clientleadRepository, IClientMasterRepository clientMasterRepository,IUnitOfWork unitOfWork)
        public ClientLeadService(IClientLeadRepository _ClientLeadRepository, IUnitOfWork unitOfWork)
        {
            this._clientleadRepository = clientleadRepository;
            this._clientMasterRepository = clientMasterRepository;
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

        public ClientLead getClientById(int id)
        {
            var client = _clientleadRepository.Get(cl => cl.ClientId == id);
            return client;
        }



        //public ClientMaster getClientById(int id)
        //{
        //    var client = _clientMasterRepository.GetById(id);
        //    return client;
        //}
    }
}




