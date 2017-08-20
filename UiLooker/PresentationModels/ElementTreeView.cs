using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiLooker.PresentationModels
{
    public class ElementTreeView
    {
        public string AutomationId { get; set; }
        public string ControlType { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }

        public List<string> Children { get; set; }
    }
}
