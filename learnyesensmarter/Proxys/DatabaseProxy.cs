using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Controllers
{
    public class DatabaseProxy : IQuestionInserter, IQuestionRetriever, ICategoryRetriever, ICategoryInserter
    {
        #region Constructors and Properties
        
        //Grabs the connection string from the config file and stores it here privately
        //so as to cache it and prevent it from being changed outside the class.
        private string _connectionString;

        private static SqlConnection _connection = null;

        public DatabaseProxy()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        #endregion

        #region Functions used throughout

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        /// <returns>a bool indicating the success of the connection.</returns>
        private bool openConnection()
        {
            if (_connection != null) return true;

            try
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            } catch(InvalidOperationException ioe)
            {
                //log error here
                throw new Exception("unlogged exception@ line 34:DatabaseHelper.cs - " + ioe.Message);
                //then return false
            } catch(SqlException sqle)
            {
                throw new Exception("unlogged exception@ line 38:DatabaseHelper.cs - " + sqle.Message);
                //then return false
            }

            return true;
        }

        private bool TalkToDB<TResult>(SqlCommand cmd, out TResult result)
        {
            try
            {
                result = (TResult)cmd.ExecuteScalar();
            }
            catch (InvalidCastException ice)
            {
                throw new Exception("unlogged exception@ line 78:DatabaseHelper.cs - " + ice.Message);
            }
            catch (SqlException sqle)
            {
                throw new Exception("unlogged exception@ line 84:DatabaseHelper.cs - " + sqle.Message);
            }
            catch (InvalidOperationException ioe)
            {
                throw new Exception("unlogged exception@ line 88:DatabaseHelper.cs - " + ioe.Message);
            }
            catch (System.IO.IOException ioe)
            {
                throw new Exception("unlogged exception@ line 92:DatabaseHelper.cs - " + ioe.Message);
            }

            return true;
        }

        private bool closeConnection()
        {
            if (_connection == null) return true;

            try
            {
                _connection.Close();
            }
            catch (SqlException sqle)
            {
                throw new Exception("unlogged exception@ line 55:DatabaseHelper.cs - " + sqle.Message);
            }

            return true;
        }

        #endregion

        #region Categories

        private string _retrieveCategorySqlStatement = "select Category from Categories where CategoryID = @category_id";
        public string RetrieveCategory(int id)
        {
            if (id < LOWEST_CATEGORY_ID) throw new Exception("unlogged exception@ RetrieveCategory ID is lower than allowed");
            string result = "";

            openConnection();
            var sql = new SqlCommand(_retrieveCategorySqlStatement, _connection);
                sql.Parameters.Add("@category_id", System.Data.SqlDbType.Int);
                sql.Parameters["@category_id"].Value = id;

            TalkToDB<string>(sql, out result);

            closeConnection();

            return result;
        }

        private string _retrieveCategoryIDSqlStatement = "select CategoryID from Categories where Category = @catergory";
        public int RetrieveCategoryID(string category)
        {
            if (String.IsNullOrEmpty(category)) {
                throw new Exception("unlogged exception@ RetrieveCategoryID IsNullOrEmpty");
            }
            int result = LOWEST_CATEGORY_ID;

            openConnection();

            var sql = new SqlCommand(_retrieveCategoryIDSqlStatement, _connection);
                sql.Parameters.Add("@category", System.Data.SqlDbType.NVarChar);
                sql.Parameters["@category"].Value = category;

            TalkToDB<int>(sql, out result);

            closeConnection();

            return -1;
        }

        private string _insertCategorySqlStatement = "insert into Categories (Category) values (@category_name)";
        const int ERROR = -1;
        public int InsertCategory(string name)
        {
            int result = ERROR;
            openConnection();
            var sql = new SqlCommand(_insertCategorySqlStatement, _connection);
            sql.Parameters.Add("@category_name", System.Data.SqlDbType.NVarChar);
            sql.Parameters["@category_name"].Value = name;

            TalkToDB<int>(sql, out result);

            closeConnection();

            return result;
        }

        #endregion

        #region Questions

        private string _retrieveSqlStatement = "Select Question from Questions where QuestionID = @questionID";
        public string RetrieveQuestion(int id)
        {
            string result = "Default";

            openConnection();

            //setup parameters used in the sql statement
            var sql = new SqlCommand(_retrieveSqlStatement, _connection);
                sql.Parameters.Add("@questionID", System.Data.SqlDbType.Int);
                sql.Parameters["@questionID"].Value = id;

            TalkToDB<string>(sql, out result);

            closeConnection();
            return result;
        }

        private string _insertSqlStatement = "insert into Questions (Question) values (@users_question)";
        private const int LOWEST_CATEGORY_ID = 1;
        public string Insert(QuestionModel user_question)
        {
            openConnection();
            string result = "";
            var sql = new SqlCommand(_insertSqlStatement, _connection);

            sql.Parameters.Add("@users_question", System.Data.SqlDbType.NVarChar);
            sql.Parameters["@users_question"].Value = user_question.Question;
            
            TalkToDB<string>(sql, out result);

            closeConnection();

            return result;
        }

        #endregion
    }
}
