using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShared
{
    public class Product
    {
        public interface IProductState
        {
            string Barcode { get; set; }
            string BarcodeType { get; set; }
            string Guid { get; set; }
            string Name { get; set; }
        }

        private IProductState _state;
        public Product()
        {
            _state = new IProductState();
        }

        public Product(IProductState state)
        {
            _state = state;
        }
    }

    public class Products
    {
        public interface IProductRepository
        {
            Product.IProductState GetOrCreateProductState(string barcode, string barcodeType);
            void PersistChanges();
        }

        private IProductRepository _repo;
        public Products(IProductRepository repo)
        {
            _repo = repo;
        }

        public Product GetOrCreateProduct(string barcode, string barcodeType)
        {
            var state = _repo.GetOrCreateProduct(barcode, barcodeType);
            return new Product(state);
        }
    }
}
