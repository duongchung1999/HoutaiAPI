# nullable enable
using Backend.Enties.PublicTestItem;
using Backend.Services;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 通用测试项目组
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class PublicTestItemGroupsController : IDynamicApiController {

        PublicTestItemGroupService service;
        PublicTestItemService publicTestItemService;

        public PublicTestItemGroupsController(IRepository<PublicTestItemGroup> personRepository) {
            service = new PublicTestItemGroupService(personRepository);
            publicTestItemService = new PublicTestItemService(Db.GetRepository<PublicTestItem>());
        }

        /// <summary>
        /// 添加测试项目组
        /// </summary>
        /// <param name="publicTestItemGroup"></param>
        /// <returns></returns>
        public async Task<PublicTestItemGroup> Add(PublicTestItemGroup publicTestItemGroup) {
            return await service.Add(publicTestItemGroup);
        }

        /// <summary>
        /// 获取通用测试项目组列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<PublicTestItemGroup>> GetList() {
            return await service.GetList();
        }

        /// <summary>
        /// 删除通用测试项目组
        /// </summary>
        /// <param name="groupId">通用测试项目组Id</param>
        /// <returns></returns>
        public async Task<bool> Delete(int groupId) {
            await service.Delete(groupId);
            await publicTestItemService.Clear(groupId);
            return true;
        }

    }
}