using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Com.Api.Sdk.Enum;
using Com.Api.Sdk.Models;

namespace Com.Db;

/// <summary>
/// 订单模型
/// 注:此表数据量超大,请使用数据库表分区功能
/// </summary>
public class OrderSell : Orders
{

}