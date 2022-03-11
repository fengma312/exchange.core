using System;
using Com.Db.Enum;

namespace Com.Db.Model;

/// <summary>
/// 响应操作动作
/// </summary>
public class Res<T> 
{
    /// <summary>
    /// 是否成功
    /// </summary>
    /// <value></value>
    public bool success { get; set; }
    /// <summary>
    /// 返回编号
    /// </summary>
    /// <value></value>
    public E_Res_Code code { get; set; }  
    /// <summary>
    /// 响应消息
    /// </summary>
    /// <value></value>
    public string? message { get; set; } = null;
    /// <summary>
    /// 数据
    /// </summary>
    /// <value></value>
    public T data { get; set; } = default!;
}