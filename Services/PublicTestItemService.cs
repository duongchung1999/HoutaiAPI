using Backend.Enties.PublicTestItem;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services {
    public class PublicTestItemService {
        private readonly IRepository<PublicTestItem> repository;
        PublicTestItemParamService paramService;

        public PublicTestItemService(IRepository<PublicTestItem> personRepository) {
            repository = personRepository;
            paramService = new PublicTestItemParamService(Db.GetRepository<PublicTestItemParam>());
        }

        /// <summary>
        /// 获取某通用测试项目组的测试项目
        /// </summary>
        /// <param name="groupId">通用测试项目组的Id</param>
        /// <returns></returns>
        public async Task<List<PublicTestItem>> GetList(int groupId) {
            var result = await repository.Where(e => e.GroupId == groupId).ToListAsync();

            foreach (var item in result) { 
               var @params = await paramService.GetList(item.Id);
                item.Params = @params;
            }

            return result;
        }

        /// <summary>
        /// 清空某个通用测试项目组的测试项目
        /// </summary>
        /// <param name="groupId">通用测试项目组的Id</param>
        /// <returns></returns>
        public async Task<int> Clear(int groupId) {
            var toDeleteEntity = await repository.Where(st => st.GroupId == groupId).ToListAsync();
            foreach (var item in toDeleteEntity) {
                await item.DeleteAsync();
                await paramService.Clear(item.Id);
            }
            return await repository.SaveNowAsync();
        }

        /// <summary>
        /// 重新分配某个测试项目组的测试项目
        /// </summary>
        /// <param name="groupId">通用测试项目组的Id</param>
        /// <param name="publicTestItems">要添加的通用测试项目</param>
        /// <returns>添加后的通用测试项目列表</returns>
        public async Task<List<PublicTestItem>> Reallocate(int groupId, List<PublicTestItem> publicTestItems) {
            await Clear(groupId);

            foreach (var item in publicTestItems) { 
                item.GroupId = groupId;
                await item.InsertNowAsync();
                item.Params = await paramService.Reallocate(item.Id, item.Params);
            }
            return await GetList(groupId);
        }

    }
}
