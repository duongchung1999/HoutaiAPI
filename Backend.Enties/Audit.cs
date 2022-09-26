using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
	/// <summary>
	/// 审计（数据库操作）
	/// </summary>
	[Table("audit")]
	public class Audit : IEntity, IEntityTypeBuilder<Audit> {
		public int Id { get; set; }
		/// <summary>
		/// 表名
		/// </summary>
		public string Table { get; set; }
		/// <summary>
		/// 列名
		/// </summary>
		public string Column { get; set; }
		/// <summary>
		/// 新值
		/// </summary>
		[Column( "new_value" )]
		public string NewValue { get; set; }
		/// <summary>
		/// 旧值
		/// </summary>
		[Column( "old_value" )]
		public string? OldValue { get; set; }
		/// <summary>
		/// 操作时间
		/// </summary>
		[Column( "create_time" )]
		[DatabaseGenerated( DatabaseGeneratedOption.Computed )]
		public DateTime? CreateTime { get; set; }
		/// <summary>
		/// 操作者
		/// </summary>
		[Column( "creator_id" )]
		public int CreatorId { get; set; }
		/// <summary>
		/// 创建者昵称(nickname)
		/// </summary>
		[Column( "creator_name" )]
		public string CreatorName { get; set; }
		/// <summary>
		/// 操作方式
		/// </summary>
		public string Operate { get; set; }

		/// <summary>
		/// 操作的实体的主键
		/// </summary>
		[Column("entity_pk")]
		public int? EntityPk { get; set; }

		/// <summary>
		/// 数据库生成的属性集合，如自增的Id
		/// </summary>
		[NotMapped]
		public PropertyEntry TempProp;

		/// <summary>
		/// 实体的主键
		/// </summary>
		[NotMapped]
		public PropertyEntry EntityPkProp;

		/// <summary>
		/// 所跟踪的实体
		/// </summary>
		[NotMapped]
        public EntityEntry EntityEntry;

        public Audit( EntityEntry entityEntry ):base() {
            this.EntityEntry = entityEntry;
        }

		public Audit() {
			NewValue = "";
			OldValue = "";
		}

        public void Configure(EntityTypeBuilder<Audit> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey(e => e.Id);
		}
	}
}
