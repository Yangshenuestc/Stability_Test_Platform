using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<CavityViewModel> Cavitys { get; set; }

        private CavityViewModel _selectedCavity;
        public CavityViewModel SelectedCavity
        {
            get => _selectedCavity;
            set => SetProperty(ref _selectedCavity, value);
        }

        public MainViewModel()
        {
            // 初始化 3 个 Chamber
            Cavitys = new ObservableCollection<CavityViewModel>
            {
                new CavityViewModel("Cavity 1"),
                new CavityViewModel("Cavity 2"),
                new CavityViewModel("Cavity 3")
            };

            // 默认选中第一个
            SelectedCavity = Cavitys[0];
        }
    }
}
