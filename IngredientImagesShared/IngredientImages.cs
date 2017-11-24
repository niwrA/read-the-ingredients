using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientImagesShared
{
    public interface IIngredientImage
    {
        Guid Guid { get; }
        string Url { get; }
        int WikiDataId { get; }
    }
    public class IngredientImage : IIngredientImage
    {
        private IngredientImages.IIngredientImageState _state;
        public IngredientImage(IngredientImages.IIngredientImageState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Url { get { return _state.Url; } }
        public int WikiDataId { get { return _state.WikiDataId; } }
    }

    public class IngredientImages
    {
        private IIngredientImageRepository _repo;
        public IngredientImages(IIngredientImageRepository repo)
        {
            _repo = repo;
        }
        public interface IIngredientImageState
        {
            Guid Guid { get; set; }
            string Url { get; set; }
            int WikiDataId { get; set; }
        }
        public interface IIngredientImageRepository
        {
            IIngredientImageState CreateIngredientImageState();
            void UpdateIngredientImageState(IIngredientImageState state);
            IEnumerable<IIngredientImageState> GetIngredientImageStates(int wikiDataId);
            void PersistChanges();
        }
        public IEnumerable<IIngredientImage> GetIngredientImages(int wikiDataId)
        {
            var states = _repo.GetIngredientImageStates(wikiDataId);
            return WrapStates(states);
        }

        private IEnumerable<IIngredientImage> WrapStates(IEnumerable<IIngredientImageState> states)
        {
            List<IngredientImage> results = new List<IngredientImage>();
            foreach(var state in states)
            {
                results.Add(new IngredientImage(state));
            }
            return results;    
        }
}
}
