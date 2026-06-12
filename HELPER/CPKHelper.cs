using System;
using System.Collections.Generic;
using System.Linq;

namespace HELPER
{
    public class CPKCalculator
    {
        /// <summary>
        /// 计算CPK值
        /// </summary>
        /// <param name="values">数据列表</param>
        /// <param name="usl">规格上限</param>
        /// <param name="lsl">规格下限</param>
        /// <returns>CPK计算结果</returns>
        public static CPKResult CalculateCPK(List<double> values, double usl, double lsl)
        {
            if (values == null || values.Count < 2)
            {
                return new CPKResult
                {
                    IsValid = false,
                    Message = "数据点不足，至少需要2个数据点"
                };
            }

            if (usl <= lsl)
            {
                return new CPKResult
                {
                    IsValid = false,
                    Message = "规格上限必须大于规格下限"
                };
            }

            try
            {
                // 计算平均值
                double mean = values.Average();

                // 计算标准差 (使用样本标准差)
                double sumOfSquares = values.Sum(v => Math.Pow(v - mean, 2));
                double variance = sumOfSquares / (values.Count - 1);
                double stdDev = Math.Sqrt(variance);

                // 防止除零错误
                if (stdDev == 0)
                {
                    return new CPKResult
                    {
                        IsValid = false,
                        Message = "标准差为零，所有数据点相同"
                    };
                }

                // 计算CPU和CPL
                double cpu = (usl - mean) / (3 * stdDev);
                double cpl = (mean - lsl) / (3 * stdDev);

                // CPK是CPU和CPL的最小值
                double cpk = Math.Min(cpu, cpl);

                // 计算CP (过程能力)
                double cp = (usl - lsl) / (6 * stdDev);

                return new CPKResult
                {
                    IsValid = true,
                    CPK = cpk,
                    CPU = cpu,
                    CPL = cpl,
                    CP = cp,
                    Mean = mean,
                    StdDev = stdDev,
                    DataCount = values.Count,
                    USL = usl,
                    LSL = lsl,
                    Message = GetCPKMessage(cpk)
                };
            }
            catch (Exception ex)
            {
                return new CPKResult
                {
                    IsValid = false,
                    Message = $"计算CPK时出错: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取CPK的评估信息
        /// </summary>
        private static string GetCPKMessage(double cpk)
        {
            if (cpk >= 1.33)
                return "过程能力充足";
            else if (cpk >= 1.0)
                return "过程能力尚可";
            else if (cpk >= 0.67)
                return "过程能力不足";
            else
                return "过程能力严重不足";
        }
    }

    /// <summary>
    /// CPK计算结果类
    /// </summary>
    public class CPKResult
    {
        public bool IsValid { get; set; }
        public double CPK { get; set; }
        public double CPU { get; set; }
        public double CPL { get; set; }
        public double CP { get; set; }
        public double Mean { get; set; }
        public double StdDev { get; set; }
        public int DataCount { get; set; }
        public double USL { get; set; }
        public double LSL { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            if (!IsValid)
                return $"CPK计算失败: {Message}";

            return $"CPK计算结果:\n" +
                   $"数据点数: {DataCount}\n" +
                   $"平均值: {Mean:F4}\n" +
                   $"标准差: {StdDev:F4}\n" +
                   $"规格上限(USL): {USL:F2}\n" +
                   $"规格下限(LSL): {LSL:F2}\n" +
                   $"CPU: {CPU:F4}\n" +
                   $"CPL: {CPL:F4}\n" +
                   $"CPK: {CPK:F4}\n" +
                   $"CP: {CP:F4}\n" +
                   $"评估: {Message}";
        }
    }

}

