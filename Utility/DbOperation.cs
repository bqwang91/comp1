using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;

namespace StockSenti
{
    class DbOperation
    {
        public static bool insertAricle(DbConnection connection, Article newArticle)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO " + newArticle.CompanyCode + "Articles(id, date, title, source, content,classification) VALUES (@id,@date,@title,@source,@content,@classification)";
            command.Prepare();

            DbParameter id = command.CreateParameter();
            id.ParameterName = "@id";
            id.Value = newArticle.Id;
            command.Parameters.Add(id);

            DbParameter date = command.CreateParameter();
            date.ParameterName = "@date";
            date.Value = newArticle.Date;
            command.Parameters.Add(date);

            DbParameter title = command.CreateParameter();
            title.ParameterName = "@title";
            title.Value = newArticle.Title;
            command.Parameters.Add(title);

            DbParameter source = command.CreateParameter();
            source.ParameterName = "@source";
            source.Value = newArticle.Source;
            command.Parameters.Add(source);

            DbParameter content = command.CreateParameter();
            content.ParameterName = "@content";
            content.Value = newArticle.Content;
            command.Parameters.Add(content);

            DbParameter classification = command.CreateParameter();
            classification.ParameterName = "@classification";
            classification.Value = newArticle.Classification;
            command.Parameters.Add(classification);

            if (command.ExecuteNonQuery() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Article> getAricles(DbConnection connection, string companyCode)
        {
            DbDataReader Reader;
            List<Article> articleList = new List<Article>();

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id,date,title,source,content,classification FROM " + companyCode + "Articles";
            command.Prepare();

            Reader = command.ExecuteReader();

            while (Reader.Read())
            {
                Article newArticle = new Article();

                newArticle.Id = Reader.GetValue(0).ToString();
                newArticle.Date = Reader.GetValue(1).ToString();
                newArticle.Title = Reader.GetValue(2).ToString();
                newArticle.Source = Reader.GetValue(3).ToString();
                newArticle.Content = Reader.GetValue(4).ToString();
                newArticle.Classification = Reader.GetValue(5).ToString();
                articleList.Add(newArticle);
            }
            Reader.Close();
            return articleList;
        }

        public static List<Article> getSomeAricles(DbConnection connection, string companyCode, int lowerBound, int higherBound)
        {
            DbDataReader Reader;
            List<Article> articleList = new List<Article>();

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id,date,title,source,content,classification FROM " + companyCode + "Articles WHERE id BETWEEN " + lowerBound.ToString() + " AND " + higherBound.ToString();
            command.Prepare();

            Reader = command.ExecuteReader();

            while (Reader.Read())
            {
                Article newArticle = new Article();

                newArticle.Id = Reader.GetValue(0).ToString();
                newArticle.Date = Reader.GetValue(1).ToString();
                newArticle.Title = Reader.GetValue(2).ToString();
                newArticle.Source = Reader.GetValue(3).ToString();
                newArticle.Content = Reader.GetValue(4).ToString();
                newArticle.Classification = Reader.GetValue(5).ToString();
                articleList.Add(newArticle);
            }
            Reader.Close();
            return articleList;
        }

        public static bool insertPriceInfo(DbConnection connection, PriceInfo pInfo)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO " + pInfo.CompanyCode + "HistoricalPrices(date, close_price,change,performance) VALUES (@date,@close_price,@price_change,@performance)";
            command.Prepare();

            DbParameter date = command.CreateParameter();
            date.ParameterName = "@date";
            date.Value = pInfo.Date;
            command.Parameters.Add(date);

            DbParameter close_price = command.CreateParameter();
            close_price.ParameterName = "@close_price";
            close_price.Value = pInfo.ClosePrice;
            command.Parameters.Add(close_price);

            DbParameter price_change = command.CreateParameter();
            price_change.ParameterName = "@price_change";
            price_change.Value = pInfo.PriceChange;
            command.Parameters.Add(price_change);

            DbParameter performance = command.CreateParameter();
            performance.ParameterName = "@performance";
            performance.Value = pInfo.Performance;
            command.Parameters.Add(performance);

            if (command.ExecuteNonQuery() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string getMarketPriceChange(DbConnection connection, string queryDate)
        {
            string result = "null";
            DbDataReader Reader;

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT change FROM ASXHistoricalPrices WHERE date=@date";
            command.Prepare();

            DbParameter date = command.CreateParameter();
            date.ParameterName = "@date";
            date.Value = queryDate;
            command.Parameters.Add(date);

            Reader = command.ExecuteReader();

            if (Reader.Read())
            {
                result = Reader.GetValue(0).ToString();
            }
            else 
            {
                result = "null";
            }
            Reader.Close();
            return result;
        }

        public static string getStockPerformance(DbConnection connection, string companyCode, string queryDate)
        {
            string result = "null";
            DbDataReader Reader;

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT performance FROM " + companyCode + "HistoricalPrices WHERE date=@date";
            command.Prepare();

            DbParameter date = command.CreateParameter();
            date.ParameterName = "@date";
            date.Value = queryDate;
            command.Parameters.Add(date);

            Reader = command.ExecuteReader();

            if (Reader.Read())
            {
                result = Reader.GetValue(0).ToString();
            }
            else
            {
                result = "null";
            }
            Reader.Close();
            return result;
        }

        public static bool isTableExist(DbConnection connection,string tableName)
        { 
            DbDataReader Reader;

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@table_name";
            command.Prepare();

            DbParameter table_name = command.CreateParameter();
            table_name.ParameterName = "@table_name";
            table_name.Value = tableName;
            command.Parameters.Add(table_name);

            Reader = command.ExecuteReader();

            if (Reader.Read())
            {
                Reader.Close();
                return true;
            }
            else 
            {
                Reader.Close();
                return false;
            }
            
        }

        public static void createNewTable(DbConnection connection, string tableName)
        {

            DbCommand command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + "(date DATE PRIMARYKEY, close_price VARCHAR(20), change VARCHAR(10), performance VARCHAR(20));";
            command.Prepare();

            command.ExecuteNonQuery();

        }

       

    }
}
