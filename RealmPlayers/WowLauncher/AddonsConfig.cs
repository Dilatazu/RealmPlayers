using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    class AddonsConfig
    {
        public delegate void ConfigValuesChangedEventHandler(string _ConfigName, string _ConfigValue);

        private AddonsConfig()
        {}

        private Func<KeyValuePair<string, Tuple<string, bool>>, string> ConfigValueSort = (_Value) =>
            {
                if (_Value.Value.Item1 == "enabled")
                    return "a" + _Value.Key;
                else if (_Value.Value.Item1 == "disabled")
                    return "b" + _Value.Key;
                else//if (_Value.Value.Item1 == "enabled(auto)")
                    return "c" + _Value.Key;
            };
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
            if (strBoolValue == "enabled") return true;
            else if (strBoolValue == "disabled") return false;
            else return _DefaultValue;
        }
        private void _SetConfigBool(string _ConfigName, bool _Bool)
        {
            _SetConfigString(_ConfigName, ((_Bool == true) ? "enabled" : "disabled"));
        }
        private int m_LastSelectedListBoxIndex = 0;
        public void EventAllConfigsLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ListBox _ListBox = (System.Windows.Forms.ListBox)sender;
            if (m_LastSelectedListBoxIndex != -1)
                _ListBox.Invalidate(_ListBox.GetItemRectangle(m_LastSelectedListBoxIndex));
            m_LastSelectedListBoxIndex = _ListBox.SelectedIndex;
        }
        public void EventAllConfigsLB_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListBox _ListBox = (System.Windows.Forms.ListBox)sender;
            int index = _ListBox.IndexFromPoint(e.Location);
            string item = (string)_ListBox.Items[index];
            string configSetting = item.Substring(0, item.IndexOf('='));
            string configValue = item.Substring(item.IndexOf('=') + 1);
            string changedValue;

            Func<string, bool> validator = (string _Value) => {
                if (_Value == "enabled" || _Value == "disabled")
                    return true;
                return false;
            };
            if (ChangeValueBox.ChangeValue("activate addon \"" + configSetting + "\" set to value(enabled / disabled)", configValue, validator, out changedValue) == true)
                _SetConfigString(configSetting, changedValue);
        }
        public void EventAllConfigsLB_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            System.Windows.Forms.ListBox _ListBox = (System.Windows.Forms.ListBox)sender;
            
            e.DrawBackground();
            System.Drawing.Brush myBrush = System.Drawing.Brushes.White;
            string currentItem = _ListBox.Items[e.Index].ToString();
            if (_ListBox.SelectedIndex != e.Index)//e.State != System.Windows.Forms.DrawItemState.Selected)//
            {
                string currItemValue = currentItem.Split('=').Last();
                if (currItemValue == "enabled")
                    myBrush = System.Drawing.Brushes.DarkGreen;
                else if (currItemValue == "disabled")
                    myBrush = System.Drawing.Brushes.DarkRed;
                else// if (currItemValue == "enabled(auto)")
                    myBrush = System.Drawing.Brushes.Black;
            }

            e.Graphics.DrawString(currentItem, e.Font, myBrush, e.Bounds, System.Drawing.StringFormat.GenericDefault);
        }
        public void InitAllConfigsLB(System.Windows.Forms.ListBox _ListBox)
        {
            _ListBox.SelectionMode = System.Windows.Forms.SelectionMode.One;
            {
                _ListBox.Items.Clear();
                List<string> configValues = new List<string>();
                var orderedConfigValues = ConfigValues.OrderBy(ConfigValueSort);
                foreach (var configValue in orderedConfigValues)
                {
                    configValues.Add(configValue.Key + "=" + configValue.Value.Item1);
                }
                _ListBox.Items.AddRange(configValues.ToArray());
            }
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                _ListBox.BeginInvoke(new Action(() =>
                {
                    if (_ConfigName != "ALL_CONFIG_VALUES_CHANGED_UPDATE_ALL")
                    {
                        for (int i = 0; i < _ListBox.Items.Count; ++i)
                        {
                            string item = (string)_ListBox.Items[i];
                            if (item.StartsWith(_ConfigName + "=") == true)
                            {
                                _ListBox.Items[i] = _ConfigName + "=" + _ConfigValue;
                                return;
                            }
                        }
                    }
                    else
                    {
                        _ListBox.Items.Clear();
                        List<string> configValues = new List<string>();
                        var orderedConfigValues = ConfigValues.OrderBy(ConfigValueSort);
                        foreach (var configValue in orderedConfigValues)
                        {
                            configValues.Add(configValue.Key + "=" + configValue.Value.Item1);
                        }
                        _ListBox.Items.AddRange(configValues.ToArray());
                        _ListBox.Invalidate();
                    }

                }));
            };

            _ListBox.DrawItem += EventAllConfigsLB_DrawItem;
            _ListBox.SelectedIndexChanged += EventAllConfigsLB_SelectedIndexChanged;
            _ListBox.MouseDoubleClick += EventAllConfigsLB_MouseDoubleClick;

            _ListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            m_LastSelectedListBoxIndex = _ListBox.SelectedIndex;
        }
        public void ReleaseAllConfigsLB(System.Windows.Forms.ListBox _ListBox)
        {
            _ListBox.DrawMode = System.Windows.Forms.DrawMode.Normal;

            _ListBox.MouseDoubleClick -= EventAllConfigsLB_MouseDoubleClick;
            _ListBox.SelectedIndexChanged -= EventAllConfigsLB_SelectedIndexChanged;
            _ListBox.DrawItem -= EventAllConfigsLB_DrawItem;

            Dispose();
        }
        void EventAllConfigsCLB_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            System.Windows.Forms.CheckedListBox _ListBox = (System.Windows.Forms.CheckedListBox)sender;
            string configSetting = _ListBox.Items[e.Index].ToString();
            if (configSetting.EndsWith("(Auto Enabled)"))
                configSetting = configSetting.SplitVF("(Auto Enabled)").First();
            if(e.NewValue == System.Windows.Forms.CheckState.Checked)
                _SetConfigString(configSetting, "enabled");
            else if (e.NewValue == System.Windows.Forms.CheckState.Unchecked)
                _SetConfigString(configSetting, "disabled");
            else
                _SetConfigString(configSetting, "enabled(auto)");
        }
        public void InitAllConfigsCLB(System.Windows.Forms.CheckedListBox _ListBox)
        {
            {
                _ListBox.Items.Clear();
                var orderedConfigValues = ConfigValues.OrderBy(ConfigValueSort);
                foreach (var configValue in orderedConfigValues)
                {
                    _ListBox.Items.Add(configValue.Key + (configValue.Value.Item1 == "enabled(auto)" ? "(Auto Enabled)" : ""), (configValue.Value.Item1 != "disabled"));
                }
            }
            _ListBox.CheckOnClick = true;
            ConfigValuesChanged += (string _ConfigName, string _ConfigValue) =>
            {
                _ListBox.BeginInvoke(new Action(() =>
                {
                    if (_ConfigName != "ALL_CONFIG_VALUES_CHANGED_UPDATE_ALL")
                    {
                        for (int i = 0; i < _ListBox.Items.Count; ++i)
                        {
                            string item = (string)_ListBox.Items[i];
                            if (item.StartsWith(_ConfigName) == true)
                            {
                                if (item == _ConfigName + "(Auto Enabled)" || item == _ConfigName)
                                {
                                    if (_ListBox.CheckedIndices.Contains(i) != (_ConfigValue != "disabled"))
                                        _ListBox.SetItemCheckState(i, (_ConfigValue != "disabled" ? System.Windows.Forms.CheckState.Checked : System.Windows.Forms.CheckState.Unchecked));
                                    _ListBox.Items[i] = _ConfigName;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        _ListBox.Items.Clear();
                        var orderedConfigValues = ConfigValues.OrderBy(ConfigValueSort);
                        foreach (var configValue in orderedConfigValues)
                        {
                            _ListBox.Items.Add(configValue.Key + (configValue.Value.Item1 == "enabled(auto)" ? "(Auto Enabled)" : ""), (configValue.Value.Item1 != "disabled"));
                        }
                        _ListBox.Invalidate();
                    }

                }));
            };
            _ListBox.ItemCheck += EventAllConfigsCLB_ItemCheck;
        }
        public void ReleaseAllConfigsCLB(System.Windows.Forms.CheckedListBox _ListBox)
        {
            _ListBox.ItemCheck -= EventAllConfigsCLB_ItemCheck;
            Dispose();
        }
        public void Dispose()
        {
            ConfigValuesChanged = null;
        }

        public void DisableAllAddons()
        {
            var keys = ConfigValues.Keys.ToArray();
            foreach (var configKeys in keys)
            {
                ConfigValues[configKeys] = Tuple.Create("disabled", true);
            }
            ConfigValuesChanged("ALL_CONFIG_VALUES_CHANGED_UPDATE_ALL", "disabled");
        }
        public void EnableAllAddons()
        {
            var keys = ConfigValues.Keys.ToArray();
            foreach (var configKeys in keys)
            {
                ConfigValues[configKeys] = Tuple.Create("enabled", true);
            }
            ConfigValuesChanged("ALL_CONFIG_VALUES_CHANGED_UPDATE_ALL", "enabled");
        }

        #region Load and Save
        private void _LoadConfigFile(string _Filename)
        {
            if (System.IO.File.Exists(_Filename) == true)
            {
                string[] configLines = System.IO.File.ReadAllLines(_Filename);
                foreach (string configLine in configLines)
                {
                    string[] addoNameAndConfig = configLine.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (addoNameAndConfig.Length == 2)
                    {
                        ConfigValues.AddOrSet(addoNameAndConfig[0], Tuple.Create(addoNameAndConfig[1], false));
                    }
                }
            }
            var addons = InstalledAddons.GetInstalledAddons(WowVersion.Vanilla);
            foreach (var addon in addons)
            {
                if (ConfigValues.ContainsKey(addon) == false)
                {
                    ConfigValues.AddOrSet(addon, Tuple.Create("enabled(auto)", false));
                }
            }
        }
        public static AddonsConfig LoadConfigFile(string _Filename)
        {
            AddonsConfig newAddonsConfig = new AddonsConfig();
            newAddonsConfig._LoadConfigFile(_Filename);
            return newAddonsConfig;
        }
        public void SaveConfigFile(string _Filename)
        {
            string file = "";
            foreach (var configValue in ConfigValues)
            {
                if (configValue.Value.Item1 != "enabled(auto)")
                    file += configValue.Key + ": " + configValue.Value.Item1 + "\r\n";
            }
            System.IO.File.WriteAllText(_Filename, file);
        }
        #endregion
    }
}
