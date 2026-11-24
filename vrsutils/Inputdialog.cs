using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace vrsutils
{
    internal class Inputdialog
    {
    }

  //  Based on the work of Bas Brekelmans above, I have also created two derivations -> "input" dialogs that allow you to receive from the user both a text value and a boolean(TextBox and CheckBox):

public static class PromptForKeyAndcert
    {
        public static string ShowDialog(string caption, string ktext, string ctext)
        {
            ArgumentNullException.ThrowIfNull(ktext);
            ArgumentNullException.ThrowIfNull(ctext);
            Form prompt = new()
            {
                Width = 420,
                Height = 230,
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog 
            };
            Label textLabel = new() { Left = 16, Top = 20, Width = 240, Text = ktext };
            TextBox textBox = new() { Left = 16, Top = 60, Width = 240, TabIndex = 0, TabStop = true, BorderStyle = BorderStyle.FixedSingle  };
            Label textLabel2 = new() { Left = 16, Top = 100, Width = 240, Text = ctext };
            TextBox textBox2 = new() { Left = 16, Top = 140, Width = 240, TabIndex = 1, TabStop = true, BorderStyle = BorderStyle.FixedSingle };
            Button confirmation = new() { Text = "Submit", Left = 280, Width = 100, Height = 38, Top = 130, TabIndex = 2, TabStop = true };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel2);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            return string.Format("{0},{1}", textBox.Text, textBox2.Text);
        }
    }
//...and text along with a selection of one of multiple options(TextBox and ComboBox) :


    }












