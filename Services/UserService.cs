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

        public UserService(IRepository<User> personRepository) {
            repository = personRepository;
        }

        public async Task<User> Add(User u) {
            u.Password = Md5(u.Password);
            var result = await u.InsertNowAsync();
            return result.Entity;
        }

        public async Task<User> Login(User u) {
            var pwd = Md5(u.Password);
            var result = await repository.FirstOrDefaultAsync(e =>  e.Username == (u.Username) && e.Password.Equals(pwd));
            return result;
        }

        public async Task<User> Get(int id) {
            var result = await repository.FindOrDefaultAsync(id);
            if (result == null) return null;
            return new User() {
                Id = result.Id,
                Nickname = result.Nickname,
                Role = result.Role
            };
        }

        public async Task<User> Get(string nickname) {
            var user = await repository.SingleOrDefaultAsync(e => e.Username == nickname);
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
            return await repository.Entities
                .Select(e => new User { 
                    Username = e.Username,
                    Id = e.Id,
                    Nickname = e.Nickname,
                    Role = e.Role
                })
                .ToPagedListAsync(filte.Page, filte.Size);
        }

        public async Task<User> UpdateUser(User u) {
            var excludeFields = new List<string>();

            if (string.IsNullOrEmpty(u.Password)) {
                excludeFields.Add("Password");
            }
            u.Password = Md5(u.Password);
            var result = await u.UpdateExcludeAsync(excludeFields);
            return result.Entity;
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
