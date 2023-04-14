using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    
    /// <summary>
    /// 权限角色选项
    /// </summary>
    public enum PermissionRoleOptions {
        /// <summary>
        /// 管理员
        /// </summary>
        ADMIN = 8,
        /// <summary>
        /// 账号管理员
        /// </summary>
        ACCOUNT_MANAGER = 7,
        /// <summary>
        /// 软体
        /// </summary>
        SW = 5,
        /// <summary>
        /// EE、PE、HW
        /// </summary>
        TE = 4,
        /// <summary>
        /// 模板程序开发者
        /// </summary>
        TEMPLATE_PROGRAM_DEVELOPER = 2,
        /// <summary>
        /// 基础权限
        /// </summary>
        BASC = 1,
    }
}
    
    
    /// <summary>
    /// 权限角色
    /// </summary>
    [Table("role")]
    public class Role : IEntity, IEntityTypeBuilder<Role> {

        public int Id { get; set; }
       
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限等级
        /// </summary>
        public int Level { get; set; }

        public void Configure(EntityTypeBuilder<Role> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
        }
    
}
