using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Db;

/// <summary>
/// Api用户
/// </summary>
public class UsersApi
{
    /// <summary>
    /// id
    /// </summary>
    /// <value></value>
    public long id { get; set; }
    /// <summary>
    /// 用户id
    /// </summary>
    /// <value></value>
    public long user_id { get; set; }
    /// <summary>
    /// 账户key
    /// </summary>
    /// <value></value>
    public string api_key { get; set; } = null!;
    /// <summary>
    /// 账户密钥
    /// </summary>
    /// <value></value>
    public string api_secret { get; set; } = null!;
    /// <summary>
    /// 是否交易
    /// </summary>
    /// <value></value>
    public bool transaction { get; set; }
    /// <summary>
    /// 是否提现
    /// </summary>
    /// <value></value>
    public bool withdrawal { get; set; }
    /// <summary>
    /// IP白名单
    /// </summary>
    /// <value></value>
    public string? white_list_ip { get; set; }
    /// <summary>
    /// 最后登录IP地址
    /// </summary>
    /// <value></value>
    public string? last_login_ip { get; set; }
}