using AutoMapper;
using EvaluationBackend.DATA;
using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.Entities;
using EvaluationBackend.Interface;
using EvaluationBackend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EvaluationBackend.Respository
{
    public class StoreRepository : GenericRepository<Store, Guid>, IStoreRespository
    {
        private DataContext _context;
        private IMapper _mapper;

        public StoreRepository(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(List<Store> data, int totalCount)> GetStores(int pageNumber)
        {
            var stores = await _context.Stores.Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
            var count = await _context.Stores.CountAsync();
            return (stores, count);
        }

        public async Task<(List<Store> data, int totalCount)> GetStores(Expression<Func<Store, bool>> predicate,
           Func<IQueryable<Store>, IIncludableQueryable<Store, object>> include, int pageNumber = 0)
        {
            var query = predicate == null
                ? _context.Set<Store>()
                : _context.Set<Store>().Where(predicate);

            query = include != null ? include(query) : query;
            query = query.OrderByDescending(store => store.CreationDate);  

            return (await (pageNumber == 0
                ? query.ToListAsync()
                : query.Skip(10 * (pageNumber - 1)).Take(10).ToListAsync()),
                await query.CountAsync());
        }

        public async Task<Store> CreateStore(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            return store;


        }
         
        public async Task<Store> UpdateStore(Store store)
        {
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
            return store;

        }
        public async Task<Store> DeleteStore(Store store)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return store;
        }

        
       
        public async Task<int> CountAsync()
        {
            return await _context.Stores.CountAsync();
        }


    }
}
