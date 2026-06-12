using SQL.Entity;
using SqlSugar;

namespace SQL
{
    public class SqliteHelper
    {
        ////private static readonly Lazy<SqliteHelper> lazy = new Lazy<SqliteHelper>(() => new SqliteHelper());
        ////public static SqliteHelper Instance { get { return lazy.Value; } }
        ////private SqliteHelper() { }

        public SqlSugarClient? DBContext { get; private set; }

        public SqliteHelper()
        {
            try
            {
                #region Connection; 
                var connectionString = $"DataSource={Path.Combine(Environment.CurrentDirectory, "KinLoPumpTest.db")}";
                //connectionString = @"DataSource=D:\William\Sqlite3\Demos\KinLoPumpTest.db";

                //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
                DBContext = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true
                },
                db =>
                {

                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {

                        //获取原生SQL推荐 5.1.4.63  性能OK
                        Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

                        //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                        //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))


                    };

                    //注意多租户 有几个设置几个
                    //db.GetConnection(i).Aop

                });
                #endregion

                #region Create tables; 
                DBContext.CodeFirst.InitTables(typeof(PumpTest));
                #endregion
            }
            catch (Exception ex)
            {

            }
        }
    }
}
