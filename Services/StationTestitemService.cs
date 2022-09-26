using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services {
    public class StationTestitemService {
        private readonly IRepository<StationTestItem> repository;
        readonly IRepository<Model> modelRepository;
        readonly IRepository<Station> stationRepository;
        readonly StationService stationService;

        public StationTestitemService(IRepository<StationTestItem> personRepository) {
            repository = personRepository;
            modelRepository = Db.GetRepository<Model>();
            stationRepository = Db.GetRepository<Station>();
            stationService = new StationService(stationRepository);

        }

        public async Task<List<StationTestItem>> GetAll(int stationId, string modelName = null, string stationName = null) {
            if (!string.IsNullOrEmpty(modelName) && !string.IsNullOrEmpty(stationName)) {
                var model = modelRepository.Where(e => e.Name.Equals(modelName)).SingleOrDefault();
                if (model != null) {
                    var station = stationRepository.Where(e => e.Name.Equals(stationName) && e.ModelId == model.Id).SingleOrDefault();
                    if (station != null) {
                        stationId = station.Id;
                    }
                }
            }
            var result = await repository
                .Where(st => st.StationId == stationId)
                .Include(st => st.TestItem)
                .OrderBy(e => e.SortIndex)
                .ToListAsync();

            return result;
        }

        public async Task<StationTestItem> Add(StationTestItem st) {
            st.SortIndex = repository.Where(e => e.StationId == st.StationId).Count() + 1;
            var result = await repository.InsertNowAsync(st);
            return await repository.Include(st => st.TestItem).Where(st => st.Id == result.Entity.Id).SingleOrDefaultAsync();
        }

        public async Task<EntityEntry<StationTestItem>> Delete(StationTestItem st) {

            if (st.Id == 0) {
                st = repository.Where(e => e.StationId == st.StationId && e.TestitemId == st.TestitemId).SingleOrDefault();
            }

            if (st is null) {
                return null;
            }
            var result = await st.DeleteNowAsync();

            var stList = await repository.Where(e => e.StationId == st.StationId).OrderBy(e => e.SortIndex).ToListAsync();

            for (int i = 0; i < stList.Count; i++) {
                var entity = stList[i];
                entity.SortIndex = i + 1;
                await entity.UpdateNowAsync();
            }

            return result;
        }

        public async Task<StationTestItem> Update(StationTestItem st) {
            var result = await st.UpdateNowAsync();
            return result.Entity;
        }

        public async Task<List<StationTestItem>> GetCollection(int stationId) {
            var result = await repository
                .Where(st => st.StationId == stationId)
                .Include(st => st.TestItem)
                .OrderBy(e => e.SortIndex)
                .ToListAsync();
            return result;
        }

        public async Task<int> Clear(int stationId) {
            var toDeleteEntity = await repository.Where(st => st.StationId == stationId).ToListAsync();
            foreach (var item in toDeleteEntity) {
                await item.DeleteAsync();
            }
            return await repository.SaveNowAsync();
        }

        public async Task<List<StationTestItem>> Reallocate(List<StationTestItem> stationTestitems) {
            await Clear(stationTestitems[0].StationId);

            for (int i = 0; i < stationTestitems.Count; i++) {
                var st = stationTestitems[i];
                st.Id = 0;
                st.SortIndex = i + 1;
                st.TestItem = null;
                await st.InsertAsync();
            }

            await repository.SaveNowAsync();
            var result = await GetAll(stationTestitems[0].StationId);
            return result;
        }
    }
}
