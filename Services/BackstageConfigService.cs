using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class BackstageConfigService {
        private readonly IRepository<BackstageConfig> repository;

        public BackstageConfigService(IRepository<BackstageConfig> personRepository) {
            repository = personRepository;
        }

        public async Task<BackstageConfig> Add(BackstageConfig entity) {
            var result = await entity.InsertNowAsync();
            return result.Entity;
        }

        public async Task<BackstageConfig> Delete(BackstageConfig entity) {
            var result = await entity.DeleteNowAsync();
            return result.Entity;
        }

        public async Task<BackstageConfig> Update(BackstageConfig entity) {
            var result = await entity.UpdateNowAsync();
            return result.Entity;
        }

        public async Task<BackstageConfig> Get(string key) {
            var result = await repository.FirstOrDefaultAsync(e => e.Key == key);
            return result;
        }
    }
}
