using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Db;

/// <summary>
/// 用户基础信息
/// </summary>
public class Users
{
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
    /// 用户密码
    /// </summary>
    /// <value></value>
    public string password { get; set; } = null!;
    /// <summary>
    /// 禁用
    /// </summary>
    /// <value></value>
    public bool disabled { get; set; }
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
    /// 用户手机号码
    /// </summary>
    /// <value></value>
    public string? phone { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value></value>
    public string? email { get; set; }
    /// <summary>
    /// 用户等级
    /// </summary>
    /// <value></value>
    public long vip { get; set; }
    /// <summary>
    /// sha公钥
    /// </summary>
    /// <value></value>
    public string public_key { get; set; } = null!;
    /// <summary>
    /// sha私钥
    /// </summary>
    /// <value></value>
    public string private_key { get; set; } = null!;
}