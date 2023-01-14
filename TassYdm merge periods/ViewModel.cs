using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TassYdm_merge_periods
{
    [ObservableObject]
    internal partial class ViewModel
    {
        [ObservableProperty]
        string sourcePath = AppSettings.SourcePath;

        [ObservableProperty]
        string destinationPath = AppSettings.DestinationPath;

        [ObservableProperty]
        string configPath = AppSettings.ConfigPath;

        [ObservableProperty]
        string status = "Getting started";

        [RelayCommand]
        async Task BrowseSourceAsync()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Text files|*.txt|All files|*.*",
            };
            if (dlg.ShowDialog() != true)
                return;

            SourcePath = dlg.FileName;
            AppSettings.SourcePath = dlg.FileName;

            await LoadSourceAsync();
        }

        [RelayCommand]
        void BrowseDestination()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = ".txt",
            };

            if (dlg.ShowDialog() != true)
                return;

            DestinationPath = dlg.FileName;
            AppSettings.DestinationPath = dlg.FileName;
        }

        public TtsTimetable TtsTimetable { get; } = new();

        internal async Task LoadSourceAsync()
        {
            if (!string.IsNullOrEmpty(SourcePath))
            {
                try
                {
                    Status = $"Loading {SourcePath}";
                    TtsTimetable.LoadFrom(await File.ReadAllLinesAsync(SourcePath));
                    Status = $"Loaded {SourcePath}";
             
                    CheckConfig();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, SourcePath);
                }
            }
        }

        public ObservableCollection<PeriodRule> Rules { get; } = new();
        public ObservableCollection<SelectableDay> Days { get; } = new();
        [ObservableProperty]
        string defaultYearGroup = "";

        partial void OnDefaultYearGroupChanged(string value)
        {
            ConfigChanged = true;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConfigColour))]
        bool configChanged = false;

        partial void OnConfigChangedChanged(bool value)
        {
            if (value)
                Status = "Config changed";
        }

        public Brush ConfigColour
        {
            get => ConfigChanged ? Brushes.Red : Brushes.Blue;
        }

        internal async Task<bool> OnWindowClosing()
        {
            if (ConfigChanged)
            {
                var result = MessageBox.Show("Configuration has been modified. Do you want to save it?", "Save changes?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                    return !await SaveConfigNowAsync();

                return result != MessageBoxResult.Cancel;
            }
            return true;
        }

        [RelayCommand]
        async Task BrowseConfigAsync()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Text files|*.txt|All files|*.*",
            };
            if (dlg.ShowDialog() != true)
                return;

            ConfigPath = dlg.FileName;
            AppSettings.ConfigPath = dlg.FileName;

            await LoadConfigAsync();
        }

        [RelayCommand]
        internal async Task LoadConfigAsync()
        {
            if (!string.IsNullOrEmpty(ConfigPath))
            {
                try
                {
                    Status = $"Loading {ConfigPath}";
                    var text = await File.ReadAllTextAsync(ConfigPath);
                    var config = JsonSerializer.Deserialize<Config>(text);

                    Rules.Clear();
                    if (config.Rules != null)
                        foreach (var rule in config.Rules)
                        {
                            rule.PropertyChanged += (s, a) => OnRuleChanged();
                            Rules.Add(rule);
                        }

                    Days.Clear();
                    if (config.Days != null)
                        foreach (var day in config.Days)
                        {
                            day.PropertyChanged += (s, a) => OnDayChanged();
                            Days.Add(day);
                        }

                    if (config.DefaultYearGroup != null)
                        DefaultYearGroup = config.DefaultYearGroup;

                    CheckConfig();

                    Status = $"Loaded {ConfigPath}";
                    ConfigChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ConfigPath);
                    Status = $"{ConfigPath}: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        async Task<bool> SaveConfigAsAsync()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = ConfigPath,
            };
            if (dlg.ShowDialog() != true)
                return false;

            ConfigPath = dlg.FileName;
            AppSettings.ConfigPath = dlg.FileName;

            return await SaveConfigNowAsync();
        }

        [RelayCommand]
        async Task<bool> SaveConfigAsync()
        {
            if (string.IsNullOrEmpty(ConfigPath))
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = ".txt",
                };
                if (dlg.ShowDialog() != true)
                    return false;

                ConfigPath = dlg.FileName;
                AppSettings.ConfigPath = dlg.FileName;
            }

            return await SaveConfigNowAsync();
        }

        internal async Task<bool> SaveConfigNowAsync()
        {
            try
            {
                Status = $"Saving {ConfigPath}";

                var config = new Config
                {
                    Rules = Rules.ToList(),
                    Days = Days.ToList(),
                    DefaultYearGroup = DefaultYearGroup,
                };

                var text = JsonSerializer.Serialize(config);
                await File.WriteAllTextAsync(ConfigPath, text);

                Status = $"Saved {ConfigPath}";
                ConfigChanged = false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ConfigPath);
                Status = $"{ConfigPath}: {ex.Message}";
                return false;
            }
        }

        void CheckConfig()
        {
            while (Rules.Count < TtsTimetable.MaxPeriodNumber)
                _ = AppendRule();
            while (Days.Count < TtsTimetable.MaxDayNumber)
                _ = AppendDay();

            int tassPeriod = 1;
            foreach (var rule in Rules)
            {
                rule.TassPeriod = tassPeriod;
                if (!rule.Merge)
                    ++tassPeriod;
            }
        }

        PeriodRule AppendRule()
        {
            var rule = new PeriodRule {
                TtsPeriod = Rules.Count + 1,
                TassPeriod = Rules.Count > 0 ? Rules[^1].TassPeriod + 1 : 1,
            };
            rule.PropertyChanged += (s, a) => OnRuleChanged();
            Rules.Add(rule);
            ConfigChanged = true;
            return rule;
        }

        SelectableDay AppendDay()
        {
            var day = new SelectableDay
            {
                DayNumber = Days.Count + 1,
            };
            day.PropertyChanged += (s, a) => OnDayChanged();
            Days.Add(day);
            ConfigChanged = true;
            return day;
        }

        void OnRuleChanged()
        {
            ConfigChanged = true;
            CheckConfig();
        }

        void OnDayChanged()
        {
            ConfigChanged = true;
        }

        [RelayCommand]
        async Task ConvertAsync()
        {
            await LoadSourceAsync(); // reload in case file has changed

            if (string.IsNullOrEmpty(DestinationPath))
            {
                BrowseDestination();
                if (string.IsNullOrEmpty(DestinationPath))
                    return;
            }

            try
            {
                Status = $"Exporting to {DestinationPath}";

                var output = new List<string>();
                foreach (var activity in TtsTimetable.Schedule)
                {
                    var year = activity.YearGroup;
                    if (string.IsNullOrEmpty(year))
                        year = DefaultYearGroup;

                    var day = Days[activity.DayNumber - 1];
                    if (day.IsSelected)
                    {
                        var rule = Rules[activity.PeriodNumber - 1];
                        output.Add($"{activity.DayNumber},{rule.TassPeriod},{year},{activity.ClassCode},{activity.RoomCode},{activity.TeacherCode}");
                        if (rule.Duplicate)
                            output.Add($"{activity.DayNumber},{rule.TassPeriod + 1},{year},{activity.ClassCode},{activity.RoomCode},{activity.TeacherCode}");
                    }
                    else
                    {
                        output.Add($"{activity.DayNumber},{activity.PeriodNumber},{year},{activity.ClassCode},{activity.RoomCode},{activity.TeacherCode}");
                    }
                }

                await File.WriteAllLinesAsync(DestinationPath, output);

                Status = $"Exported to {DestinationPath}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, DestinationPath);
                Status = $"{DestinationPath}: {ex.Message}";
            }

        }
    }
}
