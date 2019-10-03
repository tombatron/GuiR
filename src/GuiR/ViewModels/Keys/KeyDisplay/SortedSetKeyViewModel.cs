﻿using GuiR.Models;
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

        private IEnumerable<SortedSetCollectionEntry> _keyValue;

        public IEnumerable<SortedSetCollectionEntry> KeyValue
        {
            get => _keyValue;

            set
            {
                _keyValue = value;

                RaisePropertyChangedEvent(nameof(KeyValue));
            }
        }

        public ICommand LoadKeyValue =>
            new DelegateCommand(async () =>
            {
                KeyValue = await _redis.GetSortedSetAsync(Key);
            });
    }
}
