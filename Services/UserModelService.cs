using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.FriendlyException;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class UserModelService {
        private  readonly IRepository<UserModel> repository;

        public UserModelService(IRepository<UserModel> personRepository) {
            this.repository = personRepository;
        }

        public async Task<List<UserModel>> GetList(int userId) {
            return await repository.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<List<UserModel>> Reallocate(List<UserModel> newUserModels) {
            if (newUserModels.Count == 0) throw Oops.Oh("请至少分配一个机型");

            await repository.Context.DeleteRangeAsync<UserModel>(e => e.UserId == newUserModels[0].UserId);
            await repository.Context.BulkInsertAsync(newUserModels);
            return newUserModels;
        }

        /// <summary>
        /// 为一个用户添加一个机型
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<UserModel> Add(int userId, int modelId) {
            var entity = new UserModel() {
                UserId = userId,
                ModelId = modelId
            };
            await entity.InsertNowAsync();
            return entity;
        }



    }
}
