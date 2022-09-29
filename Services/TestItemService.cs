using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class TestItemService {
        private readonly IRepository<TestItem> repository;
        private readonly IRepository<StationTestItem> stationTestitemRepository;
        readonly IRepository<Model> modelRepository;
        readonly IRepository<Station> stationRepository;
        readonly ModelService modelService;

        public TestItemService(IRepository<TestItem> personRepository) {
            repository = personRepository;
            stationTestitemRepository = Db.GetRepository<StationTestItem>();
            modelRepository = Db.GetRepository<Model>();
            stationRepository = Db.GetRepository<Station>();
            modelService = new ModelService(modelRepository);
        }

        public async Task<EntityEntry<TestItem>> AddTestitem(TestItem t) {
            SetConfigOperator2Name(t);
            var result = await t.InsertNowAsync();

            var modelName = await ModelService.GetModelNameById(t.ModelId);
            var newText = CreateActionRecordText(t);
            await ActionRecordService.Add("添加测试项目", $"[{modelName}]\n{ActionRecordService.CreateDiffTextStyle("", newText)}");
            return result;
        }
            
        public async Task<EntityEntry<TestItem>> DeleteTestitem(TestItem t) {
            var result = await t.DeleteNowAsync();

            var modelName = await ModelService.GetModelNameById(t.ModelId);
            var oldText = CreateActionRecordText(t);
            await ActionRecordService.Add("删除测试项目", $"[{modelName}]\n{ActionRecordService.CreateDiffTextStyle(oldText, "")}");

            return result;
        }

        public async Task<EntityEntry<TestItem>> UpdateTestitem(TestItem t) {
            var UnUpdateFields = new List<string>();
            if (t.ModelId == 0) UnUpdateFields.Add("ModelId");
            if (string.IsNullOrEmpty(t.Name)) UnUpdateFields.Add("Name");
            if (string.IsNullOrEmpty(t.Cmd)) UnUpdateFields.Add("Cmd");

            var entry =  repository.Attach(t);
            var oldValue = await entry.GetDatabaseValuesAsync();
            var oldText = CreateActionRecordText(oldValue);
            var modelName = await ModelService.GetModelNameById(t.ModelId);

            var result = await t.UpdateExcludeNowAsync(UnUpdateFields);
            SetConfigOperator2Name(t);

            var newText = CreateActionRecordText(t);
            await ActionRecordService.Add("更新测试项目", $"[{modelName}]\n{ActionRecordService.CreateDiffTextStyle(oldText, newText)}");

            return result;
        }

        public async Task<TestItem> GetTestitem(int id) {
            return await repository.SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public List<TestItem> GetAllTestitem(int modelId, int stationId, string modelName, string stationName) {

            if (!string.IsNullOrEmpty(modelName) && !string.IsNullOrEmpty(stationName)) {
                var model = modelRepository.Where(e => e.Name.Equals(modelName)).SingleOrDefault();
                if (model != null) {
                    modelId = model.Id;
                    var station = stationRepository.Where(e => e.Name.Equals(stationName) && e.ModelId == model.Id).SingleOrDefault();
                    if (station != null) {
                        stationId = station.Id;
                    }
                }
            }

            if (stationId == 0) {
                return repository.Entities.Where(e => e.ModelId.Equals(modelId)).OrderBy(e => e.Name).ToList();
            }

            var query = from st in stationTestitemRepository.AsQueryable()
                        join t in repository.AsQueryable() on st.TestitemId equals t.Id
                        where st.StationId == stationId
                        orderby st.SortIndex
                        select new TestItem() {
                            Id = t.Id,
                            //ModelId = t.ModelId,
                            //StationId = st.StationId,
                            //StationTestitemId = st.Id,
                            Unit = t.Unit,
                            Name = t.Name,
                            LowerValue = t.LowerValue,
                            UpperValue = t.UpperValue,
                            Cmd = t.Cmd,
                            SortIndex = st.SortIndex,
                            No = t.No
                        };
            return query.OrderBy(e => e.Name).ToList();
        }

        public async Task<List<TestItem>> GetCollection(int modelId, int stationId = 0, string? keyWord = null) {
            var result = repository.Where(e => e.ModelId == modelId);
            if (stationId != 0) {
                result = result
                    //.Include(e => e.StationTestitems)
                    .Where(e => e.StationTestitems
                        .Any(st => st.StationId == stationId && st.TestitemId == st.TestitemId)
                    );
            }
            if (!string.IsNullOrEmpty(keyWord)) {
                keyWord = keyWord.ToLower();
                result = result.Where(e => e.Name.ToLower().Contains(keyWord) || e.Cmd.ToLower().Contains(keyWord));
            }
            return await result.ToListAsync();
            //return repository.Where(e => e.ModelId == modelId).ToList();
        }

        public async Task<List<TestItem>> GetCollection(string modelName, int stationId = 0, string? keyWord = null) {
            var model = await modelService.Get(modelName);
            return await GetCollection(model.Id, stationId, keyWord);
        }

        public async Task<TestItem> Get(int testItemId) {
            return await repository.FindOrDefaultAsync(testItemId);
        }

        /// <summary>
        /// 把配置符号添加到测试项目名称
        /// </summary>
        /// <param name="testItem"></param>
        void SetConfigOperator2Name(TestItem testItem) {
            if (testItem.IsAlwaysRun && !testItem.Name.Contains("+")) {
                testItem.Name = "+" + testItem.Name;
            }

            if (!testItem.IsAlwaysRun && testItem.Name.Contains("+")) {
                testItem.Name = testItem.Name.Replace("+", "");
            }

            if (testItem.IsHidden && !testItem.Name.Contains("-")) {
                testItem.Name = "-" + testItem.Name;
            }

            if (!testItem.IsHidden && testItem.Name.Contains("-")) {
                testItem.Name = testItem.Name.Replace("-", "");
            }
        }
    
        string CreateActionRecordText(TestItem testItem) {
            return $"项目名称：{testItem.Name}\n调用命令：{testItem.Cmd}\n上限：{testItem.LowerValue}\n下限：{testItem.UpperValue}\n单位：{testItem.Unit}\n比对编号：{testItem.No}\n隐藏此项：{testItem.IsHidden}\n始终执行：{testItem.IsAlwaysRun}";
        }

        string CreateActionRecordText(PropertyValues pvs) {
            return $"项目名称：{pvs["Name"]}\n调用命令：{pvs["Cmd"]}\n上限：{pvs["LowerValue"]}\n下限：{pvs["UpperValue"]}\n单位：{pvs["Unit"]}\n比对编号：{pvs["No"]}\n隐藏此项：{pvs["IsHidden"]}\n始终执行：{pvs["IsAlwaysRun"]}";
        }
    }
}
