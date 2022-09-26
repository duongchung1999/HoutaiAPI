using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class StationService {
        private readonly IRepository<Station> repository;
        private readonly IRepository<Model> modelRepository;
        ModelService modelService;

        public StationService(IRepository<Station> personRepository) {
            repository = personRepository;
            modelRepository = Db.GetRepository<Model>();
            modelService = new ModelService(modelRepository);
        }

        public async Task<EntityEntry<Station>> Add(Station s) {
            return await s.InsertNowAsync();
        }

        public async Task<EntityEntry<Station>> Delete(Station s) {
            return await s.DeleteNowAsync();
        }

        public async Task<EntityEntry<Station>> UpdateStation(Station s) {
            return await s.UpdateNowAsync();
        }

        public async Task<Station> Get(int stationId) {
            return await repository.FindOrDefaultAsync(stationId);
        }

        public async Task<Station> Get(int modelId, string stationName) {
            return await repository.SingleOrDefaultAsync(e => e.ModelId == modelId && e.Name == stationName);
        }

        public async Task<List<Station>> GetCollection(int modelId) {
            /* 
             * FIXME
             * throw following error information on add OrderBy(...) before ToListAsync()
             * A second operation started on this context before a previous operation completed. 
             * This is usually caused by different threads using the same instance of DbContext, however instance members are not guaranteed to be thread safe. 
             * This could also be caused by a nested query being evaluated on the client, if this is the case rewrite the query avoiding nested invocations.
             */
            return await repository.Entities.Where(e => e.ModelId == modelId).AsNoTracking().ToListAsync();
        }

        public async Task<List<Station>> GetCollection(string modelName) {
            var model = await modelService.Get(modelName);
            if (model is null) {
                return null;
            }
            return  await GetCollection(model.Id);
        }

        public List<Station> GetAllStation(int modelId = 0, string modelName = null) {
            if (!string.IsNullOrEmpty(modelName)) {
                var model = modelRepository.Entities.Where(e => e.Name.Equals(modelName)).SingleOrDefault();
                if (model != null) {
                    modelId = model.Id;
                }
            }
            return repository.Entities.Where(e => e.ModelId.Equals(modelId)).OrderBy(e => e.Name).ToList();
        }
    }
}
