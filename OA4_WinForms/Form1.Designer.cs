namespace OA4_WinForms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gen_matrix_btn = new System.Windows.Forms.Button();
            this.calculate_graph_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.vertices_count_inp = new System.Windows.Forms.TextBox();
            this.matrix_output = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // gen_matrix_btn
            // 
            this.gen_matrix_btn.Location = new System.Drawing.Point(256, 415);
            this.gen_matrix_btn.Name = "gen_matrix_btn";
            this.gen_matrix_btn.Size = new System.Drawing.Size(107, 23);
            this.gen_matrix_btn.TabIndex = 0;
            this.gen_matrix_btn.Text = "Generate matrix";
            this.gen_matrix_btn.UseVisualStyleBackColor = true;
            this.gen_matrix_btn.Click += new System.EventHandler(this.Gen_matrix_btn_Click);
            // 
            // calculate_graph_btn
            // 
            this.calculate_graph_btn.Location = new System.Drawing.Point(442, 415);
            this.calculate_graph_btn.Name = "calculate_graph_btn";
            this.calculate_graph_btn.Size = new System.Drawing.Size(107, 23);
            this.calculate_graph_btn.TabIndex = 0;
            this.calculate_graph_btn.Text = "Calculate graph";
            this.calculate_graph_btn.UseVisualStyleBackColor = true;
            this.calculate_graph_btn.Click += new System.EventHandler(this.Calculate_graph_btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(342, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Vertices";
            // 
            // vertices_count_inp
            // 
            this.vertices_count_inp.Location = new System.Drawing.Point(404, 12);
            this.vertices_count_inp.Name = "vertices_count_inp";
            this.vertices_count_inp.Size = new System.Drawing.Size(50, 20);
            this.vertices_count_inp.TabIndex = 2;
            // 
            // matrix_output
            // 
            this.matrix_output.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.matrix_output.Location = new System.Drawing.Point(195, 55);
            this.matrix_output.Name = "matrix_output";
            this.matrix_output.Size = new System.Drawing.Size(396, 324);
            this.matrix_output.TabIndex = 3;
            this.matrix_output.Text = "";
            this.matrix_output.WordWrap = false;
            this.matrix_output.TextChanged += new System.EventHandler(this.Matrix_output_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.matrix_output);
            this.Controls.Add(this.vertices_count_inp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.calculate_graph_btn);
            this.Controls.Add(this.gen_matrix_btn);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button gen_matrix_btn;
        private System.Windows.Forms.Button calculate_graph_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox vertices_count_inp;
        private System.Windows.Forms.RichTextBox matrix_output;
    }
}

