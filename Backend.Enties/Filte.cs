using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Enties {
    public class Filte {
        public Filte() {
            Page = 1;
            Size = 20;
        }

        /// <summary>
        /// 页数
        /// </summary>
        /// 
        public int Page { get; set; }
        /// <summary>
        /// 每页查询数量默认20条
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 当前分页偏移的位置 ( page - 1 ) * size
        /// </summary>
        [NotMapped]
        public int PageOffset { get { return (Page - 1) * Size; } }

        /// <summary>
        /// 是否倒叙查询（从末尾开始查找）
        /// </summary>
        //public bool IsDesc { get; set; }
        /// <summary>
        /// 按某属性排序
        /// </summary>
        //public string? OrderBy { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        //public DateTime ?StartTime { get; set; }
        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //public DateTime ?EndTime { get; set; }
    }
}
