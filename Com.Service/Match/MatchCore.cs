/*  
 * ......................我佛慈悲...................... 
 *                       _oo0oo_ 
 *                      o8888888o 
 *                      88" . "88 
 *                      (| -_- |) 
 *                      0\  =  /0 
 *                    ___/`---'\___ 
 *                  .' \\|     |// '. 
 *                 / \\|||  :  |||// \ 
 *                / _||||| -卍-|||||- \ 
 *               |   | \\\  -  /// |   | 
 *               | \_|  ''\---/''  |_/ | 
 *               \  .-\__  '-'  ___/-. / 
 *             ___'. .'  /--.--\  `. .'___ 
 *          ."" '<  `.___\_<|>_/___.' >' "". 
 *         | | :  `- \`.;`\ _ /`;.`/ - ` : | | 
 *         \  \ `_.   \_ __\ /__ _/   .-` /  / 
 *     =====`-.____`.___ \_____/___.-`___.-'===== 
 *                       `=---=' 
 *                        
 *..................佛祖开光 ,永无BUG................... 
 *  
 */




using Com.Model;
using Com.Model.Enum;
using Com.Service.Models;

namespace Com.Service.Match;

/// <summary>
/// 撮合算法核心类 ,注:撮合引擎不保存数据
/// </summary>
public class MatchCore
{
    /// <summary>
    /// 撮合服务对象
    /// </summary>
    /// <value></value>
    public MatchModel model { get; set; } = null!;
    /// <summary>
    /// 市价买单
    /// </summary>
    /// <typeparam name="Order">订单</typeparam>
    /// <returns></returns>
    public List<BaseOrder> market_bid = new List<BaseOrder>();
    /// <summary>
    /// 市价卖单
    /// </summary>
    /// <typeparam name="Order">订单</typeparam>
    /// <returns></returns>
    public List<BaseOrder> market_ask = new List<BaseOrder>();
    /// <summary>
    /// 限价买单 高->低
    /// </summary>
    /// <typeparam name="Order">订单</typeparam>
    /// <returns></returns>
    public List<BaseOrder> fixed_bid = new List<BaseOrder>();
    /// <summary>
    /// 限价卖单 低->高
    /// </summary>
    /// <typeparam name="Order">订单</typeparam>
    /// <returns></returns>
    public List<BaseOrder> fixed_ask = new List<BaseOrder>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="model">撮合服务对象</param>
    public MatchCore(MatchModel model)
    {
        this.model = model;
    }

    /// <summary>
    /// 主要流程
    /// </summary>
    // public void SendOrder(Order order)
    // {
    //     List<Deal> deals = Match(order);
    // this.mq.SendDeal(deals);
    // List<OrderBook> orderBooks = GetOrderBooks(order, deals);
    // this.mq.SendOrderBook(orderBooks);
    // Kline? kline = SetKlink(deals);
    // this.mq.SendKline(kline);
    // }

    /// <summary>
    /// 撤消订单
    /// </summary>
    /// <param name="order_id">订单ID</param>
    /// <returns>orderbook变更</returns>
    public List<BaseOrder> CancelOrder(List<long> order_id)
    {
        List<BaseOrder> cancel_market_bid = this.market_bid.Where(P => order_id.Contains(P.order_id)).ToList();
        this.market_bid.RemoveAll(P => cancel_market_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_market_ask = this.market_ask.Where(P => order_id.Contains(P.order_id)).ToList();
        this.market_ask.RemoveAll(P => cancel_market_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_bid = this.fixed_bid.Where(P => order_id.Contains(P.order_id)).ToList();
        this.fixed_bid.RemoveAll(P => cancel_fixed_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_ask = this.fixed_ask.Where(P => order_id.Contains(P.order_id)).ToList();
        this.fixed_ask.RemoveAll(P => cancel_fixed_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel = new List<BaseOrder>();
        cancel.AddRange(cancel_market_bid);
        cancel.AddRange(cancel_market_ask);
        cancel.AddRange(cancel_fixed_bid);
        cancel.AddRange(cancel_fixed_ask);
        cancel.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; });
        return cancel;
    }

    /// <summary>
    /// 撤消订单
    /// </summary>
    /// <param name="uid">用户ID</param>
    /// <returns>orderbook变更</returns>
    public List<BaseOrder> CancelOrder(long uid)
    {
        List<BaseOrder> cancel_market_bid = this.market_bid.Where(P => P.uid == uid).ToList();
        this.market_bid.RemoveAll(P => cancel_market_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_market_ask = this.market_ask.Where(P => P.uid == uid).ToList();
        this.market_ask.RemoveAll(P => cancel_market_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_bid = this.fixed_bid.Where(P => P.uid == uid).ToList();
        this.fixed_bid.RemoveAll(P => cancel_fixed_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_ask = this.fixed_ask.Where(P => P.uid == uid).ToList();
        this.fixed_ask.RemoveAll(P => cancel_fixed_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel = new List<BaseOrder>();
        cancel.AddRange(cancel_market_bid);
        cancel.AddRange(cancel_market_ask);
        cancel.AddRange(cancel_fixed_bid);
        cancel.AddRange(cancel_fixed_ask);
        cancel.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; });
        return cancel;
    }

    /// <summary>
    /// 撤消订单
    /// </summary>
    /// <param name="client_id">客户订单ID</param>
    /// <returns>orderbook变更</returns>
    public List<BaseOrder> CancelOrder(long[] client_id)
    {
        List<BaseOrder> cancel_market_bid = this.market_bid.Where(P => client_id.Contains(P.order_id)).ToList();
        this.market_bid.RemoveAll(P => cancel_market_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_market_ask = this.market_ask.Where(P => client_id.Contains(P.order_id)).ToList();
        this.market_ask.RemoveAll(P => cancel_market_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_bid = this.fixed_bid.Where(P => client_id.Contains(P.order_id)).ToList();
        this.fixed_bid.RemoveAll(P => cancel_fixed_bid.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel_fixed_ask = this.fixed_ask.Where(P => client_id.Contains(P.order_id)).ToList();
        this.fixed_ask.RemoveAll(P => cancel_fixed_ask.Select(P => P.order_id).Contains(P.order_id));
        List<BaseOrder> cancel = new List<BaseOrder>();
        cancel.AddRange(cancel_market_bid);
        cancel.AddRange(cancel_market_ask);
        cancel.AddRange(cancel_fixed_bid);
        cancel.AddRange(cancel_fixed_ask);
        cancel.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; });
        return cancel;
    }

    /// <summary>
    /// 撤消订单(该交易对所有订单)
    /// </summary>
    /// <returns>orderbook变更</returns>
    public List<BaseOrder> CancelOrder()
    {
        List<BaseOrder> cancel = new List<BaseOrder>();
        cancel.AddRange(this.market_bid);
        cancel.AddRange(this.market_ask);
        cancel.AddRange(this.fixed_bid);
        cancel.AddRange(this.fixed_ask);
        cancel.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; });
        return cancel;
    }

    /// <summary>
    ///  撮合算法(不考虑手续费问题)
    /// </summary>
    /// <param name="order">挂单订单(手续费问题在推送到撮合之前扣除)</param>
    /// <returns>成交情况</returns>
    public (BaseOrder? order, List<MatchDeal> deal, List<BaseOrder> cancel) Match(BaseOrder order)
    {
        List<MatchDeal> deals = new List<MatchDeal>();
        List<BaseOrder> cancel = new List<BaseOrder>();
        if (order.market != this.model.info.market || order.amount <= 0 || order.amount_unsold <= 0)
        {
            return (null, deals, cancel);
        }
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (order.side == E_OrderSide.buy)
        {
            //先市价成交,再限价成交
            if (order.type == E_OrderType.price_market)
            {
                //市价买单与市价卖市撮合
                for (int i = 0; i < market_ask.Count; i++)
                {
                    if (market_ask[i].amount_unsold >= order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountAskBid(this.model.info.market, order, market_ask[i], this.model.info.last_price, E_OrderSide.buy, now);
                        deals.Add(deal);
                        if (market_ask[i].amount_unsold == order.amount_unsold)
                        {
                            market_ask.Remove(market_ask[i]);
                        }
                        break;
                    }
                    else if (market_ask[i].amount_unsold < order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountBidAsk(this.model.info.market, order, market_ask[i], this.model.info.last_price, E_OrderSide.buy, now);
                        deals.Add(deal);
                        //市价卖单完成,从市价卖单移除
                        market_ask.Remove(market_ask[i]);
                    }
                    //量全部处理完了
                    if (order.amount_unsold <= 0)
                    {
                        break;
                    }
                }
                //市价买单与限价卖单撮合
                if (order.amount_unsold > 0 && fixed_ask.Count() > 0)
                {
                    for (int i = 0; i < fixed_ask.Count; i++)
                    {
                        //使用撮合价规则
                        decimal new_price = Util.GetNewPrice(fixed_ask[i].price, fixed_ask[i].price, this.model.info.last_price);
                        if (new_price <= 0)
                        {
                            break;
                        }
                        if (fixed_ask[i].amount_unsold >= order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountAskBid(this.model.info.market, order, fixed_ask[i], this.model.info.last_price, E_OrderSide.buy, now);
                            deals.Add(deal);
                            if (fixed_ask[i].amount_unsold == order.amount_unsold)
                            {
                                fixed_ask.Remove(fixed_ask[i]);
                            }
                            break;
                        }
                        else if (fixed_ask[i].amount_unsold < order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountBidAsk(this.model.info.market, order, fixed_ask[i], this.model.info.last_price, E_OrderSide.buy, now);
                            deals.Add(deal);
                            //市价卖单完成,从市价卖单移除
                            fixed_ask.Remove(fixed_ask[i]);
                        }
                        this.model.info.last_price = new_price;
                        //量全部处理完了
                        if (order.amount_unsold <= 0)
                        {
                            break;
                        }
                    }
                }
                //市价买单没成交部分添加到市价买单最后,(时间优先原则)
                if (order.amount_unsold > 0)
                {
                    market_bid.Add(order);
                }
            }
            else if (order.type == E_OrderType.price_fixed)
            {
                //限价买单与市价卖单撮合
                for (int i = 0; i < market_ask.Count; i++)
                {
                    if (market_ask[i].amount_unsold >= order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountAskBid(this.model.info.market, order, market_ask[i], order.price, E_OrderSide.buy, now);
                        deals.Add(deal);
                        if (market_ask[i].amount_unsold == order.amount_unsold)
                        {
                            market_ask.Remove(market_ask[i]);
                        }
                        break;
                    }
                    else if (market_ask[i].amount_unsold < order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountBidAsk(this.model.info.market, order, market_ask[i], order.price, E_OrderSide.buy, now);
                        deals.Add(deal);
                        //市价卖单完成,从市价卖单移除
                        market_ask.Remove(market_ask[i]);
                    }
                    //量全部处理完了
                    if (order.amount_unsold <= 0)
                    {
                        break;
                    }
                }
                //限价买单与限价卖单撮合
                if (order.amount_unsold > 0 && fixed_ask.Count() > 0)
                {
                    for (int i = 0; i < fixed_ask.Count; i++)
                    {
                        //使用撮合价规则
                        decimal new_price = Util.GetNewPrice(order.price, fixed_ask[i].price, this.model.info.last_price);
                        if (new_price <= 0)
                        {
                            break;
                        }
                        if (fixed_ask[i].amount_unsold >= order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountAskBid(this.model.info.market, order, fixed_ask[i], new_price, E_OrderSide.buy, now);
                            deals.Add(deal);
                            if (fixed_ask[i].amount_unsold == order.amount_unsold)
                            {
                                fixed_ask.Remove(fixed_ask[i]);
                            }
                            break;
                        }
                        else if (fixed_ask[i].amount_unsold < order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountBidAsk(this.model.info.market, order, fixed_ask[i], new_price, E_OrderSide.buy, now);
                            deals.Add(deal);
                            //市价卖单完成,从市价卖单移除
                            fixed_ask.Remove(fixed_ask[i]);
                        }
                        this.model.info.last_price = new_price;
                        //量全部处理完了
                        if (order.amount_unsold <= 0)
                        {
                            break;
                        }
                    }
                }
                //限价买单没成交部分添加到限价买单相应的位置,(价格优先,时间优先原则)
                if (order.amount_unsold > 0)
                {
                    if (fixed_bid.Count == 0)
                    {
                        fixed_bid.Add(order);
                    }
                    else
                    {
                        int index = fixed_bid.FindIndex(P => P.price <= order.price && P.create_time < order.create_time);
                        if (index == -1)
                        {
                            fixed_bid.Add(order);
                        }
                        else
                        {
                            fixed_bid.Insert(index, order);
                        }
                    }
                }
            }
        }
        else if (order.side == E_OrderSide.sell)
        {
            //先市价成交,再限价成交
            if (order.type == E_OrderType.price_market)
            {
                //市价卖单与市价买单撮合
                for (int i = 0; i < market_bid.Count; i++)
                {
                    if (market_bid[i].amount_unsold >= order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountBidAsk(this.model.info.market, market_bid[i], order, this.model.info.last_price, E_OrderSide.sell, now);
                        deals.Add(deal);
                        if (deal.bid.state == E_OrderState.completed)
                        {
                            market_bid.Remove(market_bid[i]);
                        }
                        break;
                    }
                    else if (market_bid[i].amount_unsold < order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountAskBid(this.model.info.market, market_bid[i], order, this.model.info.last_price, E_OrderSide.sell, now);
                        deals.Add(deal);
                        //市价买单完成,从市价买单移除
                        market_bid.Remove(market_bid[i]);
                    }
                    //量全部处理完了
                    if (order.amount_unsold <= 0)
                    {
                        break;
                    }
                }
                //市价卖单与限价买单撮合
                if (order.amount_unsold > 0 && fixed_bid.Count() > 0)
                {
                    for (int i = 0; i < fixed_bid.Count; i++)
                    {
                        //使用撮合价规则
                        decimal new_price = Util.GetNewPrice(fixed_bid[i].price, fixed_bid[i].price, this.model.info.last_price);
                        if (new_price <= 0)
                        {
                            break;
                        }
                        if (fixed_bid[i].amount_unsold >= order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountBidAsk(this.model.info.market, fixed_bid[i], order, new_price, E_OrderSide.sell, now);
                            deals.Add(deal);
                            if (fixed_bid[i].amount_unsold == order.amount_unsold)
                            {
                                fixed_bid.Remove(fixed_bid[i]);
                            }
                            break;
                        }
                        else if (fixed_bid[i].amount_unsold < order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountAskBid(this.model.info.market, fixed_bid[i], order, new_price, E_OrderSide.sell, now);
                            deals.Add(deal);
                            //市价买单完成,从市价买单移除
                            fixed_bid.Remove(fixed_bid[i]);
                        }
                        this.model.info.last_price = new_price;
                        //量全部处理完了
                        if (order.amount_unsold <= 0)
                        {
                            break;
                        }
                    }
                }
                //市价卖单没成交部分添加到市价卖单最后,(时间优先原则)
                if (order.amount_unsold > 0)
                {
                    market_ask.Add(order);
                }
            }
            else if (order.type == E_OrderType.price_fixed)
            {
                //限价卖单与市价买市撮合
                for (int i = 0; i < market_bid.Count; i++)
                {
                    if (market_bid[i].amount_unsold >= order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountBidAsk(this.model.info.market, order, market_bid[i], order.price, E_OrderSide.sell, now);
                        deals.Add(deal);
                        if (deal.bid.state == E_OrderState.completed)
                        {
                            market_bid.Remove(market_bid[i]);
                        }
                        break;
                    }
                    else if (market_bid[i].amount_unsold < order.amount_unsold)
                    {
                        MatchDeal deal = Util.AmountAskBid(this.model.info.market, market_bid[i], order, order.price, E_OrderSide.sell, now);
                        deals.Add(deal);
                        //市价买单完成,从市价买单移除
                        market_bid.Remove(market_bid[i]);
                    }
                    //量全部处理完了
                    if (order.amount_unsold <= 0)
                    {
                        break;
                    }
                }
                //限价卖单与限价买单撮合
                if (order.amount_unsold > 0 && fixed_bid.Count() > 0)
                {
                    for (int i = 0; i < fixed_bid.Count; i++)
                    {
                        //使用撮合价规则
                        decimal new_price = Util.GetNewPrice(fixed_bid[i].price, order.price, this.model.info.last_price);
                        if (new_price <= 0)
                        {
                            break;
                        }
                        if (fixed_bid[i].amount_unsold >= order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountBidAsk(this.model.info.market, fixed_bid[i], order, new_price, E_OrderSide.sell, now);
                            deals.Add(deal);
                            if (deal.bid.state == E_OrderState.completed)
                            {
                                fixed_bid.Remove(fixed_bid[i]);
                            }
                            break;
                        }
                        else if (fixed_bid[i].amount_unsold < order.amount_unsold)
                        {
                            MatchDeal deal = Util.AmountAskBid(this.model.info.market, fixed_bid[i], order, new_price, E_OrderSide.sell, now);
                            deals.Add(deal);
                            //市价买单完成,从市价买单移除
                            fixed_bid.Remove(fixed_bid[i]);
                        }
                        this.model.info.last_price = new_price;
                        //量全部处理完了
                        if (order.amount_unsold <= 0)
                        {
                            break;
                        }
                    }
                }
                //限价卖单没成交部分添加到限价卖单相应的位置,(价格优先,时间优先原则)
                if (order.amount_unsold > 0)
                {
                    if (fixed_ask.Count == 0)
                    {
                        fixed_ask.Add(order);
                    }
                    else
                    {
                        int index = fixed_ask.FindIndex(P => P.price > order.price && P.create_time < order.create_time);
                        if (index == -1)
                        {
                            fixed_ask.Add(order);
                        }
                        else
                        {
                            fixed_ask.Insert(index, order);
                        }
                    }
                }
            }
        }
        if (deals.Count > 0)
        {
            //触发市价撤单价格
            decimal total_price = deals.Last().price;
            List<BaseOrder> bid = this.market_bid.Where(P => P.trigger_cancel_price >= total_price).ToList();
            bid.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; P.remarks = "市价买单已高于触发价,自动撤单"; });
            cancel.AddRange(bid);
            List<BaseOrder> ask = this.market_ask.Where(P => P.trigger_cancel_price <= total_price).ToList();
            ask.ForEach(P => { P.state = E_OrderState.cancel; P.deal_last_time = DateTimeOffset.UtcNow; P.remarks = "市价卖单已低于触发价,自动撤单"; });
            cancel.AddRange(ask);
        }
        return (order, deals, cancel);
    }

}