using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Backend.Services {
    public class ModelService {

        private readonly IRepository<Model> repository;
        UserModelService UserModelService;

        public ModelService(IRepository<Model> personRepository) {
            repository = personRepository;
            UserModelService = new UserModelService(Db.GetRepository<UserModel>());
        }

        public async Task<EntityEntry<Model>> Add(Model m) {
            var result = await m.InsertAsync();
            await repository.SaveNowAsync();
            return result;
        }

        public async Task<EntityEntry<Model>> Delete(Model m) {
            return await m.DeleteNowAsync();
        }

        public async Task<Model> Get(string modelName) {
            return await repository.Entities.AsNoTracking().SingleOrDefaultAsync(e => e.Name.Equals(modelName));
        }

        public async Task<Model> Get(int id = 0) {
            return await repository.FindOrDefaultAsync(id);
        }

        public async Task<List<Model>> GetCollection(User user) {
            var result = repository.AsQueryable(false);
            var role = user.Role;

            if ((role & UserRoleOptions.ADMIN) == 0) {
                var userModels = await UserModelService.GetList(user.Id);
                var modelIds = userModels.Select(e => e.ModelId).ToList();
                result = result.Where(e => modelIds.Contains(e.Id) || e.CreatorId == user.Id);
            }

            return  await result.ToListAsync();
        }

        public async Task<EntityEntry<Model>> Update(Model m) {
            return await m.UpdateNowAsync();
        }
    }
}
