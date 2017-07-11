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

        void DeleteClientLead(ClientLead ClientLead);

        IEnumerable<ClientLead> GetActiveClientLead();
        ClientLead GetClientLeadById(int id);

        IEnumerable<ClientLead> GetAllClients();

        ClientLead getClientById(int id);
    }
}
