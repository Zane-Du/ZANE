using CommunityToolkit.Mvvm.ComponentModel;
using SqlSugar;

namespace SQL.Entity
{
    public partial class PumpTestDto : ObservableObject
    {
        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private int batchCount;

        public string 型号 { get; set; }

        public string 批次 { get; set; }

        public double 速度 { get; set; }

        public double 目标值 { get; set; }

        public double ActualValue { get; set; }

        public string 温度 { get; set; }

        public string Density { get; set; }

        public string 密封圈 { get; set; }

        public string 操作员 { get; set; }

        public DateTime CreateTime { get; set; }

        public string 当前CPK { get; set; }

        public bool IsDelete { get; set; }
        public string 转矩 { get; set; }

        public string 温度补偿量 { get; set; }
        public string 工艺补偿量 { get; set; }
        public string 温度补偿模式 { get; set; }
        public string Reserved06 { get; set; }
        public string Reserved07 { get; set; }
        public string Reserved08 { get; set; }
        public string Reserved09 { get; set; }
        public string Reserved10 { get; set; }



    }
}
