using Microsoft.Win32;
using Stability_Test_Platform.DataProcessing;
using Stability_Test_Platform.Services;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Stability_Test_Platform.ViewModels
{
    public class CavityViewModel:ViewModelBase
    {
        //表格操作测试
        private FileStorageManager file;
        private IvCurveAnalyzer _analyzer = new IvCurveAnalyzer();
        private ExcelExportService _excelService = new ExcelExportService();

        #region 函数构造
        public CavityViewModel(string name)
        {
            CavityName = name;

            // 初始化默认值
            IsFormalType = true;
            SelectedTestMode = "ISOS-LC-1/2";
            T80WarningColor = Brushes.LimeGreen;
            T80StatusText = "正常 (未触发)";
            CurrentTemperature = 25.0;
            RunningTime = TimeSpan.Zero;

            // 初始化命令
            StartTestCommand = new RelayCommand(StartTest);
            StopTestCommand = new RelayCommand(StopTest);
            SelectPathCommand = new RelayCommand(SelectPath);
            // 初始化选择器件的命令
            SelectDeviceCommand = new RelayCommand(param =>
            {
                if (param is string deviceName)
                {
                    SelectedDeviceTitle = deviceName;
                    // 实际开发中，这里可以触发图表控件的数据源刷新
                    // 例如：UpdateChartDataForDevice(deviceName);
                }
            });

            // 初始化定时器：每 1 秒触发一次
            _testTimer = new DispatcherTimer();
            _testTimer.Interval = TimeSpan.FromSeconds(1);
            _testTimer.Tick += TestTimer_Tick;
        }
        #endregion

        #region 基础属性
        /// <summary>
        /// 测试准备界面属性绑定
        /// </summary>
        //仓体名称
        public string CavityName { get; }

        //是否正在测试
        private bool _isTesting;
        public bool IsTesting
        {
            get => _isTesting;
            set => SetProperty(ref _isTesting, value);
        }

        //文件保存路径
        private string _savePath = @"Please select path...";
        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        // 路径显示颜色
        private Brush _pathColor = Brushes.Red;
        public Brush PathColor
        {
            get => _pathColor;
            set => SetProperty(ref _pathColor, value);
        }

        //文件名称
        private string _fileName = "TestResult_01";
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        //测试模式选择
        private string _selectedTestMode;
        public string SelectedTestMode
        {
            get => _selectedTestMode;
            set
            {
                if (SetProperty(ref _selectedTestMode, value))
                {
                    // 当测试模式改变时，通知界面重新评估是否解禁后续参数
                    OnPropertyChanged(nameof(HasTestMode));
                }
            }
        }

        //判断是否已经选择了测试模式
        public bool HasTestMode => !string.IsNullOrEmpty(SelectedTestMode);

        //正式结构
        private bool _isFormalType;
        public bool IsFormalType
        {
            get => _isFormalType;
            set => SetProperty(ref _isFormalType, value);
        }

        //反式结构
        private bool _isInvertedType;
        public bool IsInvertedType
        {
            get => _isInvertedType;
            set => SetProperty(ref _isInvertedType, value);
        }

        //电压范围起始值
        private string _initialVoltage = "-0.1";
        public string InitialVoltage
        {
            get => _initialVoltage;
            set => SetProperty(ref _initialVoltage, value);
        }

        //电压范围终止值
        private string _terminalVoltage = "1.2";
        public string TerminalVoltage
        {
            get => _terminalVoltage;
            set => SetProperty(ref _terminalVoltage, value);
        }

        //添加的偏压值设置
        private string _appliedVoltage = "0.8";
        public string AppliedVoltage
        {
            get => _appliedVoltage;
            set => SetProperty(ref _appliedVoltage, value);
        }

        //电压步长
        private string _voltageStep = "0.01";
        public string VoltageStep
        {
            get => _voltageStep;
            set => SetProperty(ref _voltageStep, value);
        }

        //器件面积
        private string _deviceArea = "0.06158";
        public string DeviceArea
        {
            get => _deviceArea;
            set => SetProperty(ref _deviceArea, value);
        }

        //--- ISOS-LC-1/2 专属温度参数 ---
        private string _sunTime = "12.0";//光照时间
        public string SunTime
        {
            get => _sunTime;
            set => SetProperty(ref _sunTime, value);
        }
        private string _darkTime="12.0";//黑暗时间
        public string DarkTime
        {
            get => _darkTime;
            set => SetProperty(ref _darkTime, value);
        }

        // --- ISOS-L-1 专属温度参数 ---
        private string _targetTemperature = "60.0"; //默认65度
        public string TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value);
        }

        // --- ISOS-L-2 专属循环温度参数 ---
        private string _cycleLowTemperature = "25.0";//循环低温
        public string CycleLowTemperature
        {
            get => _cycleLowTemperature;
            set => SetProperty(ref _cycleLowTemperature, value);
        }
        private string _cycleHighTemperature = "85.0";//循环高温
        public string CycleHighTemperature
        {
            get => _cycleHighTemperature;
            set => SetProperty(ref _cycleHighTemperature, value);
        }

        // 升温时间
        private string _heatingTime = "6";
        public string HeatingTime
        {
            get => _heatingTime;
            set => SetProperty(ref _heatingTime, value);
        }

        // 降温时间
        private string _coolingTime = "6";
        public string CoolingTime
        {
            get => _coolingTime;
            set => SetProperty(ref _coolingTime, value);
        }
        /// <summary>
        /// 测试过程界面属性绑定
        /// </summary>
        //当前仓体温度
        private double _currentTemperature;
        public double CurrentTemperature
        {
            get => _currentTemperature;
            set => SetProperty(ref _currentTemperature, value);
        }

        // 持续运行的时间
        private TimeSpan _runningTime;
        public TimeSpan RunningTime
        {
            get => _runningTime;
            set => SetProperty(ref _runningTime, value);
        }

        //T80预警
        private Brush _t80WarningColor;
        public Brush T80WarningColor
        {
            get => _t80WarningColor;
            set => SetProperty(ref _t80WarningColor, value);
        }
        private string _t80StatusText;
        public string T80StatusText
        {
            get => _t80StatusText;
            set => SetProperty(ref _t80StatusText, value);
        }

        //器件展示选择
        private string _selectedDeviceTitle = "Left - Device 1"; // 默认显示1号
        public string SelectedDeviceTitle
        {
            get => _selectedDeviceTitle;
            set => SetProperty(ref _selectedDeviceTitle, value);
        }
        /// <summary>
        /// 定时器和时间记录字段
        /// </summary>
        private DispatcherTimer _testTimer;
        private DateTime _testStartTime;
        private Random _random = new Random(); // 用于模拟温度波动
        #endregion

        #region 命令绑定
        public ICommand StartTestCommand { get; }
        public ICommand StopTestCommand { get; }
        public ICommand SelectPathCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        #endregion

        #region 测试功能实现
        //开始测试
        private void StartTest(object obj)
        {
            // 点击开始测试，将状态切换为 True，XAML会自动隐藏配置页，显示测试页
            IsTesting = true;
            T80WarningColor = Brushes.LimeGreen;
            T80StatusText = "监控中...";

            // 记录当前时间为测试开始时间，并启动定时器
            _testStartTime = DateTime.Now;
            RunningTime = TimeSpan.Zero;
            _testTimer.Start();
            file = new FileStorageManager(SavePath, FileName);
            _= MockMeasurementLoopAsync();
        }

        //停止测试
        private void StopTest(object obj)
        {
            // 点击停止测试，将状态切换为 False，XAML会自动恢复配置页
            IsTesting = false;

            // 停止测试时，关闭定时器
            _testTimer.Stop();
        }

        //选择文件保存路径
        private void SelectPath(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "请选择保存地址并输入文件名",
                Filter = "Excel 文件 (*.xlsx)|*.xlsx|CSV 数据文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
                DefaultExt = ".xlsx",
                AddExtension = true,
                FileName = this.FileName
            };

            if (Directory.Exists(this.SavePath))
            {
                saveFileDialog.InitialDirectory = this.SavePath;
            }

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string fullPath = saveFileDialog.FileName;

                SavePath = Path.GetDirectoryName(fullPath) + "\\";
                FileName = Path.GetFileNameWithoutExtension(fullPath);

                // 当用户实质性地选择了路径后，将颜色恢复为正常颜色
                PathColor = Brushes.Black;
            }
        }

        //计时器模拟
        private void TestTimer_Tick(object sender, EventArgs e)
        {
            // 1. 实时更新运行时间
            RunningTime = DateTime.Now - _testStartTime;

            // 2. 模拟真实环境下的温度波动 (根据测试模式)
            if (SelectedTestMode == "ISOS-LC-1/2")
            {
                // 室温模式：在 24.5 到 25.5 之间波动
                CurrentTemperature = 25.0 + (_random.NextDouble() - 0.5);
            }
            else if (SelectedTestMode == "ISOS-L-1" && double.TryParse(TargetTemperature, out double targetTemp))
            {
                // 恒温模式：在设定的目标温度上下 0.2 度波动
                CurrentTemperature = targetTemp + (_random.NextDouble() * 0.4 - 0.2);
            }
            else
            {
                // 其他情况随便给个模拟值
                CurrentTemperature = 25.0 + (_random.NextDouble() * 2);
            }

            // 3. 模拟触发 T80 衰减预警 (比如运行超过 10 秒钟演示一下变红)
            if (RunningTime.TotalSeconds > 10)
            {
                T80WarningColor = Brushes.Red;
                T80StatusText = "T80 预警！效率下降超过20%";
            }
        }
        #endregion

        #region 数据处理与存储逻辑

        /// <summary>
        /// 扫描完成后的数据处理与保存中枢
        /// </summary>
        /// <param name="deviceId">器件位置，例如 "1-1"</param>
        /// <param name="isForwardScan">是否为正扫 (true为正扫 Forward，false为反扫 Reverse)</param>
        /// <param name="vArray">电压数组</param>
        /// <param name="iArray">电流数组</param>
        private void ProcessAndSaveDeviceData(string deviceId, bool isForwardScan, double[] vArray, double[] iArray)
        {
            // 将 bool 转换为路径所需的字符串 "Forward" 或 "Reverse"
            string scanDirectionStr = isForwardScan ? "Forward" : "Reverse";

            // 1. 获取当前环境时间与温度
            double currentTime = RunningTime.TotalHours;
            double currentTemp = CurrentTemperature;

            // 2. 获取 IV 和 Result 的绝对存储路径 (使用你现有的 file 实例)
            string ivFilePath = file.GetIvFilePath(scanDirectionStr, deviceId);
            string resultFilePath = file.GetResultFilePath(scanDirectionStr, deviceId);

            // 3. 将电压电流数组直接存入 IV 表格
            _excelService.AppendIvDataToExcel(ivFilePath, deviceId, currentTime, vArray, iArray);

            // 4. 计算光伏参数 (需要将面积字符串转为 double)
            double.TryParse(DeviceArea, out double area);
            PvMeasurementData resultData = _analyzer.Analyze(vArray, iArray, area);

            // 5. 补充运行环境参数
            resultData.TimeHours = currentTime;
            resultData.SweepDirection = isForwardScan; // 对应 PvMeasurementData 里的 bool
            resultData.Temperature = currentTemp;

            // 假设这是你设定的测试延迟，实际可以从你的 SourceTableConfig 中读取
            resultData.DelaySeconds = 0.1;

            // 6. 将计算结果追加存入 Result 表格
            _excelService.AppendResultDataToExcel(resultFilePath, deviceId, resultData);
        }
        #endregion
        #region 表格循环测试

        /// <summary>
        /// 模拟后台硬件扫描的异步循环任务
        /// </summary>
        private async Task MockMeasurementLoopAsync()
        {
            // 只要处于测试状态，就一直循环
            while (IsTesting)
            {
                try
                {
                    // 1. 构造模拟的测试电压数组 (例如从 -0.1 到 1.2)
                    double[] dummyVoltage = new double[] { -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2 };
                    double[] dummyCurrent = new double[dummyVoltage.Length];

                    // 构造模拟的电流数据 (带一点随机波动，防止数据全是一模一样的)
                    for (int i = 0; i < dummyVoltage.Length; i++)
                    {
                        // 随便造点假电流数据
                        dummyCurrent[i] = 0.02 - 0.001 * dummyVoltage[i] + (_random.NextDouble() * 0.002);
                    }

                    // 2. 调用你之前写好的处理中枢，向 1-1 器件写入正扫数据
                    ProcessAndSaveDeviceData("1-1", true, dummyVoltage, dummyCurrent);

                    // 也可以顺便模拟写入反扫数据
                    // ProcessAndSaveDeviceData("1-1", false, dummyVoltage, dummyCurrent);
                }
                catch (IOException ex)
                {
                    // 捕获文件占用异常：如果你在测试期间用 Excel 软件打开了该表格，后台写入会报错。
                    // 捕获后程序不会崩溃，下一次循环如果文件关闭了，它会继续正常写入。
                    System.Diagnostics.Debug.WriteLine($"写入文件失败，可能文件正被打开: {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"模拟测试发生未知错误: {ex.Message}");
                }

                // 3. 异步等待 1 分钟 (60000 毫秒)
                // 使用 Task.Delay 不会阻塞 UI 线程，界面依然可以流畅点击
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        #endregion
    }
}
