using System;
using System.ComponentModel.DataAnnotations.Schema;
using Com.Api.Sdk;
using Com.Api.Sdk.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Com.Db;

/// <summary>
/// 计账钱包基础信息
/// </summary>
public class Wallet
{
    /// <summary>
    /// 钱包id
    /// </summary>
    /// <value></value>
    public long wallet_id { get; set; }
    /// <summary>
    /// 钱包类型
    /// </summary>
    /// <value></value>
    [JsonConverter(typeof(StringEnumConverter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public E_WalletType wallet_type { get; set; }
    /// <summary>
    /// 用户id
    /// </summary>
    /// <value></value>
    public long user_id { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    /// <value></value>
    public string user_name { get; set; } = null!;
    /// <summary>
    /// 币id
    /// </summary>
    /// <value></value>
    public long coin_id { get; set; }
    /// <summary>
    /// 币名称
    /// </summary>
    /// <value></value>
    public string coin_name { get; set; } = null!;
    /// <summary>
    /// 总额
    /// </summary>
    /// <value></value>
    [JsonConverter(typeof(JsonConverterDecimal))]
    public decimal total { get; set; }
    /// <summary>
    /// 可用
    /// </summary>
    /// <value></value>
    [JsonConverter(typeof(JsonConverterDecimal))]
    public decimal available { get; set; }
    /// <summary>
    /// 冻结
    /// </summary>
    /// <value></value>
    [JsonConverter(typeof(JsonConverterDecimal))]
    public decimal freeze { get; set; }
}