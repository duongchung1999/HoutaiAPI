using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 后台配置（一些杂项的配置在此设置）
    /// </summary>
    [Table("backstage_config")]
    public class BackstageConfig : IEntity, IEntityTypeBuilder<BackstageConfig> {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Label { get; set; }

        public void Configure(EntityTypeBuilder<BackstageConfig> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
        }
    }
}
