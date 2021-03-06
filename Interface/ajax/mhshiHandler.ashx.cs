﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Trip.Core;
using System.Data;
using Maticsoft.DBUtility;
using Interface.Common;
using System.Data.SqlClient;
using System.Net;
using System.Web.Configuration;

namespace mhshi.ajax
{
    /// <summary>
    /// baichuanHandler 的摘要说明
    /// </summary>
    public class mhshiHandler : IHttpHandler
    {
        static string CONN = WebConfigurationManager.AppSettings["conn"];

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

        #region 新增问题
        //新增问题
        private void insertquestion()
        {
            int anserType = 0;
            try
            {
                anserType = Convert.ToInt32(HttpContext.Current.Request["anserType"]);
            }
            catch { }
            int direType = 0;
            try
            {
                direType = Convert.ToInt32(HttpContext.Current.Request["direType"]);
            }
            catch { }
            string content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            string asker = "";
            try
            {
                asker = HttpContext.Current.Request["asker"];
            }
            catch { }
            int oxygen = 0;
            try
            {
                oxygen = Convert.ToInt32(HttpContext.Current.Request["oxygen"]);
            }
            catch { }
            int status = 0;
            try
            {
                status = Convert.ToInt32(HttpContext.Current.Request["status"]);
            }
            catch { }
            string askDate = "";
            try
            {
                askDate = HttpContext.Current.Request["askDate"];
            }
            catch { }
            int isPick = 0;
            try
            {
                isPick = Convert.ToInt32(HttpContext.Current.Request["isPick"]);
            }
            catch { }
            var ticket = insertQuestion(anserType, direType, content, asker, oxygen, status, askDate, isPick);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 新增问题
        /// </summary>
        public static string insertQuestion(int anserType, int direType, string content, string asker, int oxygen, int status, string askDate, int isPick)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "DECLARE @askerid int;";
            if (asker != "0")
                str += "SET @askerid = (select id from mhs_t_user where openid='" + asker + "');";
            else str += "SET @askerid=" + asker + "";
            lo_cmd.CommandText = str + " insert into mhs_t_question(anserType,direType,content,asker,oxygen,status,askDate,isPick) values(" + anserType + "," + direType + ",'" + content + "',@askerid," + oxygen + "," + status + ",'" + askDate + "'," + isPick + ")";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            // string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            string json = "{\"data\":\"添加成功\"}";
            return json;
        }

        #endregion

        #region 删除问题
        //删除问题
        private void delquestionbyid()
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = delQuestionById(id);
            HttpContext.Current.Response.Write(ticket);
        }
        /// <summary>
        /// 删除问题
        /// </summary>
        public static string delQuestionById(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            // lo_cmd.CommandText = "select * from dbo.linecategory";   //写SQL语句
            lo_cmd.CommandText = "update mhs_t_question set isDel=1 where id =" + id + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "";
            if (dbAdapter.UpdateBatchSize > 0)
                json = "{\"data\":" + dbAdapter.UpdateBatchSize + "}";
            return json;
        }
        #endregion

        #region 更新问题
        //更新问题
        private void updatequestionbyid()
        {
            int id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var ticket = updateQuestionById(id, content);
            HttpContext.Current.Response.Write(ticket);
        }
        /// <summary>
        /// 更新问题
        /// </summary>
        public static string updateQuestionById(int id, string content)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            // lo_cmd.CommandText = "select * from dbo.linecategory";   //写SQL语句
            lo_cmd.CommandText = "update mhs_t_question set content='" + content + "' where id =" + id + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "";
            if (dbAdapter.UpdateBatchSize > 0)
                json = "{\"data\":" + dbAdapter.UpdateBatchSize + "}";
            return json;
        }
        #endregion

        #region 获取问题列表
        //获取问题列表
        private void getquestionlist()
        {
            var content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var beginDate = "";
            try
            {
                beginDate = HttpContext.Current.Request["beginDate"];
            }
            catch { }
            var endDate = "";
            try
            {
                endDate = HttpContext.Current.Request["endDate"];
            }
            catch { }
            var status = "";
            try
            {
                status = HttpContext.Current.Request["status"];
            }
            catch { }
            var anserType = "";
            try
            {
                anserType = HttpContext.Current.Request["anserType"];
            }
            catch { }
            var isPick = "";
            try
            {
                isPick = HttpContext.Current.Request["isPick"];
            }
            catch { }
            var id = "";
            try
            {
                id = HttpContext.Current.Request["id"];
            }
            catch { }
            var answerid = "";
            try
            {
                answerid = HttpContext.Current.Request["answerid"];
            }
            catch { }
            var canAnswer = "";
            try
            {
                canAnswer = HttpContext.Current.Request["canAnswer"];
            }
            catch { }
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var str = " where 1=1 ";
            if (content != "" && content != null)
                str += "and ([content] like '%" + content + "%' or c.nickName like '%" + content + "%')";
            if (beginDate != "" && beginDate != null)
                str += " and askDate >=  '" + beginDate + "'";
            if (endDate != "" && endDate != null)
                str += " and askDate <=  '" + endDate + "'";
            if (status != "" && status != null)
                str += " and status =  " + status + "";
            if (anserType != "" && anserType != null)
                str += " and anserType =  " + anserType + "";
            if (isPick != "" && isPick != null)
                str += " and isPick =  " + isPick + "";
            if (id != "" && id != null)
                str += " and a.id =  " + id + "";
            if (canAnswer != "" && canAnswer != null)
                str += " and ISNULL(a.answer,0)  =  0 ";
            //增加回答者id作为筛选 , 某医生的回答
            if (answerid != "" && answerid != null && answerid != "0")
                str += " and a.answer =  " + answerid + " ";
            var ticket = getQuestionList(str, pageNum, pageSize);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 获取问题列表
        /// </summary>
        public static string getQuestionList(string str, int pageNum, int pageSize)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            //    lo_cmd.CommandText = "select a.*,b.nickName,b.avatar,c.avatar as docAvatar,c.nickName as docName,c.special as docSpe,c.fans,c.answeredNum,c.intro,c.hosName from mhs_t_question a left join mhs_t_user b on a.asker=b.id left join mhs_t_user c on a.answer=c.id " + str;
            lo_cmd.CommandText = "select top " + pageSize + "*from(select row_number()over(order by askDate desc)as rownumber,a.*,b.nickName,b.avatar,c.avatar as docAvatar,c.nickName as docName,c.special as docSpe,c.fans,c.answeredNum,c.intro,c.hosName,d.content as acontent from mhs_t_question a left join mhs_t_user b on a.asker=b.id left join mhs_t_user c on a.answer=c.id left join mhs_t_answer d on d.qid=a.id " + str + ")as t where rownumber>" + pageSize * pageNum + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"data\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion

        #region 获取医生列表
        //获取医生列表
        private void getdoc()
        {
            var str = " where 1=1 and type=2 ";
            var ticket = getdocList(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 获取医生列表
        /// </summary>
        public static string getdocList(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            // lo_cmd.CommandText = "select * from dbo.linecategory";   //写SQL语句
            lo_cmd.CommandText = "select * from mhs_t_user " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"data\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 查看是否已绑定过用户
        /// <summary>
        /// 查看是否已绑定过用户
        /// </summary>
        private void existuser()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var result = existUser(openid);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 查看是否已绑定过用户
        /// </summary>
        public static string existUser(string openid)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象

            lo_cmd.CommandText = "select * from mhs_t_user where openid=  '" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
                result = "{\"data\":\"已有会员信息,无需重复添加\"}";
            else
            {
                result = "{\"data\":\"去添加\"}";
            }
            con.Close();
            return result;
        }
        #endregion

        #region 新增用户
        /// <summary>
        /// 新增用户
        /// </summary>
        private void insertuser()
        {
            var type = "1";
            try
            {
                type = HttpContext.Current.Request["type"].ToString();
            }
            catch { }
            var nickName = "";
            try
            {
                nickName = HttpContext.Current.Request["nickName"].ToString();
            }
            catch { }
            var realName = nickName;
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var avatar = "";
            try
            {
                avatar = HttpContext.Current.Request["avatar"].ToString();
            }
            catch { }
            var oxygen = "200";
            try
            {
                oxygen = HttpContext.Current.Request["oxygen"].ToString();
            }
            catch { }
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var regTime = "";
            try
            {
                regTime = HttpContext.Current.Request["regTime"].ToString();
            }
            catch { }
            var result = insertUser(type, nickName, realName, phone, avatar, oxygen, openid, regTime);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        public static string insertUser(string type, string nickName, string realName, string phone, string avatar, string oxygen, string openid, string regTime)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象

            lo_cmd.CommandText = "select * from mhs_t_user where openid=  '" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
                result = "{\"data\":\"已有会员信息,无需重复添加\"}";
            else
            {
                lo_cmd.CommandText = "insert into mhs_t_user(type,nickName,realName,phone,avatar,oxygen,openid,regTime) values(" + type + ",'" + nickName + "','" + realName + "','" + phone + "','" + avatar + "'," + oxygen + ",'" + openid + "','" + regTime + "')";
                lo_cmd.Connection = con;             //指定连接对象，即上面创建的
                // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
                dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
                ds = new DataSet(); //创建数据集对象
                dbAdapter.Fill(ds);
                string json = ConvertJson.DataTable2Array(ds.Tables[0]);
                result = "{\"data\":\"添加成功\"}";
            }
            con.Close();
            return result;
        }
        #endregion

        #region 我提问的
        /// <summary>
        /// 我提问的
        /// </summary>
        private void myques()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var result = myQues(openid, pageNum, pageSize);
            HttpContext.Current.Response.Write(result);
        }
        /// <summary>
        /// 我提问的
        /// </summary>
        public static string myQues(string openid, int pageNum, int pageSize)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            //   lo_cmd.CommandText = "select a.*,c.nickName as docName,c.special as docSpe,c.hosName from mhs_t_question a inner join mhs_t_user b on a.asker=b.id left join mhs_t_user c on a.answer=c.id where b.openid=  '" + openid + "'";
            var str = "DECLARE @count int;set @count=(select count(*) from mhs_t_question a inner join mhs_t_user b on a.asker=b.id where b.openid='" + openid + "');";
            lo_cmd.CommandText = str + "select top " + pageSize + "* from(select row_number()over(order by askDate desc)as rownumber,a.*,c.nickName as docName,c.special as docSpe,c.hosName from mhs_t_question a inner join mhs_t_user b on a.asker=b.id left join mhs_t_user c on a.answer=c.id where b.openid= '" + openid + "') as t where rownumber>" + pageSize * pageNum + ";select @count as count;";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + ",\"count\":" + ds.Tables[1].Rows[0][0] + "}";
            return result;
        }
        #endregion

        #region 我听过的
        /// <summary>
        /// 我听过的
        /// </summary>
        private void mylisten()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var result = myListen(openid, pageNum, pageSize);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 我听过的
        /// </summary>
        public static string myListen(string openid, int pageNum, int pageSize)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            //   lo_cmd.CommandText = "select c.*,d.nickName as docName,d.special as docSpe,d.hosName from mhs_t_user a inner join mhs_t_order b on a.id=b.uid inner join mhs_t_question c on c.id=b.qid left join mhs_t_user d on c.answer=d.id where a.openid=  '" + openid + "'";
            var str = "DECLARE @count int;set @count=(select count(*) from mhs_t_user a inner join mhs_t_order b on a.id=b.uid inner join mhs_t_question c on c.id=b.qid where openid='" + openid + "');";
            lo_cmd.CommandText = str + "select top " + pageSize + "* from(select row_number()over(order by askDate desc)as rownumber,c.*,d.nickName as docName,d.special as docSpe,d.hosName from mhs_t_user a inner join mhs_t_order b on a.id=b.uid inner join mhs_t_question c on c.id=b.qid left join mhs_t_user d on c.answer=d.id where a.openid='" + openid + "')as t where rownumber>" + pageSize * pageNum + ";select @count as count;";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + ",\"count\":" + ds.Tables[1].Rows[0][0] + "}";
            return result;
        }
        #endregion

        #region 获取氧分明细
        /// <summary>
        /// 获取氧分明细
        /// </summary>
        private void getoxygendetail()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var result = getOxygenDetail(openid, pageNum, pageSize);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 获取氧分明细
        /// </summary>
        public static string getOxygenDetail(string openid, int pageNum, int pageSize)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            //lo_cmd.CommandText = "select -a.oxygen as oxygen,a.askDate,'提问（消耗）' as type from mhs_t_question a inner join mhs_t_user b on a.asker=b.id where b.openid='" + openid + "' union select -c.oxygen as oxygen,c.askDate,'偷听（消耗）' as type from mhs_t_user a inner join mhs_t_order b on a.id=b.uid inner join mhs_t_question c on c.id=b.qid and c.asker!=b.uid where a.openid='" + openid + "' and b.qid<>2 union select '200' as oxygen,regTime as 'askDate','绑定（赠送）' as type from mhs_t_user where  openid='" + openid + "' order by askDate desc";
            lo_cmd.CommandText = "select top " + pageSize + "*from(select row_number()over(order by askDate desc)as rownumber,*from(select-a.oxygen as oxygen,a.askDate,'提问（消耗）'as type,b.openid from mhs_t_question a inner join mhs_t_user b on a.asker=b.id union select-c.oxygen/2 as oxygen,c.askDate,'偷听（消耗）'as type,a.openid from mhs_t_user a inner join mhs_t_order b on a.id=b.uid inner join mhs_t_question c on c.id=b.qid and c.asker!=b.uid where b.qid<>2 union select'200'as oxygen,regTime as'askDate','绑定（赠送）'as type,openid from mhs_t_user)as t where openid='" + openid + "')tt where rownumber>" + pageSize * pageNum + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + "}";
            return result;
        }
        #endregion

        #region 根据openid获取该用户信息
        /// <summary>
        /// 根据openid获取该用户信息
        /// </summary>
        private void getuserinfobyopenid()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var result = getUserinfoByOpenid(openid);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 根据openid获取该用户信息
        /// </summary>
        public static string getUserinfoByOpenid(string openid)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select * from mhs_t_user where openid='" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + "}";
            return result;
        }
        #endregion

        #region 根据userid获取该用户信息
        /// <summary>
        /// 根据userid获取该用户信息
        /// </summary>
        private void getuserinfobyid()
        {
            var id = "";
            try
            {
                id = HttpContext.Current.Request["id"].ToString();
            }
            catch { }
            var result = getUserinfoByid(id);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 根据userid获取该用户信息
        /// </summary>
        public static string getUserinfoByid(string id)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select * from mhs_t_user where id=" + id + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + "}";
            return result;
        }
        #endregion


        #region 根据openid保存更新用户信息
        /// <summary>
        /// 根据openid保存更新用户信息
        /// </summary>
        private void updateuserinfobyopenid()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var IDNo = "";
            try
            {
                IDNo = HttpContext.Current.Request["IDNo"].ToString();
            }
            catch { }
            var height = "";
            try
            {
                height = HttpContext.Current.Request["height"].ToString();
            }
            catch { }
            var weight = "";
            try
            {
                weight = HttpContext.Current.Request["weight"].ToString();
            }
            catch { }
            var result = updateUserinfoByOpenid(openid, name, phone, IDNo, height, weight);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 根据openid获取该用户信息
        /// </summary>
        public static string updateUserinfoByOpenid(string openid, string name, string phone, string IDNo, string height, string weight)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "update mhs_t_user set realName='" + name + "',phone='" + phone + "',IDNo='" + IDNo + "',height='" + height + "',weight='" + weight + "' where openid='" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        #region 我关注的医生列表
        /// <summary>
        /// 我关注的医生列表
        /// </summary>
        private void myfollow()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var result = myFollow(openid, pageNum, pageSize);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 我关注的医生列表
        /// </summary>
        public static string myFollow(string openid, int pageNum, int pageSize)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select top " + pageSize + "*from(select row_number()over(order by c.id)as rownumber,c.* from mhs_t_follow a inner join mhs_t_user b on a.uid=b.id inner join mhs_t_user c on a.followUid=c.id where b.openid='" + openid + "')as t where rownumber>" + pageSize * pageNum + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + "}";
            return result;
        }
        #endregion


        #region 添加,取消关注
        /// <summary>
        /// 添加,取消关注
        /// </summary>
        private void editfollow()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var docid = 0;
            try
            {
                docid = Convert.ToInt32(HttpContext.Current.Request["docid"]);
            }
            catch { }
            var flag = 0;//1添加,-1删除
            try
            {
                flag = Convert.ToInt32(HttpContext.Current.Request["flag"]);
            }
            catch { }
            var result = editFollow(openid, docid, flag);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 添加,取消关注
        /// </summary>
        public static string editFollow(string openid, int docid, int flag)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象

            if (flag == 1)
                lo_cmd.CommandText = "DECLARE @id int,@following int,@fans int;SET @id = (select id from mhs_t_user where openid='" + openid + "');insert into mhs_t_follow(uid,followUid) values (@id," + docid + ");set @following = (select count(*) from mhs_t_follow where uid=@id);set @fans = (select count(*) from mhs_t_follow where followUid=" + docid + ");update mhs_t_user set following=@following where openid='" + openid + "';update mhs_t_user set fans=@fans where id='" + docid + "';";

            else if (flag == -1)
                lo_cmd.CommandText = "DECLARE @id int,@following int,@fans int;SET @id = (select id from mhs_t_user where openid='" + openid + "');delete from mhs_t_follow where uid=@id and followUid=" + docid + ";set @following = (select count(*) from mhs_t_follow where uid=@id);set @fans = (select count(*) from mhs_t_follow where followUid=" + docid + ");update mhs_t_user set following=@following where openid='" + openid + "';update mhs_t_user set fans=@fans where id='" + docid + "';";

            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion


        #region 保存录音
        /// <summary>
        /// 保存录音
        /// </summary>
        private void savevoice()
        {
            var uid = 0;
            try
            {
                uid = Convert.ToInt32(HttpContext.Current.Request["uid"]);
            }
            catch { }
            var qid = 0;
            try
            {
                qid = Convert.ToInt32(HttpContext.Current.Request["qid"]);
            }
            catch { }
            var voiceUrl = "";
            try
            {
                voiceUrl = HttpContext.Current.Request["voiceUrl"].ToString();
            }
            catch { }
            var content = "";
            try
            {
                content = HttpContext.Current.Request["content"].ToString();
            }
            catch { }
            var result = saveVoice(uid, qid, voiceUrl, content);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 保存录音
        /// </summary>
        public static string saveVoice(int uid, int qid, string voiceUrl, string content)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = " insert into mhs_t_answer(uid,qid,voiceUrl,content) values(" + uid + "," + qid + ",'" + voiceUrl + "','" + content + "');DECLARE @answerid int;set @answerid= (select isnull(answer,0) from mhs_t_question where id=35);if @answerid<>0 print @answerid; else update mhs_t_question set answer=" + uid + " where id=" + qid + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion


        #region 读取录音
        /// <summary>
        /// 读取录音
        /// </summary>
        private void getvoice()
        {
            var uid = 0;
            try
            {
                uid = Convert.ToInt32(HttpContext.Current.Request["uid"]);
            }
            catch { }
            var qid = 0;
            try
            {
                qid = Convert.ToInt32(HttpContext.Current.Request["qid"]);
            }
            catch { }
            var result = getVoice(uid, qid);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 读取录音
        /// </summary>
        public static string getVoice(int uid, int qid)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select * from mhs_t_answer where uid=" + uid + " and qid=" + qid + ";";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            result = "{\"data\":" + json.Replace("'", "\"") + "}";
            return result;
        }
        #endregion



        #region 认证营养师
        /// <summary>
        /// 认证营养师
        /// </summary>
        private void certificatebyopenid()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var hospital = "";
            try
            {
                hospital = HttpContext.Current.Request["hospital"].ToString();
            }
            catch { }
            var spec = "";
            try
            {
                spec = HttpContext.Current.Request["spec"].ToString();
            }
            catch { }
            var title = "";
            try
            {
                title = HttpContext.Current.Request["title"].ToString();
            }
            catch { }
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var IDNo = "";
            try
            {
                IDNo = HttpContext.Current.Request["IDNo"].ToString();
            }
            catch { }
            var cert = "";
            try
            {
                cert = HttpContext.Current.Request["cert"].ToString();
            }
            catch { }

            //var imgurl = "";
            //try
            //{
            //    imgurl = HttpContext.Current.Request["imgurl"].ToString();
            //}
            //catch { }

            var formData = WebUtility.HtmlDecode(HttpContext.Current.Request.Form.ToString());
            formData = HttpContext.Current.Server.UrlDecode(formData);
            formData = formData.Substring(5, formData.Length - 5);
            var formJson = JsonConvert.DeserializeObject<Imgurl>(formData);
            var imgurl = formJson.imgurl;
            var imgurl2 = formJson.imgurl2;

            var result = certificateByOpenid(openid, name, hospital, spec, title, phone, imgurl, imgurl2, IDNo, cert);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 认证营养师
        /// </summary>
        public static string certificateByOpenid(string openid, string name, string hospital, string spec, string title, string phone, string imgurl, string imgurl2, string IDNo, string cert)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "update mhs_t_user set realName='" + name + "',hosName='" + hospital + "',special='" + spec + "',title='" + title + "',phone='" + phone + "',certificateUrl='" + imgurl + "',certificateUrl2='" + imgurl2 + "',IDNo='" + IDNo + "',cert='" + cert + "' where openid='" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion



        #region 更新营养师信息
        /// <summary>
        ///  更新营养师信息
        /// </summary>
        private void updatedietitianbyopenid()
        {
            var openid = "";
            try
            {
                openid = HttpContext.Current.Request["openid"].ToString();
            }
            catch { }
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var gender = "";
            try
            {
                gender = HttpContext.Current.Request["gender"].ToString();
            }
            catch { }
            var age = "";
            try
            {
                age = HttpContext.Current.Request["age"].ToString();
            }
            catch { }
            var hosName = "";
            try
            {
                hosName = HttpContext.Current.Request["hosName"].ToString();
            }
            catch { }
            var special = "";
            try
            {
                special = HttpContext.Current.Request["special"].ToString();
            }
            catch { }
            var intro = "";
            try
            {
                intro = HttpContext.Current.Request["intro"].ToString();
            }
            catch { }
            var payed = "";
            try
            {
                payed = HttpContext.Current.Request["payed"].ToString();
            }
            catch { }
            var result = updateDietitianByOpenid(openid, name, gender, age, hosName, special, intro, payed);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 更新营养师信息
        /// </summary>
        public static string updateDietitianByOpenid(string openid, string name, string gender, string age, string hosName, string special, string intro, string payed)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "update mhs_t_user set realName='" + name + "',gender='" + gender + "',age='" + age + "',hosName='" + hosName + "',special='" + special + "',intro='" + intro + "',payed='" + payed + "' where openid='" + openid + "'";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        #region 获取患者列表
        //获取患者列表
        private void getpatient()
        {
            var str = " where 1=1 and type=1 ";
            var pageNum = 0;
            try
            {
                pageNum = Convert.ToInt32(HttpContext.Current.Request["pageNum"]);
            }
            catch { }
            var pageSize = 0;
            try
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }
            catch { }
            var ticket = getPatient(str, pageNum, pageSize);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 获取患者列表
        /// </summary>
        public static string getPatient(string str, int pageNum, int pageSize)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            //  DataSet ds = new DataSet(); //创建数据集对象
            //  dbAdapter.Fill(ds);
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            //   lo_cmd.CommandText = "select * from mhs_t_user " + str;
            lo_cmd.CommandText = "select top " + pageSize + "*from(select row_number()over(order by id)as rownumber,* from mhs_t_user " + str + ")as t where rownumber>" + pageSize * pageNum + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            // SqlDataReader lo_reader = lo_cmd.ExecuteReader();//返回结果集
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"data\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 绑定医患关系
        /// <summary>
        /// 绑定医患关系
        /// </summary>
        private void insertrelation()
        {
            var pid = "";
            try
            {
                pid = HttpContext.Current.Request["pid"].ToString();
            }
            catch { }
            var did = "";
            try
            {
                did = HttpContext.Current.Request["did"].ToString();
            }
            catch { }

            var result = insertRelation(pid, did);
            HttpContext.Current.Response.Write(result);
        }

        /// <summary>
        /// 绑定医患关系
        /// </summary>
        public static string insertRelation(string pid, string did)
        {
            string result = "";
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "insert into mhs_t_relation(pid,did) values('" + pid + "','" + did + "')";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        //以下为后台接口

        #region banner列表
        /// <summary>
        /// banner列表
        /// <summary>
        private void getbanner()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"];
            }
            catch { }
            var isList = "";
            try
            {
                isList = HttpContext.Current.Request["isList"];
            }
            catch { }
            var str = " where 1=1 ";
            if (name != "" && name != null && name != "undefined")
                str += "and (name like '%" + name + "%')";
            if (isList != "" && isList != null && isList != "undefined")
                str += " and isList =  " + isList + "";
            str += " order by sort ";
            var ticket = getBanner(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// banner列表
        /// </summary>
        public static string getBanner(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select id as [key],name as [no],bannerUrl as avatar,sort,isList as status from mhs_t_banner " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "" + json.Replace("'", "\"") + "";

            json = "{\"list\":" + json + "}";
            return json;
        }

        #endregion banner列表



        #region 新增banner
        /// <summary>
        /// 新增banner
        /// <summary>
        private void insertbanner()
        {
            var formData = WebUtility.HtmlDecode(HttpContext.Current.Request.Form.ToString());
            formData = HttpContext.Current.Server.UrlDecode(formData);
            formData = formData.Substring(5, formData.Length - 5);
            var formJson = JsonConvert.DeserializeObject<Imgurl>(formData);
            var imgurl = formJson.imgurl;

            var ticket = insertBanner(imgurl);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 新增banner
        /// </summary>
        public static string insertBanner(string imgurl)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "insert into mhs_t_banner(name,bannerUrl,sort,isList,updateTime) values('默认','" + imgurl + "',10,1,'" + DateTime.Now + "')";
            lo_cmd.CommandText = str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"添加成功\"}";
            return json;
        }

        #endregion 新增banner


        #region 编辑banner
        /// <summary>
        /// 编辑banner
        /// <summary>
        private void editbanner()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"];
            }
            catch { }
            var sort = "";
            try
            {
                sort = HttpContext.Current.Request["sort"];
            }
            catch { }
            var isList = "";
            try
            {
                isList = HttpContext.Current.Request["isList"];
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }

            var ticket = editBanner(name, sort, isList, id);
            HttpContext.Current.Response.Write(ticket);
        }


        /// <summary>
        /// 编辑banner
        /// </summary>
        public static string editBanner(string name, string sort, string isList, int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "update mhs_t_banner set name='" + name + "',sort='" + sort + "',isList='" + isList + "' where id=" + id + "";
            lo_cmd.CommandText = str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion 编辑banner


        #region 删除banner
        /// <summary>
        /// 删除banner
        /// <summary>
        private void delbanner()
        {
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }

            var ticket = delBanner(id);
            HttpContext.Current.Response.Write(ticket);
        }


        /// <summary>
        /// 删除banner
        /// </summary>
        public static string delBanner(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "delete mhs_t_banner where id=" + id + "";
            lo_cmd.CommandText = str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion 删除banner


        #region 模拟获取当前用户
        /// <summary>
        /// 模拟获取当前用户
        /// <summary>
        private void getmockuser()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"];
            }
            catch { }
            var avatar = "";
            try
            {
                avatar = HttpContext.Current.Request["avatar"];
            }
            catch { }
            var userid = "";
            try
            {
                userid = HttpContext.Current.Request["userid"];
            }
            catch { }
            var ticket = getMockuser(name, avatar, userid);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 模拟获取当前用户
        /// </summary>
        public static string getMockuser(string name, string avatar, string userid)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select 1";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            //json = "" + json.Replace("'", "\"") + "";

            json = "{\"name\":\"admin\",\"avatar\":\"https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png\",\"userid\":\"00000001\",\"notifyCount\": 12}";
            return json;
        }

        #endregion banner列表


        #region 后台获取问题列表
        //后台获取问题列表
        private void getquestionlistadmin()
        {
            var content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var direType = "";
            try
            {
                direType = HttpContext.Current.Request["direType"];
            }
            catch { }
            var docName = "";
            try
            {
                docName = HttpContext.Current.Request["docName"];
            }
            catch { }
            var str = " where 1=1 ";
            if (content != "" && content != null && content != "undefined")
                str += "and ([content] like '%" + content + "%')";
            if (direType != "" && direType != null && direType != "undefined")
                str += "and a.direType=" + direType + "";
            if (docName != "" && docName != null && docName != "undefined")
                str += "and ( c.nickName like '%" + docName + "%')";
            str += " order by a.askDate desc ";
            var ticket = getQuestionListAdmin(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取问题列表
        /// </summary>
        public static string getQuestionListAdmin(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select a.id, a.content,a.direType, b.nickName,b.avatar,c.avatar as docAvatar,c.nickName as docName,c.special as docSpe,c.fans,c.answeredNum,c.intro,c.hosName from mhs_t_question a left join mhs_t_user b on a.asker=b.id left join mhs_t_user c on a.answer=c.id  " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台更新问题
        //后台更新问题
        private void updatequestionadmin()
        {
            int direType = 0;
            try
            {
                direType = Convert.ToInt32(HttpContext.Current.Request["direType"]);
            }
            catch { }
            string content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = updateQuestionadmin(id, direType, content);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台更新问题
        /// </summary>
        public static string updateQuestionadmin(int id, int direType, string content)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "";
            if (id == 0)
                lo_cmd.CommandText = str + " insert into mhs_t_question(anserType,direType,content,asker,oxygen,status,askDate,isAnon) values(2," + direType + ",'" + content + "',14,10,1,'" + DateTime.Now + "',1)";
            else
                lo_cmd.CommandText = str + " update mhs_t_question set direType=" + direType + ",content='" + content + "' where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion


        #region 后台删除问题
        //后台删除问题
        private void deletequestionadmin()
        {
            int direType = 0;
            try
            {
                direType = Convert.ToInt32(HttpContext.Current.Request["direType"]);
            }
            catch { }
            string content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = deleteQuestionadmin(id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台删除问题
        /// </summary>
        public static string deleteQuestionadmin(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "";
            lo_cmd.CommandText = str + " delete from mhs_t_question where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion


        #region 后台获取回答列表
        //后台获取回答列表
        private void getanswerlistadmin()
        {
            var qcontent = "";
            try
            {
                qcontent = HttpContext.Current.Request["qcontent"];
            }
            catch { }
            var answer = "";
            try
            {
                answer = HttpContext.Current.Request["answer"];
            }
            catch { }
            var str = " where 1=1 ";
            if (qcontent != "" && qcontent != null && qcontent != "undefined")
                str += "and (c.content like '%" + qcontent + "%')";
            if (answer != "" && answer != null && answer != "undefined")
                str += "and ( b.nickName like '%" + answer + "%')";
            str += " order by a.id desc ";
            var ticket = getAnswerListAdmin(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取回答列表
        /// </summary>
        public static string getAnswerListAdmin(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select a.id,a.content acontent,a.voiceUrl,b.nickName answer,c.content qcontent,c.id qid from mhs_t_answer a inner join mhs_t_user b on a.uid=b.id inner join mhs_t_question c on a.qid=c.id" + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台更新回答
        //后台更新回答
        private void updateansweradmin()
        {
            int qid = 0;
            try
            {
                qid = Convert.ToInt32(HttpContext.Current.Request["qid"]);
            }
            catch { }
            string content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = updateAnsweradmin(id, qid, content);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台更新回答
        /// </summary>
        public static string updateAnsweradmin(int id, int qid, string content)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "";
            if (id == 0)
                lo_cmd.CommandText = str + " insert into mhs_t_answer(uid,qid,content) values(14," + qid + ",'" + content + "'; update mhs_t_question set answer=14 where id=" + qid + "; )";
            else
                lo_cmd.CommandText = str + " update mhs_t_answer set content='" + content + "' where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion


        #region 后台删除回答
        //后台删除回答
        private void deleteansweradmin()
        {
            int direType = 0;
            try
            {
                direType = Convert.ToInt32(HttpContext.Current.Request["direType"]);
            }
            catch { }
            string content = "";
            try
            {
                content = HttpContext.Current.Request["content"];
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = deleteAnsweradmin(id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台删除回答
        /// </summary>
        public static string deleteAnsweradmin(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            string str = "";
            lo_cmd.CommandText = str + " delete from mhs_t_answer where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = "{\"data\":\"操作成功\"}";
            return json;
        }

        #endregion


        #region 后台获取医生列表
        //后台获取医生列表
        private void getdoclist()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var str = " where 1=1 and type=2 ";
            if (name != "" && name != null && name != "undefined")
                str += "and (nickName like '%" + name + "%' or realName like '%" + name + "%')";
            if (phone != "" && phone != null && phone != "undefined")
                str += "and phone like '%" + phone + "%' ";
            var ticket = getDocList(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取医生列表
        /// </summary>
        public static string getDocList(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select * from mhs_t_user " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台获取用户列表
        //后台获取用户列表
        private void getuserlist()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var str = " where 1=1 and type=1 ";
            if (name != "" && name != null && name != "undefined")
                str += "and (nickName like '%" + name + "%' or realName like '%" + name + "%')";
            if (phone != "" && phone != null && phone != "undefined")
                str += "and phone like '%" + phone + "%' ";
            var ticket = getUserList(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取用户列表
        /// </summary>
        public static string getUserList(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select * from mhs_t_user " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion



        #region 后台获取充值列表
        //后台获取充值列表
        private void getrechargelist()
        {
            var name = 0;
            try
            {
                name = Convert.ToInt32(HttpContext.Current.Request["name"]);
            }
            catch { }
            var beginDate = "";
            try
            {
                beginDate = HttpContext.Current.Request["beginDate"].ToString();
            }
            catch { }
            var endDate = "";
            try
            {
                endDate = HttpContext.Current.Request["endDate"].ToString();
            }
            catch { }
            var str = " where 1=1 ";
            if (beginDate != "" && beginDate != null && beginDate != "undefined" && beginDate != "null")
                str += " and optime >=  '" + beginDate + "'";
            if (endDate != "" && endDate != null && endDate != "undefined" && beginDate != "null")
                str += " and optime <=  '" + endDate + "'";
            var ticket = getRechargelist(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取充值列表
        /// </summary>
        public static string getRechargelist(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select a.*,b.nickName name from mhs_t_finance a inner join mhs_t_user b on a.uid=b.id " + str + "";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台获取提现列表
        //后台获取提现列表
        private void getwithdrawlist()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var phone = "";
            try
            {
                phone = HttpContext.Current.Request["phone"].ToString();
            }
            catch { }
            var str = " where 1=1 ";
            if (name != "" && name != null && name != "undefined")
                str += "and (nickName like '%" + name + "%' or realName like '%" + name + "%')";
            if (phone != "" && phone != null && phone != "undefined")
                str += "and phone like '%" + phone + "%' ";
            var ticket = getWithdrawList(str);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取提现列表
        /// </summary>
        public static string getWithdrawList(string str)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select 1 " + str;
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台获取医院列表
        //后台获取医院列表
        private void gethospitallist()
        {
            var ticket = getHospitalList();
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取医院列表
        /// </summary>
        public static string getHospitalList()
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select id as [key], hosName from mhs_t_hos ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion


        #region 后台更新医院
        //后台更新医院
        private void hospitalupdate()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = hospitalUpdate(name, id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台更新医院
        /// </summary>
        public static string hospitalUpdate(string name, int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            if (id != 0)
                lo_cmd.CommandText = "update mhs_t_hos set hosName='" + name + "' where id=" + id + " ";
            else
                lo_cmd.CommandText = "insert into mhs_t_hos(hosName) values('" + name + "') ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        #region 后台删除医院
        //后台删除医院
        private void hospitaldelete()
        {
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = hospitalDelete(id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台删除医院
        /// </summary>
        public static string hospitalDelete(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "delete from mhs_t_hos where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion


        #region 后台获取科室列表
        //后台获取科室列表
        private void getdepartlist()
        {
            var ticket = getdepartList();
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取科室列表
        /// </summary>
        public static string getdepartList()
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select id as [key], depName from mhs_t_dep ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion

        #region 后台更新科室
        //后台更新科室
        private void departupdate()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = departUpdate(name, id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台更新科室
        /// </summary>
        public static string departUpdate(string name, int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            if (id != 0)
                lo_cmd.CommandText = "update mhs_t_dep set depName='" + name + "' where id=" + id + " ";
            else
                lo_cmd.CommandText = "insert into mhs_t_dep(depName) values('" + name + "') ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        #region 后台删除科室
        //后台删除科室
        private void departdelete()
        {
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = departDelete(id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台删除科室
        /// </summary>
        public static string departDelete(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "delete from mhs_t_dep where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion


        #region 后台获取职称列表
        //后台获取职称列表
        private void gettitlelist()
        {
            var ticket = gettitleList();
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台获取职称列表
        /// </summary>
        public static string gettitleList()
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "select id as [key], title from mhs_t_title ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            string json = ConvertJson.DataTable2Array(ds.Tables[0]);
            json = "{\"list\":" + json.Replace("'", "\"") + "}";
            return json;
        }
        #endregion

        #region 后台更新职称
        //后台更新职称
        private void titleupdate()
        {
            var name = "";
            try
            {
                name = HttpContext.Current.Request["name"].ToString();
            }
            catch { }
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = titleUpdate(name, id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台更新职称
        /// </summary>
        public static string titleUpdate(string name, int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            if (id != 0)
                lo_cmd.CommandText = "update mhs_t_title set title='" + name + "' where id=" + id + " ";
            else
                lo_cmd.CommandText = "insert into mhs_t_title(title) values('" + name + "') ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion

        #region 后台删除职称
        //后台删除职称
        private void titledelete()
        {
            var id = 0;
            try
            {
                id = Convert.ToInt32(HttpContext.Current.Request["id"]);
            }
            catch { }
            var ticket = titleDelete(id);
            HttpContext.Current.Response.Write(ticket);
        }

        /// <summary>
        /// 后台删除职称
        /// </summary>
        public static string titleDelete(int id)
        {
            SqlConnection con = new SqlConnection(CONN); //注意与上面的区分开
            con.Open();
            SqlCommand lo_cmd = new SqlCommand();   //创建命令对象
            lo_cmd.CommandText = "delete from mhs_t_title where id=" + id + " ";
            lo_cmd.Connection = con;             //指定连接对象，即上面创建的
            SqlDataAdapter dbAdapter = new SqlDataAdapter(lo_cmd); //注意与上面的区分开
            DataSet ds = new DataSet(); //创建数据集对象
            dbAdapter.Fill(ds);
            con.Close();
            var result = "{\"data\":\"操作成功\"}";
            return result;
        }
        #endregion



        /// <summary>
        /// AccessToken Class
        /// </summary>
        public class Imgurl
        {
            private string _imgurl;
            public string imgurl
            {
                set { _imgurl = value; }
                get { return _imgurl; }
            }
            private string _imgurl2;
            public string imgurl2
            {
                set { _imgurl2 = value; }
                get { return _imgurl2; }
            }
        }

    }
}