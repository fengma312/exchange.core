using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Com.Db;

/// <summary>
/// DB上下文
/// </summary>
public class DbContextEF : DbContext
{
    /// <summary>
    /// 币的基础信息
    /// </summary>
    /// <value></value>
    public DbSet<Coin> Coin { get; set; } = null!;
    /// <summary>
    /// 成交单
    /// </summary>
    /// <value></value>
    public DbSet<Deal> Deal { get; set; } = null!;
    /// <summary>
    /// K线
    /// </summary>
    /// <value></value>
    public DbSet<Kline> Kline { get; set; } = null!;
    /// <summary>
    /// 交易对基础信息
    /// </summary>
    /// <value></value>
    public DbSet<MarketInfo> MarketInfo { get; set; } = null!;
    /// <summary>
    /// 订单表
    /// </summary>
    /// <value></value>
    public DbSet<Orders> Order { get; set; } = null!;
    /// <summary>
    /// 用户基础信息
    /// </summary>
    /// <value></value>
    public DbSet<Users> Users { get; set; } = null!;
    /// <summary>
    /// 计账钱包基础信息
    /// </summary>
    /// <value></value>
    public DbSet<Wallet> Wallet { get; set; } = null!;

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public readonly string? connectionString = null!;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public DbContextEF(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public DbContextEF(DbContextOptions<DbContextEF> options) : base(options)
    {

    }

    /// <summary>
    /// 使用连接字符串来创建DB上下文
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrWhiteSpace(this.connectionString))
        {
            optionsBuilder.UseSqlServer(this.connectionString);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coin>(o =>
        {
            o.HasKey(p => p.coin_id);
            o.HasIndex(P => new { P.coin_name }).IsUnique();
            o.Property(P => P.coin_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("币id");
            o.Property(P => P.coin_name).HasColumnType("nvarchar").HasMaxLength(20).HasComment("币名称");
            o.Property(P => P.price_places).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("价格小数位数");
            o.Property(P => P.amount_places).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("量小数位数");
            o.Property(P => P.contract).HasColumnType("nvarchar").HasMaxLength(200).HasComment("合约地址");
            o.ToTable(nameof(Coin));
        });
        modelBuilder.Entity<Deal>(o =>
        {
            o.HasKey(p => p.trade_id);
            o.HasIndex(P => new { P.market, P.time });
            o.Property(P => P.trade_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("成交订单ID");
            o.Property(P => P.market).IsRequired().HasColumnType("bigint").HasComment("交易对");
            o.Property(P => P.symbol).HasColumnType("nvarchar").HasMaxLength(20).HasComment("交易对名称");
            o.Property(P => P.price).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交价");
            o.Property(P => P.amount).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交量");
            o.Property(P => P.total).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交总额");
            o.Property(P => P.trigger_side).IsRequired().HasColumnType("tinyint").HasComment("成交触发方向");
            o.Property(P => P.bid_id).IsRequired().HasColumnType("bigint").HasComment("买单id");
            o.Property(P => P.ask_id).IsRequired().HasColumnType("bigint").HasComment("卖单id");
            o.Property(P => P.time).IsRequired().HasColumnType("datetimeoffset").HasComment("成交时间");
            o.ToTable(nameof(Deal));
        });
        modelBuilder.Entity<Kline>(o =>
        {
            o.HasKey(p => p.id);
            o.HasIndex(P => new { P.market, P.type, P.time_start, P.time_end });
            o.HasIndex(P => new { P.market, P.type, P.time_start });
            o.Property(P => P.id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("K线ID");
            o.Property(P => P.market).IsRequired().HasColumnType("bigint").HasComment("交易对");
            o.Property(P => P.symbol).HasColumnType("nvarchar").HasMaxLength(20).HasComment("交易对名称");
            o.Property(P => P.type).IsRequired().HasColumnType("tinyint").HasComment("K线类型");
            o.Property(P => P.amount).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交量");
            o.Property(P => P.count).IsRequired().HasColumnType("bigint").HasComment("成交笔数");
            o.Property(P => P.total).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交总额");
            o.Property(P => P.open).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("开盘价");
            o.Property(P => P.close).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("收盘价");
            o.Property(P => P.low).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("最低价");
            o.Property(P => P.high).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("最高价");
            o.Property(P => P.time_start).IsRequired().HasColumnType("datetimeoffset").HasComment("变更开始时间");
            o.Property(P => P.time_end).IsRequired().HasColumnType("datetimeoffset").HasComment("变更开始时间");
            o.Property(P => P.time).IsRequired().HasColumnType("datetimeoffset").HasComment("更新时间");
            o.ToTable(nameof(Kline));
        });
        modelBuilder.Entity<MarketInfo>(o =>
        {
            o.HasKey(p => p.market);
            o.HasIndex(P => new { P.symbol }).IsUnique();
            o.Property(P => P.market).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("交易对");
            o.Property(P => P.symbol).HasColumnType("nvarchar").HasMaxLength(20).HasComment("交易对名称");
            o.Property(P => P.price_places).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("价格小数位数");
            o.Property(P => P.amount_places).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("量小数位数");
            o.Property(P => P.amount_multiple).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("交易量整数倍数");
            o.Property(P => P.fee).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("手续费");
            o.ToTable(nameof(MarketInfo));
        });
        modelBuilder.Entity<Orders>(o =>
        {
            o.HasKey(p => p.order_id);
            o.HasIndex(P => new { P.market, P.state });
            o.HasIndex(P => new { P.market, P.uid });
            o.HasIndex(P => new { P.create_time });
            o.Property(P => P.order_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("订单ID");
            o.Property(P => P.market).IsRequired().HasColumnType("bigint").HasComment("交易对");
            o.Property(P => P.symbol).HasColumnType("nvarchar").HasMaxLength(20).HasComment("交易对名称");
            o.Property(P => P.client_id).HasColumnType("nvarchar").HasMaxLength(50).HasComment("客户自定义订单id");
            o.Property(P => P.uid).IsRequired().HasColumnType("bigint").HasComment("用户ID");
            o.Property(P => P.price).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交价");
            o.Property(P => P.amount).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交量");
            o.Property(P => P.total).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("成交总额");
            o.Property(P => P.create_time).IsRequired().HasColumnType("datetimeoffset").HasComment("挂单时间");
            o.Property(P => P.amount_unsold).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("未成交量");
            o.Property(P => P.amount_done).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("已成交挂单量");
            o.Property(P => P.deal_last_time).HasColumnType("datetimeoffset").HasComment("最后成交时间");
            o.Property(P => P.side).IsRequired().HasColumnType("tinyint").HasComment("交易方向");
            o.Property(P => P.state).IsRequired().HasColumnType("tinyint").HasComment("成交状态");
            o.Property(P => P.type).IsRequired().HasColumnType("tinyint").HasComment("订单类型");
            o.Property(P => P.data).HasColumnType("nvarchar").HasMaxLength(200).HasComment("附加数据");
            o.Property(P => P.trigger_hanging_price).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("触发挂单价格");
            o.Property(P => P.remarks).HasColumnType("nvarchar").HasMaxLength(200).HasComment("备注");
            o.ToTable(nameof(Order));
        });
        modelBuilder.Entity<Users>(o =>
        {
            o.HasKey(p => p.user_id);
            o.HasIndex(P => new { P.user_name }).IsUnique();
            o.Property(P => P.user_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("用户id");
            o.Property(P => P.user_name).HasColumnType("nvarchar").HasMaxLength(50).HasComment("用户名");
            o.Property(P => P.password).HasColumnType("nvarchar").HasMaxLength(500).HasComment("用户密码");
            o.Property(P => P.phone).HasColumnType("nvarchar").HasMaxLength(500).HasComment("用户手机号码");
            o.Property(P => P.email).HasColumnType("nvarchar").HasMaxLength(500).HasComment("邮箱");
            o.ToTable(nameof(Users));
        });
        modelBuilder.Entity<Wallet>(o =>
        {
            o.HasKey(p => p.wallet_id);
            o.HasIndex(P => new { P.wallet_id, P.user_id, P.coin_id }).IsUnique();
            o.Property(P => P.wallet_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("钱包id");
            o.Property(P => P.user_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment("用户id");
            o.Property(P => P.user_name).HasColumnType("nvarchar").HasMaxLength(50).HasComment("用户名");
            o.Property(P => P.coin_id).IsRequired().ValueGeneratedNever().HasColumnType("bigint").HasComment(" 币id");
            o.Property(P => P.coin_name).HasColumnType("nvarchar").HasMaxLength(20).HasComment("币名称");
            o.Property(P => P.total).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("总额");
            o.Property(P => P.available).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("可用");
            o.Property(P => P.freeze).IsRequired().HasColumnType("decimal").HasPrecision(28, 16).HasComment("冻结");
            o.ToTable(nameof(Wallet));
        });


        base.OnModelCreating(modelBuilder);
    }
}

