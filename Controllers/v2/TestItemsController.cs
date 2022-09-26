# nullable enable
using Backend.Enties;
using Backend.Services;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.v2 {
    /// <summary>
    /// 测试项目
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/models/[controller]")]
    public class TestItemsController : IDynamicApiController {

        IRepository<TestItem> repository;
        IRepository<StationTestItem> stRepository;

        TestItemService service;
        const string NASTR = @"N/A";

        public TestItemsController(IRepository<TestItem> personRepository) {
            repository = personRepository;
            service = new TestItemService(repository);
            stRepository = Db.GetRepository<StationTestItem>();

        }

        /// <summary>
        /// 查询哪些站别使用了这一项
        /// </summary>
        /// <param name="testItemId">测试项目Id</param>
        /// <returns></returns>
        [Route("/api/v2/models/[controller]/{testItemId}/where-used")]
        public async Task<List<StationTestItem>> GetWhereUsed(int testItemId) {
            return await stRepository
                .Where(e => e.TestitemId == testItemId)
                .Include(st => st.Station)
                .Select(st => new StationTestItem() {
                    SortIndex = st.SortIndex,
                    Station = new Station() {
                        Name = st.Station.Name
                    }
                })
                .AsNoTracking()
                .ToListAsync();
        }

        #region 发送邮件
        //private static void sendmail(string DataTime, string path) {
        //    try {
        //        outlook.Application olapp = new outlook.Application();
        //        outlook.MailItem mailitem = (outlook.MailItem)olapp.CreateItem(outlook.OlItemType.olMailItem);
        //        //mailitem.To = "Xiaolong.Zhang (張曉龍) <xiaolong.zhang@merry-sz.com.cn>; Hangyu.Qiu (丘航宇) <hangyu.qiu@merry-sz.com.cn>; Xingyu.Chen (陳星宇) <xingyu.chen@merry-sz.com.cn>; " +
        //        //"";
        //        //发送给
        //        mailitem.To = ReadConfig("addressee");
        //        mailitem.Subject = DataTime + "_测试报表"; ;
        //        mailitem.BodyFormat = outlook.OlBodyFormat.olFormatHTML;
        //        string content = "邮件内容";
        //        mailitem.HTMLBody = content;
        //        mailitem.Attachments.Add(path);
        //        ((outlook._MailItem)mailitem).Send();
        //        mailitem = null; olapp = null;
        //    } catch (Exception ex) {
        //        Console.WriteLine(ex.ToString());
        //    }

        //}
        #endregion

        /// <summary>
        /// 添加测试项目
        /// </summary>
        /// <param name="modelId">所属机型Id</param>
        /// <param name="t">站别</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<TestItem> Add(int modelId, TestItem t) {
            t.ModelId = modelId;
            t.UpperValue = string.IsNullOrEmpty(t.UpperValue?.Trim()) ? NASTR : t.UpperValue;
            t.LowerValue = string.IsNullOrEmpty(t.LowerValue?.Trim()) ? NASTR : t.LowerValue;

            var result = await service.AddTestitem(t);
            return result.Entity;
        }

        /// <summary>
        /// 删除测试项目
        /// </summary>
        /// <param name="t"></param>
        /// <param name="modelId">所属机型Id</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<TestItem?> Delete(int modelId, TestItem t) {
            t.ModelId = modelId;
            t = await repository.FindOrDefaultAsync(t.Id);
            if (t != null) await service.DeleteTestitem(t);

            return null;
        }

        /// <summary>
        /// 搜索测试项目
        /// </summary>
        /// <param name="modelUk">所属机型的Id或名称</param>
        /// <param name="stationId">指定某个站别下的测试项目</param>
        /// <param name="keyWord">搜索关键字</param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelUk}/[controller]")]
        public async Task<List<TestItem>?> Get(string? modelUk, int stationId = 0, string? keyWord = null) {
            if (string.IsNullOrWhiteSpace(modelUk)) return null;

            if (int.TryParse(modelUk, out int modelId)) {
                return await service.GetCollection(modelId, stationId, keyWord);
            }

            return await service.GetCollection(modelUk, stationId, keyWord);
        }

        /// <summary>
        /// 更新测试项目
        /// </summary>
        /// <param name="modelId">所属机型Id</param>
        /// <param name="t"></param>
        /// <returns></returns>
        [Route("/api/v2/models/{modelId}/[controller]")]
        public async Task<TestItem> Update(int modelId, TestItem t) {
            t.ModelId = modelId;
            t.UpperValue = string.IsNullOrEmpty(t.UpperValue?.Trim()) ? NASTR : t.UpperValue;
            t.LowerValue = string.IsNullOrEmpty(t.LowerValue?.Trim()) ? NASTR : t.LowerValue;

            var result = await service.UpdateTestitem(t);
            return result.Entity;
        }
    }
}