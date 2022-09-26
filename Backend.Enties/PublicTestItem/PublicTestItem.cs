using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Enties.PublicTestItem {

    /// <summary>
    /// 通用测试项目
    /// </summary>
    [Table("public_test_item")]
    public class PublicTestItem : IEntity, IEntityTypeBuilder<PublicTestItem> {

        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 概述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        [Column("method_name")]
        public string MethodName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 所属的通用测试项目组Id
        /// </summary>
        [Column("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public string Returns { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        [NotMapped]
        public List<PublicTestItemParam> Params { get; set; }

        public void Configure(EntityTypeBuilder<PublicTestItem> entityBuilder, DbContext dbContext, Type dbContextLocator) {
           entityBuilder.HasKey(t => t.Id);
        }
    }
}
