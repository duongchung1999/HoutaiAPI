# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 后台程序配置
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class BackstageConfigurationsController : IDynamicApiController {

        private readonly IRepository<BackstageConfig> repository;
        private readonly BackstageConfigService service;

        public BackstageConfigurationsController(IRepository<BackstageConfig> personRepository) {
            repository = personRepository;
            service = new BackstageConfigService(repository);
        }

        /// <summary>
        /// 添加后台程序配置
        /// </summary>
        /// <param name="e" options="1,2,3.abc,true">
        /// 123
        /// </param>
        /// <remarks>备注</remarks>
        /// <returns>返回值</returns>
        public async Task<BackstageConfig> Add(BackstageConfig e) {
            var result = await service.Add(e);
            return result;
        }

        /// <summary>
        /// 删除后台程序配置
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<BackstageConfig?> Delete(BackstageConfig e) {
            var entity = await repository.FindOrDefaultAsync(e.Id);
            if (entity != null) await service.Delete(e);
            return null;
        }

        /// <summary>
        /// 获取后台程序配置
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>机型/机型列表</returns>
        public async Task<BackstageConfig> Get(string key) {
            return await service.Get(key);
        }

        /// <summary>
        /// 更新后台程序配置
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<BackstageConfig> Update(BackstageConfig e) {
            var result = await service.Update(e);
            return result;
        }

        /// <summary>
        /// 获取配置键列表（key、label）
        /// </summary>
        /// <returns></returns>
        public async Task<List<BackstageConfig>> GetKeys() {
            var result = await repository.AsQueryable().Select(e => new BackstageConfig() { Id = e.Id, Key = e.Key, Label = e.Label }).ToListAsync();
            return result;
        }
    }
}