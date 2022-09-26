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
    /// <summary>
    /// 通用测试项目参数
    /// </summary>
    public class PublicTestItemParamService {
        private readonly IRepository<PublicTestItemParam> repository;

        public PublicTestItemParamService(IRepository<PublicTestItemParam> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 获取某通用测试项目的参数
        /// </summary>
        /// <param name="methodId">通用测试项目的Id</param>
        /// <returns></returns>
        public async Task<List<PublicTestItemParam>> GetList(int methodId) {
            var result = await repository.Where(e => e.MethodId == methodId).ToListAsync();
            return result;
        }

        /// <summary>
        /// 清空某个通用测试项目的参数
        /// </summary>
        /// <param name="methodId">通用测试项目的Id</param>
        /// <returns></returns>
        public async Task<int> Clear(int methodId) {
            var toDeleteEntity = await repository.Where(st => st.MethodId == methodId).ToListAsync();
            foreach (var item in toDeleteEntity) {
                await item.DeleteAsync();
            }
            return await repository.SaveNowAsync();
        }

        /// <summary>
        /// 重新分配某个通用测试项目的参数
        /// </summary>
        /// <param name="methodId">通用测试项目的Id</param>
        /// <param name="publicTestItemParams">要添加的参数列表</param>
        /// <returns>添加后的参数列表</returns>
        public async Task<List<PublicTestItemParam>> Reallocate(int methodId, List<PublicTestItemParam> publicTestItemParams) {
            await Clear(methodId);
            if (publicTestItemParams == null) return new List<PublicTestItemParam>();

            foreach (var item in publicTestItemParams) {
                item.MethodId = methodId;
                await item.InsertAsync(); 
            }
            await repository.SaveNowAsync();

            return await GetList(methodId);
        }

    }
}
