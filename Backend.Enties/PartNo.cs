using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 机型
    /// </summary>
    [Table("part_no")]
    public class PartNo : IEntity, IEntityTypeBuilder<PartNo> {

        public int Id { get; set; }
       
        /// <summary>
        /// 料号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 对应的配置
        /// </summary>
        //public PartNoConfig PartNoConfig { get; set; }

        /// <summary>
        /// 对应的配置的Id
        /// </summary>
        [Column("part_no_config_id")]
        public int PartNoConfigId { get; set; }

        /// <summary>
        /// 对应的机型的Id
        /// </summary>
        [Column("model_id")]
        public int ModelId { get; set; }

        public void Configure(EntityTypeBuilder<PartNo> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
        }
    }
}
