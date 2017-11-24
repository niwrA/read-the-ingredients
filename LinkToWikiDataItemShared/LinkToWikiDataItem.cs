using System;
using System.Collections.Generic;
using System.Text;

namespace LinkToWikiDataItemShared
{
    public class LinkToWikiDataItems
    {
        public interface ILinkToWikiDataState
        {
            Guid Guid { get; set; }
            Guid ItemGuid { get; set; }
            int WikiDataId { get; set; }
        }
        public interface ILinkToWikiDataItemRepository
        {
            ILinkToWikiDataState CreateLinkToWikiDataState();
            void UpdateLinkToWikiDataState(ILinkToWikiDataState state);
            void PersistChanges();
        }

        private ILinkToWikiDataItemRepository _repo;
        public LinkToWikiDataItems(ILinkToWikiDataItemRepository repo)
        {
            _repo = repo;
        }

        public ILinkToWikiDataState LinkToWikiDataId(Guid itemGuid, int wikiDataId)
        {
            var state = _repo.CreateLinkToWikiDataState();
            state.ItemGuid = itemGuid;
            state.WikiDataId = wikiDataId;
            state.Guid = Guid.NewGuid();
            _repo.UpdateLinkToWikiDataState(state);
            return state;
        }

        //public ILinkToWikiDataState GetWikiDataIdByItemGuid(Guid itemGuid)
        //{

        //}
    }
}
