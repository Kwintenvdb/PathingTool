using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;
using Microsoft.Win32;
using PathingTool.Models;

namespace PathingTool.Helpers
{
    /// <summary>
    /// Saving and loading interface. Saves the Geometry of the Path to an XAML file, or loads it from it.
    /// </summary>
    public class XamlService
    {
        private PathContainer _container;

        public XamlService(PathContainer container)
        {
            _container = container;
        }

        public void Save()
        {
            var dlg = new SaveFileDialog {DefaultExt = ".xaml"};
            if (dlg.ShowDialog() != true) return;
            using (var writer = new StreamWriter(dlg.FileName))
            {
                XamlWriter.Save(_container.GroupGeom, writer);
            }           
        }

        public void Load()
        {
            var dlg = new OpenFileDialog {DefaultExt = ".xaml", Filter = "XAML files|*.xaml"};
            if (dlg.ShowDialog() != true) return;
            using (var stream = new FileStream(dlg.FileName, FileMode.Open))
            {
                try
                {
                    var geom = XamlReader.Load(stream);
                    _container.LoadData(geom as Geometry);
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not load data. Please select a valid .xaml file.","Error");
                }          
            }
        }
    }
}