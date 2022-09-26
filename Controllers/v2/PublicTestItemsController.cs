# nullable enable
using Backend.Enties;
using Backend.Enties.PublicTestItem;
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
    /// 通用测试项目
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class PublicTestItemsController : IDynamicApiController {

        PublicTestItemService service;
        PublicTestItemParamService paramService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personRepository"></param>
        public PublicTestItemsController(IRepository<PublicTestItem> personRepository) {
            service = new PublicTestItemService(personRepository);
            paramService = new PublicTestItemParamService(Db.GetRepository<PublicTestItemParam>());
        }

        /// <summary>
        /// 获取某通用测试项目组的测试项目
        /// </summary>
        /// <param name="groupId">通用测试项目组的Id</param>
        /// <returns></returns>
        public async Task<List<PublicTestItem>> GetList(int groupId) {
            return await service.GetList(groupId);
        }

        /// <summary>
        /// 重新分配某个测试项目组的测试项目
        /// </summary>
        /// <param name="groupId">通用测试项目组的Id</param>
        /// <param name="publicTestItems">要添加的通用测试项目</param>
        /// <returns>添加后的通用测试项目列表</returns>
        public async Task<List<PublicTestItem>> Reallocate(int groupId, List<PublicTestItem> publicTestItems) {
            var pTestItems = await service.Reallocate(groupId, publicTestItems);
            foreach (var item in pTestItems) {
                //更新测试项目的参数
                await paramService.Reallocate(item.Id, item.Params);
            }

            return pTestItems;
        }
    }
}