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
    /// 通用测试项目组
    /// </summary>
    [Table("public_test_item_group")]
    public class PublicTestItemGroup : IEntity, IEntityTypeBuilder<PublicTestItemGroup> {

        public int Id { get; set; }

        /// <summary>
        /// 概述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Dll名
        /// </summary>
        [Column("dll_name")]
        public string DllName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }


        public void Configure(EntityTypeBuilder<PublicTestItemGroup> entityBuilder, DbContext dbContext, Type dbContextLocator) {
           entityBuilder.HasKey(t => t.Id);
        }
    }
}
