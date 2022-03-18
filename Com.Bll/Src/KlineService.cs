using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Com.Bll;
using Com.Db;
using Com.Db.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using StackExchange;
using StackExchange.Redis;

namespace Com.Bll;

/// <summary>
/// Service:K线
/// </summary>
public class KlineService
{

    /// <summary>
    /// 数据库
    /// </summary>
    public DbContextEF db = null!;
    /// <summary>
    /// DB:交易记录
    /// </summary>
    public DealDb deal_db = new DealDb();
    /// <summary>
    /// DB:K线
    /// </summary>
    public KilneDb kilne_db = new KilneDb();

    /// <summary>
    /// 初始化
    /// </summary>
    public KlineService()
    {
        var scope = FactoryService.instance.constant.provider.CreateScope();
        this.db = scope.ServiceProvider.GetService<DbContextEF>()!;
    }

    #region 已确定K线

    /// <summary>
    /// 缓存预热(已确定K线)
    /// </summary>
    /// <param name="markets">交易对</param>
    /// <param name="end">结束时间</param>
    public void DBtoRedised(long market, string symbol, DateTimeOffset end)
    {
        SyncKlines(market, symbol, end);
        DbSaveRedis(market);
    }

    /// <summary>
    /// 将K线保存到Db中
    /// </summary>
    /// <param name="market">交易对</param>
    /// <param name="end">结束时间</param>
    public void SyncKlines(long market, string symbol, DateTimeOffset end)
    {
        foreach (E_KlineType cycle in System.Enum.GetValues(typeof(E_KlineType)))
        {
            Kline? last_kline = this.kilne_db.GetLastKline(market, cycle);
            List<Kline>? klines = this.kilne_db.CalcKlines(market, cycle, last_kline?.time_end ?? FactoryService.instance.system_init, end);
            if (klines != null)
            {
                int count = this.kilne_db.SaveKline(market, symbol, cycle, klines);
            }
        }
    }

    /// <summary>
    /// 将DB中的K线数据保存到Redis
    /// </summary>
    /// <param name="market">交易对</param>
    public void DbSaveRedis(long market)
    {
        foreach (E_KlineType cycle in System.Enum.GetValues(typeof(E_KlineType)))
        {
            Kline? Last_kline = GetRedisLastKline(market, cycle);
            List<Kline> klines = this.kilne_db.GetKlines(market, cycle, Last_kline?.time_end ?? FactoryService.instance.system_init, DateTimeOffset.Now);
            if (klines.Count() > 0)
            {
                SortedSetEntry[] entries = new SortedSetEntry[klines.Count()];
                for (int i = 0; i < klines.Count(); i++)
                {
                    entries[i] = new SortedSetEntry(JsonConvert.SerializeObject(klines[i]), klines[i].time_start.ToUnixTimeMilliseconds());
                }
                FactoryService.instance.constant.redis.SortedSetAdd(FactoryService.instance.GetRedisKline(market, cycle), entries);
            }
        }
    }

    /// <summary>
    /// 从redis获取最大的K线时间
    /// </summary>
    /// <param name="market">交易对</param>
    /// <param name="klineType">K线类型</param>
    /// <returns></returns>
    public Kline? GetRedisLastKline(long market, E_KlineType klineType)
    {
        RedisValue[] redisvalue = FactoryService.instance.constant.redis.SortedSetRangeByRank(FactoryService.instance.GetRedisKline(market, klineType), 0, 1, StackExchange.Redis.Order.Descending);
        if (redisvalue.Length > 0)
        {
            return JsonConvert.DeserializeObject<Kline>(redisvalue[0]);
        }
        return null;
    }

    #endregion


    #region 未确定K线

    /// <summary>
    /// 缓存预热(未确定K线)
    /// </summary>
    /// <param name="market">交易对</param>
    public void DBtoRedising(long market, string symbol)
    {
        foreach (E_KlineType cycle in System.Enum.GetValues(typeof(E_KlineType)))
        {
            DateTimeOffset start = FactoryService.instance.system_init;
            Kline? kline_last = this.kilne_db.GetLastKline(market, cycle);
            if (kline_last != null)
            {
                start = kline_last.time_end.AddMilliseconds(1);
            }
            Kline? kline_new = this.deal_db.GetKlinesByDeal(market, symbol, cycle, start, null);
            if (kline_new != null)
            {
                FactoryService.instance.constant.redis.HashSet(FactoryService.instance.GetRedisKlineing(market), cycle.ToString(), JsonConvert.SerializeObject(kline_new));
            }
        }
    }

    #endregion
}