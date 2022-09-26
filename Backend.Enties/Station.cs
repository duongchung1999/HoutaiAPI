using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
	/// <summary>
	/// 站别
	/// </summary>
	[Table("station")]
	public class Station : IEntity, IEntityTypeBuilder<Station> {
		public int Id { get; set; }
		/// <summary>
		/// 所属机型Id
		/// </summary>
		[Column("model_id")]
		public int ModelId { get; set; }
		/// <summary>
		/// 站别名称
		/// </summary>
		//[MaxLength(100, ErrorMessage = "站别名称最大长度为100")]
		//[Required(ErrorMessage ="站别名称不能为空")]
		public string Name { get; set; }

		/// <summary>
		/// 站别配置（机型配置）
		/// </summary>
		public string Config { get; set; }

		/// <summary>
		/// 所拥有的测试项目
		/// </summary>
		public List<StationTestItem> StaionTestItemList;

		public void Configure(EntityTypeBuilder<Station> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey(e => e.Id);
		}
	}
}
