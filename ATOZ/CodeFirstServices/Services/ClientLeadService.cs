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
      //  private readonly IClientMasterRepository _clientMasterRepository;
       // private readonly IClientLeadRepository _clientleadRepository;
        private readonly IUnitOfWork _unitOfWork;

     //   public ClientLeadService(IClientLeadRepository clientleadRepository, IClientMasterRepository clientMasterRepository,IUnitOfWork unitOfWork)
        public ClientLeadService(IClientLeadRepository _ClientLeadRepository, IUnitOfWork unitOfWork)
        {
            //this._clientleadRepository = clientleadRepository;
            //this._clientMasterRepository = clientMasterRepository;
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
            var client = _ClientLeadRepository.Get(cl => cl.ClientId == id);
            return client;
        }

        public ClientLead GetLastInsertedClientLead()
        {
            var client = _ClientLeadRepository.GetAll().LastOrDefault();
            return client;

        }

        //public ClientMaster getClientById(int id)
        //{
        //    var client = _clientMasterRepository.GetById(id);
        //    return client;
        //}

        public IEnumerable<ClientLead> GetAllClients()
        {
            var list = _ClientLeadRepository.GetAll();
            return list;
        }
        //public ClientLead getClientById(int id)
        //{
        //    var client = _ClientLeadRepository.Get(cl => cl.ClientLeadId == id);
        //    return client;
        //}


        public ClientLead GetLastInsertedClient()
        {
            var client = _ClientLeadRepository.GetAll().LastOrDefault();
            return client;

        }
    }
}




