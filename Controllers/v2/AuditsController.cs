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
    /// 审计
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class AuditsController : IDynamicApiController {

        private readonly IRepository<Audit> repository;
        private readonly AuditService service;

        public AuditsController(IRepository<Audit> personRepository) {
            repository = personRepository;
            service = new AuditService(repository);
        }

        /// <summary>
        /// 获取审计列表
        /// </summary>
        /// <param name="filte">分页过滤条件</param>
        /// <param name="audit">审计过滤条件</param>
        /// <returns></returns>
        [QueryParameters]
        public async Task<PagedList<Audit>> Get(Filte? filte, Audit? audit) {
            return await service.GetAllAudit(filte, audit);
        }
    }
}