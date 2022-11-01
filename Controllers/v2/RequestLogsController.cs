# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.ClayObject;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 请求记录
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
     class RequestLogsController : IDynamicApiController {

        private readonly IRepository<RequestLog> repository;
        RequestLogService requestLogService;

        public RequestLogsController(IRepository<RequestLog> personRepository) {
            
            repository = personRepository;
            requestLogService = new RequestLogService(personRepository);
        }

        /// <summary>
        /// 获取请求记录
        /// </summary>
        /// <param name="requestUserId"></param>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="ketWord"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [QueryParameters]
        [Authorize]
        public  async Task<PagedList<RequestLog>> GetList(int requestUserId, string path, string method, string ketWord, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20) {
            return await requestLogService.GetList(requestUserId, path, method, ketWord, startDate, endDate, page, pageSize);
        }

        public  async Task<object> GetOptions() {
            return await requestLogService.GetOptions();
        }

    }
}