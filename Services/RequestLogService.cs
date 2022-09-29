using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Backend.Services {
    public class RequestLogService {
        private readonly IRepository<RequestLog> repository;

        public RequestLogService(IRepository<RequestLog> personRepository) {
            this.repository = personRepository;
        }

        public  async Task<RequestLog> Add(RequestLog requestLog) {
            var result = await requestLog.InsertNowAsync();
            return result.Entity;
        }

        public  async Task<PagedList<RequestLog>> GetList(int requestUserId, string path, string method, string ketWord, DateTime? startDate, DateTime? endDate, int page, int pageSize) {
            var query = repository.AsQueryable(false);

            if (!JwtHandler.HasRoles(UserRoleOptions.ADMIN, false)) {
                query = query.Where(e => e.Path != "api/v2/users" && !e.Path.Contains("api/v2/users/"));
            }

            if (requestUserId != 0) query = query.Where(e => e.RequestUserId == requestUserId);

            if (!string.IsNullOrEmpty(path)) query = query.Where(e => e.Path.Contains(path));

            if (!string.IsNullOrEmpty(method)) query = query.Where(e => e.Method == method);
            if (!string.IsNullOrEmpty(ketWord)) query = query.Where(e => e.Parameters.Contains(ketWord));
            if (startDate != null) query = query.Where(e => e.RequestDate > startDate);
            if (endDate != null) query = query.Where(e => e.RequestDate < endDate);

            return await query.OrderByDescending(e => e.RequestDate).ToPagedListAsync(page, pageSize);

        }

        public  async Task<object> GetOptions() {
            var path = await repository.AsQueryable(false).Select(e => e.Path).Distinct().ToListAsync();

            var userIds = await repository.AsQueryable(false).Select(e => e.RequestUserId).Distinct().ToListAsync();

            var user = await repository.AsQueryable(false).Where(e => e.RequestUserId != 0).Select(e => new User() { Id = e.RequestUserId, Nickname = e.RequestUserNickanme}).Distinct().ToListAsync();

            var result = new Dictionary<string, List<object>>();
            return new { path, user };

        }
    }
}
