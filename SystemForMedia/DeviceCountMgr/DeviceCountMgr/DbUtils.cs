﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DeviceCountMgr
{
    /// <summary>
    /// 说明：数据访问通用类，供DAL内部调用
    /// </summary>
    public  class DbUtils
    {
        private string connectionString;

        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectstring">数据库连接字符串</param>
        public DbUtils(string cStr)
        {
            this.ConnectionString = cStr;
        }

        /// <summary>   
        /// 执行一个查询，并返回结果集   
        /// </summary>   
        /// <param name="sql">要执行的查询SQL文本命令</param>   
        /// <returns>返回查询结果集</returns>   
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }
        /// <summary>   
        /// 执行一个查询,并返回查询结果   
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <returns>返回查询结果集</returns>   
        public DataTable ExecuteDataTable(string sql, CommandType commandType)
        {
            return ExecuteDataTable(sql, commandType, null);
        }
        /// <summary>   
        /// 执行一个查询,并返回查询结果   
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>   
        /// <returns></returns>   
        public DataTable ExecuteDataTable(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集   
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType   
                    //如果同时传入了参数，则添加这些参数   
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter   
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    adapter.Fill(data);//填充DataTable   
                }
            }
            return data;
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的查询SQL文本命令</param>   
        /// <returns></returns>   
        public SqlDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, null);
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <returns></returns>   
        public SqlDataReader ExecuteReader(string sql, CommandType commandType)
        {
            return ExecuteReader(sql, commandType, null);
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>   
        /// <returns></returns>   
        public SqlDataReader ExecuteReader(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            //如果同时传入了参数，则添加这些参数   
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            connection.Open();
            //CommandBehavior.CloseConnection参数指示关闭Reader对象时关闭与其关联的Connection对象   
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的查询SQL文本命令</param>   
        /// <returns></returns>   
        public Object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <returns></returns>   
        public Object ExecuteScalar(string sql, CommandType commandType)
        {
            return ExecuteScalar(sql, commandType, null);
        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>   
        /// <returns></returns>   
        public Object ExecuteScalar(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            object result = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType   
                    //如果同时传入了参数，则添加这些参数   
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接   
                    result = command.ExecuteScalar();
                }
            }
            return result;//返回查询结果的第一行第一列，忽略其它行和列   
        }
        /// <summary>   
        /// 对数据库执行增删改操作   
        /// </summary>   
        /// <param name="sql">要执行的查询SQL文本命令</param>   
        /// <returns></returns>   
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>   
        /// 对数据库执行增删改操作   
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <returns></returns>   
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }
        /// <summary>   
        /// 对数据库执行增删改操作   
        /// </summary>   
        /// <param name="sql">要执行的SQL语句</param>   
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>   
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>   
        /// <returns></returns>   
        public int ExecuteNonQuery(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType   
                    //如果同时传入了参数，则添加这些参数   
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接   
                    count = command.ExecuteNonQuery();
                }
            }
            return count;//返回执行增删改操作之后，数据库中受影响的行数   
        }
        /// <summary>   
        /// 返回当前连接的数据库中所有由用户创建的数据库   
        /// </summary>   
        /// <returns></returns>   
        public DataTable GetTables()
        {
            DataTable data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();//打开数据库连接   
                data = connection.GetSchema("Tables");
            }
            return data;
        }






    }
}

