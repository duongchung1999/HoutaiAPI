# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 站别测试项目
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/models/station/test-item")]
    public class StationTestItemsController : IDynamicApiController {

        IRepository<StationTestItem> repository;
        StationTestitemService service;
        StationService stationService;
        ModelService modelService;

        public StationTestItemsController(IRepository<StationTestItem> personRepository) {
            repository = personRepository;
            service = new StationTestitemService(repository);
            stationService = new StationService(Db.GetRepository<Station>());
            modelService = new ModelService(Db.GetRepository<Model>());
        }

        /// <summary>
        /// 给站别分配测试项目
        /// </summary>
        /// <param name="stationId">要分配的站别的Id</param>
        /// <param name="testitemId">分配给站别的测试项目Id</param>
        /// <returns></returns>
        [Route("/api/v2/models/stations/{stationId}/test-items/")]
        public async Task<StationTestItem> Add(int stationId, StationTestItem st) {
            st.StationId = stationId;
            st.TestItem = null;
            var result = await service.Add(st);
            return result;
        }

        /// <summary>
        /// 删除测试项目
        /// </summary>
        /// <param name="stationId">所属站别Id</param>
        /// <param name="st">站别测试项目</param>
        /// <returns></returns>
        [Route("/api/v2/models/stations/{stationId}/test-items/")]
        public async Task<TestItem?> Delete(int stationId, StationTestItem st) {
            st.StationId = stationId;
            var entity = await repository.FindOrDefaultAsync(st.Id);
            if (entity != null) await service.Delete(entity);

            return null;
        }

        /// <summary>
        /// 更新站别测试项目
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        [Route("/api/v2/models/stations/{stationId}/test-items/")]
        public async Task<StationTestItem> Update(int stationId, StationTestItem st) {
            st.StationId = stationId;
            var result = await service.Update(st);
            return result;
        }

        /// <summary>
        /// 获取测试项目列表
        /// </summary>
        /// <param name="modelUk">所属机型Id或名称</param>
        /// <param name="stationUk">所属站别的Id或名称</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelUk}/stations/{stationUk}/test-items")]
        public async Task<List<StationTestItem>?> GetCollection(string modelUk, string stationUk) {
            if (string.IsNullOrWhiteSpace(stationUk)) return null;

            if (int.TryParse(stationUk, out int stationId)) return await service.GetCollection(stationId);

            if (string.IsNullOrWhiteSpace(modelUk)) throw Oops.Oh("请传入机型Id或名称");

            Model model;

            if (int.TryParse(modelUk, out int modelId)) {
                model = await modelService.Get(modelId);
            } else {
                model = await modelService.Get(modelUk);
            }
            if (model == null) throw Oops.Oh("没有找到此机型");

            var station = await stationService.Get(model.Id, stationUk);
            if (station == null) throw Oops.Oh("没有找到此站别");

            return await service.GetCollection(station.Id);
        }
         
        /// <summary>
        /// 重新分配测试项目
        /// </summary>
        /// <param name="stationId">站别Id</param>
        /// <param name="newItems">新的测试项目列表</param>
        /// <returns></returns>
        [Route("/api/v2/models/stations/{stationId}/test-items/distribute")]
        [Authorize]
        public async Task<List<StationTestItem>?> Distribute(int stationId, List<StationTestItem>? newItems) {
            if (newItems == null) return null;

            var oldItems = await service.GetCollection(stationId);

            var oldText = await service.CreateActionRecordText(oldItems);

            for (int i = 0; i < newItems.Count; i++) {
                newItems[i].Id = 0;
                newItems[i].SortIndex = i + 1;
            }
            await repository.Context.DeleteRangeAsync<StationTestItem>(e => e.StationId == stationId);
            await repository.Context.BulkInsertAsync(newItems);

            var newText = await service.CreateActionRecordText(newItems);

            var stationService = new StationService(Db.GetRepository<Station>());
            var station = await stationService.Get(stationId);
            var modelName = await ModelService.GetModelNameById(station.ModelId);

            ActionRecordService.Add("分配测试项目", $"[{modelName} {station.Name}]\n {ActionRecordService.CreateDiffTextStyle(oldText, newText)}");

            await repository.SaveNowAsync();
            
            var result = await service.GetCollection(stationId);

            return result;
        }

        /// <summary>
        /// 清空所有测试项目
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        [Route("/api/v2/models/stations/{stationId}/test-items/all")]
        [Authorize]
        public async Task<List<StationTestItem>?> ClearAll(int stationId) {
            var oldItems = await service.GetCollection(stationId);
            foreach (var item in oldItems) {
                await item.DeleteAsync();
            }
            await repository.SaveNowAsync();
            return null;
        }
    }
}