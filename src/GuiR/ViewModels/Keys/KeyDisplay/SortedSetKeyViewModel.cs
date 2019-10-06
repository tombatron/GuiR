using GuiR.Models;
using GuiR.Redis;
using System.Collections.Generic;
using System.Windows.Input;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class SortedSetKeyViewModel : ViewModelBase
    {
        private readonly RedisProxy _redis;

        public SortedSetKeyViewModel(RedisProxy redis) => _redis = redis;

        public string Key { get; set; }



        private bool _isGeoData;

        public bool IsGeoData
        {
            get => _isGeoData;

            set
            {
                _isGeoData = value;

                RaisePropertyChangedEvent(nameof(IsGeoData));
            }
        }


    }
}
