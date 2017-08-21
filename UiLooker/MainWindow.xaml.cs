using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using Microsoft.Win32;
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

        private CodeGeneratorService _codeGeneratorService;

        private FlaUI.Core.AutomationElements.Window _window;

        private UiLookerModel _context;

        public MainWindow()
        {
            _applicationLoader = new ApplicationLoader();
            _codeGeneratorService = new CodeGeneratorService();
            _context = new UiLookerModel();

            InitializeComponent();

            DataContext = _context;
        }

        private void LoadUiElements()
        {

            var root = new TreeViewItem() { Header = _window.Title };

            List<string> elementLines = new List<string>();

            var cacheRequest = new CacheRequest();
            cacheRequest.TreeScope = TreeScope.Subtree | TreeScope.Element;
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.AutomationId);
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.ControlType);
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.ClassName);
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.Name);
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.LocalizedControlType);
            cacheRequest.Add(_window.Automation.PropertyLibrary.Element.FrameworkId);

            List<ElementTreeView> elementTreeViewList = new List<ElementTreeView>();
            ElementTreeView rootElementTreeView;

            using (cacheRequest.Activate())
            {
                var elements = _window.FindAll(TreeScope.Subtree, new TrueCondition());

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

        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "Executables (*.exe;*.msi)|*.exe;*.msi|All files (*.*)|*.*";
            openFileDialog.FileName = (string)Properties.Settings.Default["ApplicationPath"];
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.CheckPathExists)
                {
                    _window = _applicationLoader.FetchMainWindow(openFileDialog.FileName);
                    LoadUiElements();
                    Properties.Settings.Default["ApplicationPath"] = openFileDialog.FileName;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Specified path does not exist.", "Cannot find path.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Refresh_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_window != null)
            {
                LoadUiElements();
            } else
            {
                MessageBox.Show("No application to refresh.");
            }
        }

        private void treeview_ui_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (ElementTreeView)e.NewValue;
            _context.SelectedUiElement = element;
            
            try
            {
                element.CSharpGetterCode = _codeGeneratorService.GenerateCSharpGetter(element, _window);
            } catch(Exception ex)
            {
                element.CSharpGetterCode = ex.Message;
            }

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
                automationElement = _window.FindFirstDescendant(cf => cf.ByAutomationId(_context.SelectedUiElement.AutomationId));
            }

            if (automationElement == null && !string.IsNullOrEmpty(_context.SelectedUiElement.Name))
            {
                automationElement = _window.FindFirstDescendant(cf => cf.ByName(_context.SelectedUiElement.Name));
            }
            return automationElement;
        }

        private void Invoke_Button_Click(object sender, RoutedEventArgs e)
        {
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
                    try
                    {
                        valuePattern.SetValue(changeValueTb.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error setting value: {ex.Message}", "Could not set value.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Control does not have an editable value.");
                }
            }
        }
    }
}
