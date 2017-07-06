using CodeFirstEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeFirstServices.Interfaces
{
    public interface IClientLeadService
    {

        void CreateClientLead(ClientLead ClientLead);
        void UpdateClientLead(ClientLead ClientLead);
        //ClientMaster getClientById(int id);
        ClientLead getClientById(int id);
        void DeleteClientLead(ClientLead ClientLead);
    }
}
