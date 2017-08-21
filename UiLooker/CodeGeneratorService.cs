using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiLooker.PresentationModels;

namespace UiLooker
{
    public class CodeGeneratorService
    {
        
        public string GenerateCSharpGetter(ElementTreeView elementTreeView, Window _window)
        {
            var codeFragments = new List<string>();

            ControlType controlType;
            bool hasControlType = Enum.TryParse(elementTreeView.ClassName, out controlType);

            if (!string.IsNullOrEmpty(elementTreeView.AutomationId))
            {
                string automationFragment = $"ByAutomationId(\"{elementTreeView.AutomationId}\")";
                codeFragments.Add(automationFragment);
            }
            if (!string.IsNullOrEmpty(elementTreeView.Name))
            {
                string nameFragment = $"ByName(\"{elementTreeView.Name}\")";
                codeFragments.Add(nameFragment);
            }
            if (hasControlType)
            {
                string automationFragment = $"ByControlType(ControlType.{controlType.ToString()})";
                codeFragments.Add(automationFragment);
            }

            if (codeFragments.Count == 0)
            {
                throw new InvalidOperationException("Could not generate code.");
            }

            var parameters = string.Join(".And(cf.", codeFragments);
            var code = $"var element = _window.FindFirstDescendant(cf => cf.{parameters});";

            return code;
        }
    }
}
