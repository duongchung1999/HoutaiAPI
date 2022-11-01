using Backend.Enties;
using Backend.Services;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DataEncryption;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Furion.RemoteRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Backend.Controllers.v2 {

    /// <summary>
    /// 用户
    /// </summary>
    [ApiDescriptionSettings("Version2@2")]
    [Route("api/v2/[controller]")]
    public class UsersController : IDynamicApiController {
        private readonly IRepository<User> repository;
        private readonly UserService service;

        public UsersController(IRepository<User> personRepository) {
            repository = personRepository;
            service = new UserService(repository);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="u">用户</param>
        /// <returns></returns>
        public async Task<User> Add(User u) {
            if (string.IsNullOrEmpty(u.Nickname)) throw Oops.Oh("昵称不能为空");
            if (string.IsNullOrEmpty(u.Username)) throw Oops.Oh("用户名不能为空");

            u.Role = UserRoleOptions.NONE;
            var result = await service.Add(u);
            SetToken2Header(result);

            return await GetInfo(result.Id + "");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="u">用户</param>
        /// <returns>token</returns>
        [Route("/api/v2/users/token")]
        public async Task<object> Login(User u) {
            var result = await service.Login(u);
            if (result is null) throw Oops.Oh("用户名或密码错误");
            var (token, refreshToken) = SetToken2Header(result);
            return new ObjectResult(new {
                token,
                refreshToken
            });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns> 
        [Authorize]
        public async Task<User?> Delete(User u) {
            JwtHandler.ValidateRoles(PermissionRoleOptions.ADMIN);
            //JwtHandler.ValidateRoles(UserRoleOptions.ADMIN);
            await service.DeleteUser(u);
            return null;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userUk">用户的Id或用户名，如果Id为0获取自己的</param>
        /// <returns></returns>
        [Authorize]
        public async Task<User> GetInfo(string? userUk) {
            if (int.TryParse(userUk, out int userId)) {
                if (userId == 0) {
                    var nowUser = JwtHandler.GetNowUser();
                    userId = nowUser.Id;
                }
                return await service.Get(userId);
            }

            return await service.Get(userUk);
        }

        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="filte">过滤条件</param>
        /// <returns></returns>
        [Authorize]
        public async Task<PagedList<User>> GetInfo([FromQuery] Filte? filte) {
            var users = await service.GetInfoCollection(filte);

            return users;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="u">u</param>
        /// <returns></returns>
        [Authorize]
        public async Task<User> Update(User u) {
            //JwtHandler.HasRoles(UserRoleOptions.ADMIN);
            JwtHandler.ValidateRoles(PermissionRoleOptions.ADMIN);
            var result = await service.UpdateUser(u);
            return result;
        }
        /// <summary>
        /// 修改自己账号的密码
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<User> ChangePassword(User u) {
            var nowUser = JwtHandler.GetNowUser();
            if (nowUser.Id != u.Id) {
                throw Oops.Oh("只能更改自己账号的密码");
            }
            return await service.UpdatePassword(u.Id, u.Password);

        }

        /// <summary>
        /// 创建token到响应头
        /// </summary>
        /// <param name="u"></param>
        /// <returns>用户Token和刷新Token</returns>
        static (string token, string refreshToken) SetToken2Header(User u) {
            var options = JWTEncryption.GetJWTSettings();
            options.ExpiredTime = 1 * 60 * 24 * 30;

            var userlevel = u.PermissionRole?.Level;
            userlevel ??= 0;

            var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>()
            {
                { "UserId", u.Id },  // 存储Id
                { "UserNickname",u. Nickname}, // 存储用户昵称
				{ "userRole", u.Role},
                { "userLevel",  userlevel}
            });

            // 获取刷新 token
            var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken); // 第二个参数是刷新 token 的有效期，默认三十天

            // 设置请求报文头
            var httpContext = App.HttpContext;
            httpContext.Response.Headers["access-token"] = accessToken;
            httpContext.Response.Headers["x-access-token"] = refreshToken;
            return (accessToken, refreshToken);
        }
    }
}

