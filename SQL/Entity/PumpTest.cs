using SqlSugar;

namespace SQL.Entity
{
    public class PumpTest
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(IsNullable = false)]
        public int BatchCount { get; set; }

        [SugarColumn(IsNullable = false)]
        public string 型号 { get; set; }

        [SugarColumn(IsNullable = false)]
        public string 批次 { get; set; }

        [SugarColumn(IsNullable = false)]
        public double 速度 { get; set; }

        [SugarColumn(IsNullable = false)]
        public double 目标值 { get; set; }

        [SugarColumn(IsNullable = false)]
        public double ActualValue { get; set; }

        [SugarColumn(IsNullable = false)]
        public string 温度 { get; set; }

        [SugarColumn(IsNullable = false)]
        public string Density { get; set; }

        [SugarColumn(IsNullable = false)]
        public string 密封圈 { get; set; }


        //ColumnDataType 一般用于单个库数据库，如果多库不建议用
        //[SugarColumn(ColumnDataType = "Nvarchar(255)")]
        [SugarColumn(IsNullable = false)]
        public string 操作员 { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; }

        
        [SugarColumn(IsNullable = true)]
        public string 当前CPK { get; set; }

        [SugarColumn(IsNullable = false)]
        public bool IsDelete { get; set; }


        [SugarColumn(IsNullable = true)]


        public string 转矩 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string 温度补偿量 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string 工艺补偿量 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string 温度补偿模式 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Reserved06 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Reserved07 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Reserved08 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Reserved09 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Reserved10 { get; set; }

   

    }
}
