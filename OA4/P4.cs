using System;
using System.Collections.Generic;
using Extensions;

namespace OA4
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = 5;

            for (int i = 0; i < 5; i++)
            {
                var depMatrix = new int[n, n];

                Randomize(depMatrix);

                CreateLinks(depMatrix);

                depMatrix.Print();
            }
            //{ 1, 0, 0, 0, 0 },
            //{ 0, 1, 1, 1, 1 },
            //{ 0, 1, 0, 0, 0 },
            //{ 0, 1, 1, 1, 1 },
            //{ 0, 1, 1, 1, 0 } 

            ////create a form 
            //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            ////create a viewer object 
            //Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            ////create a graph object 
            //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            ////create the graph content 
            //graph.AddEdge("A", "B");
            //graph.AddEdge("B", "C");
            //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            ////bind the graph to the viewer 
            //viewer.Graph = graph;
            ////associate the viewer with the form 
            //form.SuspendLayout();
            //viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            //form.Controls.Add(viewer);
            //form.ResumeLayout();
            ////show the form 
            //form.ShowDialog();
        }

        private static void CreateLinks(int[,] depMatrix)
        {
            var r = new int[depMatrix.GetLength(0), depMatrix.GetLength(1)];
            for (int i = 0; i < depMatrix.GetLength(0); i++)
            {

            }
        }

        static readonly Random _rand = new Random();
        private static void Randomize(int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = _rand.Next(0, 2);
                }
            }
        }
    }

    class Graph
    {
        public List<Graph> Nodes { get; }
    }
}
