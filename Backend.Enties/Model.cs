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
    [Table("model")]
    public class Model : IEntity, IEntityTypeBuilder<Model> {

        public int Id { get; set; }
        /// <summary>
        /// 机型名称
        /// </summary>
        /// 
        [MaxLength(30, ErrorMessage = "机型名称最大长度为30")]
        [Required(ErrorMessage = "机型名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Column("creator_id")]
        public int CreatorId { get; set; }

        /// <summary>
        /// 料号配置表单的模板
        /// </summary>
        [Column("pn_config_template")]
        public string PnConfigTemplate { get; set; }

        /// <summary>
		/// Config Model
		/// </summary>
		public string Config { get; set; }

        /// <summary>
        /// 动态码Id
        /// </summary>
        //[Column("dynamic_code_id")]
        //public int DynamicCodeId { get; set; }

        [NotMapped]
        public DynamicCode DynamicCode { get; set; }

        public void Configure(EntityTypeBuilder<Model> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
        }
    }
}
