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
    /// 通用测试项目参数
    /// </summary>
    [Table("public_test_item_param")]
    public class PublicTestItemParam : IEntity, IEntityTypeBuilder<PublicTestItemParam> {

        public int Id { get; set; }

        /// <summary>
        /// 概述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 选项
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 所属的方法Id
        /// </summary>
        [Column("method_id")]
        public int MethodId { get; set; }

        public void Configure(EntityTypeBuilder<PublicTestItemParam> entityBuilder, DbContext dbContext, Type dbContextLocator) {
           entityBuilder.HasKey(t => t.Id);
        }
    }
}
