using System.Runtime.InteropServices;
using System.Text;

public class IniFileHelper
{
    private string filePath;

    // 导入Windows API
    [DllImport("kernel32.dll")]
    private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    public IniFileHelper(string filePath)
    {
        this.filePath = filePath;
    }

    // 写入INI
    public void WriteValue(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value, filePath);
    }

    // 读取INI
    public string ReadValue(string section, string key, string defaultValue = "")
    {
        StringBuilder sb = new StringBuilder(255);
        int result = GetPrivateProfileString(section, key, defaultValue, sb, 255, filePath);
        return sb.ToString();
    }

    // 读取整数
    public int ReadInt(string section, string key, int defaultValue = 0)
    {
        string value = ReadValue(section, key, defaultValue.ToString());
        if (int.TryParse(value, out int result))
            return result;
        return defaultValue;
    }

    // 读取布尔值
    public bool ReadBool(string section, string key, bool defaultValue = false)
    {
        string value = ReadValue(section, key, defaultValue.ToString());
        if (bool.TryParse(value, out bool result))
            return result;
        return defaultValue;
    }

    // 读取小数
    public double ReadDouble(string section, string key, double defaultValue = 0)
    {
        string value = ReadValue(section, key, defaultValue.ToString());
        if (double.TryParse(value, out double result))
            return result;
        return defaultValue;
    }
}