using Backend.Enties;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Backend.Services {

    // TE测试服务器
    // [AppDbContext("Server=10.55.22.34;database=te-test;uid=root;pwd=merryte;AllowLoadLocalInfile=true;", DbProvider.MySql)]

#if DEBUG
    // 本机服务器

    [AppDbContext("Server=localhost;database=te_test;uid=root;pwd=root;AllowLoadLocalInfile=true;", DbProvider.MySql)]
#else
    // MES服务器
     [AppDbContext("Server=10.55.2.20;database=te_test;uid=merryte;pwd=merry@TE;AllowLoadLocalInfile=true;", DbProvider.MySql)]
    
    // 越南数据库地址 
    //[AppDbContext("Server=10.175.5.59;database=te_test;uid=merryte;pwd=merry@TE;AllowLoadLocalInfile=true;", DbProvider.MySql)]

#endif
    public class MyDbContext : AppDbContext<MyDbContext> {  

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {
        }

        public override int SaveChanges() {
            //var audits = OnBeforeSave();
            var result = base.SaveChanges();
            //OnAfterSave(audits).Wait();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            //var audits = OnBeforeSave();
            var result = await base.SaveChangesAsync(cancellationToken);
            //await OnAfterSave(audits);
            return result;
        }

        List<Audit> OnBeforeSave() {
            ChangeTracker.DetectChanges();
            var audits = new List<Audit>();
            var nowUser = JwtHandler.GetNowUser();

            var entities = ChangeTracker.Entries()
              .Where(u => u.Entity.GetType() != typeof(Audit) && (u.State == EntityState.Modified || u.State == EntityState.Deleted || u.State == EntityState.Added))
              .ToList();

            foreach (var entityEntry in entities) {
                var tempList = CreateAuditEntites(entityEntry, nowUser);
                audits = audits.Concat(tempList).ToList();
            }

            return audits;
        }

        /// <summary>
        /// 创建一个实体的多个审计操数据 (每条属性)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        static List<Audit> CreateAuditEntites(EntityEntry entity, User creator) {
            var audits = new List<Audit>();
            // 获取所有实体有效属性，排除 [NotMapper] 属性
            var props = entity.Properties;

            var databaseValues = entity.GetDatabaseValues();

            var entityPkProp = entity.Properties.Where(p => p.Metadata.IsPrimaryKey()).SingleOrDefault();

            foreach (var prop in props) {
                if (prop.Metadata.Name.Equals("CreateTime")) continue;
                var audit = new Audit {
                    Operate = entity.State.ToString(),
                    Table = entity.Metadata.GetTableName(),
                    Column = prop.Metadata.Name,
                    CreatorId = creator.Id,
                    CreatorName = creator.Nickname,
                    EntityEntry = entity,
                    EntityPkProp = entityPkProp
                };

                // 获取数据库生成的属性 之后从数据库中获取
                if (prop.IsTemporary) {
                    audit.TempProp = prop;
                    audits.Add(audit);
                    continue;
                }

                // 获取主键
                if (prop.Metadata.IsPrimaryKey()) {
                    audit.EntityPk = Convert.ToInt32(prop.CurrentValue);
                }

                switch (entity.State) {
                    case EntityState.Added:
                        audit.NewValue = prop.CurrentValue?.ToString();
                        break;

                    case EntityState.Deleted:
                        audit.OldValue = prop.OriginalValue?.ToString();
                        break;

                    case EntityState.Modified:
                        if (prop.IsModified) {
                            if (prop.Metadata.IsPrimaryKey()) continue;
                            audit.NewValue = prop.CurrentValue == null ? "" : prop.CurrentValue.ToString();
                            var oldValue = databaseValues[prop.Metadata.Name]?.ToString();
                            audit.OldValue = oldValue?? "";
                            if (audit.NewValue.Equals(audit.OldValue)) continue;
                        } else {
                            continue;
                        }
                        break;
                }

                audits.Add(audit);
            }

            return audits;
        }

        static async Task OnAfterSave(List<Audit> audits) {
            foreach (var audit in audits) {
                // 从数据库中获取生产的属性值 
                if (audit.TempProp != null) {
                    audit.NewValue = audit.TempProp.CurrentValue.ToString();
                }

                if (audit.EntityPkProp != null) {
                    audit.EntityPk = Convert.ToInt32(audit.EntityPkProp.CurrentValue);
                }

                await AuditService.AddAuditAsync(audit);
            }
        }

    }
}
