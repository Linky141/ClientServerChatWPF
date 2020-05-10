using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientServerChat
{
    class CSClient
    {
        public CSClient(MainWindow mw)
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);

            mainWindow = mw;
        }

        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //ZMIENNE

        private TcpClient klient;
        private BinaryWriter pisanie;
        private BinaryReader czytanie;
        private string serwerIP;
        private int port;
        private bool polaczeniaAktywne;

        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private BackgroundWorker backgroundWorker2 = new BackgroundWorker();

        MainWindow mainWindow = null;

        bool showerrors = false;

        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //METODY

        public void TryConnect()
        {
            serwerIP = mainWindow.cl_tbx_ip.Text;
            try
            {
                port = Int32.Parse(mainWindow.cl_tbx_port.Text);
            }
            catch
            {
                AddDial("Podano niepoprawny port!", Colors.Red);
                mainWindow.cl_tbx_port.Text = "";
            }

            try
            {
                if (polaczeniaAktywne == false)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception exc)
            {
                if (showerrors) MessageBox.Show(exc.ToString());
            }
        }

        public void TryDisconnect()
        {
            if (polaczeniaAktywne == true)
            {
                if (klient != null)
                {
                    pisanie.Write("###BYE###");
                    Task.Delay(200);
                    klient.Dispose();
                    klient.Close();
                }
                ChangeStatusOfConnection(false);
            }
        }

        public void SendString(string msg)
        {
            try
            {
                pisanie.Write(msg);
            }
            catch (Exception exc)
            {
                if (showerrors) MessageBox.Show(exc.ToString());
            }
        }


        public void AddDial(string val, Color color)
        {
            if (val != "" && val != "\n")
            {
                try
                {
                    mainWindow.cl_tbx_dialhistorry.Dispatcher.Invoke(new Action(() =>
                    {
                        Run run = new Run(val);
                        run.Foreground = new SolidColorBrush(color);
                        Paragraph paragraph = new Paragraph(run);
                        paragraph.LineHeight = 1;
                        paragraph.FontSize = 12;
                        mainWindow.cl_tbx_dialhistorry.Document.Blocks.Add(paragraph);
                        mainWindow.cl_tbx_dialhistorry.ScrollToEnd();
                    }));
                }
                catch (Exception exc) { if (showerrors) MessageBox.Show(exc.ToString()); }
            }
        }

        private void ChangeStatusOfConnection(bool val)
        {
            if (val)
            {
                polaczeniaAktywne = true;
                mainWindow.cl_btn_connect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_connect.IsEnabled = false; }));
                mainWindow.cl_btn_disconnect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_disconnect.IsEnabled = true; }));
                mainWindow.cl_btn_send.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_send.IsEnabled = true; }));
                mainWindow.cl_tbx_ip.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_ip.IsEnabled = false; }));
                mainWindow.cl_tbx_port.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_port.IsEnabled = false; }));
                mainWindow.cl_tbx_nick.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_nick.IsEnabled = true; }));
                mainWindow.cl_btn_changename.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_changename.IsEnabled = true; }));

            }
            else
            {
                polaczeniaAktywne = false;
                mainWindow.cl_btn_connect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_connect.IsEnabled = true; }));
                mainWindow.cl_btn_disconnect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_disconnect.IsEnabled = false; }));
                mainWindow.cl_btn_send.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_send.IsEnabled = false; }));
                mainWindow.cl_tbx_ip.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_ip.IsEnabled = true; }));
                mainWindow.cl_tbx_port.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_port.IsEnabled = true; }));
                mainWindow.cl_tbx_nick.Dispatcher.Invoke(new Action(() => { mainWindow.cl_tbx_nick.IsEnabled = false; }));
                mainWindow.cl_btn_changename.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_changename.IsEnabled = false; }));
            }
        }

        public string WyslijPing(string adres, int timeout, byte[] bufor, PingOptions opcje)
        {
            Ping ping = new Ping();
            try
            {
                PingReply odpowiedz = ping.Send(adres, timeout, bufor, opcje);
                if (odpowiedz.Status == IPStatus.Success)
                    return "Odpowiedź z " + adres + " bajtów=" +
                        odpowiedz.Buffer.Length + " czas=" + odpowiedz.RoundtripTime +
                        "ms TTL=" + odpowiedz.Options.Ttl;
                else return "Błąd:" + adres + " " + odpowiedz.Status.ToString();
            }
            catch (Exception ex)
            {
                return "Błąd:" + adres + " " + ex.Message;
            }
        }

        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //AddDial("Próba połączenia ... ", Colors.Gray);
                mainWindow.cl_btn_connect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_connect.IsEnabled = false; }));
                mainWindow.cl_btn_disconnect.Dispatcher.Invoke(new Action(() => { mainWindow.cl_btn_disconnect.IsEnabled = false; }));
                klient = new TcpClient(serwerIP, port);
                NetworkStream ns = klient.GetStream();
                czytanie = new BinaryReader(ns);
                pisanie = new BinaryWriter(ns);
                pisanie.Write("###HI###"); 
                //AddDial("Autoryzacja ...", Colors.Gray);
                backgroundWorker2.RunWorkerAsync();
                ChangeStatusOfConnection(true);
                while (true)
                {
                    if (polaczeniaAktywne == false) break;
                    SendString("\n");
                    await Task.Delay(100);
                }
            }
            catch
            {
                AddDial("Nie można nawiązać połączenia", Colors.Red);
                ChangeStatusOfConnection(false);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            AddDial("Połączono z " + serwerIP, Colors.Gray);
            string wiadomosc;
            try
            {
                while ((wiadomosc = czytanie.ReadString()) != "###BYE###")
                {
                    if (wiadomosc.Length > 14)
                        if (wiadomosc.Substring(0, 14) == "SERVER MESSAGE") AddDial(wiadomosc, Colors.DarkMagenta);
                        else if (wiadomosc.Substring(0, 6) == "SERVER") AddDial(wiadomosc, Colors.DarkCyan);
                        else AddDial(wiadomosc, Colors.Black);
                    else if (wiadomosc.Length > 6)
                        if (wiadomosc.Substring(0,6) == "SERVER") AddDial(wiadomosc, Colors.DarkCyan);
                        else AddDial(wiadomosc, Colors.Black);
                    else AddDial(wiadomosc, Colors.Black);
                    if (polaczeniaAktywne == false) break;
                }
                AddDial("Połączenie z " + mainWindow.cl_tbx_ip.Text +" przerwane", Colors.Gray);
                ChangeStatusOfConnection(false);
                klient.Close();
            }
            catch (Exception exc)
            {
                if (showerrors) MessageBox.Show(exc.ToString());
                AddDial("Rozłączono z serwerem", Colors.Red);
                ChangeStatusOfConnection(false);
                klient.Close();
            }
        }

        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //GETTERY I SETTERY

        public bool get_polaczeniekatywne() { return polaczeniaAktywne; }
        public void set_polaczenieaktywne(bool val) { polaczeniaAktywne = val; }
        public bool get_client_isnull() { if (klient == null) return true; else return false; }

    }
}
