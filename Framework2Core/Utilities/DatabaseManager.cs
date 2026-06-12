using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Framework2Core {

    /// <summary>
    /// 静态类：管理数据库，包括同步列、增加数据、查询数据。
    /// 使用队列缓存数据：当不在查询时，定时从队列中取数据，插入数据库
    /// </summary>
    public static class DatabaseManager {

        #region 1. 字段、普通属性

        // 只读字段：数据库连接字符串，只可在构造函数中初始化








      //  private readonly static string _strConnection = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\Database\DBADatabase.mdf;Initial Catalog=DBADatabase;Integrated Security=True;Connect Timeout=30";
        private readonly static string _strConnection = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Database\DBADatabase.mdf;Initial Catalog=DBADatabase;Integrated Security=True;Connect Timeout=30";



        // 字段：计时器。当不在查询数据库时，定时从队列中取出数据，插入数据库。默认计时间隔为 1 秒        
        private static System.Timers.Timer _timer = new System.Timers.Timer() { Interval=1000};

        // 字段：产品队列，里面装的是等待插入数据库的记录（RecordItem类型）
        private static Queue<RecordItem> QueRecords = new Queue<RecordItem>();


        /// <summary>
        /// 只读属性：数据库已连接
        /// </summary>
        public static bool IsConnected {
            get {
                return _isConnected;
            }
        }
        private static bool _isConnected = false;


        /// <summary>
        /// 只读属性：数据库正在查询中
        /// </summary>
        public static bool IsSearching {
            get {
                return _isSearching;
            }
        }
        private static bool _isSearching = false;


        /// <summary>
        /// 只读属性：产品队列中剩余的记录数
        /// </summary>
        public static int QueueCount { 
            get {
                return QueRecords.Count();
            } 
        }

        #endregion


        #region 2. 静态构造函数

        // 静态构造函数：首次被调用静态类时，自动调用此方法
        static DatabaseManager() {
            Type staticType = typeof(DatabaseManager); //本静态类的类型

            //加载配置文件
            staticType.LoadStaticConfigsFromIni();


            _strConnection = @"Data Source=(LocalDB)\" + 数据库版本 +
  @";AttachDbFilename=" + 数据库文件 +
  @";Initial Catalog=DBADatabase;Integrated Security=True;Connect Timeout=30";


            //初始化计时结束的事件处理器
            _timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, e) => {
                if (_isConnected && !_isSearching)
                { //数据库已连接，并且不在查询中
                    DequeueRecordIntoDatabase(); //队首元素出列，并插入到数据库中
                }
            });

            //保存配置文件
            staticType.SaveStaticConfigsToIni();

        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：数据库版本。VS2012 为 v11.0；VS2015、2019 为 MSSQLLocalDB。默认值为后者
        /// </summary>
        [IniConfig]
        public static string 数据库版本 { get; set; } = "MSSQLLocalDB";


        /// <summary>
        /// 可配置的属性：数据库文件的相对路径。默认路径为 \Debug\Database\DBADatabase.mdf
        /// </summary>
        [IniConfig]
        public static string 数据库文件 { get; set; } = @"\Database\DBADatabase.mdf";


        /// <summary>
        /// 可配置的属性：同步计时器的 Interval 属性。计时结束后插入记录到数据库。默认值为 1000，即 1 秒
        /// </summary>
        [IniConfig]
        public static int 计时器间隔 {
            get { return (int)_timer.Interval; }
            set {
                if (value<=0) {
                    value = 1000; //Interval 必须大于 0
                }
                _timer.Interval = value;
            }
        }

        #endregion


        #region 4. 主要功能：同步数据库表格的列

        /// <summary>
        /// 主要功能：静态方法：将字典的列信息，同步到数据库表格中
        /// </summary>
        /// <param name="tableName">数据库表格的名字</param>
        /// <param name="dic">用于同步的字典：《列名, 类型》</param>
        public static void UpdateDatabaseColumns(string tableName, Dictionary<string, string> dic) {
            try {
                //添加数据库表格中缺失的字段
                List<string> listDatabaseCols = GetDatabaseColumns(tableName); //获取数据库中指定表格的列集合
                foreach (var col in dic.Keys) {
                    //某列在字典的Key里，但是不在数据库表格中：新增
                    if (!listDatabaseCols.Contains(col)) {
                        AddColumnToDataBase(tableName, col, dic[col]); //表名，列名，类型
                    }
                }

                //删除数据库表格中多余的字段
                listDatabaseCols = GetDatabaseColumns(tableName);  //再次获取数据库中指定表格的列集合
                foreach (var col in listDatabaseCols) {
                    //某列在数据库表格中，但是不在字典的Key里：删除
                    if (!dic.ContainsKey(col)) {
                        DropColumnFromDataBase(tableName, col);//表名，列名
                    }
                }

                //如果字典只有两项：[序号]、[时间]，清空该数据库表格的数据
                if (dic.Count <= 2) {
                    TruncateTable(tableName);
                }

                _isConnected = true; //数据库连接成功
                _timer.Enabled = true; //开启定时器

            }
            catch (Exception ex) {
                MessageBox.Show("异常：\r\n" + ex.Message, "数据库连接失败", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _isConnected = false; //数据库连接失败
                _timer.Enabled = false; //关闭定时器
            }            
        }


        // 私有方法：获取数据库中指定表格的列集合
        private static List<string> GetDatabaseColumns(string tableName) {
            string sql = "select column_name from information_schema.columns where table_name = '" + tableName + "'"; //获取指定表格的列集合
            DataTable dt = ExecuteQuerySql(sql); //执行查询SQL语句
            //取查询结果每一行的第一列
            List<string> list = new List<string>();
            foreach (DataRow row in dt.Rows) {
                list.Add(row[0].ToString());
            }
            return list;
        }


        // 私有方法：向数据库的表格中添加一列，并指定其类型
        private static void AddColumnToDataBase(string tableName, string columnName, string columnType) {
            string sql = "alter table [" + tableName + "] add [" + columnName + "] " + columnType;
            ExecuteNonQuerySql(sql); //执行非查询SQL语句
        }


        // 私有方法：从数据库的表格中删除一列
        private static void DropColumnFromDataBase(string tableName, string columnName) {
            string sql = "alter table [" + tableName + "] drop column [" + columnName + "]";
            ExecuteNonQuerySql(sql); //执行非查询SQL语句
        }


        // 私有方法：清空数据库表格所有记录，但是保留列结构
        private static void TruncateTable(string tableName) {
            ExecuteNonQuerySql("truncate table [" + tableName + "]"); //执行非查询SQL语句
        }

        #endregion


        #region 4. 主要功能：缓存产品数据入队列，再出列插入到数据库

        /// <summary>
        /// 主要功能：静态方法：缓存产品记录入队列
        /// </summary>
        /// <param name="tableName">要插入的表名</param>
        /// <param name="columnList">列集合</param>
        /// <param name="valueList">值集合</param>
        public static void EnqueueRecord(string tableName, List<string> columnList, List<string> valueList) {
            QueRecords.Enqueue(new RecordItem(tableName, columnList, valueList)); //入队列
        }


        // 私有方法：取队首元素，插入数据库
        private static void DequeueRecordIntoDatabase() {
            if (QueRecords.Count>0) { //判断队列不为空
                var item = QueRecords.Dequeue(); //队首出队
                InsertRecordIntoDatabase(item.TableName, item.ColumnList, item.ValueList); //插入数据库
            }            
        }


        // 私有方法：将一行记录（列、值），插入指定的数据库表格中
        private static void InsertRecordIntoDatabase(string tableName, List<string> columnList, List<string> valueList) {

            //创建深拷贝，即全新的对象
            List<string> columnListDeepCopy = new List<string>(columnList.ToArray());
            List<string> valueListDeepCopy = new List<string>(valueList.ToArray());

            //去除序号，因为在数据库表格中，序号是自增的主键，不需要手动插入
            if (columnListDeepCopy[0] == "序号") {
                columnListDeepCopy.RemoveAt(0); //默认序号是第0个元素
                valueListDeepCopy.RemoveAt(0);
            }

            //SQL语句格式：insert into [表名] ([列1], [列2], [列3]) values (值1, 值2, 值3)                        
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into [");
            sb.Append(tableName);
            sb.Append("] ([");
            sb.Append(string.Join("], [", columnListDeepCopy)); //使用 string.Join方法 拼接字符串
            sb.Append("])\r\nvalues (N'");
            sb.Append(string.Join("', N'", valueListDeepCopy)); //中文字符串格式：N'字符串'
            sb.Append("')");
            string sql = sb.ToString();
            ExecuteNonQuerySql(sql); //执行非查询SQL语句
        }

        #endregion


        #region 6. 私有方法：执行 SQL 语句

        // 私有方法：执行非查询 SQL 语句，用于增加/删除列，插入一行数据
        private static void ExecuteNonQuerySql(string sql) {
            _isSearching = true; //查询中






            try
            {
                using (SqlConnection con = new SqlConnection(_strConnection))
                {
                    con.Open();

          

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                _isSearching = false; //查询结束
                //throw; //抛出异常给调用者处理
            }




            _isSearching = false; //查询结束
        }

        // 方法：执行查询 SQL 语句
        public static DataTable ExecuteQuerySql(string sql) {
            _isSearching = true; //查询中
            DataTable dt = new DataTable();
            try {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, _strConnection)) {
                    adapter.Fill(dt);
                }
            }
            catch (Exception) {
                _isSearching = false; //查询结束
                throw; //抛出异常给调用者处理
            }
            _isSearching = false; //查询结束
            return dt;
        }

        #endregion

    }// class DatabaseManager


    #region 辅助类

    /// <summary>
    /// 辅助类：DatabaseManager 缓存队列中的元素，包含表名、列集合、值集合
    /// </summary>
    public class RecordItem {

        #region 1. 字段
        public string TableName; //要插入的表名
        public List<string> ColumnList; //列集合
        public List<string> ValueList;  //值集合 
        #endregion

        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：初始化队列元素的各字段，包括：表名、列集合、值集合
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="columns">列集合</param>
        /// <param name="values">值集合</param>
        public RecordItem(string table, List<string> columns, List<string> values) {
            TableName = table;
            ColumnList = columns;
            ValueList = values;
        } 

        #endregion

    }// class QueInsertItem 

    #endregion

}// namespace
