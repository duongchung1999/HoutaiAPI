using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Furion.FriendlyException;
using Backend.Enties;

namespace Utils {

    /// <summary>
    /// JWT 授权自定义处理程序
    /// </summary>
    public class JwtHandler : AppAuthorizeHandler {

        public JwtHandler() {
            var options = JWTEncryption.GetJWTSettings();
            options.ExpiredTime = 1 * 60 * 24 * 30;
        }

        /// <summary>
        /// 重写 Handler 添加自动刷新收取逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context) {
            // 自动刷新 token
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext())) {
                await AuthorizeHandleAsync(context);
            } else context.Fail();    // 授权失败
        }

        /// <summary>
        /// 获取当前发起请求的用户
        /// </summary>
        /// <returns></returns>
        public static User GetNowUser() {
            var id = App.User?.FindFirstValue("UserId");
            var nickname = App.User?.FindFirstValue("UserNickname");
            var role = App.User?.FindFirstValue("userRole");
            var userLevel = App.User?.FindFirstValue("userLevel");

            return new User() {
                Id = Convert.ToInt32(id),
                Nickname = nickname is null ? "" : nickname,
                Role = (UserRoleOptions)(role is null ? UserRoleOptions.NONE : Enum.ToObject(typeof(UserRoleOptions), Convert.ToInt32(role))),
                PermissionRole = new Role() {
                    Level = Convert.ToInt32(userLevel)
                }
            };
        }

        /// <summary>
        /// 获取当前发起请求的用户Id
        /// </summary>
        /// <returns></returns>
        public static int GetNowUserId() {
            var id = App.User?.FindFirstValue("UserId");
            return Convert.ToInt32(id);
        }

        /// <summary>
        /// 校验当前用户角色是否有权限
        /// </summary>
        /// <param name="validateRole"></param>
        /// <param name="isThrowError">是否抛出异常</param>
        [Obsolete("导入权限角色管理后废弃")]
        public static bool ValidateRoles(UserRoleOptions validateRole, bool isThrowError = true) {
            var nowUser = GetNowUser();

            if ((nowUser.Role & validateRole) == 0) {
                if (isThrowError) {
                    App.HttpContext.Response.StatusCode = 403;
                    throw Oops.Oh("权限不足");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 校验当前账号是否有权限（2022.11.1之后的版本）
        /// </summary>
        /// <param name="permissionRole"></param>
        /// <param name="isThrowError"></param>
        /// <returns></returns>
        public static bool ValidateRoles(PermissionRoleOptions permissionRole, bool isThrowError = true) {
            var nowUser = GetNowUser();

            if ((nowUser.PermissionRole.Level == Convert.ToInt32(permissionRole))) {
                if (isThrowError) {
                    App.HttpContext.Response.StatusCode = 403;
                    throw Oops.Oh("权限不足");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证管道，也就是验证核心代码
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext) {
            //var path = httpContext.Request.Path.Value;
            //var method = httpContext.Request.Method;

            //var adminApiPaths = new string[]{
            //	"/api/v1/user",
            //	"",
            //};


            //foreach (var item in adminApiPaths) {
            //	var nowUser = GetNowUser();
            //	// 如果访问了管理员api 但没有权限
            //	if (path.Contains(item) && !(nowUser.Role.Equals("admin") || nowUser.Role.Equals("super-admin"))) {
            //		return Task.FromResult(false);
            //	}
            //}

            // 检查权限，如果方法时异步的就不用 Task.FromResult 包裹，直接使用 async/await 即可
            return Task.FromResult(true);
        }

        
        /// <summary>
        /// 校验当前用户角色是否有权限
        /// </summary>
        /// <param name="validateRole"></param>
        /// <param name="isThrowError">是否抛出异常</param>
        [Obsolete("导入权限角色管理后废弃")]
        public static bool HasRoles(UserRoleOptions validateRole, bool isThrowError = true) {
            return ValidateRoles(validateRole, isThrowError);
        }

        /*
         export enum PermissionRoleOptions {
    ADMIN = 8,
    ACCOUNT_MANAGER = 7,
    SW = 5,
    TE = 4,
        TEMPLATE_PROGRAM_DEVELOPER = 2,
    BASC = 1,
}
         */
    }
}