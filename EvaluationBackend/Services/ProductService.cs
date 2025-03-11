using AutoMapper;
using EvaluationBackend.Repository;
using IXIR.DATA.DTOs.Product;
using IXIR.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IXIR.Services
{

    public interface IProductService
    {

        Task<(IEnumerable<ProductDTO> products, int totalProducts, string? error)> GetProducts(int pageNumber, int pageSize);
            Task<(List<ProductDTO> data, int totalCount)> GetProducts(
            Expression<Func<Product, bool>> predicate,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include,
            int pageNumber = 0);

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


        public async Task<(IEnumerable<ProductDTO> products, int totalProducts, string? error)> GetProducts(int pageNumber, int pageSize)
        {
            var (products, totalCount) = await _repositoryWrapper.Product.GetProducts(pageNumber);

            if (products == null || !products.Any())
            {
                return (null, 0, "No products found");
            }

            var activeProducts = products.Where(product => !product.Deleted).ToList();

            var pagedProducts = activeProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(pagedProducts);
            var totalProducts = activeProducts.Count(); // Count after filtering deleted products

            return (productDtos, totalProducts, null);
        }


        public async Task<(List<ProductDTO> data, int totalCount)> GetProducts(
            Expression<Func<Product, bool>> predicate,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include,
            int pageNumber = 0)
        {
            var (products, totalCount) = await _repositoryWrapper.Product.GetProducts(predicate, include, pageNumber);
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return (productDTOs, totalCount);
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
