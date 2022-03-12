using System;
using Com.Db.Enum;

namespace Com.Db.Model;

/// <summary>
/// 聚合行情
/// </summary>
public class Ticker
{
    /// <summary>
    /// 交易对
    /// </summary>
    /// <value></value>
    public long market { get; set; }
    /// <summary>
    /// 交易对名称
    /// </summary>
    /// <value></value>
    public string symbol { get; set; } = null!;
    /// <summary>
    /// 24小时价格变化
    /// </summary>
    /// <value></value>
    public decimal price_change { get; set; }
    /// <summary>
    /// 24小时价格变化百分比
    /// </summary>
    /// <value></value>
    public decimal price_change_percent { get; set; }
    /// <summary>
    /// 最新成交价
    /// </summary>
    /// <value></value>
    public decimal last_price { get; set; }
    /// <summary>
    /// 最新成交量
    /// </summary>
    /// <value></value>
    public decimal last_amount { get; set; }
    /// <summary>
    /// 卖1价
    /// </summary>
    /// <value></value>
    public decimal ask1_price { get; set; }
    /// <summary>
    /// 买1量
    /// </summary>
    /// <value></value>
    public decimal ask1_volume { get; set; }
    /// <summary>
    /// 买1价
    /// </summary>
    /// <value></value>
    public decimal bid1_price { get; set; }
    /// <summary>
    /// 买1量
    /// </summary>
    /// <value></value>
    public decimal bid1_volume { get; set; }
    /// <summary>
    /// 24小时内开盘价
    /// </summary>
    /// <value></value>
    public decimal open { get; set; }
    /// <summary>
    /// 24小时内最高价
    /// </summary>
    /// <value></value>
    public decimal high { get; set; }
    /// <summary>
    /// 24小时内最低价
    /// </summary>
    /// <value></value>
    public decimal low { get; set; }
    /// <summary>
    /// 24小时内交易量
    /// </summary>
    /// <value></value>
    public decimal volume { get; set; }
    /// <summary>
    /// 24小时内交易额
    /// </summary>
    /// <value></value>
    public decimal volume_currency { get; set; }
    /// <summary>
    /// 24小时内交易笔数
    /// </summary>
    /// <value></value>
    public int count { get; set; }
    /// <summary>
    /// 24小时内第一笔成交时间
    /// </summary>
    /// <value></value>
    public DateTimeOffset open_time { get; set; }
    /// <summary>
    /// 24小时内最后一笔成交时间
    /// </summary>
    /// <value></value>
    public DateTimeOffset close_time { get; set; }
    /// <summary>
    /// 记录时间
    /// </summary>
    /// <value></value>
    public DateTimeOffset time { get; set; }

}