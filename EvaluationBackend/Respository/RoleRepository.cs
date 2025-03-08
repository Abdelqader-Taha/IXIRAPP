using AutoMapper;
using EvaluationBackend.DATA;
using EvaluationBackend.Interface;
using Role = EvaluationBackend.Entities.Role;

namespace EvaluationBackend.Repository
{
    public class RoleRepository : GenericRepository<Entities.Role,int>, IRoleRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(DataContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}