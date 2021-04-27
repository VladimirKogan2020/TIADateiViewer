using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml;

namespace ProbeaufgabeKogan
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<string> elementTypes = new ObservableCollection<string>();
        ObservableCollection<GroupElement> groupElements = new ObservableCollection<GroupElement>();

        XmlNodeList nodes;
        string filePathName = String.Empty;
        string originTitle = String.Empty;

        public MainWindow()
        {
            InitializeComponent();
            lbNodeGroups.ItemsSource = elementTypes;
            lbGroupElements.ItemsSource = groupElements;
            originTitle = Title;
        }

        private void FileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Title = originTitle;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TIA-files (*.tia)|*.tia";
            if (dlg.ShowDialog() == true)
            {
                elementTypes.Clear();
                filePathName = dlg.FileName;
                Title = Title + "-" + System.IO.Path.GetFileName(filePathName);
                ShowIncludedTypes(GetIncludedTypes(filePathName));
            }
        }     

        private void ShowIncludedTypes(Dictionary<string, int> nodeTypesDictionary)
        {
            foreach (var key in nodeTypesDictionary.Keys)
            {
                elementTypes.Add($"{key}" + " (" + nodeTypesDictionary[key] + ")");
            }
        }

        private Dictionary<string, int> GetIncludedTypes(string fileName)
        {
            List<string> nodeTypeNames = new List<string>();
            nodes = FindXmlNodes(fileName, "//node[@Type]");
            Dictionary<string, int> nodeTypesDictionary = new Dictionary<string, int>();

            foreach (XmlNode node in nodes)
            {
                string typeName = node.Attributes[0].Value;
                if (!nodeTypeNames.Contains(typeName))
                    nodeTypeNames.Add(node.Attributes[0].Value);
            }

            foreach (string nodeTypeName in nodeTypeNames)
            {
                int count = 0;
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes[0].Value == nodeTypeName)
                    {
                        count++;
                    }
                }
                nodeTypesDictionary.Add(nodeTypeName, count);
            }
            return nodeTypesDictionary;

        }       

        private void NodeGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            groupElements.Clear();
            GetNodeGroupElements("//node[@Type=");
           
        }

        private void GetNodeGroupElements(string firstToken)
        {
            if (lbNodeGroups.SelectedIndex < 0) return;
            string search = firstToken + "\""
                     + elementTypes[lbNodeGroups.SelectedIndex].Substring(0, elementTypes[lbNodeGroups.SelectedIndex].IndexOf(" "))
                     + "\""
                     + "]";
            nodes = FindXmlNodes(filePathName, search);
            string description;
            int countProperties;
            foreach (XmlNode node in nodes)
            {
                description = string.Empty;
                countProperties = node.FirstChild.ChildNodes.Count;// /properties/property
                foreach (XmlNode propertyNode in node.FirstChild.ChildNodes)
                {
                    if (propertyNode.FirstChild.InnerText == "Name")// /properties/property/key
                    {
                        description = propertyNode.LastChild.InnerText;// /properties/property/key/value
                        groupElements.Add(new GroupElement(description, countProperties));
                        break; 
                    }
                }

                if (string.IsNullOrEmpty(description))
                {
                    foreach (XmlNode propertyNode in node.FirstChild.ChildNodes)
                    {
                        if (propertyNode.FirstChild.InnerText == "Id")
                        {
                            description = propertyNode.LastChild.InnerText;
                            groupElements.Add(new GroupElement(description, countProperties));
                            break;
                        }
                    }
                }
            }
        }

        private XmlNodeList FindXmlNodes(string fileName, string search)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            var root = xmlDoc.DocumentElement;
            return root.SelectNodes(search);
        }


    }
}
