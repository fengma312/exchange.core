using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Com.Db.Enum;

namespace Com.Db;

/// <summary>
/// 成交单
/// </summary>
public class Deal
{
    /// <summary>
    /// 成交id
    /// </summary>
    /// <value></value>
    public long trade_id { get; set; }
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
    /// 成交价
    /// </summary>
    /// <value></value>
    public decimal price { get; set; }
    /// <summary>
    /// 成交量
    /// </summary>
    /// <value></value>
    public decimal amount { get; set; }
    /// <summary>
    /// 成交总额
    /// </summary>
    /// <value></value>
    public decimal total { get; set; }
    /// <summary>
    /// 成交触发方向
    /// </summary>
    /// <value></value>
    public E_OrderSide trigger_side { get; set; }
    /// <summary>
    /// 买单id
    /// </summary>
    /// <value></value>
    public long bid_id { get; set; }
    /// <summary>
    /// 买单用户id
    /// </summary>
    /// <value></value>
    public long bid_uid { get; set; }
    /// <summary>
    /// 买单挂单量
    /// </summary>
    /// <value></value>
    public decimal bid_amount { get; set; }
    /// <summary>
    /// 买单未成交量
    /// </summary>
    /// <value></value>
    public decimal bid_amount_unsold { get; set; }
    /// <summary>
    /// 买单已成交量
    /// </summary>
    /// <value></value>
    public decimal bid_amount_done { get; set; }
    /// <summary>
    /// 买单订单状态
    /// </summary>
    /// <value></value>
    public E_OrderState bid_state { get; set; }
    /// <summary>
    /// 卖单id
    /// </summary>
    /// <value></value>
    public long ask_id { get; set; }
    /// <summary>
    /// 卖单用户id
    /// </summary>
    /// <value></value>
    public long ask_uid { get; set; }
    /// <summary>
    /// 卖单挂单量
    /// </summary>
    /// <value></value>
    public decimal ask_amount { get; set; }
    /// <summary>
    /// 卖单未成交量
    /// </summary>
    /// <value></value>
    public decimal ask_amount_unsold { get; set; }
    /// <summary>
    /// 卖单已成交量
    /// </summary>
    /// <value></value>
    public decimal ask_amount_done { get; set; }
    /// <summary>
    /// 卖单订单状态
    /// </summary>
    /// <value></value>
    public E_OrderState ask_state { get; set; }
    /// <summary>
    /// 成交时间
    /// </summary>
    /// <value></value>
    public DateTimeOffset time { get; set; }
    // /// <summary>
    // /// 买订单
    // /// </summary>
    // /// <value></value>
    // [NotMapped]
    // [JsonIgnore]
    // public Orders bid { get; set; } = null!;
    // /// <summary>
    // /// 卖订单
    // /// </summary>
    // /// <value></value>
    // [NotMapped]
    // [JsonIgnore]
    // public Orders ask { get; set; } = null!;
}