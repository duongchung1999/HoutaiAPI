using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.FriendlyException;
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
            await CheckIsExist(partNo.No);
            var result = await partNo.UpdateNowAsync();
            return result.Entity;
        }
        // 
        /// <summary>
        /// 添加料号
        /// </summary>
        /// <returns></returns>
        public async Task<PartNo> Add(PartNo partNo) {
            await CheckIsExist(partNo.No);

            var model = await GetModel(partNo.ModelId);
            var partNoConfig = await GetPartNoConfig(partNo.PartNoConfigId);
            
            var result = await partNo.InsertNowAsync();
            await ActionRecordService.Add("添加料号", $"[{model.Name} {partNoConfig.Title}] 添加了料号{ActionRecordService.CreateDiffTextStyle("", partNo.No)}");
            return result.Entity;
        }

        /// <summary>
        /// 删除料号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id) {
            var partNo = await repository.FindOrDefaultAsync(id);
            var model = await GetModel(partNo.ModelId);
            var partNoConfig = await GetPartNoConfig(partNo.PartNoConfigId);

            await repository.DeleteNowAsync(id);
            await ActionRecordService.Add("删除料号", $"[{model.Name} {partNoConfig.Title}] 删除了料号{ActionRecordService.CreateDiffTextStyle(partNo.No, "")}");
        }

        /// <summary>
        /// 清除某个料号配置的料号
        /// </summary>
        /// <param name="partNoConfigId"></param>
        /// <returns></returns>
        public async Task Clear(int partNoConfigId) {
            var partNos = await repository.Where(e => e.PartNoConfigId == partNoConfigId).ToListAsync();
            //var partNoConfig = await GetPartNoConfig(partNoConfigId);
            //var model = await GetModel(partNoConfig.ModelId);
            var deleteText = "";
            foreach (var pn in partNos) {
                await pn.DeleteAsync();
                deleteText += $"{pn.No}\n";
            }
            await repository.SaveNowAsync();
            //await ActionRecordService.Add("删除料号", $"[{model.Name} {partNoConfig.Title}] 删除了料号\n{ActionRecordService.CreateDiffTextStyle(deleteText, "")}");
        }

        async Task<bool> CheckIsExist(string no) {
            var partNo = await repository.FirstOrDefaultAsync(e => e.No == no);
            if (partNo != null) {
                var model = await GetModel(partNo.ModelId);
                var partNoConfig = await GetPartNoConfig(partNo.PartNoConfigId);

                throw Oops.Oh($"料号 {no}，已经在 {model.Name} 的 {partNoConfig.Title} 中");
            }
            return true;
        }

        async Task<Model> GetModel(int modelId) {
            ModelService modelService = new(Db.GetRepository<Model>());
            var model = await modelService.Get(modelId);
            return model;
        }

        async Task<PartNoConfig> GetPartNoConfig(int partNoConfigId) {
            PartNoConfigService partNoConfigService = new(Db.GetRepository<PartNoConfig>());
            var partNoConfig = await partNoConfigService.Get(partNoConfigId);
            return partNoConfig;
        }

    }
}
