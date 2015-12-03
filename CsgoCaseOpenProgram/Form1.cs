using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CsgoCaseOpenProgram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // rollOutputRichTextBox.ForeColor = System.Drawing.Color.Red;
            // ^ How to change text color
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ^ "Import XML -> Open" clicked
            // Display XML import file dialog
            DialogResult importedXmlResult = xmlImportDialog.ShowDialog();
            if (importedXmlResult == DialogResult.OK)
            {
                XmlDocument configXml = new XmlDocument();
                configXml.Load(xmlImportDialog.FileName);

                XmlNode configXmlRoot = configXml.FirstChild;
                XmlNode configXmlCaseList = configXmlRoot["cases"];

                string currentCaseName = null;

                for (int i = 0; i < configXmlCaseList.ChildNodes.Count; i++)
                {
                    currentCaseName = configXmlCaseList.ChildNodes[i].Attributes["caseName"].Value;
                    caseListBox.Items.Add(currentCaseName);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Reset text box color and value
            rollOutputRichTextBox.Text = "";
            rollOutputRichTextBox.ForeColor = Color.White;

            // Load XML file
            XmlDocument configXml = new XmlDocument();
            configXml.Load(xmlImportDialog.FileName);

            // Select possible guns list from selected case; stored in XmlNodeList `possibleGunsList`
            XmlNode caseList = configXml.FirstChild["cases"];
            XmlNode selectedCase = caseList.SelectNodes("case[@caseName='" + caseListBox.SelectedItem.ToString() + "']")[0];
            XmlNodeList possibleGunsList = selectedCase["possibleGuns"].ChildNodes;

            // Read odds, store in `classOddsDict`
            XmlNode classOddsXml = configXml.FirstChild["odds"]["classOdds"];
            int classes = classOddsXml.ChildNodes.Count;
            OrderedDictionary classOddsDict = new OrderedDictionary();
            string className = null;
            double classOdds = 0;
            for (int i = 0; i < classes; i++)
            {
                className = classOddsXml.ChildNodes[i].SelectNodes("className")[0].InnerText;
                classOdds = Convert.ToDouble(classOddsXml.ChildNodes[i].SelectNodes("classOdds")[0].InnerText);
                classOddsDict.Add(className, classOdds);
            }

            // Choose random class, stores color in `selectedColor`
            Random randomObject = new Random();
            double randomColorRoll = randomObject.NextDouble();
            double cumulative = 0.0;
            int selectedClassIndex = 0;
            for (int i = 0; i < classOddsDict.Count; i++)
            {
                cumulative += Convert.ToDouble(classOddsDict[i]);
                if (randomColorRoll < cumulative)
                {
                    selectedClassIndex = i;
                    break;
                }
            }
            string selectedColor = classOddsXml.ChildNodes[selectedClassIndex].SelectNodes("classColor")[0].InnerText;

            // Creates list of possible guns based off class color, stores in XmlNodeList `classGunsArr`
            XmlNodeList classGunsArr = selectedCase["possibleGuns"].SelectNodes("gun[gunClass='" + selectedColor + "']");

            // Selects random gun, stores in XmlNode `selectedGun`
            int randomGunRoll = randomObject.Next(0, classGunsArr.Count);
            XmlNode selectedGun = classGunsArr[randomGunRoll];

            // Display roll output
            Color selectedColorVar = Color.Green;
            switch (selectedClassIndex) {
                case 0:
                    selectedColorVar = Color.Blue;
                    break;
                case 1:
                    selectedColorVar = Color.FromArgb(92, 61, 153);
                    break;
                case 2:
                    selectedColorVar = Color.FromArgb(204, 51, 153);
                    break;
                case 3:
                    selectedColorVar = Color.Red;
                    break;
                case 4:
                    selectedColorVar = Color.Gold;
                    break;
            }
            rollOutputRichTextBox.ForeColor = selectedColorVar;
            rollOutputRichTextBox.Text = (selectedGun["gunName"].InnerText + " | " + selectedGun["gunSkin"].InnerText);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (caseListBox.SelectedItem != null)
            {
                string selectedCaseName = caseListBox.SelectedItem.ToString();
                rollOutputRichTextBox.Text = ("Case selected: " + selectedCaseName);
            }
        }
    }
}
