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
            var model = await modelService.Get(s.ModelId);
            s.Config = (!string.IsNullOrEmpty(model.Config) ? model.Config : null);
            var result = await s.InsertNowAsync();
            await ActionRecordService.Add(ActionRecord.ActionOptions.ADD_STATION, $"[{model.Name}] {ActionRecordService.CreateDiffTextStyle("", s.Name)}" );

            return result;
        }

        public async Task<EntityEntry<Station>> Delete(Station s) {
            var result = await s.DeleteNowAsync();

            var model = await modelService.Get(s.ModelId);
            await ActionRecordService.Add(ActionRecord.ActionOptions.DELETE_STATION, $"[{model.Name}] {ActionRecordService.CreateDiffTextStyle(s.Name, "")}");

            return result;
        }

        public async Task<EntityEntry<Station>> UpdateStation(Station s) {
            var entry = repository.Attach(s);

            var oldStation = await entry.GetDatabaseValuesAsync();
            var oldStationName = oldStation["Name"].ToString();
            var oldStationConfig = oldStation["Config"]?.ToString();
            var result = await s.UpdateNowAsync();

            var model = await modelService.Get(s.ModelId);

            if (oldStationName != s.Name) {
                await ActionRecordService.Add("更新站别名称", $"[{model.Name}] {ActionRecordService.CreateDiffTextStyle(oldStationName, s.Name)}");
            }

            if (oldStationConfig != s.Config) {
                await ActionRecordService.Add("更新站别配置", $"[{model.Name}] {ActionRecordService.CreateDiffTextStyle(oldStationConfig, s.Config)}");
            }

            return result;
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
            return await GetCollection(model.Id);
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
