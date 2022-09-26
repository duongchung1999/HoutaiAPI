using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class PartNoService {
        private readonly IRepository<PartNo> repository;

        public PartNoService(IRepository<PartNo> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 获取料号配置对应的料号列表
        /// </summary>
        /// <param name="partNoConfigID">料号配置Id</param>
        /// <returns></returns>
        public async Task<List<PartNo>> GetList(int partNoConfigID) {
            return await repository.DetachedEntities.Where(e => e.PartNoConfigId == partNoConfigID).ToListAsync();
        }

        /// <summary>
        /// 更新料号
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public async Task<PartNo> Update(PartNo partNo) {
            var result = await partNo.UpdateNowAsync();           
            return result.Entity;
        }

        /// <summary>
        /// 添加料号
        /// </summary>
        /// <returns></returns>
        public async Task<PartNo> Add(PartNo partNo) {
            var result = await partNo.InsertNowAsync();
            return result.Entity;
        }

        /// <summary>
        /// 删除料号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id) {
            await repository.DeleteNowAsync(id);
        }
   }
}
