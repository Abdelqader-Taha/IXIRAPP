using EvaluationBackend.Interface;
using EvaluationBackend.Services;
using EvaluationBackend.Interface;  // Corrected name of the namespace

namespace EvaluationBackend.Repository
{
    public interface IRepositoryWrapper
    {
   
        IUserRepository User { get; }
        IRoleRepository Role { get; }
        IStoreRespository Store { get; }
       
        Task Save();
    }
}