using System.Drawing;
using System.Windows.Forms;

namespace vrsutils
{
    partial class Form1
    {
        private ListView listView1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private TextBox textBox6;
        private TextBox textBox7;
        private TextBox textBox8;
        private TextBox textBox9;
        private ComboBox cmb_prov;
        private ComboBox cmb_mun;
        private PictureBox pictureBox1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private GroupBox groupBox1;
        private GroupBox groupBox2;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            listView1 = new ListView();
            columnHeader2 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            columnHeader9 = new ColumnHeader();
            columnHeader10 = new ColumnHeader();
            columnHeader11 = new ColumnHeader();
            columnHeader12 = new ColumnHeader();
            columnHeader13 = new ColumnHeader();
            columnHeader14 = new ColumnHeader();
            columnHeader15 = new ColumnHeader();
            USER = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel5 = new Panel();
            panel1 = new Panel();
            button10 = new Button();
            button9 = new Button();
            button4 = new Button();
            button5 = new Button();
            pictureBox1 = new PictureBox();
            tabPage2 = new TabPage();
            button3 = new Button();
            groupBox2 = new GroupBox();
            button2 = new Button();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            label3 = new Label();
            label4 = new Label();
            groupBox1 = new GroupBox();
            button1 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            tabPage3 = new TabPage();
            panel4 = new Panel();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            textBox8 = new TextBox();
            textBox9 = new TextBox();
            cmb_prov = new ComboBox();
            cmb_mun = new ComboBox();
            button8 = new Button();
            panel3 = new Panel();
            button7 = new Button();
            textBox7 = new TextBox();
            label6 = new Label();
            panel2 = new Panel();
            label5 = new Label();
            textBox5 = new TextBox();
            button6 = new Button();
            textBox6 = new TextBox();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel5.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage2.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage3.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader2, columnHeader8, columnHeader9, columnHeader10, columnHeader11, columnHeader12, columnHeader13, columnHeader14, columnHeader15 });
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(0, 0);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(685, 417);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "ID";
            columnHeader2.Width = 50;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "User";
            columnHeader8.Width = 120;
            // 
            // columnHeader9
            // 
            columnHeader9.Text = "Password";
            columnHeader9.Width = 180;
            // 
            // columnHeader10
            // 
            columnHeader10.Text = "Role";
            columnHeader10.Width = 120;
            // 
            // columnHeader11
            // 
            columnHeader11.Text = "Name";
            columnHeader11.Width = 150;
            // 
            // columnHeader12
            // 
            columnHeader12.Text = "Lastname";
            columnHeader12.Width = 150;
            // 
            // columnHeader13
            // 
            columnHeader13.Text = "Locked";
            // 
            // columnHeader14
            // 
            columnHeader14.Text = "Deleted";
            // 
            // columnHeader15
            // 
            columnHeader15.Text = "Fingerprint";
            columnHeader15.Width = 150;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(887, 451);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel5);
            tabPage1.Controls.Add(panel1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(879, 423);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "User Management";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            panel5.Controls.Add(listView1);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(3, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(685, 417);
            panel5.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Controls.Add(button10);
            panel1.Controls.Add(button9);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button5);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(688, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(188, 417);
            panel1.TabIndex = 0;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button10.Location = new Point(21, 367);
            button10.Name = "button10";
            button10.Size = new Size(149, 38);
            button10.TabIndex = 0;
            button10.Text = "Exit";
            button10.UseVisualStyleBackColor = false;
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button9.Location = new Point(21, 322);
            button9.Name = "button9";
            button9.Size = new Size(149, 38);
            button9.TabIndex = 1;
            button9.Text = "Delete";
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button4.Location = new Point(21, 277);
            button4.Name = "button4";
            button4.Size = new Size(149, 38);
            button4.TabIndex = 2;
            button4.Text = "Toggle Role";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button5.Location = new Point(21, 231);
            button5.Name = "button5";
            button5.Size = new Size(149, 38);
            button5.TabIndex = 3;
            button5.Text = "Show Password";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(188, 196);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button3);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Controls.Add(groupBox1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(879, 423);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Accesscode Generator";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(-233, 118);
            button3.Name = "button3";
            button3.Size = new Size(83, 29);
            button3.TabIndex = 0;
            button3.Text = "Generate repair code";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button2);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label4);
            groupBox2.Dock = DockStyle.Right;
            groupBox2.Location = new Point(430, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(446, 417);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Access code generator for undelete/edit and FTV";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Location = new Point(240, 361);
            button2.Name = "button2";
            button2.Size = new Size(175, 34);
            button2.TabIndex = 0;
            button2.Text = "Generate code";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // textBox3
            // 
            textBox3.BorderStyle = BorderStyle.FixedSingle;
            textBox3.Location = new Point(51, 65);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(354, 23);
            textBox3.TabIndex = 1;
            // 
            // textBox4
            // 
            textBox4.BorderStyle = BorderStyle.FixedSingle;
            textBox4.Location = new Point(51, 123);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(354, 23);
            textBox4.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(51, 42);
            label3.Name = "label3";
            label3.Size = new Size(233, 15);
            label3.TabIndex = 3;
            label3.Text = "Request code: (Undelete/Edit/Change FTV)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(51, 100);
            label4.Name = "label4";
            label4.Size = new Size(130, 15);
            label4.TabIndex = 4;
            label4.Text = "Generated access code:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(873, 417);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Register machine code generator";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(209, 361);
            button1.Name = "button1";
            button1.Size = new Size(175, 34);
            button1.TabIndex = 0;
            button1.Text = "Generate code";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Location = new Point(29, 65);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(355, 23);
            textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Location = new Point(29, 123);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(355, 23);
            textBox2.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 42);
            label1.Name = "label1";
            label1.Size = new Size(162, 15);
            label1.TabIndex = 3;
            label1.Text = "Serial Number / Mac address:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 100);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 4;
            label2.Text = "Generated access code:";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel4);
            tabPage3.Controls.Add(panel3);
            tabPage3.Controls.Add(panel2);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(879, 234);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Script tools";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Controls.Add(label7);
            panel4.Controls.Add(label8);
            panel4.Controls.Add(label9);
            panel4.Controls.Add(label10);
            panel4.Controls.Add(label11);
            panel4.Controls.Add(textBox8);
            panel4.Controls.Add(textBox9);
            panel4.Controls.Add(cmb_prov);
            panel4.Controls.Add(cmb_mun);
            panel4.Controls.Add(button8);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(3, 426);
            panel4.Name = "panel4";
            panel4.Size = new Size(873, 0);
            panel4.TabIndex = 2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Top;
            label7.Location = new Point(0, 0);
            label7.Name = "label7";
            label7.Size = new Size(538, 15);
            label7.TabIndex = 0;
            label7.Text = "Extend Allowed No. of Days for Editing/Correction of Registration Records after the Last ERB Hearing:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(28, 30);
            label8.Name = "label8";
            label8.Size = new Size(73, 15);
            label8.TabIndex = 1;
            label8.Text = "ERB hearing:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(264, 29);
            label9.Name = "label9";
            label9.Size = new Size(56, 15);
            label9.TabIndex = 2;
            label9.Text = "End date:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(502, 28);
            label10.Name = "label10";
            label10.Size = new Size(56, 15);
            label10.TabIndex = 3;
            label10.Text = "Province:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(741, 28);
            label11.Name = "label11";
            label11.Size = new Size(76, 15);
            label11.TabIndex = 4;
            label11.Text = "Municipality:";
            // 
            // textBox8
            // 
            textBox8.BorderStyle = BorderStyle.FixedSingle;
            textBox8.Location = new Point(28, 60);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(227, 23);
            textBox8.TabIndex = 5;
            // 
            // textBox9
            // 
            textBox9.BorderStyle = BorderStyle.FixedSingle;
            textBox9.Location = new Point(264, 59);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(227, 23);
            textBox9.TabIndex = 6;
            // 
            // cmb_prov
            // 
            cmb_prov.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_prov.Location = new Point(502, 59);
            cmb_prov.Name = "cmb_prov";
            cmb_prov.Size = new Size(223, 23);
            cmb_prov.TabIndex = 7;
            cmb_prov.SelectedIndexChanged += cmb_prov_SelectedIndexChanged;
            cmb_prov.KeyPress += cmb_prov_KeyPress;
            // 
            // cmb_mun
            // 
            cmb_mun.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_mun.Location = new Point(741, 58);
            cmb_mun.Name = "cmb_mun";
            cmb_mun.Size = new Size(223, 23);
            cmb_mun.TabIndex = 8;
            cmb_mun.TextChanged += cmb_mun_TextChanged;
            cmb_mun.KeyPress += cmb_mun_KeyPress;
            // 
            // button8
            // 
            button8.Location = new Point(997, 40);
            button8.Name = "button8";
            button8.Size = new Size(113, 41);
            button8.TabIndex = 9;
            button8.Text = "Create script";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(button7);
            panel3.Controls.Add(textBox7);
            panel3.Controls.Add(label6);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(3, 228);
            panel3.Name = "panel3";
            panel3.Size = new Size(873, 198);
            panel3.TabIndex = 0;
            // 
            // button7
            // 
            button7.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button7.Location = new Point(739, 137);
            button7.Name = "button7";
            button7.Size = new Size(113, 41);
            button7.TabIndex = 2;
            button7.Text = "Create script";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // textBox7
            // 
            textBox7.BorderStyle = BorderStyle.FixedSingle;
            textBox7.Dock = DockStyle.Left;
            textBox7.Location = new Point(0, 15);
            textBox7.Multiline = true;
            textBox7.Name = "textBox7";
            textBox7.ScrollBars = ScrollBars.Both;
            textBox7.Size = new Size(964, 183);
            textBox7.TabIndex = 1;
            textBox7.Text = resources.GetString("textBox7.Text");
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Top;
            label6.Location = new Point(0, 0);
            label6.Margin = new Padding(3, 0, 3, 15);
            label6.Name = "label6";
            label6.Size = new Size(110, 15);
            label6.TabIndex = 0;
            label6.Text = "Script creation tool:";
            // 
            // panel2
            // 
            panel2.Controls.Add(label5);
            panel2.Controls.Add(textBox5);
            panel2.Controls.Add(button6);
            panel2.Controls.Add(textBox6);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(873, 225);
            panel2.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Top;
            label5.Location = new Point(0, 0);
            label5.Name = "label5";
            label5.Size = new Size(133, 15);
            label5.TabIndex = 0;
            label5.Text = "Decrypt and view script:";
            // 
            // textBox5
            // 
            textBox5.BorderStyle = BorderStyle.FixedSingle;
            textBox5.Location = new Point(9, 22);
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.Size = new Size(809, 23);
            textBox5.TabIndex = 1;
            // 
            // button6
            // 
            button6.FlatStyle = FlatStyle.Flat;
            button6.Location = new Point(825, 22);
            button6.Name = "button6";
            button6.Size = new Size(34, 23);
            button6.TabIndex = 2;
            button6.Text = "...";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // textBox6
            // 
            textBox6.BorderStyle = BorderStyle.FixedSingle;
            textBox6.Dock = DockStyle.Bottom;
            textBox6.Location = new Point(0, 51);
            textBox6.Multiline = true;
            textBox6.Name = "textBox6";
            textBox6.ScrollBars = ScrollBars.Both;
            textBox6.Size = new Size(873, 174);
            textBox6.TabIndex = 3;
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(879, 234);
            tabPage4.TabIndex = 3;
            tabPage4.Text = " Voters";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(879, 234);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Registrants";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(887, 451);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Quick and Dirty VRS Utility";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage2.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        private ColumnHeader columnHeader1;
        private ColumnHeader USER;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private ColumnHeader columnHeader11;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader13;
        private ColumnHeader columnHeader14;
        private ColumnHeader columnHeader15;
    }
}
