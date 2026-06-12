using System;
using System.Collections.Generic;
using System.Text;

namespace COMMUNICATION
{
    public class Helper
    {
        public static string ConvertHexListToString(List<string> hexList)
        {
            byte[] bytes = new byte[hexList.Count];

            for (int i = 0; i < hexList.Count; i++)
            {
                bytes[i] = Convert.ToByte(hexList[i], 16);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        public static string ExtractWTDataBySplit(string input)
        {
            // 按行分割
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("WT:"))
                {
                    // 返回 WT: 后面的内容，并去除首尾空格
                    return line.Substring(3).Trim();
                }
            }

            return null;
        }
    }
}
