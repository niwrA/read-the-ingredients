using System;
using System.Collections.Generic;
using System.Text;

namespace NEVONutrientListOnlineFacade
{
    public class NEVOProduct
    {
        private INEVOProductState _state;
        public NEVOProduct(INEVOProductState state)
        {
            _state = state;
        }
        public int Id { get { return _state.Id; } set { _state.Id = value; } }
        public int GroupId { get { return _state.GroupId; } set { _state.GroupId = value; } }
        public int CheckId { get { return _state.Id; } set { _state.CheckId = value; } }
        public string NLDescription { get { return _state.NLDescription; } set { _state.NLDescription = value; } }
        public string ENDescription { get { return _state.ENDescription; } set { _state.ENDescription = value; } }
        public string ManufacturerName { get { return _state.ManufacturerName; } set { _state.ManufacturerName = value; } }
        public bool InActive { get { return _state.InActive; } set { _state.InActive = value; } }
        public int Quantity { get { return _state.Quantity; } set { _state.Quantity = value; } }
        public string Unit { get { return _state.Unit; } set { _state.Unit = value; } }
        public string EdiblePart { get { return _state.EdiblePart; } set { _state.EdiblePart = value; } }
        public bool Confidential { get { return _state.Confidential; } set { _state.Confidential = value; } }
        public string Comment { get { return _state.Comment; } set { _state.Comment = value; } }
    }

    public interface INEVOProductState
    {
        int Id { get; set; }
        int GroupId { get; set; }
        int CheckId { get; set; }
        string NLDescription { get; set; }
        string ENDescription { get; set; }
        string ManufacturerName { get; set; }
        bool InActive { get; set; }
        int Quantity { get; set; }
        string Unit { get; set; }
        string EdiblePart { get; set; }
        bool Confidential { get; set; }
        string Comment { get; set; }
    }

    public class NEVOProducts
    {
        private INEVOProductsRepository _repo;
        public NEVOProducts(INEVOProductsRepository repo)
        {
            _repo = repo;
        }
        public interface INEVOProductsRepository
        {
            INEVOProductState CreateNEVOProductState();
        }

        public NEVOProduct CreateNEVOProduct()
        {
            var state = _repo.CreateNEVOProductState();
            return new NEVOProduct(state);
        }
    }
}
