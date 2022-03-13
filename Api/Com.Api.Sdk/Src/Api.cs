using System;
using Com.Db.Enum;

namespace Com.Api.Sdk;

public class Api
{
    /// <summary>
    /// 请求web数据
    /// </summary>
    /// <param name="url">接口地址</param>
    /// <param name="input">输入数据</param>
    /// <typeparam name="T">输出类型</typeparam>
    /// <returns>返回数据</returns>
    // public AjaxResult<T> GetPost<T>(string url, object input = null, Method method = Method.POST)
    // {
    //     RestRequest request = new RestRequest(url, method);
    //     request.AddHeader("X-IDCM-APIKEY", this.api_key);
    //     string json = "";
    //     try
    //     {
    //         if (input != null)
    //         {
    //             json = JsonConvert.SerializeObject(input);
    //             if (input is Dictionary<string, string>)
    //             {
    //                 foreach (var item in input as Dictionary<string, string>)
    //                 {
    //                     request.AddParameter(item.Key, item.Value);
    //                 }
    //             }
    //             else
    //             {
    //                 request.AddParameter("application/json", json, ParameterType.RequestBody);
    //                 this.logger.LogTrace(eventId, url + ":" + json);
    //             }
    //         }
    //         IRestResponse<AjaxResult<T>> asyncHandle = client.Execute<AjaxResult<T>>(request);
    //         if (asyncHandle.ErrorException != null)
    //         {
    //             const string message = "Error retrieving response.  Check inner details for more info.";
    //             var twilioException = new ApplicationException(message, asyncHandle.ErrorException);
    //             throw twilioException;
    //         }
    //         if (asyncHandle.StatusCode == HttpStatusCode.OK && asyncHandle.Data != null)
    //         {
    //             return asyncHandle.Data;
    //         }
    //         else if (!string.IsNullOrWhiteSpace(asyncHandle.Content))
    //         {
    //             return JsonConvert.DeserializeObject<AjaxResult<T>>(asyncHandle.Content);
    //         }
    //         else
    //         {
    //             this.logger.LogError(eventId, asyncHandle.ErrorException, $"url:{url},input:{json},result:{asyncHandle.Content}");
    //             return default(AjaxResult<T>);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         this.logger.LogError(eventId, ex, $"url:{url},input:{json}");
    //     }
    //     return default(AjaxResult<T>);
    // }

}