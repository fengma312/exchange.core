using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Db;

/// <summary>
/// 交易对基础信息
/// </summary>
public class MarketInfo
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
    /// 基础币种id
    /// </summary>
    /// <value></value>
    public long coin_id_base { get; set; }
    /// <summary>
    /// 基础币种名
    /// </summary>
    /// <value></value>
    public string coin_name_base { get; set; } = null!;
    /// <summary>
    /// 报价币种id
    /// </summary>
    /// <value></value>
    public long coin_id_quote { get; set; }
    /// <summary>
    /// 报价币种名
    /// </summary>
    /// <value></value>
    public string coin_name_quote { get; set; } = null!;
    /// <summary>
    /// 分隔符
    /// </summary>
    /// <value></value>
    public string separator { get; set; } = "/";
    /// <summary>
    /// 价格小数位数
    /// </summary>
    /// <value></value>
    public decimal price_places { get; set; }
    /// <summary>
    /// 量小数位数
    /// </summary>
    /// <value></value>
    public decimal amount_places { get; set; }
    /// <summary>
    /// 交易量整数倍数
    /// </summary>
    /// <value></value>
    public int amount_multiple { get; set; }
    /// <summary>
    /// 市价买手续费
    /// </summary>
    /// <value></value>
    public decimal rate_market_buy { get; set; }
    /// <summary>
    /// 市价卖手续费
    /// </summary>
    /// <value></value>
    public decimal rate_market_sell { get; set; }
    /// <summary>
    /// 限价买手续费
    /// </summary>
    /// <value></value>
    public decimal fee_fixed_buy { get; set; }
    /// <summary>
    /// 限价卖手续费
    /// </summary>
    /// <value></value>
    public decimal fee_fixed_sell { get; set; }   
    /// <summary>
    /// 作市账号
    /// </summary>
    /// <value></value>
    public long market_uid { get; set; }
    /// <summary>
    /// 最后的成交价
    /// </summary>
    /// <value></value>
    [NotMapped]
    public decimal last_price { get; set; }
}