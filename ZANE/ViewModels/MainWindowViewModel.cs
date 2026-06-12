
using Autofac;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQL;
using SQL.Entity;
using Framework2Core;
using HZH_Controls;
using HZH_Controls.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics.Statistics.Mcmc;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.X500;
using SixLabors.ImageSharp.ColorSpaces;
using SqlSugar;
using System;
//
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using ZANE.Views;
using static SkiaSharp.HarfBuzz.SKShaper;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using COMMUNICATION;
using OpenTK.Graphics.OpenGL;
using HELPER;


namespace ZANE.ViewModels
{
    #region 数据结构体类
    public class ZANE
    {
        public string? operatorName2 { get; set; }
        public string? num2 { get; set; }
        public string? temp2 { get; set; }
        public string? num22 { get; set; }
        public string? p2 { get; set; }
        public string? circle2 { get; set; }
        public string? speed2 { get; set; }
        public string? target2 { get; set; }
        public string? max2 { get; set; }
        public string? min2 { get; set; }
        public string? wt2 { get; set; }
        public string actual2 { get; set; }  // 用于实际值列
        public DateTime createtime2 { get; set; }  // 用于录入时间列

    }

    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }

    }

    public class ExcelHelper
    {
        public bool WriteExcel<T>(IEnumerable<T> entities, Type type) where T : class
        {
            try
            {
                // Create a new workbook and sheet
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(type.Name);

                // Create the header row
                IRow headerRow = sheet.CreateRow(0);

                // Get the properties of the type T dynamically (using reflection)
                PropertyInfo[] properties = type.GetProperties();

                // Create the header cells based on the properties
                for (int i = 0; i < properties.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(properties[i].Name);
                }

                // Add the data rows
                int rowIndex = 1;
                foreach (var entity in entities)
                {
                    IRow row = sheet.CreateRow(rowIndex);

                    // Get the property values for each entity
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(entity);
                        row.CreateCell(i).SetCellValue(value?.ToString() ?? string.Empty);  // Handle null values
                    }

                    rowIndex++;
                }

                // Show SaveFileDialog to choose the file location
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Write the workbook to the selected file path
                    using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(file);
                    }

                    Console.WriteLine($"File saved successfully at {filePath}");
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<T> ReadExcel<T>(Type type) where T : class
        {
            var entities = new List<T>();

            try
            {
                // Open the Excel file (this can be done using FileStream or OpenXml)
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Open the workbook and sheet
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var workbook = new XSSFWorkbook(fileStream);
                        var sheet = workbook.GetSheetAt(0);  // Get the first sheet (can modify if there are multiple sheets)

                        // Get the properties of the target type (T)
                        PropertyInfo[] properties = type.GetProperties();

                        // Loop through the rows in the sheet, starting from row 1 (skip header row)
                        for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            var row = sheet.GetRow(rowIndex);
                            if (row == null) continue;

                            // Create an instance of the entity
                            T entity = Activator.CreateInstance<T>();

                            // Loop through the columns in the row (map each cell to the corresponding property)
                            for (int colIndex = 0; colIndex < row.Cells.Count; colIndex++)
                            {
                                var cell = row.GetCell(colIndex);
                                var property = properties.FirstOrDefault(p => p.Name.Equals(sheet.GetRow(0).GetCell(colIndex)?.ToString(), StringComparison.OrdinalIgnoreCase));

                                if (property != null && cell != null)
                                {
                                    var cellValue = GetCellValue(cell);
                                    property.SetValue(entity, Convert.ChangeType(cellValue, property.PropertyType));
                                }
                            }

                            entities.Add(entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return entities;
        }

        // Helper method to get the cell value based on its type
        private object GetCellValue(ICell cell)
        {
            if (cell == null) return null;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue; // If it's a date, return DateTime
                    return cell.NumericCellValue; // Otherwise return numeric
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Formula:
                    return cell.CellFormula; // Formula (not evaluated, so you might want to handle this case)
                default:
                    return null;
            }
        }
    }

    #endregion

    public partial class MainWindowViewModel : ObservableObject
    {
        #region 构造函数
        public MainWindowViewModel(SqliteHelper sqliteHelper, IMapper mapper)
        {
            _mapper = mapper;
            _sqliteHelper = sqliteHelper;
            INI();


            DisplayPumpTestDtos = CurrentPumpTestDtos;




        }

        #endregion

        #region 属性和字段
        private ModbusTcpService _modbusTcpService;
        private ModbusAsciiService _modbusAsciiService;
        private ModbusRtuService _modbusRtuService;

        private CancellationTokenSource _monitorCts;
        private bool _isMonitoring = false;


        private ExcelHelper _excelHelper;

        private IMapper _mapper;
        private int BatchCount = 0;


        private SqliteHelper _sqliteHelper;

        private List<double> cpkHistory = new List<double>();
        public IniFileHelper _iniHelper;
        public IniFileHelper _iniHelperLog;

        //public static SPA5000Mobus plasmaCleaner1;



        [ObservableProperty]
        private int? pageSize = 100;


        [ObservableProperty]
        private ObservableCollection<PumpTestDto> displayPumpTestDtos = new();
        [ObservableProperty]
        private ObservableCollection<PumpTestDto> currentPumpTestDtos = new();
        [ObservableProperty]
        private ObservableCollection<PumpTestDto> historyPumpTestDtos = new();

        [ObservableProperty]
        private ObservableCollection<ISeries?> seriesCollection;


        [ObservableProperty]
        public bool autoTareEnabled = false;

        // 日志集合 - ObservableCollection会在UI发生变化时自动更新
        [ObservableProperty]
        private ObservableCollection<LogEntry> logMessages = new();

        [ObservableProperty]
        public bool kk = false;

        [ObservableProperty]
        public bool kk2 = false;


        [ObservableProperty]
        public string buttonContent;

        [ObservableProperty]
        public string buttonContent2;


        [ObservableProperty]
        public System.Windows.Media.Brush buttonBackground;

        [ObservableProperty]
        public System.Windows.Media.Brush buttonBackground2;



        #region 曲线相关属性
        [ObservableProperty]
        private SeriesCollection actualValueSeries;//重量曲线

        [ObservableProperty]
        private SeriesCollection actualValueSeries2;//CPK曲线

        [ObservableProperty]
        private SeriesCollection actualValueSeries3;//CPK全屏曲线

        [ObservableProperty]
        private SeriesCollection actualValueSeries4;//转矩曲线

        [ObservableProperty]
        private List<string> pointLabels;

        [ObservableProperty]
        private List<string> pointLabels2;

        [ObservableProperty]
        private List<string> pointLabels4;

        [ObservableProperty]
        private Func<double, string> yFormatter;

        [ObservableProperty]
        private Func<double, string> yFormatter2;

        [ObservableProperty]
        private Func<double, string> yFormatter4;

        #endregion







        [ObservableProperty]
        private ObservableCollection<string>? comPorts;

        [ObservableProperty]
        private int? comPortsSelected;


        [ObservableProperty]
        private ObservableCollection<string>? baudRates;
        [ObservableProperty]
        private int? baudRatesSelected;


        [ObservableProperty]
        private ObservableCollection<string>? dataBitsList;
        [ObservableProperty]
        private int? dataBitsSelected;


        [ObservableProperty]
        private ObservableCollection<string>? parityList;
        [ObservableProperty]
        private int? paritySelected;


        [ObservableProperty]
        private ObservableCollection<string>? stopBitsList;
        [ObservableProperty]
        private int? stopBitsSelected;





        [ObservableProperty]
        private string? operatorName;

        [ObservableProperty]
        private string? torque;//转矩

        [ObservableProperty]
        private string? temperatureCompensationValue;//温度补偿量

        [ObservableProperty]
        private string? processcompensation;//工艺补偿量

        [ObservableProperty]
        private string? temperatureCompensationMode;//温度补偿模式


        [ObservableProperty]
        private int? autoTareDelay;

        [ObservableProperty]
        private string? ip;

        [ObservableProperty]
        private string? port;

        [ObservableProperty]
        private string? frequency;


        [ObservableProperty]
        private string? virtualDataMax;

        [ObservableProperty]
        private string? virtualDataMin;

        [ObservableProperty]
        private bool? virtualDataEnabled;



        [ObservableProperty]
        private string? num;

        [ObservableProperty]
        private string? temp;

        [ObservableProperty]
        private string? num2;

        [ObservableProperty]
        private string? p;

        [ObservableProperty]
        private string? circle;

        [ObservableProperty]
        private string? speed;

        [ObservableProperty]
        private string? target;

        [ObservableProperty]
        private string? max;

        [ObservableProperty]
        private string? min;

        [ObservableProperty]
        private string? wt;

        [ObservableProperty]
        private string? wtCPK;


        [ObservableProperty]
        private ObservableCollection<ZANE>? zane;
        #endregion

        #region 命令

        [RelayCommand]
        private void RefreshData()
        {
            CurrentPumpTestDtos.Clear();
            // CreateChart(new double[0]);
            BatchCount = 0;
            // CPK = 0;
        }

        [RelayCommand]
        private async Task SaveData()
        {

            await savedata();

        }

        [RelayCommand]
        private void Delete(PumpTestDto pumpTest)
        {
            CurrentPumpTestDtos.Remove(pumpTest);
            BatchCount = 0;
            for (int i = CurrentPumpTestDtos.Count - 1; i > -1; i--)
            {
                CurrentPumpTestDtos[i].BatchCount = ++BatchCount;
            }
        }

        [RelayCommand]
        private async Task Record()
        {
            await record();

        }

        [RelayCommand]
        private async Task Record2()
        {
            await record2();

        }

        [RelayCommand]
        private async Task Record3()
        {
            await record();
            //  MessageBox.Show("记录称！");
        }

        [RelayCommand]
        private void Zero()
        {
            zero();
            //  MessageBox.Show("记录称！");
        }

        [RelayCommand]
        private void Save()
        {
            SaveSettings();
            AddLog("保存成功！");
        }

        [RelayCommand]
        private void Search()
        {
            //Winform窗体
            //var form = new FormSearch();
            //form.ShowDialog();

            //WPF窗体
            var searchWindow = App.container.Resolve<SearchView>();
            searchWindow.ShowDialog();



        }

        [RelayCommand]
        private void Refresh()
        {

        }

        [RelayCommand]
        private void ClearLog()
        {
            LogMessages.Clear();
        }
        #endregion

        #region 方法
        public void INI()
        {
            _excelHelper = new ExcelHelper();

            string iniPath = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Settings.ini");
            string iniPathLog = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Log.ini");
            _iniHelper = new IniFileHelper(iniPath);
            _iniHelperLog = new IniFileHelper(iniPathLog);

            LoadSettings();
            //Kk = _modbusAsciiService.IsConnected;
            updatestates();
            // infoDataGrid0.LoadConfigs();

            //  infoDataTable0.LoadConfigs();
            ComPorts = new ObservableCollection<string>()
            {
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6",
                "COM7", "COM8", "COM9", "COM10", "COM11", "COM12",
            };
            BaudRates = new ObservableCollection<string>()
            {
                "110","300","600","1200", "2400", "4800", "9600", "14400", "19200",
                "38400", "56000", "57600", "115200",
            };
            DataBitsList = new ObservableCollection<string>()
            {
                                "5 bit", "6 bit", "7 bit", "8 bit",

            };
            ParityList = new ObservableCollection<string>()
            {
                                "NONE", "ODD", "EVEN", "MARK", "SPACE",


            };
            StopBitsList = new ObservableCollection<string>()
            {
                "1 bit", "1.5 bit", "2 bit",
            };

            Type staticType = typeof(MainWindowViewModel); //本静态类的类型
            //加载配置文件
            staticType.LoadStaticConfigsFromIni();
            staticType.SaveStaticConfigsToIni();





            //zane = new ObservableCollection<ZANE>();
            //Zane.CollectionChanged += (s, e) => UpdateChart(); // 监听集合变化
            //Zane.CollectionChanged += (s, e) => UpdateCPKChart(); // 监听集合变化


            CurrentPumpTestDtos.CollectionChanged += (s, e) => UpdateChart(); // 监听集合变化
            CurrentPumpTestDtos.CollectionChanged += (s, e) => UpdateCPKChart(); // 监听集合变化
            CurrentPumpTestDtos.CollectionChanged += (s, e) => UpdateChart4(); // 监听集合变化





            ActualValueSeries = new SeriesCollection();
            PointLabels = new List<string>();
            YFormatter = value => value.ToString("F2"); // 格式化Y轴显示
            ActualValueSeries2 = new SeriesCollection();
            PointLabels2 = new List<string>();
            YFormatter2 = value => value.ToString("F2"); // 格式化Y轴显示
            ActualValueSeries4 = new SeriesCollection();
            PointLabels4 = new List<string>();
            YFormatter4 = value => value.ToString("F2"); // 格式化Y轴显示

            queryDataFrom = DateTime.Now.Date;
            queryDataTo = DateTime.Now.Date.AddDays(1);

            _modbusAsciiService = new ModbusAsciiService(ComPortsSelected switch
            {
                0 => "COM1",
                1 => "COM2",
                2 => "COM3",
                3 => "COM4",
                4 => "COM5",
                5 => "COM6",
                6 => "COM7",
                7 => "COM8",
                8 => "COM9",
                9 => "COM10",
                10 => "COM11",
                11 => "COM12",
            }, BaudRatesSelected switch
            {
                0 => 110,
                1 => 300,
                2 => 600,
                3 => 1200,
                4 => 2400,
                5 => 4800,
                6 => 9600,
                7 => 14400,
                8 => 19200,
                9 => 38400,
                10 => 56000,
                11 => 57600,
                12 => 1152000,

            });


        }

        #region ModbucAscii连接方法
        [RelayCommand]
        private void Connect()
        {
            try
            {
                _modbusAsciiService.ConnectAsync();
            }
            catch
            {
                AddLog("连接失败，请检查串口设置和设备状态！");
                return;
            }

            Kk = _modbusAsciiService.IsConnected;
            updatestates();

        }

        [RelayCommand]
        private void DisConnect()
        {

            _modbusAsciiService.Close();
            Kk = _modbusAsciiService.IsConnected;
            updatestates();
        }
        #endregion


        #region ModbusTCP连接方法
        private async Task InitModbusTCPAsync()
        {
            if (string.IsNullOrEmpty(Ip) || string.IsNullOrEmpty(Port))
            {
                AddLog("Modbus配置未设置");
                return;
            }

            _modbusTcpService = new ModbusTcpService(Ip, int.Parse(Port));

            try
            {
                bool success = await _modbusTcpService.ConnectAsync();
                if (success)
                {
                    AddLog($"Modbus连接成功 {Ip}:{Port}");
                }
                else
                {
                    AddLog("Modbus连接失败");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Modbus连接异常: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Connect2()
        {
            try
            {

                _ = InitModbusTCPAsync();
                await Task.Delay(200);
                if (_modbusTcpService != null && _modbusTcpService.IsConnected)
                {
                    StartMonitor();
                }

            }
            catch
            {
                AddLog("连接失败，请检查网口设置和设备状态！");
                return;
            }

            Kk2 = _modbusTcpService.IsConnected;
            updatestates();
            //MessageBox.Show("连接成功！");
        }



        [RelayCommand]
        private void DisConnect2()
        {
            StopMonitor();
            Kk2 = _modbusTcpService.IsConnected;
            updatestates();
            Cleanup();

            StopMonitor();
            Kk2 = _modbusTcpService.IsConnected;
            updatestates();
            Cleanup();
        }

        #endregion


        #region 保存数据方法
        public async Task savedata()
        {
            try
            {
                if (CurrentPumpTestDtos == null || CurrentPumpTestDtos.Count == 0)
                {
                    MessageBox.Show("无数据保存！");
                    return;
                }
                int saveCount = 0;
                int notSaveCount = 0;
                var latestData = currentPumpTestDtos.FirstOrDefault();
                latestData.当前CPK = WtCPK;




                var reversedList = new ObservableCollection<PumpTestDto>(CurrentPumpTestDtos.Reverse());

                await Task.Run(async () =>
                {
                    foreach (var item in reversedList)
                    {
                        if (item.Id > 0)
                        {
                            notSaveCount++;
                            continue;
                        }



                        var pumpTest = _mapper.Map<PumpTest>(item);


                        var id = await _sqliteHelper.DBContext.Insertable(pumpTest).ExecuteReturnIdentityAsync();


                        if (id <= 0)
                        {
                            AddLog("记录失败!");
                            MessageBox.Show("记录失败!");
                        }
                        else
                        {
                            saveCount++;
                            item.Id = id;
                        }
                    }

                });




                //await Task.Run(async () =>
                //{
                //    foreach (var item in CurrentPumpTestDtos)
                //    {
                //        if (item.Id > 0)
                //        {
                //            notSaveCount++;
                //            continue;
                //        }



                //        var pumpTest = _mapper.Map<PumpTest>(item);


                //        var id = await _sqliteHelper.DBContext.Insertable(pumpTest).ExecuteReturnIdentityAsync();


                //        if (id <= 0)
                //        {
                //            AddLog("记录失败!");
                //            MessageBox.Show("记录失败!");
                //        }
                //        else
                //        {
                //            saveCount++;
                //            item.Id = id;
                //        }



                //        //if (run != null)
                //        //    await Application.Current.Dispatcher.InvokeAsync(() => run.Text = saveCount.ToString());




                //    }

                //});

                string msg = $"保存[{saveCount}]条数据成功！{(notSaveCount > 0 ? $"[{notSaveCount}]条数据为之前已经保存数据！" : "")}";
                AddLog(msg);
                MessageBox.Show("保存数据成功！");
            }
            catch (Exception ex)
            {
                string msg = $"保存数据异常：{ex}！";
                AddLog(msg);
            }


        }

        #endregion
        private List<double> ExtractDoubleValues(List<string> stringValues)
        {
            var result = new List<double>();
            foreach (var value in stringValues)
            {
                if (double.TryParse(value, out double doubleValue))
                {
                    result.Add(doubleValue);
                }
                else
                {
                    result.Add(0);  // 或跳过
                }
            }
            return result;
        }
        private void AddCPKReferenceLines()
        {
            // 添加CPK=1.33的参考线（能力充足线）
            ActualValueSeries2.Add(new LineSeries
            {
                Title = "CPK=1.33",
                Values = new ChartValues<double>(Enumerable.Repeat(1.33, CurrentPumpTestDtos.Count).ToList()),
                StrokeThickness = 1,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 4 },  // 虚线
                Stroke = System.Windows.Media.Brushes.Green,
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometry = null  // 不显示点
            });

            // 添加CPK=1.0的参考线（能力及格线）
            ActualValueSeries2.Add(new LineSeries
            {
                Title = "CPK=1.0",
                Values = new ChartValues<double>(Enumerable.Repeat(1.0, CurrentPumpTestDtos.Count).ToList()),
                StrokeThickness = 1,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 4 },
                Stroke = System.Windows.Media.Brushes.Orange,
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometry = null
            });

            // 添加CPK=0.67的参考线（能力不足线）
            ActualValueSeries2.Add(new LineSeries
            {
                Title = "CPK=0.67",
                Values = new ChartValues<double>(Enumerable.Repeat(0.67, CurrentPumpTestDtos.Count).ToList()),
                StrokeThickness = 1,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 4 },
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometry = null
            });
        }
        List<double> HistoryList = new List<double>();

        private void UpdateCPKChart()
        {
            // 提取所有speed2值用于CPK计算
            var allSpeedValues = ExtractDoubleValues(CurrentPumpTestDtos.Select(x => x.ActualValue.ToString()).ToList());

            // 清空CPK历史并重新计算（确保数据一致性）
            cpkHistory.Clear();

            // 计算累积CPK值
            var cpkValues = new List<double>();
            var tempList = new List<double>();



            for (int i = 0; i < allSpeedValues.Count; i++)
            {
                tempList.Add(allSpeedValues[i]);

            }
            // 至少需要2个点才能计算CPK
            if (tempList.Count >= 10)
            {
                var result = CPKCalculator.CalculateCPK(new List<double>(tempList), Max.ToDouble(), Min.ToDouble());
                if (result.IsValid)
                {
                    cpkValues.Add(result.CPK);
                    cpkHistory.Add(result.CPK);
                }
                else
                {
                    // 如果CPK计算无效，添加0或上一个有效值
                    cpkValues.Add(cpkValues.LastOrDefault());
                }
            }
            else
            {
                // 数据点不足时，添加0
                cpkValues.Add(0);
            }

            WtCPK = cpkValues[0].ToString("F3");
            HistoryList.Add(cpkValues[0]);
            // 更新CPK图表l
            ActualValueSeries2.Clear();

            if (HistoryList.Any())
            {
                ActualValueSeries2.Add(new LineSeries
                {
                    Title = "实时CPK",
                    Values = new ChartValues<double>(HistoryList),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Red,  // 红色表示CPK曲线

                    // 可以根据CPK值设置不同颜色
                    // Stroke = GetCPKColor(cpkValues.LastOrDefault())
                });

                // 可以添加参考线
                AddCPKReferenceLines();

                // 更新X轴标签
                PointLabels2 = Enumerable.Range(1, HistoryList.Count)
                    .Select(i => i.ToString())
                    .ToList();
            }

            OnPropertyChanged(nameof(ActualValueSeries2));


        }
        private void UpdateChart()
        {
            if (CurrentPumpTestDtos == null || CurrentPumpTestDtos.Count == 0) return;

            // 提取actual2的值并转换为double
            var actualValues = CurrentPumpTestDtos
                .Select((item, index) =>
                {
                    string newstr = "";
                    if (item.ActualValue != null && item.ActualValue.ToString() != "" && item.ActualValue.ToString().Contains("g"))
                    {
                        newstr = item.ActualValue.ToString().Substring(0, item.ActualValue.ToString().Length - 1);

                    }
                    else if (item.ActualValue.ToString() != null && item.ActualValue.ToString() != "" && !item.ActualValue.ToString().Contains("g"))
                    {
                        newstr = item.ActualValue.ToString();

                    }

                    double.TryParse(newstr, out double value);
                    return value;
                })
                .ToList();

            List<double> actualValuesInvert = actualValues.Reverse<double>().ToList();

            // 更新图表
            ActualValueSeries.Clear();
            ActualValueSeries.Add(new LineSeries
            {
                Title = "实际值",
                Values = new ChartValues<double>(actualValuesInvert),
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            });

            // 更新X轴标签（显示序号或时间）
            PointLabels = CurrentPumpTestDtos
                .Select((item, index) => (index + 1).ToString())
                .ToList();
        }
        private void UpdateChart4()
        {
            if (CurrentPumpTestDtos == null || CurrentPumpTestDtos.Count == 0) return;

            // 提取actual2的值并转换为double
            var actualValues = CurrentPumpTestDtos
                .Select((item, index) =>
                {
                    string newstr = "";
                    if (item.ActualValue != null && item.ActualValue.ToString() != "" && item.ActualValue.ToString().Contains("g"))
                    {
                        newstr = item.ActualValue.ToString().Substring(0, item.ActualValue.ToString().Length - 1);

                    }
                    else if (item.ActualValue.ToString() != null && item.ActualValue.ToString() != "" && !item.ActualValue.ToString().Contains("g"))
                    {
                        newstr = item.ActualValue.ToString();

                    }

                    double.TryParse(newstr, out double value);
                    return value;
                })
                .ToList();


            if (Torque != null)
            {
                List<double> actualValuesInvert4 = Torque.Split(',').Select(double.Parse).ToList();

                // 更新图表
                ActualValueSeries4.Clear();
                ActualValueSeries4.Add(new LineSeries
                {
                    Title = "实际值",
                    Values = new ChartValues<double>(actualValuesInvert4),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent
                });

                // 更新X轴标签（显示序号或时间）
                PointLabels4 = CurrentPumpTestDtos
                    .Select((item, index) => (index + 1).ToString())
                    .ToList();

            }

        }
        public void LoadSettings()
        {
            // Wt = _iniHelper.ReadValue("UserSettings", "Wt");
            Target = _iniHelper.ReadValue("UserSettings", "Target");
            Max = _iniHelper.ReadValue("UserSettings", "Max");
            Min = _iniHelper.ReadValue("UserSettings", "Min");
            Num2 = _iniHelper.ReadValue("UserSettings", "Num2");
            Speed = _iniHelper.ReadValue("UserSettings", "Speed");
            Temp = _iniHelper.ReadValue("UserSettings", "Temp");
            Circle = _iniHelper.ReadValue("UserSettings", "Circle");
            Num = _iniHelper.ReadValue("UserSettings", "Num");
            P = _iniHelper.ReadValue("UserSettings", "P");
            OperatorName = _iniHelper.ReadValue("UserSettings", "OperatorName");
            Ip = _iniHelper.ReadValue("UserSettings", "Ip");
            Port = _iniHelper.ReadValue("UserSettings", "Port");
            Frequency = _iniHelper.ReadValue("UserSettings", "Frequency");
            VirtualDataEnabled = _iniHelper.ReadValue("UserSettings", "VirtualDataEnabled").ToBool();
            VirtualDataMax = _iniHelper.ReadValue("UserSettings", "VirtualDataMax");
            VirtualDataMin = _iniHelper.ReadValue("UserSettings", "VirtualDataMin");

            AutoTareEnabled = _iniHelper.ReadValue("UserSettings", "AutoTareEnabled").ToBool();


            AutoTareDelay = _iniHelper.ReadValue("UserSettings", "AutoTareDelay").ToInt();

            ComPortsSelected = _iniHelper.ReadValue("UserSettings", "ComPortsSelected").ToInt();
            BaudRatesSelected = _iniHelper.ReadValue("UserSettings", "BaudRatesSelected").ToInt();
            DataBitsSelected = _iniHelper.ReadValue("UserSettings", "DataBitsSelected").ToInt();
            ParitySelected = _iniHelper.ReadValue("UserSettings", "ParitySelected").ToInt();
            StopBitsSelected = _iniHelper.ReadValue("UserSettings", "StopBitsSelected").ToInt();



        }
        public void SaveSettings()
        {

            _iniHelper.WriteValue("UserSettings", "Num", Num);
            _iniHelper.WriteValue("UserSettings", "Speed", Speed);
            _iniHelper.WriteValue("UserSettings", "Max", Max);
            _iniHelper.WriteValue("UserSettings", "Min", Min);
            _iniHelper.WriteValue("UserSettings", "Wt", Wt);
            _iniHelper.WriteValue("UserSettings", "Num2", Num2);
            _iniHelper.WriteValue("UserSettings", "Circle", Circle);
            _iniHelper.WriteValue("UserSettings", "Temp", Temp);
            _iniHelper.WriteValue("UserSettings", "P", P);
            _iniHelper.WriteValue("UserSettings", "Target", Target);
            _iniHelper.WriteValue("UserSettings", "OperatorName", OperatorName);
            _iniHelper.WriteValue("UserSettings", "Ip", Ip);
            _iniHelper.WriteValue("UserSettings", "Port", Port);
            _iniHelper.WriteValue("UserSettings", "Frequency", Frequency);
            _iniHelper.WriteValue("UserSettings", "VirtualDataEnabled", VirtualDataEnabled.ToString());
            _iniHelper.WriteValue("UserSettings", "VirtualDataMax", VirtualDataMax);
            _iniHelper.WriteValue("UserSettings", "VirtualDataMin", VirtualDataMin);

            _iniHelper.WriteValue("UserSettings", "AutoTareEnabled", AutoTareEnabled.ToString());
            _iniHelper.WriteValue("UserSettings", "ComPortsSelected", ComPortsSelected.ToString());
            _iniHelper.WriteValue("UserSettings", "BaudRatesSelected", BaudRatesSelected.ToString());
            _iniHelper.WriteValue("UserSettings", "DataBitsSelected", DataBitsSelected.ToString());
            _iniHelper.WriteValue("UserSettings", "ParitySelected", ParitySelected.ToString());
            _iniHelper.WriteValue("UserSettings", "StopBitsSelected", StopBitsSelected.ToString());
            //          _iniHelper.WriteValue("UserSettings", "ComPortsSelected", ComPortsSelected.ToString());


            _iniHelper.WriteValue("UserSettings", "AutoTareDelay", AutoTareDelay.ToString());




        }

        public void SaveSettingsLog()
        {


        }
        public void updatestates()
        {
            if (Kk == true)
            {
                ButtonContent = "已连接";
                ButtonBackground = new SolidColorBrush(Colors.Green);
                AddLog("  串口连接成功");
            }
            else
            {
                ButtonContent = "未连接";
                ButtonBackground = new SolidColorBrush(Colors.Red);
                AddLog("串口断开成功");
            }

            if (Kk2 == true)
            {
                ButtonContent2 = "已连接";
                ButtonBackground2 = new SolidColorBrush(Colors.Green);
                AddLog("  网口连接成功");
            }
            else
            {
                ButtonContent2 = "未连接";
                ButtonBackground2 = new SolidColorBrush(Colors.Red);
                AddLog("网口断开成功");
            }
            // 这个属性会自动在KK变化时更新

            //public string ButtonContent => Kk ? "已连接" : "未连接";

            //// 这个属性也会自动在KK变化时更新
            //public System.Windows.Media.Brush ButtonBackground => Kk ?
            //    new SolidColorBrush(Colors.Green) :
            //    new SolidColorBrush(Colors.Red);
        }
        public bool stableornot(double a1, double a2, double a3)
        {
            // 两两比较，检查差值是否都在0.05以内
            if (Math.Abs(a1 - a2) <= 0.05 &&
                Math.Abs(a1 - a3) <= 0.05 &&
                Math.Abs(a2 - a3) <= 0.05)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        int i = 0;

        public void AddLog(string message)
        {
            // 使用Dispatcher确保在UI线程上添加（因为可能被后台线程调用）
            App.Current.Dispatcher.Invoke(() =>
            {
                LogMessages.Add(new LogEntry
                {
                    Time = DateTime.Now,
                    Message = message
                });

                //_iniHelperLog.WriteValue("Log", "Time", DateTime.Now.ToString());
                _iniHelperLog.WriteValue("Log", DateTime.Now.ToString("HH: mm:ss.fff"), message);


                // 可选：自动滚动到底部，需要配合ListView的行为
                // 如果日志太多，可以限制数量
                if (LogMessages.Count > 100)
                {
                    LogMessages.RemoveAt(0);
                }
            });
        }

        #region 查找数据和导出csv
        [ObservableProperty]
        private int? totalPage;

        [ObservableProperty]
        private int? pageNumber = 1;

        [ObservableProperty]
        private DateTime? queryDataFrom;

        [ObservableProperty]
        private DateTime? queryDataTo;





        [RelayCommand]
        private void QueryCondition() => QueryPumpTest();
        private async Task QueryPumpTest()
        {
            try
            {
                RefAsync<int> total = 0;
                PageSize = 5000;//不要从sql分页，分页不能做整体数据分析
                                //  var list = await _sqliteHelper.DBContext.Queryable<PumpTest>().Where(item => item.CreateTime >= queryDataFrom && item.CreateTime <= queryDataTo && item.IsDelete == false)
                var list = await _sqliteHelper.DBContext.Queryable<PumpTest>().Where(item => item.CreateTime >= queryDataFrom && item.CreateTime <= queryDataTo)
                 .OrderByDescending(item => item.Id).ToPageListAsync((int)pageNumber, (int)pageSize, total); //ToPageAsync
                                                                                                             // }
                int count = list.Count;
                AddLog($"共查询出{count}条");
                TotalPage = total / pageSize + 1;

                HistoryPumpTestDtos.Clear();

                int index = 0;
                var chartValues = new double[count];
                foreach (var item in list)
                {
                    var itemDto = _mapper.Map<PumpTestDto>(item);
                    HistoryPumpTestDtos.Add(itemDto);

                    chartValues[index] = item.ActualValue;
                    index++;
                }

                //CreateChart(chartValues.Reverse().ToArray());


                if (HistoryPumpTestDtos.Count >= 10)
                {

                    //CPK = CpkHelper.CalculateCpk(HistoryPumpTestDtos.Select(x => x.ActualValue).ToArray(), USL, LSL, Target);


                }
            }
            catch (Exception ex)
            {
                string msg = $"查询异常：{ex}！";
                AddLog(msg);
                MessageBox.Show(msg);
            }

        }


        [ObservableProperty]
        private string countFrom = "1";

        [ObservableProperty]
        private string countTo = "100";
        [ObservableProperty]
        private bool isTareAuto = true;

        //[RelayCommand]
        //private async Task ExportExcel()
        //{
        //    List<PumpTestDto> pumpTestDtos = new List<PumpTestDto>();
        //    int countFromTemp = 0, countToTemp = 0;
        //    if (int.TryParse(CountFrom, out countFromTemp) && int.TryParse(CountTo, out countToTemp) && countFromTemp < countToTemp)
        //    {

        //    }
        //    else
        //    {
        //        MessageBox.Show("导出数据失败：输入的字符不是数字!");
        //        return;
        //    }
        //    var list = await _sqliteHelper.DBContext.Queryable<PumpTest>().Where(item => item.Id >= countFromTemp && item.Id <= countToTemp).OrderBy(item => item.CreateTime).ToListAsync();

        //    pumpTestDtos.Clear();
        //    foreach (var item in list)
        //    {
        //        var itemDto = _mapper.Map<PumpTestDto>(item);
        //        pumpTestDtos.Add(itemDto);
        //    }
        //    var result = _excelHelper.WriteExcel<PumpTestDto>(pumpTestDtos, typeof(PumpTestDto));
        //    //MessageBox.Show(result ? "导出成功！" : "导出失败！");
        //}


        [RelayCommand]
        private async Task ExportExcel()
        {
            try
            {
                // 1. 验证日期范围
                if (!queryDataFrom.HasValue || !queryDataTo.HasValue)
                {
                    MessageBox.Show("请先选择导出日期范围！");
                    return;
                }

                // 2. 查询数据
                var list = await _sqliteHelper.DBContext.Queryable<PumpTest>()
                    .Where(item => item.CreateTime >= queryDataFrom &&
                                  item.CreateTime <= queryDataTo)
                    .OrderBy(item => item.CreateTime)
                    .ToListAsync();

                // 3. 检查是否有数据
                if (list == null || list.Count == 0)
                {
                    MessageBox.Show("所选日期范围内没有数据可导出！");
                    return;
                }

                // 4. ✅ 将查询结果转换为 DTO 并添加到集合中
                List<PumpTestDto> pumpTestDtos = new List<PumpTestDto>();
                foreach (var item in list)
                {
                    var itemDto = _mapper.Map<PumpTestDto>(item);
                    pumpTestDtos.Add(itemDto);
                }

                // 5. 导出 Excel
                var result = _excelHelper.WriteExcel<PumpTestDto>(pumpTestDtos, typeof(PumpTestDto));

                // 6. 提示结果
                if (result)
                {
                    AddLog($"成功导出 {pumpTestDtos.Count} 条数据");
                    MessageBox.Show($"导出成功！共导出 {pumpTestDtos.Count} 条数据");
                }
                else
                {
                    MessageBox.Show("导出失败！");
                }
            }
            catch (Exception ex)
            {
                AddLog($"导出异常：{ex.Message}");
                MessageBox.Show($"导出失败：{ex.Message}");
            }
        }
        #endregion


        #region CPK曲线全屏显示
        [RelayCommand]
        private void ShowCPKFullScreen()
        {
            var fullScreenWindow = new CPKFullScreenWindow
            {
                // 重要：将当前窗口的 DataContext 传递给新窗口，确保数据共享
                DataContext = this
            };
            ActualValueSeries3 = ActualValueSeries2;

            fullScreenWindow.ShowDialog(); // 或者 Show() 根据需求选择模态或非模态
        }

        #endregion


        #region 测试模拟随机数生成方法
        public static int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                MessageBox.Show("模拟数据最小值不能大于最大值！");
                throw new ArgumentOutOfRangeException(nameof(minValue), "最小值不能大于最大值");

            }

            // 使用 RandomNumberGenerator 生成真正的随机字节
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[4]; // 4 字节 = 32 位整数
                rng.GetBytes(randomBytes);

                // 将字节转换为无符号 32 位整数，确保均匀分布
                uint randomUInt = BitConverter.ToUInt32(randomBytes, 0);

                // 计算范围长度
                long range = (long)maxValue - minValue + 1;

                // 将随机数映射到 [minValue, maxValue] 区间
                long randomValue = (long)(randomUInt % range) + minValue;

                return (int)randomValue;
            }
        }


        #endregion

        #endregion

        #region Modbus信号监控

        private void StartMonitor()
        {
            if (_isMonitoring) return;
            _monitorCts = new CancellationTokenSource();
            _isMonitoring = true;
            _ = RunMonitorAsync(_monitorCts.Token);
            AddLog("注液监控已启动");
        }

        private void StopMonitor()
        {
            _monitorCts?.Cancel();
            _isMonitoring = false;
            AddLog("注液监控已停止");
        }

        private async Task RunMonitorAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_modbusTcpService == null || !_modbusTcpService.IsConnected)
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }

                    // 每20ms读取6061
                    short signal = await _modbusTcpService.ReadIntRegisterAsync(0x1798);//伺服状态

                    if (signal == 1)
                    {
                        AddLog("收到注液信号");
                        await DoInjectionWork();
                        await Task.Delay(500, token); // 避免重复触发
                    }

                    await Task.Delay(100, token);
                }
                catch (Exception ex)
                {
                    AddLog($"监控异常: {ex.Message}");
                    await Task.Delay(500, token);
                }
            }
        }


        #endregion

        public void Cleanup()
        {
            _modbusTcpService?.Close();
        }



        public void zero()
        {
            _modbusAsciiService.SendCommand000();

        }

        private async Task DoInjectionWork()
        {
            try
            {
                // 1. 读取4个地址
                float val1 = await _modbusTcpService.ReadFloatRegisterAsync(0x03FA);//当前温度
              
                float val2 = await _modbusTcpService.ReadFloatRegisterAsync(0x1006);//温度补偿量
           
                float val3 = await _modbusTcpService.ReadFloatRegisterAsync(0x1270);//工艺补偿量
               
                float val4 = await _modbusTcpService.ReadIntRegisterAsync(0x0FE6);//温度补偿模式

                AddLog($"参数: {val1}, {val2}, {val3}, {val4}");
                TemperatureCompensationMode = val1.ToString();
                Temp = val2.ToString();
                TemperatureCompensationValue = val3.ToString();
                Processcompensation = val4.ToString();


                // 2. 循环读取6050直到6061=2
                List<float> flowList = new List<float>();

                while (true)
                {
                    float flow = await _modbusTcpService.ReadFloatRegisterAsync(0x17A2);//转矩地址
                    flowList.Add(flow);

                    short status = await _modbusTcpService.ReadIntRegisterAsync(0x1798);//伺服状态
                    if (status == 2)
                    {
                        AddLog($"注液完成，共采集{flowList.Count}个数据点");
                        Torque = string.Join(",", flowList);
                        break;
                    }

                    await Task.Delay(Frequency.ToInt());
                }

            }
            catch (Exception ex)
            {
                AddLog($"注液过程异常: {ex.Message}");
            }
        }


        public async Task record()
        {
            #region 读取电子秤数据
            i++;
            string k1 = "";
            string k2 = "";
            string k3 = "";

            while (true)  // 用while循环代替goto
            {
                #region 启用虚拟数据/如果电子称未连接，直接跳出循环
                if (_modbusAsciiService.IsConnected == false)
                {
                    if (VirtualDataEnabled == true)
                    {
                        int min = (int)(float.Parse(VirtualDataMin.ToString()) * 100);
                        int max = (int)(float.Parse(VirtualDataMax.ToString()) * 100);
                        float jks = (Next(min, max));
                        float sjkd = jks / 100;
                        Wt = sjkd.ToString("F2");  // 使用真正的随机数生成器

                        Temp = "1";
                        Torque = "1.1,1.2,1.4,1.2";
                        TemperatureCompensationValue = "1";
                        Processcompensation = "1";
                        temperatureCompensationMode = "1";


                        //float val1 = await _modbusService.ReadFloatRegisterAsync(0x03FA);//当前温度
                        //float val2 = await _modbusService.ReadFloatRegisterAsync(0x1006);//温度补偿量
                        //float val3 = await _modbusService.ReadFloatRegisterAsync(0x1270);//工艺补偿量
                        //float val4 = await _modbusService.ReadFloatRegisterAsync(0x17A2);//转矩

                        //float val5 = await _modbusService.ReadIntRegisterAsync(0x1798);//伺服状态
                        //float val6 = await _modbusService.ReadIntRegisterAsync(0x0FE6);//温度补偿模式



                    }
                    else
                    {
                        Wt = "0.00";
                    }


                    break;  // 如果设备未连接，跳出循环
                }
                #endregion

                #region 读取电子称三次判断稳定性(修改，需要验证)
                // 第一次读取

                // byte[] byteSendCmd1 = new byte[4] { 0x30, 0x30, 0x31, 0x3F };
                //byte[] res1= await _modbusAsciiService.SendRawBytesAsync(byteSendCmd1);
                // List<string> a1 = new List<string>();
                // string asciiString1 = Helper.ConvertHexListToString(a1);
                // k1 = Helper.ExtractWTDataBySplit(asciiString1);


                _modbusAsciiService.SendCommand3(out k1);
       
                await Task.Delay(300);  // 不阻塞UI线程

                // 第二次读取
                _modbusAsciiService.SendCommand3(out k2);

                await Task.Delay(300);

                // 第三次读取
                _modbusAsciiService.SendCommand3(out k3);




                bool isStable = false;
                if (k1 != null && k2 != null && k3 != null)
                {
                    try
                    {
                        double val1 = k1.Substring(0, k1.Length - 1).ToDouble();
                        double val2 = k2.Substring(0, k2.Length - 1).ToDouble();
                        double val3 = k3.Substring(0, k3.Length - 1).ToDouble();
                        isStable = stableornot(val1, val2, val3);
                    }
                    catch (Exception ex)
                    {
                        AddLog($"数据转换错误: {ex.Message}");
                        await Task.Delay(300);
                        continue;  // 继续下一次循环
                    }
                }

                if (isStable && k1 != null && k2 != null && k3 != null)
                {
                    double average = (k1.Substring(0, k1.Length - 1).ToDouble() +
                                     k2.Substring(0, k2.Length - 1).ToDouble() +
                                     k3.Substring(0, k3.Length - 1).ToDouble()) / 3;

                    // UI更新需要在UI线程上执行
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Wt = average.ToString("F2");
                        AddLog($"稳定记录，值为{Wt}");
                    });

                    break;  // 跳出循环，继续执行后面的代码
                }
                else
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        AddLog("不稳定，重新记录");
                    });

                    await Task.Delay(300);
                    // 继续循环，不需要goto
                }
                #endregion
            }

            #endregion

            #region 主界面更新数据


            PumpTestDto pumpTestDto = new PumpTestDto();
            pumpTestDto = new PumpTestDto()
            {
                BatchCount = ++BatchCount,
                型号 = Num,//型号
                批次 = Num2,//批次
                速度 = Speed.ToDouble(),
                目标值 = Target.ToDouble(),
                //ActualValue = 499.6 + i * 0.05,
                ActualValue = Math.Round(Wt.ToDouble(), 3),

                温度 = Temp,
                Density = P,
                密封圈 = Circle,
                操作员 = OperatorName,
                CreateTime = DateTime.Now,
                转矩 = Torque,
                温度补偿量 = TemperatureCompensationValue,
                工艺补偿量 = Processcompensation,
                温度补偿模式 = TemperatureCompensationMode
            };




            Application.Current.Dispatcher.Invoke(() => CurrentPumpTestDtos.Insert(0, pumpTestDto));
            //CreateChart(CurrentPumpTestDtos.Select(x => x.ActualValue).Reverse().ToArray());
            //if (CurrentPumpTestDtos.Count >= 10)
            //{
            //    CPK = CpkHelper.CalculateCpk(CurrentPumpTestDtos.Select(x => x.ActualValue).ToArray(), USL, LSL, Target);
            //}

            #endregion


            #region 如果启用自动去皮，进行清零操作
            if (AutoTareEnabled == true)
            {
                zero();
                await Task.Delay(AutoTareDelay.ToInt());
                AddLog("自动去皮已启用，已执行去皮命令");
            }
            #endregion
        }


        float var1=0, var2=0, var3=0, var4=0, var5, var6=0;
        public async Task record2()
        {

            i++;
            string k1 = "";
            string k2 = "";
            string k3 = "";


            #region 读取网口数据

            if (VirtualDataEnabled == true)
            {
                int min = (int)(float.Parse(VirtualDataMin.ToString()) * 100);
                int max = (int)(float.Parse(VirtualDataMax.ToString()) * 100);
                float jks = (Next(min, max));
                float sjkd = jks / 100;
                Wt = sjkd.ToString("F2");  // 使用真正的随机数生成器

                var1 = 1;
                var2 = 2;
                var3 = 3;
                var4 = 4;
                var5 = 5;
                var6 = 6;
       




            }
            else
            {

                Wt = "0";


               var1 = await _modbusTcpService.ReadFloatRegisterAsync(0x03FA);//当前温度
              
               var2 = await _modbusTcpService.ReadFloatRegisterAsync(0x1006);//温度补偿量
               var3 = await _modbusTcpService.ReadFloatRegisterAsync(0x1270);//工艺补偿量
               var4 = await _modbusTcpService.ReadFloatRegisterAsync(0x17A2);//转矩
               var5 = await _modbusTcpService.ReadIntRegisterAsync(0x1798);//伺服状态

                var6 = await _modbusTcpService.ReadIntRegisterAsync(0x0FE6);//温度补偿模式
                int aa = 0;
                int bb = 0;
                int cc = 0;

            }




            #endregion



            #region 主界面更新数据
            PumpTestDto pumpTestDto = new PumpTestDto();
            pumpTestDto = new PumpTestDto()
            {
                BatchCount = ++BatchCount,
                型号 = Num,//型号
                批次 = Num2,//批次
                速度 = Speed.ToDouble(),
                目标值 = Target.ToDouble(),
                //ActualValue = 499.6 + i * 0.05,
                ActualValue = Math.Round(Wt.ToDouble(), 3),

                温度 = var1.ToString(),
                Density = P,
                密封圈 = Circle,
                操作员 = OperatorName,
                CreateTime = DateTime.Now,
                转矩 = var4.ToString(),
                温度补偿量 = var2.ToString(),
                工艺补偿量 = var3.ToString(),
                温度补偿模式 = var6.ToString()
            };


            Application.Current.Dispatcher.Invoke(() => CurrentPumpTestDtos.Insert(0, pumpTestDto));

            #endregion
        
        }

    }
}
