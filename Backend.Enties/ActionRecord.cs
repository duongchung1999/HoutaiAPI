using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
	/// <summary>
	/// 操作记录
	/// </summary>
	[Table("action_record")]
	public class ActionRecord : IEntity, IEntityTypeBuilder<ActionRecord> {

		/// <summary>
		/// 主键Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 操作名称
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// 附带信息
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 操作者昵称
		/// </summary>
		public string Operator { get; set; }

		/// <summary>
		/// 操作日期
		/// </summary>
		[Column("create_date")]
		public DateTime CreateDate { get; set; }


        public void Configure(EntityTypeBuilder<ActionRecord> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey(e => e.Id);
		}

		/// <summary>
		/// 动作选项
		/// </summary>
		public static class ActionOptions {
			public const string DELETE_MODEL = "删除机型";
			public const string ADD_MODEL = "添加机型";
			public const string UPDATE_MODEL = "更新机型";
			public const string UPDATE_MODEL_NAME = "更新机型名称";

			public const string DELETE_STATION = "删除站别";
			public const string ADD_STATION = "添加站别";
			public const string UPDATE_STATION = "更新站别";
			public const string REALLOCATE_TEST_ITEM_2_STATION = "给站别分配测项目";

			public const string DELETE_TEST_ITEM = "删除测试项目";
			public const string ADD_TEST_ITEM = "添加测试项目";
			public const string UPDATE_TEST_ITEM = "更新测试项目";

			public const string DELETE_PN_CONFG_TEMPLATE = "删除料号配置模板";
			public const string ADD_PN_CONFG_TEMPLATE = "添加料号配置模板";
			public const string UPDATE_PN_CONFG_TEMPLATE = "更新料号配置模板";

			public const string DELETE_PN_CONFG = "删除料号配置";
			public const string ADD_PN_CONFG = "添加料号配置";
			public const string UPDATE_PN_CONFG = "更新料号配置";
		}
	}
}
