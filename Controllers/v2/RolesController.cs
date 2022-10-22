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
    /// 角色
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class RolesController: IDynamicApiController {

        private readonly RoleService service;

        public RolesController(IRepository<Role> personRepository) {
            service = new RoleService(personRepository);
        }

        /// <summary>
        /// 根据Id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Role> Get(int id) {
            return await service.GetById(id);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> GetList() {
            return await service.GetList();
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role> Update(Role role) {
            return await service.Update(role);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role> Add(Role role) {
            return await service.Add(role);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id) {
            await service.Delete(id);
        }
    }
}