
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;


public class DatabaseManager
{
    public bool Create(string instanceName, string databaseName)
    {
        var result = false;
        try
        {
            using (var sqlConnection = new SqlConnection(string.Concat(@"Data Source=(LocalDb)\", instanceName, ";Initial Catalog=Master;Integrated Security=True")))
            {
                sqlConnection.Open();
                var command = new SqlCommand();
                command.Connection = sqlConnection;
                command.CommandText = string.Format(@"
 	                IF EXISTS(SELECT * FROM sys.databases WHERE name='{0}') 
 	                BEGIN 
 		                ALTER DATABASE [{0}] 
 		                SET SINGLE_USER 
 		                WITH ROLLBACK IMMEDIATE 
 		                DROP DATABASE [{0}] 
 	                END 
  
 	                DECLARE @FILENAME AS VARCHAR(255) 
  
 	                SET @FILENAME = CONVERT(VARCHAR(255), SERVERPROPERTY('instancedefaultdatapath')) + '{0}' + '.mdf'; 
  
 	                EXEC ('CREATE DATABASE [{0}] ON PRIMARY  
 		                (NAME = [{0}],  
 		                FILENAME =''' + @FILENAME + ''',  
 		                SIZE = 6400KB,  
 		                MAXSIZE = UNLIMITED,  
 		                FILEGROWTH = 1024KB )')", databaseName);

                command.ExecuteNonQuery();
                result = true;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(true, ex);
        }

        LogHelper.WriteLine("Database creation : ", result.ToString());

        return result;
    }


    public bool Delete(string instanceName, string databaseName)
    {
        var result = false;
        try
        {
            using (var sqlConnection = new SqlConnection(string.Concat(@"Data Source=(LocalDb)\", instanceName, ";Initial Catalog=Master;Integrated Security=True")))
            {
                sqlConnection.Open();
                var command = new SqlCommand();
                command.Connection = sqlConnection;
                command.CommandText = string.Format(@"
 	                IF EXISTS(SELECT * FROM sys.databases WHERE name='{0}') 
 	                BEGIN 
 		                ALTER DATABASE [{0}] 
 		                SET SINGLE_USER 
 		                WITH ROLLBACK IMMEDIATE 
 		                DROP DATABASE [{0}] 
 	                END ", databaseName);

                command.ExecuteNonQuery();
                result = true;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(true, ex);
        }
        LogHelper.WriteLine("Database Dropped Status : ", result.ToString());
        return result;
    }

    public bool InsertData(string instanceName, string databaseName)
    {
        var result = false;
        try
        {
            var queries = GetQueries("schema.txt");
            if (queries.Count > 0)
            {
                using (var sqlConnection = new SqlConnection(string.Concat(@"Data Source=(LocalDb)\", instanceName, ";Initial Catalog=", databaseName, ";Integrated Security=True")))
                {
                    var command = sqlConnection.CreateCommand();
                    sqlConnection.Open();
                    foreach (string query in queries)
                    {
                        if (query.Length > 0)
                        {
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            result = true;
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(false, ex);
        }

        LogHelper.WriteLine("Database initialization : ", result.ToString());
        return result;
    }

    public bool CreateUser(string instanceName, string databaseName)
    {
        var result = false;
        try
        {
            using (var sqlConnection = new SqlConnection(string.Concat(@"Data Source=(LocalDb)\", instanceName, ";Initial Catalog=", databaseName, ";Integrated Security=True")))
            {
                sqlConnection.Open();
                var command = new SqlCommand();
                command.Connection = sqlConnection;

                try
                {
                    command.CommandText = @"DROP USER [buzzuser];DROP LOGIN [buzzlogin];";
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }

                command.CommandText = @"CREATE LOGIN [buzzlogin] WITH PASSWORD = '339.KgcK6x779KH'; CREATE USER [buzzuser] FOR LOGIN [buzzlogin]; exec sp_addrolemember 'db_owner', 'buzzuser';";
                command.ExecuteNonQuery();
                result = true;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(true, ex);
        }
        LogHelper.WriteLine("Database User Creation Status : ", result.ToString());
        return result;
    }


    private static List<string> GetQueries(string name)
    {
        var list = new List<string>();
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + name);
        var reader = new StreamReader(stream);

        var scriptContent = reader.ReadToEnd();
        if (string.IsNullOrEmpty(scriptContent) == false)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            list.AddRange(regex.Split(scriptContent));
        }
        return list;
    }
}

