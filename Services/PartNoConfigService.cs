using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class PartNoConfigService {
        private readonly IRepository<PartNoConfig> repository;

        public PartNoConfigService(IRepository<PartNoConfig> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 获取某个机型的料号配置列表（只包含ID和标题）
        /// </summary>
        /// <returns></returns>
        public async Task<List<PartNoConfig>> GetList(int modelId) {
            return await repository.DetachedEntities.Where(e => e.ModelId == modelId ).Select(e => new PartNoConfig() { Id = e.Id, Title = e.Title }).ToListAsync();
        }

        /// <summary>
        /// 根据Id获取料号配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> Get(int id) {
            return await repository.FindOrDefaultAsync(id);
        }
        
        /// <summary>
        /// 添加料号配置
        /// </summary>
        /// <param name="partNoConfig"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> Add(PartNoConfig partNoConfig) {
            var result = await partNoConfig.InsertNowAsync();
            return result.Entity;
        }
        
        /// <summary>
        /// 更新料号配置
        /// </summary>
        /// <param name="partNoConfig"></param>
        /// <returns></returns>
        public async Task<PartNoConfig> Update(PartNoConfig partNoConfig) {
            var result = await partNoConfig.UpdateNowAsync();
            return result.Entity;
        }

        /// <summary>
        /// 删除料号配置
        /// </summary>
        /// <param name="partNoConfig"></param>
        /// <returns></returns>
        public async Task Delete(int id) {
           await repository.DeleteNowAsync(id);
        }
    }
}
