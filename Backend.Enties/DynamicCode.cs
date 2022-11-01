using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 动态码
    /// </summary>
    [Table("dynamic_code")]
    public class DynamicCode : IEntity, IEntityTypeBuilder<DynamicCode> {

        public int Id { get; set; }

        /// <summary>
        /// 动态码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>

        [Column("expire_date")]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// 所属机型Id
        /// </summary>
        [Column("model_id")]
        public int ModelId { get; set; }
        
        [Column("model_name")]
        public string ModelName { get; set; }

        public void Configure(EntityTypeBuilder<DynamicCode> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
        }
    }
}
