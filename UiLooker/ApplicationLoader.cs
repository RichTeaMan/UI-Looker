using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiLooker
{
    public class ApplicationLoader
    {

        public Window FetchMainWindow(string applicationPath)
        {
            var processInfo = new ProcessStartInfo(applicationPath);
            var application = Application.AttachOrLaunch(processInfo);

            var automation = new UIA3Automation();
            var window = application.GetMainWindow(automation);
            return window;
        }
    }
}
