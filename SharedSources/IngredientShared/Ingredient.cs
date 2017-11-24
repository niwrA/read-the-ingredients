using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientShared
{
    public interface IIngredient
    {
        Guid Guid { get; }
    }
    public class Ingredient : IIngredient
    {
        public interface IIngredientState
        {
            Guid Guid { get; set; }
        }

        private IIngredientState _state;

        public Guid Guid { get { return _state.Guid; } }

        public Ingredient(IIngredientState state)
        {
            _state = state;
        }
    }

    public interface IIngredients
    {
        Ingredient CreateIngredient();
    }
    public class Ingredients : IIngredients
    {
        public interface IIngredientRepository
        {
            Ingredient.IIngredientState CreateIngredientState();
            void PersistChanges();
        }

        private IIngredientRepository _repo;
        public Ingredients(IIngredientRepository repo)
        {
            _repo = repo;
        }

        public Ingredient CreateIngredient()
        {
            var state = _repo.CreateIngredientState();
            state.Guid = Guid.NewGuid();
            return new Ingredient(state);
        }
    }
}
