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
    /// 用户机型
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class UserModelsController : IDynamicApiController {

        private readonly IRepository<UserModel> repository;
        UserModelService userModelService;

        public UserModelsController(IRepository<UserModel> personRepository) {
            repository = personRepository;
            userModelService = new UserModelService(personRepository);
        }

        public async Task<List<UserModel>> GetList(int userId) {
            return await userModelService.GetList(userId);
        }

        public async Task<List<UserModel>> Reallocate(List<UserModel> newUserModels) {
           return await userModelService.Reallocate(newUserModels);
        }

    }
}