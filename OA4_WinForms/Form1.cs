using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Extensions;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = Microsoft.Msagl.Drawing.Color;

namespace OA4_WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        private static void CreateLinks(int[,] depMatrix)
        {
            var r = new int[depMatrix.GetLength(0), depMatrix.GetLength(1)];
            for (int i = 0; i < depMatrix.GetLength(0); i++)
            {

            }
        }

        static readonly Random _rand = new Random();
        private int[,] depMatrix = { };

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
        private void Gen_matrix_btn_Click(object sender, EventArgs e)
        {
            int n;
            if (!int.TryParse(vertices_count_inp.Text, out n) || n < 0)
                n = 5;

            depMatrix = new int[n, n];

            Randomize(depMatrix);

            CreateLinks(depMatrix);

            matrix_output.ResetText();

            matrix_output.AppendText(string.Join("", PrintTo(depMatrix)));
        }

        private IEnumerable<char> PrintTo(int[,] depMatrix)
        {
            for (int i = 0; i < depMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < depMatrix.GetLength(1); j++)
                {
                    yield return depMatrix[i, j].ToString()[0];
                    yield return ' ';
                }

                yield return '\n';
            }
        }

        private void Calculate_graph_btn_Click(object sender, EventArgs e)
        {
            var form = new Form();
            var viewer = new GViewer();
            var graph = GenerateGraph();
            if (graph == default) return;
            //Node c = graph.FindNode("C"); 
            //c.Attr.Shape = Shape.Circle;
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.CalculateLayout(graph);
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            form.ShowDialog();
        }

        private Graph GenerateGraph()
        {
            if (depMatrix.Length == 0) return default;
            var graph = new Graph("graph");
            for (int i = 0; i < depMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < depMatrix.GetLength(1); j++)
                {
                    if (depMatrix[i, j] == 1)
                    {
                        var node = graph.AddEdge((i + 1).ToString(), (j + 1).ToString());
                        node.SourceNode.Attr.Shape = Shape.Circle;
                    }
                }
            }

            return graph;
        }

        private void Matrix_output_TextChanged(object sender, EventArgs e)
        {
            var text = matrix_output.Text;

            try
            {
                var table = text.Split('\n')
                    .Select(x => x.Split(' ')
                        .Select(int.Parse).ToArray()).ToArray();
                depMatrix = new int[table.Length, table[0].Length];
                for (int i = 0; i < table.Length; i++)
                {
                    for (int j = 0; j < table[i].Length; j++)
                    {
                        depMatrix[i, j] = table[i][j];
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
