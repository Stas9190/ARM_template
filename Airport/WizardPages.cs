using System;
using System.Windows.Forms;

namespace Train
{
    class WizardPages : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            //Скрыть заголовки tabpage
            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
            else
                base.WndProc(ref m);
        }
    }
}
