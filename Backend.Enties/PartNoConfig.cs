using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 机型
    /// </summary>
    [Table("part_no_config")]
    public class PartNoConfig : IEntity, IEntityTypeBuilder<PartNoConfig> {

        public int Id { get; set; }

        /// <summary>
        /// 对应的配置
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// 配置的标题
        /// </summary>
        public string Title { get; set; }

        [Column("model_id")]
        public int ModelId { get; set; }

        public void Configure(EntityTypeBuilder<PartNoConfig> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
            //entityBuilder.Property(
            //    e => e.PartNoIdList).HasConversion(
            //    e => JsonConvert.SerializeObject(e), 
            //    e => JsonConvert.DeserializeObject<List<int>>(e)
            //);
        }
    }
}
