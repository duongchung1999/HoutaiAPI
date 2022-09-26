using Backend.Enties.PublicTestItem;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.FriendlyException;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services {
    public class PublicTestItemGroupService {
        private readonly IRepository<PublicTestItemGroup> repository;

        public PublicTestItemGroupService(IRepository<PublicTestItemGroup> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 获取通用测试项目组列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<PublicTestItemGroup>> GetList() {
            var result = await repository.Entities.ToListAsync();
            return result;
        }

        /// <summary>
        /// 删除通用测试项目组
        /// </summary>
        /// <param name="groupId">通用测试项目组Id</param>
        /// <returns></returns>
        public async Task<bool> Delete(int groupId) {
            var toDeleteGroup  = await repository.FindOrDefaultAsync(groupId);
            if (toDeleteGroup != null) await toDeleteGroup.DeleteNowAsync();
            return true;
        }

        /// <summary>
        /// 添加测试项目组
        /// </summary>
        /// <param name="publicTestItemGroup"></param>
        /// <returns></returns>
        public async Task<PublicTestItemGroup> Add(PublicTestItemGroup publicTestItemGroup) {
           var isExsit =  repository.Any(e => e.DllName == publicTestItemGroup.DllName || e.Summary == publicTestItemGroup.Summary);

            if (isExsit) Oops.Oh("概述或Dll名称重复");

            var result = await publicTestItemGroup.InsertNowAsync();
            return result.Entity;
        }

    }
}
