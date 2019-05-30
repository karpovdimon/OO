using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace OA4_WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            if (!int.TryParse(vertices_count_inp.Text, out n) || n < 1)
            {
                //show error??
                n = 5;
            } 
            depMatrix = new int[n, n];

            Randomize(depMatrix);

            matrix_output.ResetText();

            matrix_output.AppendText(string.Join("", ShowMatrix()));
        }

        private IEnumerable<char> ShowMatrix()
        {
            for (int i = 0; i < depMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < depMatrix.GetLength(1); j++)
                {
                    yield return (char)(depMatrix[i, j] + '0');
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
            viewer.Graph = graph;
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
                // show error??
                // ignored
            }
        }
    }
}
