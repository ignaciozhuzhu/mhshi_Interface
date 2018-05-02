using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Trip.Core;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Interface.Common;
using System.Web.Configuration;

namespace sms.ajax
{
    /// <summary>
    /// baichuanHandler 的摘要说明
    /// </summary>
    public class smsHandler : IHttpHandler
    {
        static string accessKeyId = WebConfigurationManager.AppSettings["accessKeyId"];
        const string accessKeySecret = "JEAhNWGo75Ootz8sVI7vSkO0EWfYnH";
        const string product = "Dysmsapi";//短信API产品名称
        const string domain = "dysmsapi.aliyuncs.com";//短信API产品域名

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Type type = this.GetType();
                string fn = context.Request["fn"].ToString();
                MethodInfo method = type.GetMethod(fn, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(this, null);
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write("接口出错!");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private void ResponseWriteEnd(HttpContext context, string msg)
        {
            context.Response.Write(msg);
            context.Response.End();
        }

        public static string querySendDetailsByMobile(string mobile)
        {
            //初始化acsClient,暂不支持region化
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            //组装请求对象
            QuerySendDetailsRequest request = new QuerySendDetailsRequest();
            //必填-号码
            request.PhoneNumber = mobile;
            //可选-流水号
            // request.BizId = bizId;
            //必填-发送日期 支持30天内记录查询，格式yyyyMMdd       
            request.SendDate = DateTime.Now.ToString("yyyyMMdd");
            //必填-页大小
            request.PageSize = 10;
            //必填-当前页码从1开始计数
            request.CurrentPage = 1;
            QuerySendDetailsResponse querySendDetailsResponse = null;
            try
            {
                querySendDetailsResponse = acsClient.GetAcsResponse(request);
            }
            catch (ServerException e)
            {
                return "查询出错!";
            }
            string json = "";
            json = "{\"data\":\"" + querySendDetailsResponse.SmsSendDetailDTOs[0].Content + "\"}";
            return json;
        }
        public static string sendSMS(string mobile)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = mobile;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "美好食";
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = "SMS_126290084";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = "{\"code\":\"" + ConvertJson.randomCode() + "\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                // request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                //  return sendSmsResponse.Message;
                string json = "";
                if (sendSmsResponse.Message == "OK")
                    json = "{\"data\":\"发送成功\"}";
                else json = "{\"data\":\"" + sendSmsResponse.Message + "\"}";
                return json;
            }
            catch (ServerException e)
            {
                return "发送失败!";
            }
        }
        private void getsms()
        {
            string mobile = "";
            try
            {
                mobile = HttpContext.Current.Request["mobile"];
            }
            catch { }
            var result = querySendDetailsByMobile(mobile);
            HttpContext.Current.Response.Write(result);
        }

        private void send()
        {
            string mobile = "";
            try
            {
                mobile = HttpContext.Current.Request["mobile"];
            }
            catch { }
            var result = sendSMS(mobile);
            HttpContext.Current.Response.Write(result);
        }

    }
}