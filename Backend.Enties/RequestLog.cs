using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
	/// <summary>
	/// 审计（数据库操作）
	/// </summary>
	[Table("request_log")]
	public class RequestLog : IEntity, IEntityTypeBuilder<RequestLog> {
		public int Id { get; set; }


        public string Method { get; set; }


        [Column("request_url")]
        public string RequestUrl { get; set; }

        [Column("refer_url")]
        public string ReferUrl { get; set; }

        public string Ip { get; set; }

        public string Parameters { get; set; }

        public string Result { get; set; }


        [Column("request_user_id")]
        public int RequestUserId { get; set; }


        [Column("request_user_nickname")]
        public string RequestUserNickanme { get; set; }


        [Column("is_succeed")]
        public bool IsSucceed { get; set; }

        [Column("request_date")]
        public DateTime RequestDate { get; set; }

        public string Path { get; set; }


        public void Configure(EntityTypeBuilder<RequestLog> entityBuilder, DbContext dbContext, Type dbContextLocator) {
			entityBuilder.HasKey(e => e.Id);
		}
	}
}
