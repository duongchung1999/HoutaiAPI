# nullable enable

using Backend.Enties;
using DiffMatchPatch;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Backend.Services {
    /// <summary>
    /// 通用测试项目参数
    /// </summary>
    public class ActionRecordService {
        private readonly IRepository<ActionRecord> repository;

        public ActionRecordService(IRepository<ActionRecord> personRepository) {
            repository = personRepository;
        }

        /// <summary>
        /// 添加一条操作记录到数据库
        /// </summary>
        /// <param name="actionRecord"></param>
        /// <returns></returns>
        static async Task Add(ActionRecord actionRecord) {
            Task.Run(async () => {
                await actionRecord.InsertNowAsync();
            });
            //return actionRecord;
        }

        /// <summary>
        /// 添加一操作记录到数据库
        /// </summary>
        /// <param name="actionName">行动名称</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        public static async Task Add(string actionName, string message) {
            var nowUser = JwtHandler.GetNowUser();

            await Add(new ActionRecord() {
                Message = message,
                Name = actionName,
                Operator = nowUser.Nickname,
                CreateDate = DateTime.Now
            });
        }

        /// <summary>
        /// 比对差异文本，生成HTML
        /// </summary>
        /// <param name="oldValue">旧文本</param>
        /// <param name="newValue">新文本</param>
        /// <returns></returns>
        public static string CreateDiffTextStyle(string oldValue, string newValue) {
            oldValue ??= string.Empty;
            newValue ??= string.Empty;
            var diffMatche = new diff_match_patch();
            var diffList = diffMatche.diff_main(oldValue, newValue);
            diffMatche.diff_cleanupSemantic(diffList);
            var html = diffMatche.diff_prettyHtml(diffList);
            // 删除&para;
            html = $"<span class=\"diff-text-area\">{html}</span>".Replace("&para;", "");
            return html;
        }

        /// <summary>
        /// 模糊查询操作记录
        /// </summary>
        /// <param name="options">搜索选项</param>
        /// <returns></returns>
        public async Task<PagedList<ActionRecord>> Search(string? keywords, string? name, string? operatorName, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 20) {
            var queryable = repository.AsQueryable(false);

            if (!string.IsNullOrWhiteSpace(keywords)) {
                var kw = keywords.Trim();
                queryable = queryable.Where(e => e.Message.Contains(kw) || e.Name.Contains(kw) || e.Operator.Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(name)) {
                queryable = queryable.Where(e => e.Name == name);
            }

            if (startDate != null) {
                queryable = queryable.Where(e => e.CreateDate > startDate);
            }

            if (endDate != null) {
                queryable = queryable.Where(e => startDate < endDate);
            }

            if (!string.IsNullOrEmpty(operatorName)) {
                queryable = queryable.Where(e => e.Operator == operatorName);
            }

            

            return await queryable.OrderByDescending(e => e.CreateDate).ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// 获取操作者选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetOperatorOptions() {
            return await repository.AsQueryable(false).Select(e => e.Operator).Distinct().ToListAsync();
        }

        /// <summary>
        /// 获取操作选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetActionOptions() {
            return await repository.AsQueryable(false).Select(e => e.Name).Distinct().ToListAsync();
        }

    }
}
