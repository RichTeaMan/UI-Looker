using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UiLooker.PresentationModels;

namespace UiLooker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Application loader.
        /// </summary>
        private ApplicationLoader _applicationLoader;

        private UiLookerModel _context;

        public MainWindow()
        {
            _applicationLoader = new ApplicationLoader();
            _context = new UiLookerModel();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            DataContext = _context;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUiElements();
        }

        private void LoadUiElements()
        {
            var mainWindow = _applicationLoader.FetchMainWindow();

            var root = new TreeViewItem() { Header = mainWindow.Title };

            List<string> elementLines = new List<string>();

            var cacheRequest = new CacheRequest();
            cacheRequest.TreeScope = TreeScope.Subtree | TreeScope.Element;
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.AutomationId);
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.ControlType);
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.ClassName);
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.Name);
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.LocalizedControlType);
            cacheRequest.Add(mainWindow.Automation.PropertyLibrary.Element.FrameworkId);

            List<ElementTreeView> elementTreeViewList = new List<ElementTreeView>();
            ElementTreeView rootElementTreeView;

            using (cacheRequest.Activate())
            {
                var elements = mainWindow.FindAll(TreeScope.Subtree, new TrueCondition());

                // first should be the main window
                var main = elements.First();
                rootElementTreeView = AddChildElements(main);
            }

            _context.UiElementTree = rootElementTreeView;
            _context.SelectedUiElement = rootElementTreeView;

            // can't get tree view to bind directly from the xaml. I'm over it.
            treeview_ui.ItemsSource = new List<ElementTreeView> { rootElementTreeView };
        }


        private ElementTreeView AddChildElements(AutomationElement element)
        {
            var elementTreeView = new ElementTreeView()
            {
                AutomationId = element.Properties.AutomationId.ValueOrDefault,
                Name = element.Properties.Name.ValueOrDefault,
                ClassName = element.Properties.ClassName.ValueOrDefault,
                ControlType = element.Properties.ControlType.ValueOrDefault.ToString()
            };
            var children = new List<ElementTreeView>();
            foreach (var child in element.CachedChildren)
            {
                var childView = AddChildElements(child);
                children.Add(childView);
            }
            elementTreeView.Children = children;
            return elementTreeView;
        }

        private void treeview_ui_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (ElementTreeView)e.NewValue;
            _context.SelectedUiElement = element;

            var automationElement = FetchSelectedElement();
            var patterns = new List<UiPattern>();
            if (null != automationElement)
            {
                patterns.AddRange(automationElement.GetSupportedPatterns().Select(p => new UiPattern() { Name = p.Name }));
            }
            _context.SupportedPatterns = patterns.AsReadOnly();
        }

        /// <summary>
        /// Fetches the selected automation element. May return null if the element cannot be found.
        /// </summary>
        /// <returns></returns>
        private AutomationElement FetchSelectedElement()
        {
            AutomationElement automationElement = null;
            if (!string.IsNullOrEmpty(_context.SelectedUiElement.AutomationId))
            {
                var mainWindow = _applicationLoader.FetchMainWindow();
                automationElement = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(_context.SelectedUiElement.AutomationId));
            }

            if (automationElement == null && !string.IsNullOrEmpty(_context.SelectedUiElement.Name))
            {
                var mainWindow = _applicationLoader.FetchMainWindow();
                automationElement = mainWindow.FindFirstDescendant(cf => cf.ByName(_context.SelectedUiElement.Name));
            }
            return automationElement;
        }

        private void Invoke_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = _applicationLoader.FetchMainWindow();
            var element = FetchSelectedElement();
            if (element == null)
            {
                MessageBox.Show("Element could not be found.");
            }
            else
            {
                var invokePattern = element.Patterns.Invoke.PatternOrDefault;
                var selectionItemPattern = element.Patterns.SelectionItem.PatternOrDefault;
                if (invokePattern != null)
                {
                    invokePattern.Invoke();
                }
                else if (selectionItemPattern != null)
                {
                    selectionItemPattern.Select();
                }
                else
                {
                    MessageBox.Show("Control is not invokable.");
                }
            }
        }

        private void Change_Value_Btn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = _applicationLoader.FetchMainWindow();
            var element = FetchSelectedElement();
            if (element == null)
            {
                MessageBox.Show("Element could not be found.");
            }
            else
            {
                var valuePattern = element.Patterns.Value.PatternOrDefault;
                if (valuePattern != null)
                {
                    valuePattern.SetValue(changeValueTb.Text);
                }
                else
                {
                    MessageBox.Show("Control does not have an editable value.");
                }
            }
        }
    }
}
