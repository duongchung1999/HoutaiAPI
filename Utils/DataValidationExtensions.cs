using Furion;
using Furion.DataValidation;
using Furion.FriendlyException;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Utils.DataValidation {
	public static class DataValidationExtensions {
		/// <summary>
		/// 拓展方法，验证单个值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="errorMessage">验证失败的异常信息</param>
		/// <param name="validateAllProperties">是否验证所有属性</param>
		public static void TryValidate(this object obj, string errorMessage, bool validateAllProperties = true) {
			var result = obj.TryValidate(validateAllProperties);
			Oh(result, errorMessage);
		}

		/// <summary>
		/// 拓展方法，验证单个值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="errorMessage">验证失败的异常信息</param>
		/// <param name="validationAttributes">验证特性</param>
		public static void TryValidate(this object obj, string errorMessage, params ValidationAttribute[] validationAttributes) {
			var result = obj.TryValidate(validationAttributes);
			Oh(result, errorMessage);
		}

		/// <summary>
		/// 拓展方法，验证单个值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="errorMessage">验证失败的异常信息</param>
		/// <param name="validationTypes">验证类型</param>
		public static void TryValidate(this object obj, string errorMessage, params object[] validationTypes) {
			var result = obj.TryValidate(validationTypes);
			Oh(result, errorMessage);
		}

		/// <summary>
		/// 拓展方法，验证单个值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="errorMessage">验证失败的异常信息</param>
		/// <param name="validationOptionss">验证逻辑</param>
		/// <param name="validationTypes">验证类型</param>
		public static void TryValidate(this object obj, string errorMessage, ValidationPattern validationOptionss, params object[] validationTypes) {
			var result = obj.TryValidate(validationOptionss, validationTypes);
			Oh(result, errorMessage);
		}

		/// <summary>
		///  拓展方法，验证单个值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="errorMessage">验证失败的异常信息</param>
		/// <param name="regexPattern">正则表达式</param>
		/// <param name="regexOptions">正则表达式选项</param>
		public static void TryValidate(this object obj, string errorMessage, string regexPattern, RegexOptions regexOptions = RegexOptions.None) {
			var result = obj.TryValidate(regexPattern: regexPattern, regexOptions: regexOptions);
			if (result) throw Oops.Oh(errorMessage);
		}

		 static void Oh (DataValidationResult r, string m, int statusCode = 500) {
			if (!r.IsValid) {
				App.HttpContext.Response.StatusCode = statusCode;
				throw Oops.Oh(m);
			};
		}
	}
}
