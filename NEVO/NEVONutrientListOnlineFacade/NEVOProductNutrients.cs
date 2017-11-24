using System;
using System.Collections.Generic;
using System.Text;

namespace NEVONutrientListOnlineFacade
{
    public class NEVOProductNutrient
    {
        private INEVOProductNutrientState _state;
        public NEVOProductNutrient(INEVOProductNutrientState state)
        {
            _state = state;
        }
        public interface INEVOProductNutrientState
        {
            int ProductId { get; set; }
            string NutrientUID { get; set; }
            string Quantity { get; set; }
            string SourceUID { get; set; }
            string MutationDate { get; set; }
        }
        public int ProductId { get { return _state.ProductId; } }
        public string NutrientUID { get { return _state.NutrientUID; } }
        public string Quantity { get { return _state.Quantity; } }
        public string SourceUID { get { return _state.SourceUID; } }
        public string MutationDate { get { return _state.MutationDate; } }
    }
    public class NEVOProductNutrients
    {
        private INEVOProductNutrientsRepository _repo;

        public interface INEVOProductNutrientsRepository
        {
            NEVOProductNutrient.INEVOProductNutrientState CreateNEVOProductNutrientState();
        }
        public NEVOProductNutrients(INEVOProductNutrientsRepository repo)
        {
            _repo = repo;
        }

        public NEVOProductNutrient CreateNEVOProductNutrient(int productId, string nutrientUID, string quantity, string sourceUID, string mutationDate)
        {
            var state = _repo.CreateNEVOProductNutrientState();
            state.ProductId = productId;
            state.NutrientUID = nutrientUID;
            state.Quantity = quantity;
            state.SourceUID = sourceUID;
            state.MutationDate = mutationDate;
            return new NEVOProductNutrient(state);
        }
    }
}
