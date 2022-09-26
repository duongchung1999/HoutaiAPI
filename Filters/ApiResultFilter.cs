using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Filters {
	public class ApiResultFilter : ActionFilterAttribute {
		public override void OnResultExecuting(ResultExecutingContext context) {
			if (context.Result is ObjectResult) {
				
				var objectResult = context.Result as ObjectResult;

				if (objectResult.Value == null) {
					context.Result = new ObjectResult(new { code = 200, data = objectResult.Value });
				} else {
					context.Result = new ObjectResult(new { code = 200, data = objectResult.Value });
				}
			} else if (context.Result is EmptyResult) {
				context.Result = new ObjectResult(new { code = 404, error = "未找到资源", result = (context.Result as ContentResult).Content });
			} else if (context.Result is ContentResult) {
				context.Result = new ObjectResult(new { code = 200, result = (context.Result as ContentResult).Content });
			} else if (context.Result is StatusCodeResult) {
				context.Result = new ObjectResult(new { code = (context.Result as StatusCodeResult).StatusCode });
			}
		}
	}
}
