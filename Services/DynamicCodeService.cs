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
    /// <summary>
    /// 动态码服务
    /// </summary>
    public class DynamicCodeService {
        private readonly IRepository<DynamicCode> repository;

        public DynamicCodeService(IRepository<DynamicCode> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 获取某个机型的动态码
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>s
        public async Task<DynamicCode> GetByModelId(int modelId) {
            return await repository.FirstOrDefaultAsync(e => e.ModelId == modelId);
        }

        public async Task<List<DynamicCode>> GetList() {
            return await repository.AsQueryable(false).ToListAsync();
        }

        public async Task<DynamicCode> Add(DynamicCode code) {
            // 如果机型已经有未过期的动态码
            var isExist = await repository.AnyAsync(e => e.ModelId == code.ModelId && DateTime.Now < e.ExpireDate);
            if (isExist) {
                throw Oops.Oh("当前机型已经过设置密码了");
            }
            var result = await code.InsertNowAsync();
            return result.Entity;
        }

        public async Task Delete(int id) {
            await repository.DeleteNowAsync(id);
        }
    
        /// <summary>
        /// 删除过期的动态码
        /// </summary>
        /// <returns></returns> 
        public async Task DeleteExpiredCode() {
            await repository.Context.DeleteRangeAsync<DynamicCode>(e => e.ExpireDate < DateTime.Now);
        }
    }
}
