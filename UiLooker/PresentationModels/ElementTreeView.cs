﻿using System;
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

        public string Display { get { return $"Id: {AutomationId}, Name: {Name}, Control Type: {ControlType}"; } }

        public string CSharpGetterCode { get; set; }

        public List<ElementTreeView> Children { get; set; }
    }
}
