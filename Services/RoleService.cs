using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class RoleService {
        private readonly IRepository<Role> repository;

        public RoleService(IRepository<Role> personRepository) {
            repository = personRepository;
        }

        public async Task<Role> GetById(int id) {
            return await repository.FindOrDefaultAsync(id);
        }

        public async Task<List<Role>> GetList() {
            return await repository.AsQueryable(false).ToListAsync();
        }

        public async Task<Role> Update(Role role) {
            var result = await role.UpdateNowAsync();
            return result.Entity;
        }

        public async Task<Role> Add(Role role) {
            var result = await role.InsertNowAsync();
            return result.Entity;
        }

        public async Task Delete(int id) {
            await repository.DeleteNowAsync(id);
        }
    }
}
