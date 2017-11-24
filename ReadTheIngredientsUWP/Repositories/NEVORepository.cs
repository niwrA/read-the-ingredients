using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEVONutrientListOnlineFacade;

namespace ReadTheIngredientsUWP.Repositories
{
    public class NEVOProductState : INEVOProductState
    {
        public Guid Guid { get; set; }
        public int CheckId { get; set; }

        public string Comment { get; set; }
        public bool Confidential { get; set; }

        public string EdiblePart { get; set; }

        public string ENDescription { get; set; }

        public int GroupId { get; set; }
        public int Id { get; set; }

        public bool InActive { get; set; }

        public string ManufacturerName { get; set; }

        public string NLDescription { get; set; }

        public int Quantity { get; set; }

        public string Unit { get; set; }
    }

    public class NEVOProductNutrientState : NEVOProductNutrient.INEVOProductNutrientState
    {
        public Guid Guid { get; set; }
        public string MutationDate { get; set; }

        public string NutrientUID { get; set; }
        public int ProductId { get; set; }

        public string Quantity { get; set; }

        public string SourceUID { get; set; }
    }

    public class NEVORepository : NEVONutrientListOnlineFacade.NEVONutrientDataImporter.INEVOImporterRepository
    {
        public NEVORepository()
        {
            _NEVOProductNutrientStateDictionary = new Dictionary<Guid, NEVOProductNutrientState>();
            _NEVOProductStateDictionary = new Dictionary<Guid, NEVOProductState>();
        }
        private Dictionary<Guid, NEVOProductState> _NEVOProductStateDictionary;
        private Dictionary<Guid, NEVOProductNutrientState> _NEVOProductNutrientStateDictionary;
        public NEVOProductNutrient.INEVOProductNutrientState CreateNEVOProductNutrientState()
        {
            var state = new NEVOProductNutrientState();
            state.Guid = Guid.NewGuid();
            _NEVOProductNutrientStateDictionary.Add(state.Guid, state);
            return state;
        }

        public INEVOProductState CreateNEVOProductState()
        {
            var state = new NEVOProductState();
            state.Guid = Guid.NewGuid();
            _NEVOProductStateDictionary.Add(state.Guid, state);
            return state;
        }
    }
}
