using Backend.Enties;
using Furion.DatabaseAccessor;
using Furion.DatabaseAccessor.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services {
    public class AuditService {
        private readonly IRepository<Audit> repository;

        public AuditService( IRepository<Audit> personRepository ) {
            repository = personRepository;
        }

        public static async Task<EntityEntry<Audit>> AddAuditAsync( Audit a ) {
            return await a.InsertAsync();
        }

        public async Task<PagedList<Audit>> GetAllAudit( Filte filter, Audit? a ) {
            IQueryable<Audit> audits = repository.Entities;

            // 根据传过来的实体查询 
            if ( a != null ) {
                if ( a.Id != 0 ) audits = audits.Where( e => e.Id == a.Id );
                if ( !string.IsNullOrEmpty( a.Table ) ) audits = audits.Where( e => e.Table == a.Table );
                if ( !string.IsNullOrEmpty( a.Column ) ) audits = audits.Where( e => e.Column == a.Column );
                if ( !string.IsNullOrEmpty( a.CreatorName ) ) audits = audits.Where( e => e.CreatorName == a.CreatorName );
                if ( !string.IsNullOrEmpty( a.Operate ) ) audits = audits.Where( e => e.Operate == a.Operate );
                //if ( filter.StartTime != null ) audits = audits.Where( e => e.CreateTime >= filter.StartTime );
                //if ( filter.EndTime != null ) audits = audits.Where( e => e.CreateTime <= filter.EndTime );
                //if ( filter.IsDesc ) audits = audits.OrderByDescending( e => e.Id );
            }

            // 默认按entityPK排序
            //var temp = audits.AsEnumerable().GroupBy( e =>new { e.EntityPk , e.Table} ).ToList();

            return await audits.ToPagedListAsync( filter.Page, filter.Size );
        }
    }
}
