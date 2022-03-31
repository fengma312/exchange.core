using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Com.Api.Sdk.Enum;
using Com.Api.Sdk.Models;
using Com.Bll.Util;
using Com.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Com.Bll;

/// <summary>
/// Service:用户
/// </summary>
public class ServiceUser
{
    /// <summary>
    /// 公共类
    /// </summary>
    /// <returns></returns>
    private Common common = new Common();

    /// <summary>
    /// 初始化
    /// </summary>
    public ServiceUser()
    {
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="user_name"></param>
    /// <param name="password"></param>
    /// <param name="phone"></param>
    /// <param name="email"></param>
    /// <param name="app"></param>
    /// <param name="ip"></param>
    public Res<long> Register(string user_name, string password, string phone, string email, string app, string ip)
    {
        Res<long> res = new Res<long>();
        res.success = false;
        res.code = E_Res_Code.fail;
        if (string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(password))
        {
            res.message = "用户名或密码不能为空";
            return res;
        }
        (string public_key, string private_key) key_btc_user = Encryption.GetRsaKey();
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                if (db.Users.Any(P => P.phone == phone))
                {
                    res.message = "手机号已重复";
                    return res;
                }
                if (db.Users.Any(P => P.email == email))
                {
                    res.message = "邮箱已重复";
                    return res;
                }
                Users settlement_btc_usdt = new Users()
                {
                    user_id = FactoryService.instance.constant.worker.NextId(),
                    user_name = user_name,
                    password = Encryption.SHA256Encrypt(password),
                    disabled = false,
                    transaction = true,
                    withdrawal = true,
                    phone = phone,
                    email = email,
                    vip = 0,
                    public_key = key_btc_user.public_key,
                    private_key = key_btc_user.private_key,
                };
                db.Users.Add(settlement_btc_usdt);
                db.SaveChanges();
            }
        }
        return res;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="account">账号</param>
    /// <param name="password">密码</param>
    /// <param name="no">验证码编号</param>
    /// <param name="code">验证码</param>
    /// <param name="app">登录终端</param>
    /// <param name="ip">登录ip</param>
    /// <returns></returns>
    public Res<ResUser> Login(string account, string password, long no, string code, string app, string ip)
    {
        Res<ResUser> res = new Res<ResUser>();
        res.success = false;
        res.code = E_Res_Code.fail;
        if (!VerificationCode(no, code))
        {
            res.message = "验证码错误";
            return res;
        }
        FactoryService.instance.constant.redis.KeyDelete(FactoryService.instance.GetRedisVerificationCode(no));
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                var user = db.Users.FirstOrDefault(P => P.disabled == false && (P.user_name == account || P.phone == account || P.email == account) && P.password == password);
                if (user == null)
                {
                    res.message = "账户名或密码错误";
                    return res;
                }
                var token = GenerateToken(FactoryService.instance.constant.worker.NextId(), user, app);
                res.data = user;
                res.data.token = token;
                res.success = true;
                res.code = E_Res_Code.ok;
                return res;
            }
        }
    }

    public void logout()
    {

    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    public (long, string) GetVerificationCode()
    {
        long no = FactoryService.instance.constant.worker.NextId();
        string verify = common.CreateRandomCode(4);
        byte[] b = common.CreateImage(verify);
        FactoryService.instance.constant.redis.StringSet(FactoryService.instance.GetRedisVerificationCode(no), verify, TimeSpan.FromMinutes(5));
        return (no, Convert.ToBase64String(b));
    }

    /// <summary>
    /// 校验验证码
    /// </summary>
    /// <param name="no">编号</param>
    /// <param name="code">验证码</param>
    /// <returns></returns>
    public bool VerificationCode(long no, string code)
    {
        string verify = FactoryService.instance.constant.redis.StringGet(FactoryService.instance.GetRedisVerificationCode(no));
        if (verify != null && verify.ToLower() == code.ToLower())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public Users? GetUser(long uid)
    {
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                return db.Users.AsNoTracking().SingleOrDefault(P => P.user_id == uid);
            }
        }
    }

    /// <summary>
    /// 获取vip
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public Vip? GetVip(long id)
    {
        using (var scope = FactoryService.instance.constant.provider.CreateScope())
        {
            using (DbContextEF db = scope.ServiceProvider.GetService<DbContextEF>()!)
            {
                return db.Vip.AsNoTracking().SingleOrDefault(P => P.id == id);
            }
        }
    }

    /// <summary>
    /// 生成token
    /// </summary>
    /// <param name="no">登录唯一码</param>
    /// <param name="user">用户信息</param>
    /// <param name="timeout"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public string GenerateToken(long no, Users user, string app)
    {
        var claims = new[]
            {
                new Claim("no",no.ToString()),
                new Claim("user_id",user.user_id.ToString()),
                new Claim("user_name",user.user_name),
                new Claim("app", app),
                new Claim("public_key", user.public_key),
            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FactoryService.instance.constant.config["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: FactoryService.instance.constant.config["Jwt:Issuer"],// 签发者
            audience: FactoryService.instance.constant.config["Jwt:Audience"],// 接收者
            expires: DateTime.Now.AddMinutes(double.Parse(FactoryService.instance.constant.config["Jwt:Expires"])),// 过期时间
            claims: claims,// payload
            signingCredentials: creds);// 令牌
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}