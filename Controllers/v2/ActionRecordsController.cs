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
    /// 操作记录
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class ActionRecordsController : IDynamicApiController {

        private readonly IRepository<ActionRecord> repository;

        ActionRecordService service;
        
        public ActionRecordsController(IRepository<ActionRecord> personRepository) {
            repository = personRepository;
            service = new ActionRecordService(repository);
        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="keywords">搜索关键字</param>
        /// <param name="name">动作名称</param>
        /// <param name="operatorName">操作者名称</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        [QueryParameters]
        public async Task<PagedList<ActionRecord>> Search(string? keywords, string? name, string? operatorName, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 20) {
            return await service.Search( keywords,  name, operatorName, startDate,  endDate, pageIndex , pageSize);
        }

        /// <summary>
        /// 获取操作者选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetOperatorOptions() {
            return await service.GetOperatorOptions();
        }

        /// <summary>
        /// 获取操作选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetActionOptions() {
            return await service.GetActionOptions();
        }

    }
}