using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.JsonSerialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utils {
    public class RequestLogFilter : IAsyncActionFilter {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {

            await next();
            ////============== 这里是执行方法之前获取数据 ====================

            //// 获取控制器、路由信息
            //var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            //// 获取请求的方法

            //// 获取 HttpContext 和 HttpRequest 对象
            //var httpContext = context.HttpContext;
            //var httpRequest = httpContext.Request;

            //var method = httpContext.Request.Method;
            //var path = actionDescriptor.AttributeRouteInfo.Template;

            //if (method == "GET") {
            //    await next();
            //    return;
            //};

            //// 获取客户端 Ipv4 地址
            //var remoteIPv4 = httpContext.GetRemoteIpAddressToIPv4();

            //// 获取请求的 Url 地址
            //var requestUrl = httpRequest.GetRequestUrlAddress();

            //// 获取来源 Url 地址
            //var refererUrl = httpRequest.GetRefererUrlAddress();

            //// 获取请求参数（写入日志，需序列化成字符串后存储）
            //var parameters = context.ActionArguments;

            //// 获取操作人（必须授权访问才有值）"userId" 为你存储的 claims type，jwt 授权对应的是 payload 中存储的键名
            //var userId = httpContext.User?.FindFirstValue("userId");
            //var requestUserNickme = httpContext.User?.FindFirstValue("userNickname");

            //// 请求时间
            //var requestedTime = DateTimeOffset.Now;


            ////============== 这里是执行方法之后获取数据 ====================
            //var actionContext = await next();

            //// 获取返回的结果
            //var returnResult = actionContext.Result;

            //// 判断是否请求成功，没有异常就是请求成功
            //var isRequestSucceed = actionContext.Exception == null;

            //// 获取调用堆栈信息，提供更加简单明了的调用和异常堆栈
            //var stackTrace = EnhancedStackTrace.Current();

            //// 这里写入日志，或存储到数据库中！！！~~~~~~~~~~~~~~~~~~~~

            //var jsonSerializeSetting = new JsonSerializerSettings {
            //    NullValueHandling = NullValueHandling.Ignore,
            //    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            //    DateFormatString = "yyyy-MM-dd HH:mm:ss",
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //};

            //var repository = Db.GetRepository<RequestLog>();
            //var log = new RequestLog() {
            //    RequestUrl = requestUrl,
            //    ReferUrl = refererUrl,
            //    Ip = remoteIPv4,
            //    Method = method,
            //    Path = path,
            //    Parameters = JsonConvert.SerializeObject(parameters, jsonSerializeSetting),
            //    RequestUserId = Convert.ToInt32(userId),
            //    RequestUserNickanme = requestUserNickme,
            //    IsSucceed = isRequestSucceed,
            //    RequestDate = DateTime.Now,
            //    Result = JsonConvert.SerializeObject(returnResult, jsonSerializeSetting)
            //};
            //await repository.InsertNowAsync(log);
        }
    }
}