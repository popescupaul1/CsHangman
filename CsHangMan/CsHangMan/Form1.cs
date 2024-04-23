using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CsHangMan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string word = "";
        string newWord = "";
        List<Label> labels = new List<Label>();
        int missedAmount = 0;
        enum BodyParts
        {
            Head,
            Face,
            Body,
            Right_Arm,
            Left_Arm,
            Right_Leg,
            Left_Leg
        }

        void drawingPost()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.MidnightBlue, 10);
            g.DrawLine(p, new Point(275, 426), new Point(275, 5));
            g.DrawLine(p, new Point(280, 5), new Point(140, 5));
            g.DrawLine(p, new Point(135, 0), new Point(135, 50));
        }

        void drawingBody()
        {
            drawingBodyPart(BodyParts.Head);
            drawingBodyPart(BodyParts.Face);
            drawingBodyPart(BodyParts.Body);
            drawingBodyPart(BodyParts.Left_Arm);
            drawingBodyPart(BodyParts.Right_Arm);
            drawingBodyPart(BodyParts.Left_Leg);
            drawingBodyPart(BodyParts.Right_Leg);
        }

        void drawingBodyPart(BodyParts bp)
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.White, 2);
            SolidBrush s = new SolidBrush(Color.White);
            if (bp == BodyParts.Head)
                g.DrawEllipse(p, 98, 50, 75, 75);
            
            else if (bp == BodyParts.Face)
            {
                g.FillEllipse(s, 117, 70, 5, 5); // left eye
                g.FillEllipse(s, 147, 70, 5, 5); // right eye
                g.DrawArc(p, 122, 70, 25, 25, 45, 90); // mouth (cute)
            }
            else if (bp == BodyParts.Body)
            {
                g.DrawLine(p, new Point(135, 125), new Point(135, 250));
            }
            else if (bp == BodyParts.Right_Arm) 
            {
                g.DrawLine(p, new Point(135, 140), new Point(85, 170));
            }
            else if (bp == BodyParts.Left_Arm)
            {
                g.DrawLine(p, new Point(135, 140), new Point(185, 170));
            }
            else if (bp == BodyParts.Right_Leg)
            {
                g.DrawLine(p, new Point(135, 250), new Point(85, 300));
            }
            else if (bp == BodyParts.Left_Leg)
            {
                g.DrawLine(p, new Point(135, 250), new Point(185, 300));
            }
        }

        string getRandomWordWeb()
        {
            WebClient wc = new WebClient();
            string wordlist = wc.DownloadString("https://www.dictionary-thesaurus.com/wordlists/ElectronicsTerms%28435%29.txt");
            string[] word = wordlist.Split('\n');

            Random rand = new Random();
            return word[rand.Next(0, word.Length - 1)];
        }

        string getRandomWordFile()
        {
            string[] word;

            string filePath = "cuvinte.txt";
            using (StreamReader sr = new StreamReader(filePath))
            {
                string wordlist = sr.ReadToEnd();
                word = wordlist.Split('\n');
            }

            Random rand = new Random();
            return word[rand.Next(0, word.Length - 1)];
        }

        void MakeLabels()
        {
            word = getRandomWordWeb();
            //word = getRandomWordFile(); 
            char[] chars = word.ToCharArray();
            int between = 450 / chars.Length - 1;
            for (int i = 0; i < chars.Length - 1; i++) 
            {
                labels.Add(new Label());
                labels[i].Location = new Point((i * between) + 45, 170);
                labels[i].Text = "_";
                labels[i].Parent = groupBox2;
                labels[i].ForeColor = Color.White;
                labels[i].BringToFront();
                labels[i].CreateControl();
            }
            wordCount.Text = "Word Length: " + (chars.Length - 1).ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(letterTBox.Text))
            {
                MessageBox.Show("A letter must be submitted!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            char letter = letterTBox.Text.ToLower().ToCharArray()[0];
            if(!char.IsLetter(letter)) 
            {
                MessageBox.Show("Submit a letter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (word.Contains(letter))
            {
                char[] letters = word.ToCharArray();
                for (int i = 0; i < letters.Length; i++) 
                {
                    if (letters[i] == letter)
                        labels[i].Text = letter.ToString();
                }
                foreach(Label lab in labels)
                    if (lab.Text == "_") return;
                MessageBox.Show("You Have Won!", "Congrats!");
                resetGame();
            }
            else
            {
                MessageBox.Show("The letter that you guesses isn't in the word.", "Sorry :(");
                missedletter.Text += " " + letter.ToString() + ", ";
                drawingBodyPart((BodyParts)missedAmount);
                missedAmount++;
                if(missedAmount == 8)
                {
                    MessageBox.Show("Sorry, you lost :( \n The word was " + word);
                    resetGame();
                }
            }
            letterTBox.Text = "";
        }

        void resetGame()
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(panel1.BackColor);

            newWord = getRandomWordWeb();
            //word = getRandomWordFile();
            char[] chars = newWord.ToCharArray();

            MakeLabels();
            drawingPost();
            missedAmount = 0;
            missedletter.Text = "Missed: ";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textbox2.Text))
            {
                MessageBox.Show("Enter a word first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textbox2.Text == word)
            {
                MessageBox.Show("You Have Won!", "Congrats!");
                resetGame();
            }
            else
            {
                MessageBox.Show("The word that you guessed is wrong!", "Sorry!");
                drawingBodyPart((BodyParts)missedAmount);
                missedAmount++;
                if (missedAmount == 8)
                {
                    MessageBox.Show("Sorry, you lost :( \n The word was " + word);
                    resetGame();
                }
            }
        }
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            drawingPost();
            MakeLabels();
        }
    }
}
