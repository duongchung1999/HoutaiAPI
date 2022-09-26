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
	[Table("user_model")]
	public class UserModel : IEntity, IEntityTypeBuilder<UserModel> {
		public int Id { get; set; }
		
		[Column("user_id")]
		public int UserId { get; set; }


		[Column("model_id")]
		public int ModelId { get; set; }

        public void Configure(EntityTypeBuilder<UserModel> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey(e => e.Id);
		}
	}
}
