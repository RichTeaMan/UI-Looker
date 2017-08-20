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
        public Application BindToOpenApp()
        {
            var processInfo = new ProcessStartInfo(@"D:\Projects\White\src\TestApplications\WindowsFormsTestApplication\bin\Debug\WindowsFormsTestApplication.exe");
            var application = Application.AttachOrLaunch(processInfo);

            return application;
        }

        public Window FetchMainWindow()
        {
            var application = BindToOpenApp();
            var automation = new UIA3Automation();
            var window = application.GetMainWindow(automation);
            return window;
        }
    }
}
