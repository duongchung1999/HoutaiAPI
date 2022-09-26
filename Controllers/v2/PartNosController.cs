# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.ClayObject;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 料号
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class PartNosController : IDynamicApiController {

        private readonly IRepository<PartNo> repository;
        private readonly PartNoService service;
        PartNoConfigService partNoConfigService;

        public PartNosController(IRepository<PartNo> personRepository) {
            repository = personRepository;
            service = new PartNoService(repository);
            partNoConfigService = new PartNoConfigService(Db.GetRepository<PartNoConfig>());
        }

        /// <summary>
        /// 获取料号配置的的料号列表
        /// </summary>
        /// <param name="partNoConfigID">料号配置ID</param>
        /// <returns></returns>
        public async Task<List<PartNo>> GetList(int partNoConfigID) {
            return await service.GetList(partNoConfigID);
        }

        /// <summary>
        /// 更新料号
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public async Task<PartNo> Update(PartNo partNo) {
            return await service.Update(partNo);
        }

        /// <summary>
        /// 添加料号
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public async Task<PartNo> Add(PartNo partNo) {
            return await service.Add(partNo);
        }

        /// <summary>
        /// 删除料号
        /// </summary>
        /// <param name="partNoId"></param>
        /// <returns></returns>
        public async Task Delete(int partNoId) {
            await service.Delete(partNoId);
        }

        /// <summary>
        /// 获取料号配置列表（包含ID和标题）
        /// </summary>
        /// <returns></returns>
        public async Task<List<PartNoConfig>> GetPartNoConfigList(int modelId) {
            return await partNoConfigService.GetList(modelId);
        }

        /// <summary>
        /// 根据ID获取料号配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> GetPartNoConfig(int id) {
            return await partNoConfigService.Get(id);
        }

        /// <summary>
        /// 更新料号配置
        /// </summary>
        /// <param name="partNoConfig"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> UpdatePartNoConfig(PartNoConfig partNoConfig) {
            return await partNoConfigService.Update(partNoConfig);
        }

        /// <summary>
        /// 添加料号配置
        /// </summary>
        /// <param name="partNoConfig"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> AddPartNoConfig(PartNoConfig partNoConfig) {
            return await partNoConfigService.Add(partNoConfig);
        }

        /// <summary>
        /// 删除料号配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeletePartNoConfig(int id) {
            await partNoConfigService.Delete(id);
        }
    }
}