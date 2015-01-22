using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace PathingTool.Helpers
{
    public class Bezier
    {
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public Point P3 { get; set; }

        public Bezier(Point p1, Point p2, Point p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", P1, P2, P3);
        }
    }

    public class ScriptExporter
    {
        private static List<Bezier> _beziers = new List<Bezier>();

        public static void ClearExporter()
        {
            _beziers.Clear();
        }

        public static void AddSegment(Bezier bez)
        {
            _beziers.Add(bez);
        }

        public static void ExportScript()
        {
            var dlg = new SaveFileDialog {DefaultExt = ".path"};
            if (dlg.ShowDialog() != true) return;

            using (var writer = new StreamWriter(dlg.FileName))
            {
                writer.WriteLine(MainWindow.Duration);
                foreach (var bez in _beziers)
                {
                    writer.WriteLine(bez.ToString());
                }
            }  
        }
    }
}