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
    /// 动态码
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class DynamicCodesController : IDynamicApiController {

        private readonly DynamicCodeService service;

        public DynamicCodesController(IRepository<DynamicCode> personRepository) {
            service = new DynamicCodeService(personRepository);
        }
       
        /// <summary>
        /// 获取某个机型的动态码
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<DynamicCode> GetByModelId(int modelId) {
            return await service.GetByModelId(modelId);
        }

        public async Task<List<DynamicCode>> GetList() {
            return await service.GetList();
        }

        public async Task<DynamicCode> Add(DynamicCode code) {
            return await service.Add(code);
        }

        public async Task Delete(int id) {
            await service.Delete(id);
        }
    }
}