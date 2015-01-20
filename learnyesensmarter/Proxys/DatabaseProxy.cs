using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

using learnyesensmarter.Models;
using learnyesensmarter.Interfaces;

namespace learnyesensmarter.Proxys
{
    public class DatabaseProxy : IQuestionInserter, IQuestionRetriever, ICategoryRetriever, ICategoryInserter, IPriorityUpdater
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
            try
            {
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

        /// <summary>
        /// Mainly handles the exceptions thrown if something goes wrong reading the sql data reader.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private SqlDataReader TalkToDB(SqlCommand cmd)
        {
            try
            {
                return cmd.ExecuteReader();
            }
            catch (InvalidCastException e)
            {
                throw new Exception("Unlogged exception in DatabaseHelper.cs - " + e.Message);
            }
            catch (SqlException e)
            {
                throw new Exception("Unlogged exception in DatabaseHelper.cs - " + e.Message);
            }
            catch (InvalidOperationException e)
            {
                throw new Exception("Unlogged exception in DatabaseHelper.cs - " + e.Message);
            }
        }

        private bool closeConnection()
        {
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
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();
                var sql = new SqlCommand(_retrieveCategorySqlStatement, _connection);
                sql.Parameters.Add("@category_id", System.Data.SqlDbType.Int);
                sql.Parameters["@category_id"].Value = id;

                TalkToDB<string>(sql, out result);

                closeConnection();
            }
            return result;
        }

        private string _retrieveCategoryIDSqlStatement = "select CategoryID from Categories where Category = @catergory";
        public int RetrieveCategoryID(string category)
        {
            if (String.IsNullOrEmpty(category)) {
                throw new Exception("unlogged exception@ RetrieveCategoryID IsNullOrEmpty");
            }
            int result = LOWEST_CATEGORY_ID;

            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();

                var sql = new SqlCommand(_retrieveCategoryIDSqlStatement, _connection);
                sql.Parameters.Add("@category", System.Data.SqlDbType.NVarChar);
                sql.Parameters["@category"].Value = category;

                TalkToDB<int>(sql, out result);

                closeConnection();
            }

            return -1;
        }

        private string _insertCategorySqlStatement = "insert into Categories (Category) values (@category_name)";
        const int ERROR = -1;
        public int InsertCategory(string name)
        {
            int result = ERROR;
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();
                var sql = new SqlCommand(_insertCategorySqlStatement, _connection);
                sql.Parameters.Add("@category_name", System.Data.SqlDbType.NVarChar);
                sql.Parameters["@category_name"].Value = name;

                TalkToDB<int>(sql, out result);

                closeConnection();
            }

            return result;
        }

        #endregion

        #region Questions

        private string _retrieveSqlStatement = "Select Question from Questions where QuestionID = @questionID";
        public string RetrieveQuestion(int id)
        {
            string result = "Default";
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();

                //setup parameters used in the sql statement
                var sql = new SqlCommand(_retrieveSqlStatement, _connection);
                sql.Parameters.Add("@questionID", System.Data.SqlDbType.Int);
                sql.Parameters["@questionID"].Value = id;

                TalkToDB<string>(sql, out result);

                closeConnection();
            }
            return result;
        }

        // change to join on QuestionId Review and Questions Table, then select Question, QuestionId and QuestionType and order by Priority
        private string _retrieveQuestionsSqlStatement = "Select Question, QuestionID, QuestionType from Questions where QuestionID BETWEEN @startID AND @finishID";
        public QuestionPerformModel[] RetrieveQuestions(int startID, int quantity)
        {
            var results = new List<QuestionPerformModel>();
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();
                var sql = new SqlCommand(_retrieveQuestionsSqlStatement, _connection);
                sql.Parameters.Add("@startID", System.Data.SqlDbType.Int);
                sql.Parameters["@startID"].Value = startID;
                sql.Parameters.Add("@finishID", System.Data.SqlDbType.Int);
                sql.Parameters["@finishID"].Value = startID + quantity;

                SqlDataReader reader = TalkToDB(sql);
                
                while (reader.Read())
                {
                    var temp = new QuestionPerformModel();
                    temp.question = reader.GetString(0);
                    temp.questionID = reader.GetInt32(1);
                    temp.questionType = reader.GetInt32(2);
                    results.Add(temp);
                }
                closeConnection();
            }
            return results.ToArray();
        }

        private string _insertSqlStatement = "insert into Questions (Question, QuestionType) values (@users_question, @users_question_type); select scope_identity()";
        private string _insertReviewStatement = "insert into Review (UserID, QuestionId, LastTook, Priority) values (@user_id, @question_id, @last_took, @new_priority)";
        private const int LOWEST_CATEGORY_ID = 1;
        private const float DEFAULT_PRIORITY = 1.0f;
        public int Insert(QuestionModel user_question)
        {
            decimal result = 0;
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();
                //Insert the question into the questions table
                #region Questions Table
                var sql = new SqlCommand(_insertSqlStatement, _connection);

                sql.Parameters.Add("@users_question", System.Data.SqlDbType.NVarChar);
                sql.Parameters["@users_question"].Value = user_question.Question;

                sql.Parameters.Add("@users_question_type", System.Data.SqlDbType.Int);
                sql.Parameters["@users_question_type"].Value = user_question.QuestionType;

                TalkToDB<decimal>(sql, out result);
                #endregion

                //insert the question into the Review Table
                #region Review Table

                var reviewSql = new SqlCommand(_insertReviewStatement, _connection);

                reviewSql.Parameters.Add("@user_id", System.Data.SqlDbType.Int);
                reviewSql.Parameters["@user_id"].Value = 0; //DEBUGGING PURPOSES because theres no user/account info added yet

                reviewSql.Parameters.Add("@question_id", System.Data.SqlDbType.Int);
                reviewSql.Parameters["@question_id"].Value = (int)result; //result of the previous sql statement is the question id

                reviewSql.Parameters.Add("@last_took", System.Data.SqlDbType.DateTime);
                reviewSql.Parameters["@last_took"].Value = DateTime.Now; //Assume the creation of the task is the same as the completion of the task
                                                                           //or in other words, there's no point in reviewing something you've just learnt

                reviewSql.Parameters.Add("@new_priority", System.Data.SqlDbType.Float);
                reviewSql.Parameters["@new_priority"].Value = DEFAULT_PRIORITY;

                object this_is_a_code_smell;
                TalkToDB<object>(reviewSql, out this_is_a_code_smell);

                #endregion
                closeConnection();
            }
            return (int)result;
        }

        private string _updatePriorityStatement = "update Review set Priority=@NewPriority, LastTook=@last_took where QuestionID=@questionID";
        public float UpdatePriority(int questionID, float priority)
        {
            float result = 0;
            using (_connection = new SqlConnection(_connectionString))
            {
                openConnection();

                var sql = new SqlCommand(_updatePriorityStatement, _connection);
                sql.Parameters.Add("@NewPriority", System.Data.SqlDbType.Float);
                sql.Parameters["@NewPriority"].Value = priority;

                sql.Parameters.Add("@last_took", System.Data.SqlDbType.DateTime);
                sql.Parameters["@last_took"].Value = DateTime.Now;

                sql.Parameters.Add("@questionID", System.Data.SqlDbType.Int);
                sql.Parameters["@questionID"].Value = questionID;

                //var yaeh = TalkToDB(sql);
                sql.ExecuteNonQuery();

                closeConnection();
            }
            return result;
        }

        #endregion
    }
}
