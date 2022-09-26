using Furion;
using Furion.DataValidation;
using Furion.DependencyInjection;
using Furion.UnifyResult;
using Furion.UnifyResult.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utils {
    /// <summary>
    /// RESTful 风格返回值
    /// </summary>
    [SuppressSniffer, UnifyModel(typeof(RESTfulResult<>))]
    public class RESTfulResultProvider : IUnifyResultProvider {
        /// <summary>
        /// sql索引异常信息
        /// </summary>
        static readonly Dictionary<string, string> SqlErrorDict = new() {
             { "IX_User_Username_Uk", "账号已被注册"},
             { "IX_User_Nickname_Uk", "昵称已被使用"},

             { "IX_Model_Name_Uk", "机型名称已被使用"},

             { "IX_Station_Name_ModelId_Uk", "已有此站别"},

             { "IX_Testitem_ModelId_Name_Uk", "已有这个测试名称"},
             { "IX_Testitem_ModelId_Cmd_Uk", "已有这个调用命令"},

            { "IX_StationTestitem_StationId_TestitemId_Uk", "测试项目重复"},
            { "IX_StationTestitem_StationId_SortIndex_Uk", "测试顺序重复"},

            { "IX_DeviceDll_Name_Uk", "dll名称重复"},

            { "IX_DeviceDllParam_Name_Uk","已有此参数名"},

            { "IX_ModelDll_Commit_ModelId_UK", "已有这个Dll版本"},
            {"IX_PublicDll_Name_Uk", "已有此Dll"  },
            { "IX_PublicDllFunc_FuncName_PublicDllId", "此Dll已有此方法"},

            { "ix_modelPermission_modelId_userId", "这个用户已经分配过这个机型的权限了" },

            { "IX_UserPermission_Uk", "此用户已经分配过这个权限了"},

            { "IX_BackstageConfig_Key_Uk", "已经有这一项配置了"},

            { "PartNo_No_UK", "已有这个料号了"},
            { "IX_PartNoConfig_Title_UK", "已经有这个名称的配置了" },

            { "PublicTestItemGroup_DllName_Uk", "已经有这个dllname的通用测试项目组了" },
            { "PublicTestItemGroup_Summary_Uk", "已经有这个概述的通用测试项目组了" },
        };

        /// <summary>
        /// 异常返回
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata) {
            var StatusCode = metadata.StatusCode;
            var Errors = metadata.Errors;
            //if (Errors.Equals("权限不足")) {
            //    StatusCode = StatusCodes.Status403Forbidden;
            //}
            object sqlError = null;
            if (context.Exception.InnerException != null) {
                sqlError = ParseSqlError(context.Exception.InnerException);
            }

            return new JsonResult(new RESTfulResult<object> {
                StatusCode = StatusCode,
                Succeeded = false,
                Data = null,
                Errors = sqlError is null ? (Errors is null ? context.Exception : Errors) : sqlError
            });
        }

        /// <summary>
        /// 验证失败返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="modelStates"></param>
        /// <param name="validationResults"></param>
        /// <param name="validateFailedMessage"></param>
        /// <returns></returns>
        public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata) {
            return new JsonResult(new RESTfulResult<object> {
                StatusCode = StatusCodes.Status400BadRequest,
                Succeeded = false,
                Data = new EmptyResult(),
                Errors = metadata.ValidationResult,
            });
        }

        /// <summary>
        /// 处理输出状态码
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings) {
            // 设置响应状态码
            UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

            switch (statusCode) {
                // 处理 401 状态码
                case StatusCodes.Status401Unauthorized:
                    await context.Response.WriteAsJsonAsync(new RESTfulResult<object> {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Succeeded = false,
                        Data = new EmptyResult(),
                        Errors = "401 Unauthorized",
                        Extras = UnifyContext.Take(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;
                // 处理 403 状态码
                case StatusCodes.Status403Forbidden:
                    await context.Response.WriteAsJsonAsync(new RESTfulResult<object> {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Succeeded = false,
                        Data = null,
                        Errors = "403 Forbidden",
                        Extras = UnifyContext.Take(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 成功返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IActionResult IUnifyResultProvider.OnSucceeded(ActionExecutedContext context, object data) {
            if (data == null) {
                data = new EmptyResult();
            }
            return new JsonResult(new RESTfulResult<object> {
                StatusCode = StatusCodes.Status200OK,
                Succeeded = true,
                Data = data,
            });
        }

        public static string ParseSqlError(Exception e) {
            var errMsg = e.Message;
            string indexName = null;

            if (errMsg.StartsWith("Duplicate entry")) {
                var index1 = errMsg.IndexOf("for key");
                index1 += 2 + 7;
                indexName = errMsg[index1..];
                indexName = indexName[0..^1];

                indexName = indexName.Substring(indexName.IndexOf(".") + 1);

                if (SqlErrorDict.ContainsKey(indexName)) {
                    return SqlErrorDict[indexName];
                }
            }
            return null;
        }
    }
}