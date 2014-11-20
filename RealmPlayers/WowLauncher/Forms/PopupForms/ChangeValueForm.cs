using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VF_WoWLauncher
{
    public partial class ChangeValueForm : Form
    {
        string m_Description = "";
        string m_OldValue = "";
        Func<string, bool> m_ValueValidator = null;
        Action<string> m_SetValueFunc = null;
        public ChangeValueForm(string _Description, string _OldValue, Func<string, bool> _ValueValidator, Action<string> _SetValueFunc)
        {
            m_Description = _Description;
            m_OldValue = _OldValue;
            m_ValueValidator = _ValueValidator;
            m_SetValueFunc = _SetValueFunc;
            InitializeComponent();
        }

        private void ChangeValueForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            //this.TopMost = true;
            this.Text = "Change Value";
            c_lblDescription.Text = m_Description;
            c_txtValue.Text = m_OldValue;
        }

        private void c_btnOK_Click(object sender, EventArgs e)
        {
            if (c_txtValue.Text == m_OldValue)
            {
                Close();
                return;
            }
            if (m_ValueValidator != null)
            {
                if (m_ValueValidator(c_txtValue.Text) == false)
                {
                    string oldDescValue = c_lblDescription.Text;
                    c_lblDescription.ForeColor = Color.Red;
                    c_lblDescription.Text = c_lblDescription.Text += "\r\nNot Valid Value! Try another value or press cancel";
                    System.Threading.Tasks.Task newTask = new System.Threading.Tasks.Task(() =>
                    {
                        System.Threading.Thread.Sleep(5000);
                        c_lblDescription.BeginInvoke(new Action(() =>
                        {
                            c_lblDescription.ForeColor = Color.Black;
                            c_lblDescription.Text = oldDescValue;
                        }));
                    });
                    newTask.Start();
                    return;
                }
            }
            else
            {
                if (Utility.MessageBoxShow("Are you sure you want to change the value from \"" + m_OldValue + "\" to \"" + c_txtValue.Text + "\"?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }
            if(m_SetValueFunc != null)
                m_SetValueFunc(c_txtValue.Text);
            Close();
        }

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
    public class ChangeValueBox
    {
        public static void ChangeValueAsync(string _Description, string _OldValue, Func<string, bool> _ValueValidator, Action<string> _SetValueFunc)
        {
            ChangeValueForm newForm = new ChangeValueForm(_Description, _OldValue, _ValueValidator, _SetValueFunc);
            newForm.Show();
        }
        public static bool ChangeValue(string _Description, string _OldValue, Func<string, bool> _ValueValidator, out string _NewValue)
        {
            string retValue = null;
            Action<string> setValueFunc = (string _Value) =>
            {
                retValue = _Value;
            };
            ChangeValueForm newForm = new ChangeValueForm(_Description, _OldValue, _ValueValidator, setValueFunc);
            newForm.ShowDialog();
            _NewValue = retValue;
            return retValue != null;
        }
    }
}
