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
            
            int childrenFilled = 0;
            using (cacheRequest.Activate())
            {
                var elements = mainWindow.FindAll(TreeScope.Subtree, new TrueCondition());
                //Console.WriteLine(elementCached.Properties.AutomationId.Value);
                //Console.WriteLine(elementCached.Properties.Name.Value);

                foreach (var e in elements)
                {
                    var elementTreeView = new ElementTreeView()
                    {
                        AutomationId = e.Properties.AutomationId.ValueOrDefault,
                        Name = e.Properties.Name.ValueOrDefault,
                        ClassName = e.Properties.ClassName.ValueOrDefault
                    };
                    elementTreeViewList.Add(elementTreeView);

                    if (e.CachedChildren.Length > 0)
                    {
                        childrenFilled++;
                    }
                }
            }

            var elementsAndParents = new Dictionary<ElementTreeView, List<ElementTreeView>>();

            foreach (var element in elementTreeViewList)
            {
                elementLines.Add($"{element.Name}");

                List<ElementTreeView> parentList;
                if (!elementsAndParents.TryGetValue(element, out parentList))
                {
                    parentList = new List<ElementTreeView>();
                    elementsAndParents.Add(element, parentList);
                }
                //parentList.AddRange(element.Children);
            }

            uiResults.Text = string.Join(Environment.NewLine, elementLines);


            treeview_ui.Items.Add(root);
            AddChildElements(elementTreeViewList.First(), root, elementsAndParents);
        }


        private void AddChildElements(ElementTreeView element, TreeViewItem treeViewItem, Dictionary<ElementTreeView, List<ElementTreeView>> elementsAndParents)
        {
            List<ElementTreeView> elements;
            if (elementsAndParents.TryGetValue(element, out elements))
            {
                foreach (var child in elements)
                {
                    var childTreeItem = new TreeViewItem() { Header = $"{child.Name}" };
                    treeViewItem.Items.Add(childTreeItem);
                    AddChildElements(child, childTreeItem, elementsAndParents);
                }
            }
        }
    }
}
