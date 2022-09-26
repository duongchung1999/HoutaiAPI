# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 站别
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/models/[controller]")]
    public class StationsController : IDynamicApiController {

        private readonly IRepository<Station> repository;

        private readonly StationService service;

        ModelsController modelsController;

        public StationsController(IRepository<Station> personRepository) {
            repository = personRepository;
            service = new StationService(repository);
            modelsController = new ModelsController(Db.GetRepository<Model>());
        }

        /// <summary>
        /// 添加站别
        /// </summary>
        /// <param name="modelId">所属机型Id</param>
        /// <param name="s">站别</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<Station> Add(int modelId, Station s) {
            s.ModelId = modelId;
            var result = await service.Add(s);
            return result.Entity;
        }

        /// <summary>
        /// 删除站别
        /// </summary>
        /// <param name="modelId">所属机型Id</param>
        /// <param name="s"></param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<Station?> Delete(int modelId, Station s) {
            s.ModelId = modelId;
            s = await repository.FindOrDefaultAsync(s.Id);
            if (s != null) await service.Delete(s);

            return null;
        }

        /// <summary>
        /// 获取某机型的站别列表
        /// </summary>
        /// <param name="modelUk">机型的Id或名称</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelUk}/[controller]")]
        public async Task<List<Station>> GetCollection(string? modelUk) {
            if (string.IsNullOrWhiteSpace(modelUk)) return null;

            if (int.TryParse(modelUk, out int modelId)) {
                return await service.GetCollection(modelId);
            }

            return await service.GetCollection(modelUk);
        }

        /// <summary>
        /// 获取某个机型的某个站别
        /// </summary>
        /// <param name="modelUk">机型名称或机型Id</param>
        /// <param name="stationName">站别名称</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelUk}/[controller]/{stationName}")]
        public async Task<Station>? Get(string modelUk, string? stationName) {
            var model = await modelsController.Get(modelUk);
            if (model is null) {
                throw Oops.Oh("[unknow modelUk] 没有找到此机型");
            }
            return await repository.SingleOrDefaultAsync(e => e.ModelId == model.Id && e.Name == stationName);
        }

        /// <summary>
        /// 获取某个机型的某个站别
        /// </summary>
        /// <param name="modelUk">机型名称或机型Id</param>
        /// <param name="stationId">站别Id</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelUk}/[controller]/{stationId}")]
        public async Task<Station>? Get(string modelUk, int stationId) {
            var model = await modelsController.Get(modelUk);
            if (model is null) {
                throw Oops.Oh("[unknow modelUk] 没有找到此机型");
            }
            return await repository.SingleOrDefaultAsync(e => e.Id == stationId);
        }

        /// <summary>
        /// 更新站别
        /// </summary>
        /// <param name="modelId">所属机型Id</param>
        /// <param name="s"></param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<Station> Update(int modelId, Station s) {
            s.ModelId = modelId;
            var result = await service.UpdateStation(s);
            return result.Entity;
        }

    }
}