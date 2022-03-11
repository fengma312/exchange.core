using System;
using Com.Model.Enum;

namespace Com.Model;

/// <summary>
/// 成交单
/// </summary>
public class MatchDeal : BaseDeal
{
    /// <summary>
    /// 买订单
    /// </summary>
    /// <value></value>
    public BaseOrder bid { get; set; } = null!;
    /// <summary>
    /// 卖订单
    /// </summary>
    /// <value></value>
    public BaseOrder ask { get; set; } = null!;
}