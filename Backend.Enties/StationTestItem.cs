using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
	/// <summary>
	/// 站别测试项目分配表
	/// </summary>
	[Table("station_testitem")]
	public class StationTestItem: IEntity, IEntityTypeBuilder<StationTestItem> {

		public int Id { get; set; }
		/// <summary>
		/// 所属站别Id
		/// </summary>
		[Column("station_id")]
		public int StationId { get; set; }
		/// <summary>
		/// 所属测试项目Id
		/// </summary>
		[Column("testitem_id")]
		public int TestitemId { get; set; }
		/// <summary>
		/// 测试顺序
		/// </summary>
		[Column("sort_index")]
		public int SortIndex { get; set; }

		/// <summary>
		/// 测试项目 从表
		/// </summary>
		public TestItem TestItem { get; set; }

		/// <summary>
		/// 所属站别
		/// </summary>
		public Station Station { get; set; }

		public void Configure(EntityTypeBuilder<StationTestItem> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey( st => st.Id );
            entityBuilder.HasOne(st => st.Station).WithMany(s => s.StaionTestItemList).HasForeignKey(st => st.StationId);
        }
	}
}
