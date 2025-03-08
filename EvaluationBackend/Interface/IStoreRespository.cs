using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EvaluationBackend.Interface
{
    public interface IStoreRespository : IGenericRepository<Store, Guid>
    {
        Task<(List<Store> data, int totalCount)> GetStores(int pageNumber);

        Task<(List<Store> data, int totalCount)> GetStores(Expression<Func<Store, bool>> predicate,
           Func<IQueryable<Store>, IIncludableQueryable<Store, object>> include, int pageNumber = 0);

        Task<Store> CreateStore(Store store);
        Task<Store> UpdateStore(Store store);
        Task<Store> DeleteStore(Store store);
        Task<List<string>> GetDistinctProductTypes();
        Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByProductType(List<string> productTypes);



    }
}