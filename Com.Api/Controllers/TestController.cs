﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Com.Bll;
using Com.Db;
using Com.Api.Sdk.Enum;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Com.Api.Sdk.Models;
using Com.Bll.Util;
using Newtonsoft.Json;

namespace Com.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Authorize]
[AllowAnonymous]
[Route("[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public DbContextEF db = null!;

    /// <summary>
    /// 登录玩家id
    /// </summary>
    /// <value></value>
    public int user_id
    {
        get
        {
            Claim? claim = User.Claims.FirstOrDefault(P => P.Type == JwtRegisteredClaimNames.Aud);
            if (claim != null)
            {
                return Convert.ToInt32(claim.Value);
            }
            return 5;
        }
    }

    /// <summary>
    /// 交易对基础信息
    /// </summary>
    /// <returns></returns>
    public ServiceMarket market_info_db = new ServiceMarket();
    /// <summary>
    /// Service:订单
    /// </summary>
    public ServiceOrder order_service = new ServiceOrder();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <param name="db"></param>
    /// <param name="provider"></param>
    /// <param name="logger"></param>
    public TestController(IServiceProvider provider, IConfiguration configuration, IHostEnvironment environment, DbContextEF db, ILogger<OrderController> logger)
    {
        this.db = db;
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("Init")]
    public int Init()
    {
        List<Coin> coins = new List<Coin>();
        Coin usdt = new Coin()
        {
            coin_id = FactoryService.instance.constant.worker.NextId(),
            coin_name = "usdt",
            price_places = 8,
            amount_places = 8,
            contract = "",
        };
        Coin btc = new Coin()
        {
            coin_id = FactoryService.instance.constant.worker.NextId(),
            coin_name = "btc",
            price_places = 8,
            amount_places = 8,
            contract = "",
        };
        Coin eth = new Coin()
        {
            coin_id = FactoryService.instance.constant.worker.NextId(),
            coin_name = "eth",
            price_places = 8,
            amount_places = 8,
            contract = "",
        };
        this.db.Coin.Add(usdt);
        this.db.Coin.Add(btc);
        this.db.Coin.Add(eth);
        coins.Add(usdt);
        coins.Add(btc);
        coins.Add(eth);
        Vip vip0 = new Vip()
        {
            id = FactoryService.instance.constant.worker.NextId(),
            name = "vip0",
            fee_maker = 0.0002m,
            fee_taker = 0.0004m,
        };
        Vip vip1 = new Vip()
        {
            id = FactoryService.instance.constant.worker.NextId(),
            name = "vip1",
            fee_maker = 0.00016m,
            fee_taker = 0.0004m,
        };
        Vip vip2 = new Vip()
        {
            id = FactoryService.instance.constant.worker.NextId(),
            name = "vip2",
            fee_maker = 0.00014m,
            fee_taker = 0.00035m,
        };
        this.db.Vip.Add(vip0);
        this.db.Vip.Add(vip1);
        this.db.Vip.Add(vip2);
        for (int i = 0; i < 10; i++)
        {
            (string public_key, string private_key) key = Encryption.GetRsaKey();
            Users user = new Users()
            {
                user_id = FactoryService.instance.constant.worker.NextId(),
                user_name = "user" + i,
                password = Encryption.SHA256Encrypt("123456_" + i),
                disabled = false,
                transaction = true,
                withdrawal = false,
                phone = null,
                email = null,
                vip = vip0.id,
                public_key = key.public_key,
                private_key = key.private_key,
            };
            this.db.Users.Add(user);
            Wallet wallet_usdt = new Wallet()
            {
                wallet_id = FactoryService.instance.constant.worker.NextId(),
                wallet_type = E_WalletType.spot,
                user_id = user.user_id,
                user_name = user.user_name,
                coin_id = usdt.coin_id,
                coin_name = usdt.coin_name,
                total = 5_000_000_000,
                available = 5_000_000_000,
                freeze = 0,
            };
            Wallet wallet_btc = new Wallet()
            {
                wallet_id = FactoryService.instance.constant.worker.NextId(),
                wallet_type = E_WalletType.spot,
                user_id = user.user_id,
                user_name = user.user_name,
                coin_id = btc.coin_id,
                coin_name = btc.coin_name,
                total = 10_000_000,
                available = 10_000_000,
                freeze = 0,
            };
            Wallet wallet_eth = new Wallet()
            {
                wallet_id = FactoryService.instance.constant.worker.NextId(),
                wallet_type = E_WalletType.spot,
                user_id = user.user_id,
                user_name = user.user_name,
                coin_id = eth.coin_id,
                coin_name = eth.coin_name,
                total = 50_000_000,
                available = 50_000_000,
                freeze = 0,
            };
            this.db.Wallet.Add(wallet_usdt);
            this.db.Wallet.Add(wallet_btc);
            this.db.Wallet.Add(wallet_eth);
        }
        (string public_key, string private_key) key_btc_user = Encryption.GetRsaKey();
        Users settlement_btc_usdt = new Users()
        {
            user_id = FactoryService.instance.constant.worker.NextId(),
            user_name = "settlement_btc/usdt",
            password = Encryption.SHA256Encrypt("123456"),
            disabled = false,
            transaction = true,
            withdrawal = false,
            phone = null,
            email = null,
            vip = vip1.id,
            public_key = key_btc_user.public_key,
            private_key = key_btc_user.private_key,
        };
        this.db.Users.Add(settlement_btc_usdt);
        (string public_key, string private_key) key_eth_user = Encryption.GetRsaKey();
        Users settlement_eth_usdt = new Users()
        {
            user_id = FactoryService.instance.constant.worker.NextId(),
            user_name = "settlement_eth/usdt",
            password = Encryption.SHA256Encrypt("123456"),
            disabled = false,
            transaction = true,
            withdrawal = false,
            phone = null,
            email = null,
            vip = vip1.id,
            public_key = key_eth_user.public_key,
            private_key = key_eth_user.private_key,
        };
        this.db.Users.Add(settlement_eth_usdt);
        foreach (var item in coins)
        {
            this.db.Wallet.Add(new Wallet()
            {
                wallet_id = FactoryService.instance.constant.worker.NextId(),
                wallet_type = E_WalletType.spot,
                user_id = settlement_btc_usdt.user_id,
                user_name = settlement_btc_usdt.user_name,
                coin_id = item.coin_id,
                coin_name = item.coin_name,
                total = 0,
                available = 0,
                freeze = 0,
            });
            this.db.Wallet.Add(new Wallet()
            {
                wallet_id = FactoryService.instance.constant.worker.NextId(),
                wallet_type = E_WalletType.spot,
                user_id = settlement_eth_usdt.user_id,
                user_name = settlement_eth_usdt.user_name,
                coin_id = item.coin_id,
                coin_name = item.coin_name,
                total = 0,
                available = 0,
                freeze = 0,
            });
        }
        Market btcusdt = new Market()
        {
            market = FactoryService.instance.constant.worker.NextId(),
            symbol = "btc/usdt",
            coin_id_base = btc.coin_id,
            coin_name_base = btc.coin_name,
            coin_id_quote = usdt.coin_id,
            coin_name_quote = usdt.coin_name,
            places_price = 2,
            places_amount = 6,
            trade_min = 10,
            trade_min_market_sell = 0.0002m,
            market_uid = 0,
            status = false,
            transaction = true,
            settlement_uid = settlement_btc_usdt.user_id,
            last_price = Math.Round((decimal)FactoryService.instance.constant.random.NextDouble(), 2),
            service_url = "http://localhost:8080",
        };
        Market ethusdt = new Market()
        {
            market = FactoryService.instance.constant.worker.NextId(),
            symbol = "eth/usdt",
            coin_id_base = eth.coin_id,
            coin_name_base = eth.coin_name,
            coin_id_quote = usdt.coin_id,
            coin_name_quote = usdt.coin_name,
            places_price = 2,
            places_amount = 4,
            trade_min = 10,
            trade_min_market_sell = 0.002m,
            market_uid = 0,
            status = false,
            transaction = true,
            settlement_uid = settlement_eth_usdt.user_id,
            last_price = Math.Round((decimal)FactoryService.instance.constant.random.NextDouble(), 2),
            service_url = "http://localhost:8080",
        };
        this.db.Market.Add(btcusdt);
        this.db.Market.Add(ethusdt);
        return this.db.SaveChanges();
    }


    /// <summary>
    /// 模拟下单
    /// </summary>
    /// <param name="count">次数</param>
    /// <returns></returns>
    [HttpPost]
    [Route("PlaceOrderText")]
    public ResCall<List<ResOrder>> PlaceOrderText(int count)
    {
        ResCall<List<ResOrder>> res = new ResCall<List<ResOrder>>();
        res.success = true;
        res.code = E_Res_Code.ok;
        res.data = new List<ResOrder>();
        List<Users> users = this.db.Users.ToList();
        List<Market> markets = this.db.Market.ToList();
        for (int i = 0; i < count; i++)
        {
            Users user = users[FactoryService.instance.constant.random.Next(0, 10)];
            Market market = markets[FactoryService.instance.constant.random.Next(0, 2)];
            List<ReqOrder> reqOrders = new List<ReqOrder>();
            for (int j = 0; j < 1; j++)
            {
                decimal amount = FactoryService.instance.constant.random.NextInt64(0, 5) + (decimal)FactoryService.instance.constant.random.NextDouble() + market.trade_min;
                decimal? price = null;
                E_OrderSide side = FactoryService.instance.constant.random.Next(0, 2) == 0 ? E_OrderSide.buy : E_OrderSide.sell;
                E_OrderType type = FactoryService.instance.constant.random.Next(0, 2) == 0 ? E_OrderType.price_limit : E_OrderType.price_market;
                if (market.symbol == "btc/usdt")
                {
                    if (type == E_OrderType.price_market && side == E_OrderSide.sell)
                    {
                        amount = FactoryService.instance.constant.random.NextInt64(0, 3) + (decimal)FactoryService.instance.constant.random.NextDouble() + market.trade_min_market_sell;
                        amount = Math.Round(amount, market.places_amount);
                    }
                    else if (type == E_OrderType.price_limit)
                    {
                        price = FactoryService.instance.constant.random.NextInt64(45000, 60000) + (decimal)FactoryService.instance.constant.random.NextDouble();
                        amount = Math.Round(amount, market.places_amount);
                    }
                    else
                    {
                        amount = Math.Round(amount, market.places_amount + market.places_price);
                    }
                }
                else if (market.symbol == "eth/usdt")
                {
                    if (type == E_OrderType.price_market && side == E_OrderSide.sell)
                    {
                        amount = FactoryService.instance.constant.random.NextInt64(0, 10) + (decimal)FactoryService.instance.constant.random.NextDouble() + market.trade_min_market_sell;
                        amount = Math.Round(amount, market.places_amount);
                    }
                    else if (type == E_OrderType.price_limit)
                    {
                        price = FactoryService.instance.constant.random.NextInt64(3000, 5000) + (decimal)FactoryService.instance.constant.random.NextDouble();
                        amount = Math.Round(amount, market.places_amount);
                    }
                    else
                    {
                        amount = Math.Round(amount, market.places_amount + market.places_price);
                    }
                }
                if (price != null)
                {
                    price = Math.Round(price ?? 0, market.places_price);
                }
                ReqOrder order = new ReqOrder()
                {
                    client_id = FactoryService.instance.constant.worker.NextId().ToString(),
                    symbol = market.symbol,
                    side = side,
                    type = type,
                    price = price,
                    amount = amount,
                    trigger_hanging_price = 0,
                    trigger_cancel_price = 0,
                    data = null,
                };
                reqOrders.Add(order);
            }
            ResCall<List<ResOrder>> aaa = order_service.PlaceOrder(market.symbol, user.user_id, user.user_name, reqOrders);
            res.data.AddRange(aaa.data);
        }
        return res;
    }

}
