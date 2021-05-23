using System;

namespace Com.Model.Base
{
    /// <summary>
    /// 盘口
    /// </summary>
    public struct OrderBook
    {
        /// <summary>
        /// 序列,从1档口开始
        /// </summary>
        /// <value></value>
        public int no { get; set; }
        /// <summary>
        /// 交易对
        /// </summary>
        /// <value></value>
        public string name { get; set; }
        /// <summary>
        /// 挂单价
        /// </summary>
        /// <value></value>
        public decimal price { get; set; }
        /// <summary>
        /// 挂单总量
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 挂单总额
        /// </summary>
        /// <value></value>
        public decimal total
        {
            get
            {
                return price * amount;
            }
        }
        /// <summary>
        /// 挂单笔数量
        /// </summary>
        /// <value></value>
        public decimal count { get; set; }
        /// <summary>
        /// 变更时间
        /// </summary>
        /// <value></value>
        public DateTimeOffset last_time { get; set; }
        /// <summary>
        /// 交易方向
        /// </summary>
        /// <value></value>
        public E_Direction direction { get; set; }
    }
}
