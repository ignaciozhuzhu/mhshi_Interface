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
using System.IO;
using System.Web.Configuration;

namespace demo.ajax
{
    /// <summary>
    /// baichuanHandler 的摘要说明
    /// </summary>
    public class demoHandler : IHttpHandler
    {

        //美人芯微信公众号
        const string appid = "wxbf5afc94a591cdd8";
        static string secret = WebConfigurationManager.AppSettings["mrxsecret"]; 


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


        //根据openid获取用户信息
        private void getuserinfobyopenid()
        {
            var openid = (HttpContext.Current.Request["openid"]).ToString();
            string api = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + gettoken() + "&openid=" + openid + "&lang=zh_CN";
            var response = HttpUtil.Get(api);
            HttpContext.Current.Response.Write(response);
        }

        //显示token
        private void showtoken()
        {
            string api = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret + "";
            var response = HttpUtil.Get(api);
            var result = JsonConvert.DeserializeObject<AccessToken>(response);
            var access_token = result.access_token;
            string json = "{\"data\":\"" + access_token + "\"}";
            HttpContext.Current.Response.Write(json);
        }

        //获取token
        private string gettoken()
        {
            string api = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret + "";
            var response = HttpUtil.Get(api);
            var result = JsonConvert.DeserializeObject<AccessToken>(response);
            var access_token = result.access_token;
            //HttpContext.Current.Response.Write(access_token);
            return access_token;
        }
        //获取ticket
        //如果sig invalid ,https://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=jsapisign可以在线调试
        //优先查看是否是getticket方法获取不到ticket,因为ip变化所以白名单需要修改.
        //其次查看是否是使用https://www.mymengqiqi.com/ 要一模一样的url才可以,而不是http://www.mymengqiqi.com/
        private string getticket()
        {
            string api = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + gettoken() + "&type=jsapi";
            var response = HttpUtil.Get(api);
            var result = JsonConvert.DeserializeObject<Ticket>(response);
            var ticket = result.ticket;
            //HttpContext.Current.Response.Write(ticket);
            return ticket;
        }

        //根据token和 mediaID获取服务器的文件,返回base64 ,保存在本地服务器
        private void getbase64bymedia()
        {
            var media_id = (HttpContext.Current.Request["media_id"]).ToString();
            string api = "https://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + gettoken() + "&media_id=" + media_id + "";
          //  var response = HttpUtil.Get(api);

            //将string结果集转为base64
          //  byte[] b = System.Text.Encoding.Default.GetBytes(response);
         //   response = Convert.ToBase64String(b);
        //    string json = "{\"data\":\"" + response + "\"}";
            HttpContext.Current.Response.Write(api);
        }

        private void getsig()
        {
            var url = (HttpContext.Current.Request["url"]).ToString();
            string str = "";
            try
            {
                string from = (HttpContext.Current.Request["from"]).ToString();
                if (from != "")
                {
                    str += "&from=" + from + "";
                }
            }
            catch { }
            try
            {
                string isappinstalled = (HttpContext.Current.Request["isappinstalled"]).ToString();
                if (isappinstalled != "")
                {
                    str += "&isappinstalled=" + isappinstalled + "";
                }
            }
            catch { }
            string string1 = "jsapi_ticket=" + getticket() + "&noncestr=zhusheng123&timestamp=1387970259&url=" + url + str + "";
            HttpContext.Current.Response.Write(EncryptToSHA1(string1));
        }
        private void getuserinfo()
        {
            var appid = (HttpContext.Current.Request["appid"]).ToString();
            var appsecret = (HttpContext.Current.Request["appsecret"]).ToString();
            var code = (HttpContext.Current.Request["code"]).ToString();
            //获取token和openid
            string api0 = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + code + "&grant_type=authorization_code";
            var response0 = HttpUtil.Get(api0);
            var result0 = JsonConvert.DeserializeObject<TokenAndOpenid>(response0);
            var access_token = result0.access_token;
            var openid = result0.openid;

            //获取userinfo
           // string api = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN";
            string api = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN";
            var response = HttpUtil.Get(api);
            var result = JsonConvert.DeserializeObject<UserInfo>(response);
            //  var json = "{\"data\":\"" + response + "\"}";
            //  var json = "{" + response.Replace("\"", "/\"") + "}";
            //  string json = "{\"data\":\"添加成功\"}";
            HttpContext.Current.Response.Write(response);
        }

        private void getopenid()
        {
            var url = (HttpContext.Current.Request["url"]).ToString();
            string str = "";
            try
            {
                string from = (HttpContext.Current.Request["from"]).ToString();
                if (from != "")
                {
                    str += "&from=" + from + "";
                }
            }
            catch { }
            try
            {
                string isappinstalled = (HttpContext.Current.Request["isappinstalled"]).ToString();
                if (isappinstalled != "")
                {
                    str += "&isappinstalled=" + isappinstalled + "";
                }
            }
            catch { }
            string string1 = "jsapi_ticket=" + getticket() + "&noncestr=zhusheng123&timestamp=1387970259&url=" + url + str + "";
            HttpContext.Current.Response.Write(EncryptToSHA1(string1));
        }

        #region 获取由SHA1加密的字符串
        public string EncryptToSHA1(string str)
        {
            // return  System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");

            //建立SHA1对象

            SHA1 sha = new SHA1CryptoServiceProvider();

            //将mystr转换成byte[]

            ASCIIEncoding enc = new ASCIIEncoding();

            byte[] dataToHash = enc.GetBytes(str);

            //Hash运算

            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string

            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash;

        }
        #endregion


        /// <summary>
        /// AccessToken Class
        /// </summary>
        public class AccessToken
        {
            private string _accesstoken;
            public string access_token
            {
                set { _accesstoken = value; }
                get { return _accesstoken; }
            }
        }
        /// <summary>
        /// Ticket Class
        /// </summary>
        public class Ticket
        {
            private string _ticket;
            public string ticket
            {
                set { _ticket = value; }
                get { return _ticket; }
            }
        }

        /// <summary>
        /// TokenAndOpenid Class
        /// </summary>
        public class TokenAndOpenid
        {
            private string _accesstoken;
            public string access_token
            {
                set { _accesstoken = value; }
                get { return _accesstoken; }
            }
            private string _openid;
            public string openid
            {
                set { _openid = value; }
                get { return _openid; }
            }
        }

        /// <summary>
        /// userInfo Class
        /// </summary>
        public class UserInfo
        {
            private string _openid;
            private string _nickname;
            private string _sex;
            private string _province;
            private string _city;
            private string _country;
            private string _headimgurl;
            public string openid
            {
                set { _openid = value; }
                get { return _openid; }
            }
            public string nickname
            {
                set { _nickname = value; }
                get { return _nickname; }
            }
            public string sex
            {
                set { _sex = value; }
                get { return _sex; }
            }
            public string province
            {
                set { _province = value; }
                get { return _province; }
            }
            public string city
            {
                set { _city = value; }
                get { return _city; }
            }
            public string country
            {
                set { _country = value; }
                get { return _country; }
            }
            public string headimgurl
            {
                set { _headimgurl = value; }
                get { return _headimgurl; }
            }
        }

    }
}