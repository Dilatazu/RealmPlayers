using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    public class ConfigWTF
    {
        public delegate void ConfigValuesChangedEventHandler(string _ConfigName, string _ConfigValue);

        private ConfigWTF()
        {}

        private Dictionary<string, Tuple<string, bool>> ConfigValues = new Dictionary<string, Tuple<string, bool>>();
        event ConfigValuesChangedEventHandler ConfigValuesChanged;
        private List<string> UnknownConfigLines = new List<string>();

        private string _GetConfigString(string _ConfigName, string _DefaultValue = "")
        {
            Tuple<string, bool> stringValue;
            if (ConfigValues.TryGetValue(_ConfigName, out stringValue) == true)
                return stringValue.Item1;
            return _DefaultValue;
        }
        private void _SetConfigString(string _ConfigName, string _Value)
        {
            ConfigValues.AddOrSet(_ConfigName, Tuple.Create(_Value, true));
            if(ConfigValuesChanged != null)
                ConfigValuesChanged(_ConfigName, _Value);
            Logger.LogText("Changed Setting \"" + _ConfigName + "\" to value \"" + _Value + "\"");
        }
        private bool _GetConfigBool(string _ConfigName, bool _DefaultValue = false)
        {
            string strBoolValue = _GetConfigString(_ConfigName);
            if (strBoolValue == "1") return true;
            else if (strBoolValue == "0") return false;
            else return _DefaultValue;
        }
        private void _SetConfigBool(string _ConfigName, bool _Bool)
        {
            _SetConfigString(_ConfigName, ((_Bool == true) ? "1" : "0"));
        }
        private int _GetConfigInt(string _ConfigName, int _DefaultValue = 0)
        {
            int intValue = 0;
            if (int.TryParse(_GetConfigString(_ConfigName), out intValue) == true)
                return intValue;
            return _DefaultValue;
        }
        private void _SetConfigInt(string _ConfigName, int _Int)
        {
            _SetConfigString(_ConfigName, _Int.ToString());
        }

        #region Gets And Sets
        public bool WindowMode
        {
            get { return _GetConfigBool("gxWindow"); }
            set { _SetConfigBool("gxWindow", value); }
        }
        public bool Maximized
        {
            get { return _GetConfigBool("gxMaximize"); }
            set { _SetConfigBool("gxMaximize", value); }
        }
        public string Resolution
        {
            get { return _GetConfigString("gxResolution"); }
            set {
                if (StaticValues.Resolutions.Contains(value) == true)
                    _SetConfigString("gxResolution", value);
                else
                    Logger.LogText("Can not set Resolution \"" + value + "\" because it is not valid");
            }
        }
        public int ScriptMemory
        {
            get { return _GetConfigInt("scriptMemory", 32768); }
            set { _SetConfigInt("scriptMemory", value); }
        }
        public void EventResolutionDDL_Changed(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox _Combobox = (System.Windows.Forms.ComboBox)sender;
            Resolution = (string)_Combobox.SelectedItem;
        }
        public void InitResolutionDDL(System.Windows.Forms.ComboBox _Combobox)
        {
            _Combobox.Items.Clear();
            _Combobox.Items.AddRange(StaticValues.Resolutions.ToArray());
            _Combobox.SelectedItem = Resolution;
            //_Combobox.SelectedIndexChanged += (object sender, EventArgs e) => 
            //{
            //    Resolution = (string)_Combobox.SelectedItem;
            //};
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                if (_ConfigName == "gxResolution")
                {
                    if (StaticValues.Resolutions.Contains(_ConfigValue) == false)
                        StaticValues.Resolutions.Add(_ConfigValue);
                    _Combobox.BeginInvoke(new Action(() => 
                    {
                        if (_Combobox.Items.Contains(_ConfigValue) == false)
                            _Combobox.Items.Add(_ConfigValue);
                        _Combobox.SelectedItem = _ConfigValue;
                    }));
                }
            };
        }
        public void EventMaximizedCB_Changed(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox _CheckBox = (System.Windows.Forms.CheckBox)sender;
            Maximized = _CheckBox.Checked;
        }
        public void InitMaximizedCB(System.Windows.Forms.CheckBox _CheckBox)
        {
            _CheckBox.Checked = Maximized;
            //_CheckBox.CheckedChanged += (object sender, EventArgs e) =>
            //{
            //    Maximized = _CheckBox.Checked;
            //};
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                if (_ConfigName == "gxMaximize")
                {
                    _CheckBox.BeginInvoke(new Action(() => 
                    {
                        _CheckBox.Checked = (_ConfigValue == "1" ? true : false);
                    }));
                }
            };
        }
        public void EventWindowModeCB_Changed(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox _CheckBox = (System.Windows.Forms.CheckBox)sender;
            WindowMode = _CheckBox.Checked;
        }
        public void InitWindowModeCB(System.Windows.Forms.CheckBox _CheckBox)
        {
            _CheckBox.Checked = WindowMode;
            //_CheckBox.CheckedChanged += (object sender, EventArgs e) =>
            //{
            //    WindowMode = _CheckBox.Checked;
            //};
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                if (_ConfigName == "gxWindow")
                {
                    _CheckBox.BeginInvoke(new Action(() => 
                    {
                        _CheckBox.Checked = (_ConfigValue == "1" ? true : false);
                    }));
                }
            };
        }
        public void EventScriptMemoryDDL_Changed(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox _Combobox = (System.Windows.Forms.ComboBox)sender;
            ScriptMemory = StaticValues.ScriptMemorys[(string)_Combobox.SelectedItem];
        }
        public void InitScriptMemoryDDL(System.Windows.Forms.ComboBox _Combobox)
        {
            int currValue = ScriptMemory;
            string currValueStr = (currValue / 1024).ToString() + "MB";
            if (StaticValues.ScriptMemorys.ContainsValue(currValue) == false)
                StaticValues.ScriptMemorys.AddOrSet(currValueStr, currValue);
            else
                currValueStr = StaticValues.ScriptMemorys.First((_Value) => _Value.Value == currValue).Key;
            _Combobox.Items.Clear();
            _Combobox.Items.AddRange(StaticValues.ScriptMemorys.Keys.ToArray());
            _Combobox.SelectedItem = currValueStr;
            //_Combobox.SelectedIndexChanged += (object sender, EventArgs e) =>
            //{
            //    ScriptMemory = StaticValues.ScriptMemorys[(string)_Combobox.SelectedItem];
            //};
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                if (_ConfigName == "scriptMemory")
                {
                    int newValue = int.Parse(_ConfigValue);
                    string newValueStr = (newValue / 1024).ToString() + "MB";
                    if (StaticValues.ScriptMemorys.ContainsValue(newValue) == false)
                        StaticValues.ScriptMemorys.AddOrSet(newValueStr, newValue);
                    else
                        newValueStr = StaticValues.ScriptMemorys.First((_Value) => _Value.Value == newValue).Key;
                    _Combobox.BeginInvoke(new Action(() =>
                    {
                        if (_Combobox.Items.Contains(newValueStr) == false)
                            _Combobox.Items.Add(newValueStr);
                        _Combobox.SelectedItem = newValueStr;
                    }));
                }
            };
        }
        public void EventAllConfigsLB_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListBox _ListBox = (System.Windows.Forms.ListBox)sender;
            int index = _ListBox.IndexFromPoint(e.Location);
            string item = (string)_ListBox.Items[index];
            string configSetting = item.Substring(0, item.IndexOf('='));
            string configValue = item.Substring(item.IndexOf('=') + 1);
            string changedValue;

            Func<string, bool> validator = null;
            StaticValues.SettingsValidateFunctions.TryGetValue(configSetting, out validator);
            if (ChangeValueBox.ChangeValue("Change Value for Setting(" + configSetting + ")", configValue, validator, out changedValue) == true)
                _SetConfigString(configSetting, changedValue);
        }
        public void InitAllConfigsLB(System.Windows.Forms.ListBox _ListBox)
        {
            _ListBox.SelectionMode = System.Windows.Forms.SelectionMode.One;
            _ListBox.Items.Clear();
            List<string> configValues = new List<string>();
            foreach(var configValue in ConfigValues)
            {
                configValues.Add(configValue.Key + "=" + configValue.Value.Item1);
            }
            _ListBox.Items.AddRange(configValues.ToArray());
            //_ListBox.MouseDoubleClick += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            //{
            //    int index = _ListBox.IndexFromPoint(e.Location);
            //    string item = (string)_ListBox.Items[index];
            //    string configSetting = item.Substring(0, item.IndexOf('='));
            //    string configValue = item.Substring(item.IndexOf('=') + 1);
            //    string changedValue;
                
            //    Func<string, bool> validator = null;
            //    StaticValues.SettingsValidateFunctions.TryGetValue(configSetting, out validator);
            //    if (ChangeValueBox.ChangeValue("Change Value for Setting(" + configSetting + ")", configValue, validator, out changedValue) == true)
            //        _SetConfigString(configSetting, changedValue);
            //};
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                _ListBox.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < _ListBox.Items.Count; ++i)
                    {
                        string item = (string)_ListBox.Items[i];
                        if (item.StartsWith(_ConfigName) == true)
                        {
                            _ListBox.Items[i] = _ConfigName + "=" + _ConfigValue;
                            return;
                        }
                    }
                }));
            };
        }
        public void Dispose()
        {
            ConfigValuesChanged = null;
        }
        #endregion

        #region Load and Save
        private void _LoadConfigFile(string _Filename)
        {
            string[] configLines = System.IO.File.ReadAllLines(_Filename);
            foreach (string configLine in configLines)
            {
                if (configLine.StartsWith("SET "))
                {
                    try
                    {
                        string configSetting = configLine.Substring(4, configLine.IndexOf(' ', 4) - 4);
                        string configValue = configLine.Substring(configLine.IndexOf(' ', 4) + 1);
                        configValue = configValue.Trim('\"');
                        ConfigValues.AddOrSet(configSetting, Tuple.Create(configValue, false));//Använd INTE _SetConfigString här då den antar att changevärdet ska vara true!
                        Logger.LogText(configSetting + "=\"" + configValue + "\"");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
                else
                {
                    UnknownConfigLines.Add(configLine);
                }
            }
        }
        public static ConfigWTF LoadConfigFile(string _Filename)
        {
            ConfigWTF newConfigWTF = new ConfigWTF();
            newConfigWTF._LoadConfigFile(_Filename);
            return newConfigWTF;
        }
        internal static ConfigWTF LoadWTFConfigFile(WowVersion _WowVersion)
        {
            return LoadConfigFile(Settings.GetWowDirectory(_WowVersion) + "WTF\\Config.wtf");
        }
        internal void SaveWTFConfigFile(WowVersion _WowVersion)
        {
            SaveConfigFile(Settings.GetWowDirectory(_WowVersion) + "WTF\\Config.wtf");
        }
        public void SaveConfigFile(string _Filename)
        {
            string file = "";
            foreach (var configValue in ConfigValues)
            {
                file += "SET " + configValue.Key + " \"" + configValue.Value.Item1 + "\"\r\n";
            }
            foreach (var configLine in UnknownConfigLines)
            {
                file += configLine + "\r\n";
            }
            if(System.IO.File.Exists(_Filename) == false || System.IO.File.ReadAllText(_Filename) != file)
            {
                System.IO.File.WriteAllText(_Filename, file);
            }
        }
        #endregion
    }
}
