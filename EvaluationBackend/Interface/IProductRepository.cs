using EvaluationBackend.Entities;
using EvaluationBackend.Interface;
using IXIR.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IXIR.Interface
{
    public interface IProductRepository : IGenericRepository<Product, Guid>
    {
        Task<(List<Product> data, int totalCount)> GetProducts(int pageNumber);

        Task<(List<Product> data, int totalCount)> GetProducts(
            Expression<Func<Product, bool>> predicate,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include,
            int pageNumber = 0);

        Task<Product> CreateProduct(Product product);  

        Task<Product> UpdateProduct(Product product);

        Task<Product> DeleteProduct(Product product);
    }
}
