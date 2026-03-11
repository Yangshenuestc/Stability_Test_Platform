using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Stability_Test_Platform.ViewModels
{
    public class CavityViewModel:ViewModelBase
    {
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

        //Excel文件保存路径
        private string _savePath = @"C:\TestResults\";
        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
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

        //电压范围最小值设置
        private string _minVoltage = "-0.1";
        public string MinVoltage
        {
            get => _minVoltage;
            set => SetProperty(ref _minVoltage, value);
        }

        //电压范围最大值设置
        private string _maxVoltage = "1.2";
        public string MaxVoltage
        {
            get => _maxVoltage;
            set => SetProperty(ref _maxVoltage, value);
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
        }

        private void StopTest(object obj)
        {
            // 点击停止测试，将状态切换为 False，XAML会自动恢复配置页
            IsTesting = false;

            // 停止测试时，关闭定时器
            _testTimer.Stop();
        }

        private void SelectPath(object obj)
        {
            // 这里通常会调用 FolderBrowserDialog 或 OpenFileDialog
            // 为了演示，这里仅做一个简单的字符串改变
            SavePath = @"D:\NewDataFolder\ChamberData\";
        }

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
    }
}
