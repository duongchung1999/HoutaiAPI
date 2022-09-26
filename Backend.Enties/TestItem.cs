using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 测试项目
    /// </summary>
    [Table("testitem")]
    public class TestItem : IEntity, IEntityTypeBuilder<TestItem> {
        public TestItem() {
            Unit = "N/A";
            UpperValue = "N/A";
            LowerValue = "N/A";
        }

        public int Id { get; set; }
        /// <summary>
        /// 所属机型Id
        /// </summary>
        [Column( "model_id" )]
        public int ModelId { get; set; }
        /// <summary>
        /// 测试项目名称
        /// </summary>
        /// 
        [MaxLength( 100, ErrorMessage = "测试项目名称最大长度为100" )]
        [Required( ErrorMessage = "测试项目名称不能为空" )]
        public string Name { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength( 20, ErrorMessage = "单位最大长度为20" )]
        public string? Unit { get; set; }
        /// <summary>
        /// 调用命令（方法名）
        /// </summary>
        [MaxLength( 255, ErrorMessage = "调用命令最大长度为255" )]
        public string Cmd { get; set; }
        /// <summary>
        /// 测试最大值
        /// </summary>
        [Column( "upper_value" )]
        [MaxLength( 100, ErrorMessage = "测试最大值最大长度为100" )]
        public string UpperValue { get; set; }
        /// <summary>
        /// 测试最小值
        /// </summary>
        [Column( "lower_value" )]
        [MaxLength( 100, ErrorMessage = "测试最小值最大长度为100" )]
        public string? LowerValue { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 是否在模板程序中隐藏
        /// </summary>
        [Column("is_hidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// 是否在模板程序中始终会执行这一项（即使测试fail）
        /// </summary>
        [Column("is_always_run")]
        public bool IsAlwaysRun { get; set; }

        /// <summary>
        /// 测试顺序
        /// </summary>
        [NotMapped]
        public int? SortIndex { get; set; }

        public IEnumerable<StationTestItem> StationTestitems { get; set; }

        public void Configure( EntityTypeBuilder<TestItem> entityBuilder, DbContext dbContext, Type dbContextLocator ) {
            entityBuilder.HasKey( e => e.Id );
            entityBuilder
                .HasMany( t => t.StationTestitems )
                .WithOne( st => st.TestItem )
                .HasForeignKey( st => st.TestitemId );
        }
    }
}
