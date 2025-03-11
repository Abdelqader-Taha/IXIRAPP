using AutoMapper;
using EvaluationBackend.DATA;
using EvaluationBackend.Entities;
using EvaluationBackend.Interface;
using EvaluationBackend.Repository;
using IXIR.Entities;
using IXIR.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IXIR.Repository
{
    public class ProductRepository : GenericRepository<Product, Guid>, IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<Product> UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<(List<Product> data, int totalCount)> GetProducts(int pageNumber)
        {
            var products = await _context.Products
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();

            var count = await _context.Products.CountAsync();
            return (products, count);
        }

        public async Task<(List<Product> data, int totalCount)> GetProducts(
            Expression<Func<Product, bool>> predicate,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include,
            int pageNumber = 0)
        {
            var query = predicate == null
                ? _context.Set<Product>()
                : _context.Set<Product>().Where(predicate);

            query = include != null ? include(query) : query;
            query = query.OrderByDescending(product => product.CreationDate);

            var data = await (pageNumber == 0
                ? query.ToListAsync()
                : query.Skip(10 * (pageNumber - 1)).Take(10).ToListAsync());
            var totalCount = await query.CountAsync();

            return (data, totalCount);
        }
    }
}
