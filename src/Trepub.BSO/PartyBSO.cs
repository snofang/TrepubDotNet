using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Trepub.Common;
using Trepub.Common.Entities;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using Trepub.Common.Utils;

namespace Trepub.BSO
{
    public class PartyBSO
    {
        private readonly IRepository<Party> partyRepo;
        private readonly ILogger<PartyBSO> logger;
        private readonly IDMapBSO idMapBSO;

        public PartyBSO()
        {
            this.partyRepo = FacadeProvider.IfsFacade.GetRepository<Party>();
            this.logger = FacadeProvider.IfsFacade.GetLogger<PartyBSO>();
            this.idMapBSO = BusinessFacade.GetBSO<IDMapBSO>();
        }

        public virtual Party CreatePersonParty()
        {
            //creating party
            var party = new Party();
            party.Type = EntityConstants.PARTY_TYPE_PERSON;
            CreateParty(party);

            return party;
        }

        public virtual Party UpdateParty(Party party)
        {
            var fetcheParty = GetPartySlim(party.PartyId);
            party.Status = fetcheParty.Status;
            party.Type = fetcheParty.Type;
            this.partyRepo.Update(party);
            this.logger.LogInformation($"party update: {ObjectDumper.Dump(party)}");
            return GetParty(party.PartyId);
        }

        public virtual Party GetParty(int partyId)
        {
            var party = GetPartySlim(partyId);
            return party;
        }

        private void CreateParty(Party party)
        {
            party.Status = EntityConstants.PARTY_STATUS_ACTIVE;
            party.PartyId = this.idMapBSO.GetNextID(EntityConstants.IDMAP_PARTYID);
            this.partyRepo.Add(party);
            this.logger.LogInformation($"party created:\n{ObjectDumper.Dump(party)}");
        }
        
        private Party GetPartySlim(int partyId)
        {
            return this.partyRepo.GetSingle(p => p.PartyId == partyId);
        }

        
    }
}
