using System.Text;
using Com.Db;
using Com.Db.Enum;
using Com.Db.Model;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Com.Bll;

/// <summary>
/// 服务工厂
/// </summary>
public class FactoryService
{
    /// <summary>
    /// 单例类的实例
    /// </summary>
    /// <returns></returns>
    public static readonly FactoryService instance = new FactoryService();
    /// <summary>
    /// 常用接口
    /// </summary>
    public FactoryConstant constant = null!;
    // /// <summary>
    // /// DB:交易记录
    // /// </summary>
    // public DealDb deal_db = new DealDb();
    // /// <summary>
    // /// DB:K线
    // /// </summary>
    // public KilneDb kilne_db = new KilneDb();
    // /// <summary>
    // /// 交易对基础信息
    // /// </summary>
    // /// <returns></returns>
    // public MarketInfoDb market_info_db = new MarketInfoDb();
    // /// <summary>
    // /// Service:订单
    // /// </summary>
    // public OrderService order_service = new OrderService();
    // /// <summary>
    // /// Service:交易记录
    // /// </summary>
    // public DealService deal_service = new DealService();
    // /// <summary>
    // /// Service:K线
    // /// </summary>
    // public KlineService kline_service = new KlineService();
    /// <summary>
    /// 系统初始化时间  初始化  注:2017-1-1 此时是一年第一天，一年第一月，一年第一个星期日(星期日是一个星期开始的第一天)
    /// </summary>   
    public DateTimeOffset system_init = new DateTimeOffset(2017, 1, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// private构造方法
    /// </summary>
    private FactoryService()
    {

    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <param name="constant"></param>
    public void Init(FactoryConstant constant)
    {
        this.constant = constant;

    }

    /// <summary>
    /// redis(zset)键 已生成交易记录 交易时间=>deal:btc/usdt 
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetRedisDeal(long market)
    {
        return string.Format("deal:{0}", market);
    }

    /// <summary>
    /// redis hast 深度行情 depth:{market}
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetRedisDepth(long market)
    {
        return string.Format("depth:{0}", market);
    }

    /// <summary>
    /// redis hast 深度行情 depth
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetRedisTicker()
    {
        return string.Format("ticker");
    }

    /// <summary>
    /// redis(zset)键 已生成K线 K线开始时间=>kline:btc/usdt:main1
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetRedisKline(long market, E_KlineType type)
    {
        return string.Format("kline:{0}:{1}", market, type);
    }

    /// <summary>
    /// redis(hash)键 正在生成K线 K线类型=>klineing:btc/usdt
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetRedisKlineing(long market)
    {
        return string.Format("klineing:{0}", market);
    }

    /// <summary>
    /// MQ:发送历史成交记录
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetMqOrderDeal(long market)
    {
        return string.Format("deal_{0}", market);
    }

    /// <summary>
    /// MQ:挂单队列
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetMqOrderPlace(long market)
    {
        return string.Format("order_place_{0}", market);
    }

    /// <summary>
    /// MQ:撤单队列
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public string GetMqOrderCancel(long market)
    {
        return string.Format("order_cancel_{0}", market);
    }  

    /// <summary>
    /// MQ:订阅
    /// </summary>
    /// <param name="channel">订阅频道</param>
    /// <param name="market">交易对</param>
    /// <returns></returns>
    public string GetMqSubscribe(E_WebsockerChannel channel, long market)
    {
        return string.Format("{0}_{1}", channel, market);
    }

    /// <summary>
    /// MQ:订阅(有权限)
    /// </summary>
    /// <param name="channel">订阅频道</param>
    /// <param name="market">交易对</param>
    /// <param name="uid">用户</param>
    /// <returns></returns>
    public string GetMqSubscribe(E_WebsockerChannel channel, long market, long uid)
    {
        return string.Format("{0}_{1}_{2}", channel, market, uid);
    }



}