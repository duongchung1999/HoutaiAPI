using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Furion.RemoteRequest;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services {
    public class UserService {
        private readonly IRepository<User> repository;
        RoleService roleService;

        public UserService(IRepository<User> personRepository) {
            repository = personRepository;
            roleService = new RoleService(Db.GetRepository<Role>());
        }

        public async Task<User> Add(User u) {
            u.Password = Md5(u.Password);
            var role = await roleService.GetById(u.RoleId);
            u.PermissionRole = role;

            await u.InsertNowAsync();
            return await Get(u.Id);
        }

        public async Task<User> Login(User u) {
            var pwd = Md5(u.Password);
            var isExist = await repository.AnyAsync(e => e.Username == (u.Username) && e.Password.Equals(pwd));
            if (!isExist) return null;
            return await Get(u.Username);
        }

        public async Task<User> Get(int id) {
            var result = await repository.FindOrDefaultAsync(id);
            if (result == null) return null;

            var role = await roleService.GetById(result.RoleId);

            return new User() {
                Id = result.Id,
                Nickname = result.Nickname,
                //Role = result.Role,
                PermissionRole = role,
                RoleId = result.RoleId,
                Lang = result.Lang
            };
        }

        public async Task<User> Get(string username) {
            var user = await repository.SingleOrDefaultAsync(e => e.Username == username);
            if (user == null) return null;
            return await Get(user.Id);
        }

        public async Task<PagedList<User>> GetList(int page, int size) {
            return await repository.Entities.ToPagedListAsync(page, size);
        }

        public async Task<PagedList<User>> GetCollection(Filte filte) {
            return await repository.Entities.ToPagedListAsync(filte.Page, filte.Size);
        }

        public async Task<PagedList<User>> GetInfoCollection(Filte? filte) {
            var users = await repository.Entities
                .Select(e => new User {
                    Username = e.Username,
                    Id = e.Id,
                    Nickname = e.Nickname,
                    //Role = e.Role,
                    RoleId = e.RoleId,
                    Lang = e.Lang
                })
                .ToPagedListAsync(filte.Page, filte.Size);

            foreach (var u in users.Items) {
                var role = await roleService.GetById(u.RoleId);
                u.PermissionRole = role;
            }

            return users;
        }

        public async Task<User> UpdateUser(User u) {
            var excludeFields = new List<string>();

            if (string.IsNullOrEmpty(u.Password)) {
                excludeFields.Add("Password");
            }
            u.Password = Md5(u.Password);
            await u.UpdateExcludeNowAsync(excludeFields);
            var role = await roleService.GetById(u.RoleId);

            // 取消附加实体，防止下面两行更改到数据库
            repository.Detach(u);
            u.Password = "";
            u.PermissionRole = role;
            return u;
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notEncryptPassword"></param>
        /// <returns></returns>
        public async Task<User> UpdatePassword(int userId, string notEncryptPassword) {
            var includeFields = new List<string>() { "Password" };
            var u = await repository.FindOrDefaultAsync(userId);
            var pwd = Md5(notEncryptPassword);
            u.Password = pwd;
            var result = await u.UpdateIncludeNowAsync(includeFields);
            return result.Entity;
        }

        public async Task<User> DeleteUser(User u) {

            var result = await u.DeleteNowAsync();
            return result.Entity;
        }

        public async Task<bool> UpdateUserLang(int userId, string lang) {
            var user = await repository.FindOrDefaultAsync(userId);
            if (user == null) { return false; }

            user.Lang = lang;
            await user.UpdateNowAsync();
            return true;

        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="content">要加密的内容</param>
        /// <param name="isUpper">是否大写，默认小写</param>
        /// <param name="is16">是否是16位，默认32位</param>
        /// <returns></returns>
        public static string Md5(string content, bool isUpper = false, bool is16 = false) {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            string md5Str = BitConverter.ToString(result);
            md5Str = md5Str.Replace("-", "");
            md5Str = isUpper ? md5Str : md5Str.ToLower();
            return is16 ? md5Str.Substring(8, 16) : md5Str;
        }
    }
}
