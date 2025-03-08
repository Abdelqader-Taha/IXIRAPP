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

namespace EvaluationBackend.Services
{
    public interface IStoreService
    {
        Task<(StoreDTO? store, string? error)> CreateStore(CreateStoreForm store);
        Task<(StoreDTO? store, string? error)> GetStoreById(Guid id);
        Task<(IEnumerable<StoreDTO> stores,int totalStors, string? error)> GetAllStores(int pageNumber, int pageSize);
        Task<(StoreDTO? store, string? error)> UpdateStore(Guid id,UpDateStoreForm store);
        Task<(bool success, string? error)> DeleteStore(Guid id);
        Task<(StoreDTO? store, string? error)> UnDeleteStore(Guid id); 
        Task<(IEnumerable<string> productTypes, string? error)> GetDistinctProductTypes();
        Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByProductType(List<string> productTypes);


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
        public async Task<(IEnumerable<StoreDTO> stores, string? error)> GetStoresByProductType(List<string> productTypes)
        {
            try
            {
                // Validate if the productTypes list is not empty
                if (productTypes == null || !productTypes.Any())
                {
                    return (null, "Product types are required.");
                }

                // Call the repository method to get stores based on product types
                var (stores, error) = await _repositoryWrapper.Store.GetStoresByProductType(productTypes);

                // If there was an error or no stores found
                if (!string.IsNullOrEmpty(error))
                {
                    return (null, error);
                }

                // If no stores are found
                if (stores == null || !stores.Any())
                {
                    return (null, "No stores found for the selected product types.");
                }

                // Return the mapped stores as DTOs
                return (_mapper.Map<IEnumerable<StoreDTO>>(stores), null);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return (null, $"An error occurred: {ex.Message}");
            }
        }





        public async Task<(IEnumerable<string> productTypes, string? error)> GetDistinctProductTypes()
        {
            try
            {
                var productTypes = await _repositoryWrapper.Store.GetDistinctProductTypes();
                return (productTypes, null);  // Return the product types with no error
            }
            catch (Exception ex)
            {
                return (null, $"An error occurred: {ex.Message}");  // Return error message if exception occurs
            }
        }


        public async Task<(IEnumerable<StoreDTO> stores, int totalStors, string? error)> GetAllStores(int pageNumber, int pageSize)
        {
            var store = await _repositoryWrapper.Store.GetAll();

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
            var store= await _repositoryWrapper.Store.GetById(id);

            if (store == null ||store.Deleted) { return (null, $"Store With  id {id} Not found or has been deleted"); }
            var storeDto = _mapper.Map<StoreDTO>(store);
            return (storeDto,null);
        }

        public async Task<(StoreDTO? store, string? error)> CreateStore(CreateStoreForm storeForm)
        {
            try
            {
                // Check if the User exists
                var user = await _repositoryWrapper.User.GetById(storeForm.UserId);
                if (user == null)
                {
                    return (null, "User not found.");
                }

             

               
                var storeEntity = new Store
                {
                    UserId = storeForm.UserId,
                    StoreName = storeForm.StoreName,
                    ProductType = storeForm.ProductType,
                    StoreType = storeForm.StoreType,
                    City = storeForm.City,
                    PhoneNumber = storeForm.PhoneNumber,
                    StoreLogo = storeForm.StoreLogo, 
                    PlatformType = storeForm.PlatformType,
                    Followers = storeForm.Followers,
                    Link = storeForm.Link
                };

                var createdStore = await _repositoryWrapper.Store.CreateStore(storeEntity);

                user.StoreCount++;
                await _repositoryWrapper.User.Update(user);

                await _repositoryWrapper.Save();

                
                var storeDto = _mapper.Map<StoreDTO>(createdStore);

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
             var  store=await _repositoryWrapper.Store.GetById(id);

            if(store == null||store.Deleted) { return (null,"Store  Not Found or has been deleted"); }
            store.StoreName = storeForm.StoreName;
            store.StoreType = storeForm.StoreType;
            store.StoreLogo = storeForm.StoreLogo;
            store.City = storeForm.City;
            store.Followers = storeForm.Followers;
            store.Link = storeForm.Link;
            store.PhoneNumber = storeForm.PhoneNumber;
            store.PlatformType = storeForm.PlatformType;
            store.ProductType = storeForm.ProductType;

             var updatedstore= await _repositoryWrapper.Store.UpdateStore(store);
            await _repositoryWrapper.Save();
            var storeDto=_mapper.Map<StoreDTO>(updatedstore);
            return(storeDto,null);


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
