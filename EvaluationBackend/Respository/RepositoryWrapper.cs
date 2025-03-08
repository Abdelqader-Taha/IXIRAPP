
using AutoMapper;
using EvaluationBackend.DATA;
using EvaluationBackend.Interface;
using EvaluationBackend.Respository;
using EvaluationBackend.Interface;  // Corrected name of the namespace
using EvaluationBackend.Repository;


namespace EvaluationBackend.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        private IUserRepository _user;  
        private IRoleRepository _role;
        private IStoreRespository _store;

        
        public IRoleRepository Role {  get {
            if(_role == null)
            {
                _role = new RoleRepository(_context,_mapper);
            }
            return _role;
        } }
        
       

       
        
        public IUserRepository User {  get {
            if(_user == null)
            {
                _user = new UserRepository(_context,_mapper);
            }
            return _user;
        } }

        public IStoreRespository Store

        {
            get
            {
                if (_store == null)
                {
                    _store = new  StoreRepository(_context, _mapper);
                }
                return _store;
            }
        }



        public RepositoryWrapper(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }
        
    }
}