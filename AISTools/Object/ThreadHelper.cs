using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISTools.Object
{
    class ThreadHelper
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);

        public static void SetText(Form form, Control ctrl, string text)
        {

            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text;
            }
        }
        delegate void SetDataCallback(Form f, DataGridView ctrl, DataTable data);
        public static void SetData(Form form, DataGridView ctrl, DataTable data)
        {

            if (ctrl.InvokeRequired)
            {
                SetDataCallback d = new SetDataCallback(SetData);
                form.Invoke(d, new object[] { form, ctrl, data });
            }
            else
            {
                ctrl.DataSource = data;
            }
        }
    }
}
