using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffMatchPatch;

namespace Backend.Services {
    public class ModelService {

        private readonly IRepository<Model> repository;
        UserModelService UserModelService;
        //DynamicCodeService dynamicCodeService;

        public ModelService(IRepository<Model> personRepository) {
            repository = personRepository;
            UserModelService = new UserModelService(Db.GetRepository<UserModel>());
            //dynamicCodeService = new DynamicCodeService(Db.GetRepository<DynamicCode>());
        }

        public async Task<EntityEntry<Model>> Add(Model m) {
            var result = await m.InsertAsync();
            await repository.SaveNowAsync();

            //await ActionRecordService.Add(ActionRecord.ActionOptions.ADD_MODEL, $"添加了机型{m.Name}");
            await ActionRecordService.Add(ActionRecord.ActionOptions.ADD_MODEL, ActionRecordService.CreateDiffTextStyle("", m.Name));
            return result;
        }

        public async Task<EntityEntry<Model>> Delete(Model m) {
            await ActionRecordService.Add(ActionRecord.ActionOptions.DELETE_MODEL, ActionRecordService.CreateDiffTextStyle(m.Name, ""));
            return await m.DeleteNowAsync();
        }

        public async Task<Model> Get(string modelName) {
            var reuslt = await repository.Entities.AsNoTracking().SingleOrDefaultAsync(e => e.Name.Equals(modelName));

            //var dynamicCode = dynamicCodeService.GetByModelId(reuslt.Id);
            //reuslt.

            return reuslt;
        }
        
        public async Task<Model> Get(int id = 0) {
            return await repository.FindOrDefaultAsync(id);
        }

        public async Task<List<Model>> GetCollection(User user) {
            var result = repository.AsQueryable(false);
            var role = user.Role;

            if ((role & UserRoleOptions.ADMIN) == 0) {
                var userModels = await UserModelService.GetList(user.Id);
                var modelIds = userModels.Select(e => e.ModelId).ToList();
                result = result.Where(e => modelIds.Contains(e.Id) || e.CreatorId == user.Id);
            }

            return await result.ToListAsync();
        }

        public async Task<Model> Update(Model m) {
            var entry = repository.Attach(m);

            var oldValues = await entry.GetDatabaseValuesAsync();
            var oldModelName = oldValues["Name"].ToString();
            var oldPNConfigTemplate = oldValues["PnConfigTemplate"]?.ToString();

            await m.UpdateNowAsync();

            if (oldModelName != m.Name) {
                await ActionRecordService.Add("更新机型名称", ActionRecordService.CreateDiffTextStyle(oldModelName, m.Name));
            }

            if (oldPNConfigTemplate != m.PnConfigTemplate) {
                await ActionRecordService.Add("更新机型模板配置", $"[{m.Name}] {ActionRecordService.CreateDiffTextStyle(oldPNConfigTemplate, m.PnConfigTemplate)}");
            }

            return m;
        }
   
        /// <summary>
        /// 根据机型Id获取机型名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<string> GetModelNameById(int id) {
            IRepository<Model> repository = Db.GetRepository<Model>();
            var result = await repository.AsQueryable(false).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return result.Name;
        }
    }
}
