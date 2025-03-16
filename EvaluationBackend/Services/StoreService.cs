using AutoMapper;
using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.DATA.DTOs.User;
using EvaluationBackend.Entities;
using EvaluationBackend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using Polly;
using Microsoft.IdentityModel.Tokens;
using IXIR.DATA.DTOs.Product;
using OneSignalApi.Model;

namespace EvaluationBackend.Services
{
    public interface IStoreService
    {
        Task<(StoreDTO? store, string? error)> CreateStore(CreateStoreForm store,Guid userId);
        Task<(StoreDTO? store, string? error)> GetStoreById(Guid id);
        Task<(IEnumerable<StoreDTO> stores,int totalStors, string? error)> GetAllStores(int pageNumber, int pageSize);
        Task<(StoreDTO? store, string? error)> UpdateStore(Guid id,UpDateStoreForm store);
        Task<(bool success, string? error)> DeleteStore(Guid id);
        Task<(StoreDTO? store, string? error)> UnDeleteStore(Guid id);
        Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByUserId(Guid userId, int pageNumber = 1, int pageSize = 10);
       // Task<(IEnumerable<string> productTypes, string? error)> GetDistinctProductTypes();
       // Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByProductType(List<string> productTypes);


    }


    public class StoreService : IStoreService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public StoreService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }



        public async Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByUserId(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Get all stores associated with the userId
                var stores = await _repositoryWrapper.Store.GetAll(
                    include: query => query.Include(s => s.Products)
                );

                if (stores.data == null || !stores.data.Any())
                {
                    return (new List<StoreDTO>(), "No stores found");
                }

                // Filter stores by the userId
                var userStores = stores.data
                    .Where(s => !s.Deleted && s.UserId == userId)
                    .ToList();

                if (!userStores.Any())
                {
                    return (new List<StoreDTO>(), "No stores found for the user");
                }

                // Apply pagination
                var totalStores = userStores.Count();
                var pagedStores = userStores
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var storeDtos = _mapper.Map<IEnumerable<StoreDTO>>(pagedStores);

                return (storeDtos, null);
            }
            catch (Exception ex)
            {
                return (new List<StoreDTO>(), ex.Message); // Return an empty list and error message if an exception occurs
            }
        }




        public async Task<(IEnumerable<StoreDTO> stores, int totalStors, string? error)> GetAllStores(int pageNumber, int pageSize)
        {
            var store = await _repositoryWrapper.Store.GetAll(

                include: query =>query.Include(s=> s.Products)
                );

            if (store.data == null || !store.data.Any())
            {
                return (null, 0, "Store Not Found");
            }


            var activestore = store.data.Where(store=> !store.Deleted).ToList();

            var pagedStores = activestore
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize);

            var storeDtos = _mapper.Map<IEnumerable<StoreDTO>>(pagedStores);
            var totalStors = activestore.Count(); 

            return (storeDtos, totalStors, null);
        }




        public async Task<(StoreDTO? store, string? error)> GetStoreById(Guid id)
        {
            var store = await _repositoryWrapper.Store.Get(
                predicate: s => s.Id == id,
                include: query => query.Include(s => s.Products));

            if (store == null ||store.Deleted) { return (null, $"Store With  id {id} Not found or has been deleted"); }
            var storeDto = _mapper.Map<StoreDTO>(store);
            return (storeDto,null);
        }



        public async Task<(StoreDTO? store, string? error)> CreateStore(CreateStoreForm storeForm, Guid userId)
        {
            try
            {
                var user = await _repositoryWrapper.User.GetById(userId);
                if (user == null)
                {
                    return (null, "User not found.");
                }

                var storeEntity = new Store
                {
                    UserId = userId,
                    StoreName = storeForm.StoreName,
                    StoreType = storeForm.StoreType,
                    City = storeForm.City,
                    PhoneNumber = storeForm.PhoneNumber,
                    StoreLogo = storeForm.StoreLogo,
                    PlatformType = storeForm.PlatformType,
                    Followers = storeForm.Followers,
                    Link = storeForm.Link,
                    CreationDate=storeForm.CreationDate
                };

                // Fetch products based on the ProductIds from CreateStoreForm
                var products = await _repositoryWrapper.Product.GetAll(p => storeForm.ProductIds.Contains(p.Id));

                storeEntity.Products = products.data;

                var createdStore = await _repositoryWrapper.Store.Add(storeEntity);

                user.StoreCount++;
                await _repositoryWrapper.User.Update(user);
                await _repositoryWrapper.Save();

                var storeDto = _mapper.Map<StoreDTO>(createdStore);

                storeDto.Products = products.data.Select(p => _mapper.Map<ProductDTO>(p)).ToList();

                return (storeDto, null);
            }

            catch (DbUpdateException ex)
            {
                return (null, $"An error occurred while saving the entity changes: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return (null, $"An unexpected error occurred: {ex.Message}");
            }
        }




        public async Task<(StoreDTO? store, string? error)> UpdateStore(Guid id, UpDateStoreForm storeForm)
        {
            var store = await _repositoryWrapper.Store.GetById(id);

            if (store == null || store.Deleted)
            {
                return (null, "Store not found or has been deleted");
            }

            store.StoreName = string.IsNullOrEmpty(storeForm.StoreName) ? store.StoreName : storeForm.StoreName;
            store.StoreType = string.IsNullOrEmpty(storeForm.StoreType) ? store.StoreType : storeForm.StoreType;
            store.StoreLogo = string.IsNullOrEmpty(storeForm.StoreLogo) ? store.StoreLogo : storeForm.StoreLogo;
            store.City = string.IsNullOrEmpty(storeForm.City) ? store.City : storeForm.City;
            store.Followers = storeForm.Followers.HasValue ? storeForm.Followers.Value : store.Followers;
            store.Link = string.IsNullOrEmpty(storeForm.Link) ? store.Link : storeForm.Link;
            store.PhoneNumber = string.IsNullOrEmpty(storeForm.PhoneNumber) ? store.PhoneNumber : storeForm.PhoneNumber;
            store.PlatformType = string.IsNullOrEmpty(storeForm.PlatformType) ? store.PlatformType : storeForm.PlatformType;

            
            try
            {
                // Save the updated store
                var updatedStore = await _repositoryWrapper.Store.UpdateStore(store);
                await _repositoryWrapper.Save();

                // Map the updated store to a DTO
                var storeDto = _mapper.Map<StoreDTO>(updatedStore);
                return (storeDto, null);
            }
            catch (Exception ex)
            {
                // Log or return a more specific error message
                return (null, $"An error occurred while saving the store: {ex.Message}");
            }
        }


        public async Task<(bool success, string? error)> DeleteStore(Guid id)
        {
            var store = await _repositoryWrapper.Store.GetById(id);

            if (store == null||store.Deleted)
            {
                return (false, $"Store with id {id} not found or has been deleted");
            }
            store.Deleted = true;

            await _repositoryWrapper.Store.Update(store);
            await _repositoryWrapper.Save();

            return (true, null);

        }
        public async Task<(StoreDTO? store, string? error)> UnDeleteStore(Guid id)
        {
            var store = await _repositoryWrapper.Store.GetById(id);

            if (store == null || !store.Deleted)
            {
                return (null, $"Store with id {id} not found or is not deleted.");
            }
            store.Deleted = false;
            var updatedstore = await _repositoryWrapper.Store.Update(store);
            await _repositoryWrapper.Save();
            var storeDto= _mapper.Map<StoreDTO>(updatedstore);
            return(storeDto,null);

        }










    }
}
