# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.ClayObject;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 机型
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class ModelsController : IDynamicApiController {

        private readonly IRepository<Model> repository;
        private readonly ModelService service;
        private readonly UserService userService;  
        DynamicCodeService dynamicCodeService;
        
        public ModelsController(IRepository<Model> personRepository) {
            repository = personRepository;
            service = new ModelService(repository);
            userService = new UserService(Db.GetRepository<User>());
            dynamicCodeService = new DynamicCodeService(Db.GetRepository<DynamicCode>());
        }

        /// <summary>
        /// 添加机型
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<Model> Add(Model m) {
            var nowUser = JwtHandler.GetNowUser();
            m.CreatorId = nowUser.Id;
            var result = await service.Add(m);
            return result.Entity;
        }

        /// <summary>
        /// 删除机型
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task<Model?> Delete(Model m) {
            await service.Delete(m);
            return null;
        }

        /// <summary>
        /// 获取机型
        /// </summary>
        /// <param name="modelUK">机型的名称或Id</param>
        /// <returns>机型/机型列表</returns>
        public async Task<Model> Get(string? modelUK) {
            Model result = null;
            if (int.TryParse(modelUK, out int modelId)) {
                result = await service.Get(modelId);
            } else {
                result = await service.Get(modelUK);
            }

            result.DynamicCode = await dynamicCodeService.GetByModelId(result.Id);

            return result;
        }

        /// <summary>
        /// 获取（被分配的）机型列表
        /// </summary>
        /// <param name="filte">过滤条件</param>
        /// <returns></returns>
        [Authorize]
        public async Task<List<Model>> GetList(int userId) {
            if (userId == 0) {
                var nowUser = JwtHandler.GetNowUser();
                userId = nowUser.Id;
            }

            var user = await userService.Get(userId);

            var result = await service.GetCollection(user);
            foreach (var item in result) {
                item.DynamicCode = await dynamicCodeService.GetByModelId(item.Id);
            }
            return result;
        }

        /// <summary>
        /// 更新机型
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task<Model> Update(Model m) {
            var result = await service.Update(m);

            result.DynamicCode = await dynamicCodeService.GetByModelId(result.Id);
            return result;
        }
    }
}