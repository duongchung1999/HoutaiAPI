using Furion.UnifyResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
namespace Backend.Filters {
	public class ApiExceptionFilter : IAsyncExceptionFilter {
		public Task OnExceptionAsync(ExceptionContext context) {
			var (StatusCode, Errors, _) = UnifyContext.GetExceptionMetadata(context);
			context.Result = new JsonResult(new {
				code = StatusCode,
				error = Errors is null ? 
					(context.Exception.InnerException is null ?
						context.Exception.Message : context.Exception.InnerException.Message  )
					: Errors,
			});

			//告诉异常已经处理 (此行必写)
			context.ExceptionHandled = true;
			return Task.CompletedTask;
		}
	}
}
