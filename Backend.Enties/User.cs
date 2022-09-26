using Furion.DatabaseAccessor;
using Furion.DataValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    /// <summary>
    /// 用户角色选项
    /// </summary>
    [Flags]
    public enum UserRoleOptions {
        NONE = 1 << 0,
        NORMAL = 1 << 2,
        SW = 1 << 3,
        ACCOUNT_MANAGER = 1 << 4,
        ADMIN = 1 << 5,
    }

    /// <summary>
    /// 用户
    /// </summary>
    [Table("user")]
    public class User : IEntity, IEntityTypeBuilder<User> {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        //[MaxLength(20, ErrorMessage = "昵称最大长度为20")]
        //[Required(ErrorMessage = "昵称不能为空")]
        public string Nickname { get; set; }
        /// <summary>
        /// 用户名（账号）
        /// </summary>
        //[MaxLength(20, ErrorMessage = "用户名（账号）最大长度为20")]
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        //[Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        /// <summary>
        /// 权限角色
        /// </summary>
        [Column("user_role")]
        public UserRoleOptions Role { get; set; }

        /// <summary>
        /// 机型权限列表
        /// </summary>
        //public List<ModelPermission> ModelPermissions { get; set; }

        public void Configure(EntityTypeBuilder<User> entityBuilder, DbContext dbContext, Type dbContextLocator) {
            entityBuilder.HasKey(e => e.Id);
            //entityBuilder.HasMany(e => e.ModelPermissions).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            entityBuilder.Property(e => e.Role)
                .HasConversion(v => (int)v, v =>  (UserRoleOptions)v);
        }
    }
}
