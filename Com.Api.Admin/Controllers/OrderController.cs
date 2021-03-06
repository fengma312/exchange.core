using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Com.Api.Sdk.Enum;
using Com.Api.Sdk.Models;
using Com.Bll;
using Com.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Api.Admin.Controllers;

/// <summary>
/// 订单接口
/// </summary>
[Route("[controller]")]
// [Authorize]
[ApiController]
public class OrderController : ControllerBase
{
    /// <summary>
    /// 日志接口
    /// </summary>
    private readonly ILogger<OrderController> logger;
    /// <summary>
    /// 登录信息
    /// </summary>
    private (long no, long user_id, string user_name, E_App app, string public_key) login
    {
        get
        {
            return this.service_user.GetLoginUser(User);
        }
    }
    /// <summary>
    /// service:公共服务
    /// </summary>
    private ServiceCommon service_common = new ServiceCommon();
    /// <summary>
    /// Service:用户服务
    /// </summary>
    /// <returns></returns>
    private ServiceUser service_user = new ServiceUser();
    /// <summary>
    /// Service:订单
    /// </summary>
    private ServiceOrder service_order = new ServiceOrder();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="logger">日志接口</param>
    public OrderController(ILogger<OrderController> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 订单查询
    /// </summary>
    /// <param name="symbol">交易对</param>
    /// <param name="user_name">用户名</param>
    /// <param name="state">订单状态</param>
    /// <param name="order_id">订单id</param>
    /// <param name="start">开始时间</param>
    /// <param name="end">结束时间</param>
    /// <param name="skip">跳过多少行</param>
    /// <param name="take">提取多少行</param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetOrder")]
    [ResponseCache(CacheProfileName = "cache_1")]
    public Res<List<Orders>> GetOrder(string symbol, string? user_name = null, E_OrderState? state = null, long? order_id = null, DateTimeOffset? start = null, DateTimeOffset? end = null, int skip = 0, int take = 50)
    {
        return this.service_order.GetOrder(symbol: symbol, user_name: user_name, state: state, order_id: order_id, start: start, end: end, skip: skip, take: take);
    }


    /// <summary>
    /// 按交易对全部撤单
    /// </summary>
    /// <param name="symbol">交易对</param>
    /// <returns></returns>
    [HttpPost]
    [Route("OrderCancelBySymbol")]
    public Res<bool> OrderCancelBySymbol(string symbol)
    {
        Res<bool> result = new Res<bool>();
        return this.service_order.CancelOrder(symbol, 0, 1, new List<long>());
    }

}