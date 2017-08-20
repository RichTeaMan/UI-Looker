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

        private ApplicationLoader _applicationLoader;
        private HashSet<AutomationElement> autoSet = new HashSet<AutomationElement>();
        public MainWindow()
        {
            _applicationLoader = new ApplicationLoader();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
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

            treeview_ui.ItemsSource = new List<ElementTreeView> { rootElementTreeView };
        }


        private ElementTreeView AddChildElements(AutomationElement element)
        {
            var elementTreeView = new ElementTreeView()
            {
                AutomationId = element.Properties.AutomationId.ValueOrDefault,
                Name = element.Properties.Name.ValueOrDefault,
                ClassName = element.Properties.ClassName.ValueOrDefault
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
    }
}
