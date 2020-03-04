using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net;

namespace TaktzeitMonitor
{
    public partial class Form1 : Form
    {

        double LastWertX = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //Vorgabe laden
        private void button1_Click(object sender, EventArgs e)
        {


            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Konfigurationsdatei auswählen";
            dialog.Filter = "CSV-Files |*.csv" + "|All Files|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                chart1.Series.Add("Vorgabe");
                // Set SplineArea chart type
                chart1.Series["Vorgabe"].ChartType = SeriesChartType.Area;
                //Enable data points markers
                //chart1.Series["Vorgabe"].MarkerStyle = MarkerStyle.Circle;
                //chart1.Series["Vorgabe"].MarkerColor = Color.Magenta;
                // Show transparent color
                chart1.Series["Vorgabe"].Color = Color.FromArgb(60, 255, 0, 000);
                try
                {
                    System.IO.StreamReader SRConf = new System.IO.StreamReader(dialog.FileName);

                    while (SRConf.EndOfStream == false)
                    {
                        String Zeilenstream = SRConf.ReadLine();
                        String[] Zeilenarray = Zeilenstream.Split(';');
                        double WertX = Convert.ToDouble(Zeilenarray[0]);
                        double WertY = Convert.ToDouble(Zeilenarray[1]);
                        chart1.Series["Vorgabe"].Points.AddXY(WertX, WertY);
                    }
                    SRConf.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Eine Konfiguration wurde nicht gefunden. \r Die Konfigurationsdatei wird neu erstellt!");
                }
            }
      
        }

        //Aufnahme starten
        private void button2_Click(object sender, EventArgs e)
        {
            LastWertX = 0;
            chart1.Series["Aktuell"].Points.Clear();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void Abfrage()
        {
            //try
            //{
                String Requeststring = t_url.Text + "?";
                Requeststring = Requeststring + "&request=" + "/channel/channelDiagnose/cycleTime";
                Requeststring = Requeststring + "&request=" + "/Channel/ProgramInfo/actLineNumber";
                String Antwortstring = HTTPAbfrage(Requeststring);
                Antwortstring = Antwortstring.Replace('.', ',');
                String[] Antwortarray = Antwortstring.Split(';');
                double WertX = Convert.ToDouble(Antwortarray[0]);
                double WertY = Convert.ToDouble(Antwortarray[1]);
                Console.WriteLine(WertX);
                if (WertX > LastWertX)
                {
                    chart1.Series["Aktuell"].Points.AddXY(WertX, WertY);
                    LastWertX = WertX;
                }

                


           //}
            //catch (Exception e)
            //{
            //    timer1.Stop();
            //    MessageBox.Show("Fehler bei der Abfrage!\nBitte stoppen, dann alle Kanäle kontrollieren und erneut auf Start klicken!\n", "Fehler bei der Abfrage");

            //}

        }

        string HTTPAbfrage(string requeststring)
        {
            HttpWebRequest Anfrage = (HttpWebRequest)WebRequest.Create(requeststring);
            HttpWebResponse Antwort = (HttpWebResponse)Anfrage.GetResponse();
            System.IO.Stream Antwort_Stream = Antwort.GetResponseStream();
            System.IO.StreamReader Antwort_Streamreader = new System.IO.StreamReader(Antwort_Stream);
            String Ausgabe = Antwort_Streamreader.ReadToEnd();
            return Ausgabe;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Abfrage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            //Anpassen der Achsen
            chart1.ChartAreas[0].Axes[0].ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].Axes[1].ArrowStyle = AxisArrowStyle.Triangle;
            chart1.ChartAreas[0].Axes[0].Title = "Job Time (D)";
            chart1.ChartAreas[0].Axes[1].Title = "NC-Line (-)";
            chart1.ChartAreas[0].Axes[0].Minimum = 0;
            chart1.ChartAreas[0].Axes[1].Minimum = 0;
            // Disable X axis margin
            chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
            //Zoomfunktion Aktivieren
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.ResetZoom;
            chart1.ChartAreas[0].AxisX.ScrollBar.Size = 20;
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.Enabled = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.ResetZoom;
            chart1.ChartAreas[0].AxisY.ScrollBar.Size = 20;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            chart1.Series.Add("Aktuell");
            chart1.Series["Aktuell"].ChartType = SeriesChartType.Line;
            chart1.Series["Aktuell"].MarkerStyle = MarkerStyle.Circle;
            chart1.Series["Aktuell"].MarkerColor = Color.Black;
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }
    }
}
