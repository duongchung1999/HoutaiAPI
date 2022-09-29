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
        public ModelsController(IRepository<Model> personRepository) {
            repository = personRepository;
            service = new ModelService(repository);
            userService = new UserService(Db.GetRepository<User>());
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
            if (int.TryParse(modelUK, out int modelId)) {
                return await service.Get(modelId);
            }

            return await service.Get(modelUK);
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

            return await service.GetCollection(user);
        }

        /// <summary>
        /// 更新机型
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task<Model> Update(Model m) {
            var result = await service.Update(m);


            return result;
        }
    }
}