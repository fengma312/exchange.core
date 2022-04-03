using System.Linq.Expressions;
using Com.Db;
using Com.Api.Sdk.Enum;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Com.Bll;

/// <summary>
/// Service:计账钱包
/// </summary>
public class ServiceWallet
{
    /// <summary>
    /// 初始化
    /// </summary>
    public ServiceWallet()
    {

    }

    /// <summary>
    /// 资产冻结变更
    /// </summary>
    /// <param name="wallet_type">钱包类型</param>
    /// <param name="uid">用户</param>
    /// <param name="coin_base">币种</param>
    /// <param name="amount_base">正数:增加冻结,负数:减少冻结</param>
    /// <returns></returns>
    public bool FreezeChange(E_WalletType wallet_type, long uid, long coin_base, decimal amount_base)
    {
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                Wallet? wallet_base = db.Wallet.Where(P => P.wallet_type == wallet_type && P.user_id == uid && P.coin_id == coin_base).SingleOrDefault();
                if (wallet_base == null)
                {
                    return false;
                }
                if (amount_base > 0)
                {
                    if (wallet_base.available < amount_base)
                    {
                        return false;
                    }
                }
                else if (amount_base < 0)
                {
                    if (wallet_base.freeze < Math.Abs(amount_base))
                    {
                        return false;
                    }
                }
                else if (amount_base == 0)
                {
                    return false;
                }
                wallet_base.freeze += amount_base;
                wallet_base.available -= amount_base;
                return db.SaveChanges() > 0;
            }
        }
    }

    /// <summary>
    /// 资产冻结变更
    /// </summary>
    /// <param name="wallet_type">钱包类型</param>
    /// <param name="uid"></param>
    /// <param name="coin_base"></param>
    /// <param name="amount_base">正数:增加冻结,负数:减少冻结</param>
    /// <returns></returns>
    public bool FreezeChange(E_WalletType wallet_type, long uid, long coin_base, decimal amount_base, long coin_quote, decimal amount_quote)
    {
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                Wallet? wallet_base = db.Wallet.Where(P => P.wallet_type == wallet_type && P.user_id == uid && P.coin_id == coin_base).SingleOrDefault();
                if (wallet_base == null)
                {
                    return false;
                }
                if (amount_base > 0)
                {
                    if (wallet_base.available < amount_base)
                    {
                        return false;
                    }
                }
                else if (amount_base < 0)
                {
                    if (wallet_base.freeze < Math.Abs(amount_base))
                    {
                        return false;
                    }
                }
                else if (amount_base == 0)
                {
                    return false;
                }
                Wallet? wallet_quote = db.Wallet.Where(P => P.wallet_type == wallet_type && P.user_id == uid && P.coin_id == coin_quote).SingleOrDefault();
                if (wallet_quote == null)
                {
                    return false;
                }
                if (amount_quote > 0)
                {
                    if (wallet_quote.available < amount_quote)
                    {
                        return false;
                    }
                }
                else if (amount_quote < 0)
                {
                    if (wallet_quote.freeze < Math.Abs(amount_quote))
                    {
                        return false;
                    }
                }
                else if (amount_base == 0)
                {
                    return false;
                }
                wallet_base.freeze += amount_base;
                wallet_base.available -= amount_base;
                wallet_quote.freeze += amount_quote;
                wallet_quote.available -= amount_quote;
                return db.SaveChanges() > 0;
            }
        }
    }

    /// <summary>
    /// 可用余额转账
    /// </summary>
    /// <param name="wallet_type">钱包类型</param>
    /// <param name="coin_id">币ID</param>
    /// <param name="from">来源:用户id</param>
    /// <param name="to">目的:用户id</param>
    /// <param name="amount">数量</param>
    /// <returns></returns>
    public bool TransferAvailable(E_WalletType wallet_type, long coin_id, long from, long to, decimal amount)
    {
        if (amount == 0)
        {
            return false;
        }
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Wallet? wallet_from = db.Wallet.Where(P => P.wallet_type == wallet_type && P.coin_id == coin_id && P.user_id == from).SingleOrDefault();
                        Wallet? wallet_to = db.Wallet.Where(P => P.wallet_type == wallet_type && P.coin_id == coin_id && P.user_id == to).SingleOrDefault();
                        if (wallet_from == null || wallet_to == null)
                        {
                            return false;
                        }
                        if (amount > 0 && wallet_from.available < amount)
                        {
                            return false;
                        }
                        else if (amount < 0 && wallet_to.available < Math.Abs(amount))
                        {
                            return false;
                        }
                        wallet_from.available -= amount;
                        wallet_from.total = wallet_from.available + wallet_from.freeze;
                        wallet_to.available += amount;
                        wallet_to.total = wallet_to.available + wallet_to.freeze;
                        db.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        FactoryService.instance.constant.logger.LogError(ex, ex.Message);
                        return false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 撮合成交后资产变动(批量)
    /// </summary>
    /// <param name="wallet_type">钱包类型</param>
    /// <param name="coin_id_base">基础币种id</param>
    /// <param name="coin_id_quote">报价币种id</param>
    /// <param name="buy_uid">买用户</param>
    /// <param name="sell_uid">卖用户</param>
    /// <param name="rate_buy">买手续费</param>
    /// <param name="rate_sell">卖手续费</param>
    /// <param name="amount">成交量</param>
    /// <param name="price">成交价</param>
    /// <returns>是否成功</returns>
    public (bool result, List<Running> running) Transaction(E_WalletType wallet_type, Market market, List<Deal> deals)
    {
        List<Running> runnings = new List<Running>();
        if (deals == null || deals.Count == 0)
        {
            return (true, runnings);
        }
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<long> user_id = deals.Select(T => T.bid_uid).ToList();
                        user_id.AddRange(deals.Select(T => T.ask_uid).ToList());
                        user_id = user_id.Distinct().ToList();
                        List<Users> users = db.Users.AsNoTracking().Where(P => user_id.Contains(P.user_id)).ToList();
                        List<Vip> vips = db.Vip.AsNoTracking().Where(P => users.Select(P => P.vip).Distinct().Contains(P.id)).ToList();
                        List<Wallet> wallets = db.Wallet.Where(P => P.wallet_type == wallet_type && user_id.Contains(P.user_id) && (P.coin_id == market.coin_id_base || P.coin_id == market.coin_id_quote)).ToList();
                        Wallet? settlement_base = db.Wallet.Where(P => P.wallet_type == E_WalletType.main && P.user_id == market.settlement_uid && P.coin_id == market.coin_id_base).FirstOrDefault();
                        Wallet? settlement_quote = db.Wallet.Where(P => P.wallet_type == E_WalletType.main && P.user_id == market.settlement_uid && P.coin_id == market.coin_id_quote).FirstOrDefault();
                        foreach (var item in deals)
                        {
                            Users? user_buy = users.FirstOrDefault(P => P.user_id == item.bid_uid);
                            Users? user_sell = users.FirstOrDefault(P => P.user_id == item.ask_uid);
                            if (user_buy == null || user_sell == null)
                            {
                                return (false, runnings);
                            }
                            Vip? vip_buy = vips.FirstOrDefault(P => P.id == user_buy.vip);
                            Vip? vip_sell = vips.FirstOrDefault(P => P.id == user_sell.vip);
                            if (vip_buy == null || vip_sell == null)
                            {
                                return (false, runnings);
                            }
                            Wallet? buy_base = wallets.Where(P => P.coin_id == market.coin_id_base && P.user_id == item.bid_uid).FirstOrDefault();
                            Wallet? buy_quote = wallets.Where(P => P.coin_id == market.coin_id_quote && P.user_id == item.bid_uid).FirstOrDefault();
                            Wallet? sell_base = wallets.Where(P => P.coin_id == market.coin_id_base && P.user_id == item.ask_uid).FirstOrDefault();
                            Wallet? sell_quote = wallets.Where(P => P.coin_id == market.coin_id_quote && P.user_id == item.ask_uid).FirstOrDefault();
                            if (buy_base == null || buy_quote == null || sell_base == null || sell_quote == null)
                            {
                                return (false, runnings);
                            }
                            decimal fee_maker = 0;
                            decimal fee_taker = 0;
                            if (item.trigger_side == E_OrderSide.buy)
                            {
                                // 买单为吃单,卖单为挂单
                                fee_maker = vip_sell.fee_maker * item.total;
                                fee_taker = vip_buy.fee_taker * item.amount;
                            }
                            else if (item.trigger_side == E_OrderSide.sell)
                            {
                                // 卖单为吃单,买单为挂单
                                fee_maker = vip_buy.fee_maker * item.amount;
                                fee_taker = vip_sell.fee_taker * item.total;
                            }
                            sell_base.freeze -= item.amount;
                            buy_base.available += (item.amount - fee_taker);
                            buy_quote.freeze -= item.total;
                            sell_quote.available += (item.total - fee_maker);
                            buy_base.total = buy_base.available + buy_base.freeze;
                            buy_quote.total = buy_quote.available + buy_quote.freeze;
                            sell_base.total = sell_base.available + sell_base.freeze;
                            sell_quote.total = sell_quote.available + sell_quote.freeze;
                            runnings.Add(AddRunning(item.trade_id, wallet_type, item.amount - fee_taker, sell_base, buy_base, $"交易:{sell_base.user_name}=>{buy_base.user_name},{item.amount - fee_taker}{sell_base.coin_name}"));
                            runnings.Add(AddRunning(item.trade_id, wallet_type, item.total - fee_maker, buy_quote, sell_quote, $"交易:{buy_quote.user_name}=>{sell_quote.user_name},{item.total - fee_maker}{buy_quote.coin_name}"));
                            if (settlement_base != null)
                            {
                                runnings.Add(AddRunning(item.trade_id, E_WalletType.main, fee_taker, buy_base, settlement_base, $"手续费(吃单):{buy_base.user_name}=>{settlement_base.user_name},{fee_taker}{buy_base.coin_name}"));
                            }
                            if (settlement_quote != null)
                            {
                                runnings.Add(AddRunning(item.trade_id, E_WalletType.main, fee_maker, sell_quote, settlement_quote, $"手续费(挂单):{sell_quote.user_name}=>{settlement_quote.user_name},{fee_maker}{sell_quote.coin_name}"));
                            }
                        }
                        db.SaveChanges();
                        transaction.Commit();
                        return (true, runnings);
                    }
                    catch (Exception ex)
                    {
                        runnings.Clear();
                        transaction.Rollback();
                        FactoryService.instance.constant.logger.LogError(ex, ex.Message);
                        return (false, runnings);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 添加钱包流水
    /// </summary>
    /// <param name="relation_id">关联id</param>
    /// <param name="wallet_type_to">到账钱包类型</param>
    /// <param name="amount">量</param>
    /// <param name="wallet_from">来源钱包</param>
    /// <param name="wallet_to">到账钱包</param>
    /// <param name="remarks">备注</param>
    /// <returns></returns>
    public Running AddRunning(long relation_id, E_WalletType wallet_type_to, decimal amount, Wallet wallet_from, Wallet wallet_to, string remarks)
    {
        return new Running
        {
            id = FactoryService.instance.constant.worker.NextId(),
            relation_id = relation_id,
            coin_id = wallet_from.coin_id,
            coin_name = wallet_from.coin_name,
            wallet_from = wallet_from.wallet_id,
            wallet_to = wallet_to.wallet_id,
            wallet_type_from = E_WalletType.main,
            wallet_type_to = wallet_type_to,
            uid_from = wallet_from.user_id,
            uid_to = wallet_to.user_id,
            user_name_from = wallet_from.user_name,
            user_name_to = wallet_to.user_name,
            amount = amount,
            operation_uid = 0,
            time = DateTimeOffset.UtcNow,
            remarks = remarks,
        };
    }

    /// <summary>
    /// 添加资金流水
    /// </summary>
    /// <param name="runnings"></param>
    public void AddRunning(List<Running> runnings)
    {
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                db.Running.AddRange(runnings);
                db.SaveChanges();
            }
        }
    }




}