using AutoMapper;
using EvaluationBackend.Repository;
using IXIR.DATA.DTOs.Product;
using IXIR.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IXIR.Services
{

    public interface IProductService
    {

        Task<(IEnumerable<ProductDTO> products, int totalProducts, string? error)> GetAllProducts(int pageNumber, int pageSize);          
        
        Task<ProductDTO> CreateProduct(string name);
        Task<ProductDTO> UpdateProduct(Guid id, string name);
        Task<bool> DeleteProduct(Guid id);
        Task<ProductDTO?> GetProductById(Guid id);

    }



    public class ProductService:IProductService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        public ProductService(IRepositoryWrapper repositoryWrapper,IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            
        }
        public async Task<ProductDTO?> GetProductById(Guid id)
        {
            var product = await _repositoryWrapper.Product.GetById(id);
            if (product == null)
            {
                return null; // Product not found
            }

            return _mapper.Map<ProductDTO>(product);
        }


        public async Task<(IEnumerable<ProductDTO> products, int totalProducts, string? error)> GetAllProducts(int pageNumber, int pageSize)
        {
            try
            {
                // Fetch products and total count
                var (products, totalCount) = await _repositoryWrapper.Product.GetProducts(pageNumber);

                if (products == null || !products.Any())
                {
                    return (new List<ProductDTO>(), 0, ""); // Return empty list if no products found
                }

                var activeProducts = products.Where(product => !product.Deleted).ToList();
                var totalProducts = activeProducts.Count();

                var pagedProducts = activeProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(pagedProducts);

                return (productDtos, totalProducts, null); 
            }
            catch (Exception ex)
            {
                return (new List<ProductDTO>(), 0, ex.Message); 
            }
        }




        public async Task<ProductDTO> CreateProduct(string name)
        {
            var product = new Product { Name = name };

            var createdProduct = await _repositoryWrapper.Product.CreateProduct(product);

            return _mapper.Map<ProductDTO>(createdProduct);
        }

        public async Task<ProductDTO> UpdateProduct(Guid id, string name)
        {
            var product = await _repositoryWrapper.Product.GetById(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Name = name;
            var updatedProduct = await _repositoryWrapper.Product.UpdateProduct(product);

            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var product = await _repositoryWrapper.Product.GetById(id);
            if (product == null)
            {
                return false; // Product not found
            }

            await _repositoryWrapper.Product.DeleteProduct(product);
            return true;
        }

    }
}
