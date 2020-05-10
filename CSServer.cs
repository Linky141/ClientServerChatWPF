using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace ClientServerChat
{
    class CSServer
    {
        public CSServer(MainWindow mainwindow)
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);

            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            this.mainwindow = mainwindow;
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //ZMIENNE

        private TcpListener server = null;
        private List<CSServerClientField> clients = new List<CSServerClientField>();
        private string adresIP = "";
        private string port = "";
        private bool polaczenieAktywne = false;
        private int lastclient = 0;

        private List<string> bannedIP = new List<string>();

        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private BackgroundWorker backgroundWorker2 = new BackgroundWorker();

        MainWindow mainwindow = null;
        bool showerrors = false;
               

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //METODY

        public void AddDial(string val, Color color)
        {
            if (val != "" && val != "\n")
            {
                try
                {
                    mainwindow.srv_tbx_dialhistorry.Dispatcher.Invoke(new Action(() =>
                    {
                        Run run = new Run(val);
                        run.Foreground = new SolidColorBrush(color);
                        Paragraph paragraph = new Paragraph(run);
                        paragraph.LineHeight = 1;
                        paragraph.FontSize = 12;
                        mainwindow.srv_tbx_dialhistorry.Document.Blocks.Add(paragraph);
                        mainwindow.srv_tbx_dialhistorry.ScrollToEnd();
                    }));
                }
                catch (Exception exc) { if (showerrors) MessageBox.Show(exc.ToString()); }
            }
        }

        public void ClientListRefreash()
        {
            try
            {
                mainwindow.srv_lbx_users.Dispatcher.Invoke(new Action(() => {
                    mainwindow.srv_lbx_users.Items.Clear();
                    for (int clk = 0; clk < clients.Count; clk++)
                    {
                        mainwindow.srv_lbx_users.Items.Add(GetClientIP(clients[clk]));
                    }
                }));
            }
            catch (Exception exc) { if (showerrors) MessageBox.Show(exc.ToString()); }
        }

        public char TryStartServer()
        {
            try
            {
                if (polaczenieAktywne == false)
                {
                    polaczenieAktywne = true;
                    backgroundWorker1.RunWorkerAsync();
                    return 'S';
                }
                else
                {
                    polaczenieAktywne = false;
                    for (int clk = 0; clk < clients.Count; clk++) CloseClient(clients[clk]);
                    server.Stop();
                    backgroundWorker1.CancelAsync();
                    if (backgroundWorker2.IsBusy)
                        backgroundWorker2.CancelAsync();
                    return 'N';
                }
            } catch(Exception exc) { if (showerrors) MessageBox.Show(exc.ToString()); return 'F'; }
        }

        public char TryStopServer()
        {
            try
            {
                polaczenieAktywne = false;
                for (int clk = 0; clk < clients.Count; clk++) CloseClient(clients[clk]);
                if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
                if (backgroundWorker2.IsBusy) backgroundWorker2.CancelAsync();
                if (server != null) server.Stop();
                return 'S';
            }
            catch(Exception exc)
            {
                if (showerrors) MessageBox.Show(exc.ToString());
                AddDial("Błąd podczas zatrzymywania serwera", Colors.Red);
                return 'F';
            }
        }

        public void SendString(string val)
        {
            try
            {
                for (int clk = 0; clk < clients.Count; clk++) clients[clk].writer.Write(val);
            }
            catch(Exception ex)
            { if (showerrors) MessageBox.Show(ex.ToString()); }
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
                else return "Błąd: " + adres + " " + odpowiedz.Status.ToString();
            }
            catch (Exception ex)
            {
                return "Błąd:" + adres + " " + ex.Message;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            IPAddress serwerIP;
            int portval;
            try
            {
                serwerIP = IPAddress.Parse(adresIP);
                portval = Int32.Parse(port);
            }
            catch
            {
                AddDial("Błędny adres IP lub port", Colors.Red);
                polaczenieAktywne = false;
                return;
            }
            server = new TcpListener(serwerIP, portval);
            try
            {
                server.Start();
                bool first = true;
                while (true)
                {
                    if (polaczenieAktywne == false) break;
                    if (server.Pending())
                    {
                        //AddDial("Oczekuje na połączenie ...", Colors.Gray);
                        clients.Add(new CSServerClientField());
                        clients[clients.Count - 1].Initialization(server);
                        lastclient = clients.Count - 1;


                        if (clients[lastclient].reader.ReadString() == "###HI###")
                        {
                            if (CheckBannedClient(clients[lastclient]))
                            {
                                clients[lastclient].writer.Write("SERVER MESSAGE: Jesteś zbanowany na serwerze!");
                                Task.Delay(100);
                                clients[lastclient].writer.Write("###BYE###");
                                CloseClient(clients[lastclient]);
                            }
                            else
                            {
                                AddDial(GetClientIP(clients[lastclient]) + " właśnie podłączył się do serwera!", Colors.Gray);
                                SendString("SERVER MESSAGE: " + GetClientIP(clients[lastclient]) + " właśnie podłączył się do serwera!");
                                ClientListRefreash();
                                if (first == true) { backgroundWorker2.RunWorkerAsync(); first = false; }
                            }
                        }
                        else
                        {
                            AddDial(GetClientIP(clients[lastclient]) + " nie wykonał wymaganej autoryzacji. Połączenie przerwane", Colors.Gray);
                            CloseClient(clients[lastclient]);
                            ClientListRefreash();
                            //server.Stop();
                            //polaczenieAktywne = false;
                        }
                        
                    }
                }
            }
            catch (Exception exc)
            {
                if (showerrors) MessageBox.Show(exc.ToString());
                AddDial("Połączenie zostało przerwane", Colors.Red);
                polaczenieAktywne = false;
            }
        }

        private void CheckConnectedClients()
        {
            try
            {
                foreach (var val in clients)
                {
                    if (val.client.Connected == false) CloseClient(val);
                }
            }catch(Exception exc) { if (showerrors) MessageBox.Show(exc.ToString()); }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            string wiadomosc = ""; int indextoclose = 0; int clk = 0;
            while (true)
            {
                
                try
                {
                    while (true)
                    {
                        //CheckConnectedClients();
                        clk += 1;
                        while(clients.Count == 0) { Task.Delay(100); }
                        if (clk >= clients.Count) clk = 0;

                        wiadomosc = "";
                        if (polaczenieAktywne == false || clients.Count == 0) break;

                        wiadomosc = clients[clk].reader.ReadString();
                        //CheckConnectedClients();
                        if (wiadomosc == "###BYE###") { AddDial(GetClientIP(clients[clk]) + " wyszedł z serwera", Colors.Gray); SendString("SERVER MESSAGE: " + GetClientIP(clients[clk]) + " wyszedł z serwera"); CloseClient(clients[clk]);  break; }
                        if (wiadomosc != "\n" && wiadomosc != "") { indextoclose = clk; break; }
                    }
                }
                catch (Exception exc)
                {
                    if (showerrors) MessageBox.Show(exc.ToString());
                    AddDial("Klient rozłączony", Colors.Gray);
                    CheckConnectedClients();
                    //CloseClient(clients[indextoclose]);
                    //polaczenieAktywne = false;
                }
                if (polaczenieAktywne == false) break;
                if (wiadomosc != "###BYE###" && wiadomosc != "###HI###")
                {
                    if (wiadomosc.Length > 14)
                    {
                        if (wiadomosc.Substring(0, 14) == "SERVER MESSAGE") AddDial(wiadomosc, Colors.Gray);
                        else AddDial(wiadomosc, Colors.Black);
                    }
                    else AddDial(wiadomosc, Colors.Black);
                    SendString(wiadomosc);
                }
            }
        }

        private void CloseClient(CSServerClientField client)
        {
            clients[clients.IndexOf(client)].CloseAll();
            clients.Remove(client);
            lastclient = clients.Count - 1;
            
            ClientListRefreash();
        }



        private string GetClientIP(CSServerClientField client)
        {
            return ((IPEndPoint)client.client.Client.RemoteEndPoint).Address.ToString();
        }

        public void TryBanClient(int id)
        {
            if (id == -1 || id >= clients.Count) return;
            AddDial("SERVER MESSAGE: " + GetClientIP(clients[id]) + " został zbanowany na serwerze!", Colors.Gray);
            SendString("SERVER MESSAGE: " + GetClientIP(clients[id]) + " został zbanowany na serwerze!");
            bannedIP.Add(GetClientIP(clients[id]));
            CloseClient(clients[id]);

        }

        public void TryKickClient(int id)
        {
            if (id == -1 || id >= clients.Count) return;
            AddDial("SERVER MESSAGE: " + GetClientIP(clients[id]) + " został wyrzuconu z serwera!", Colors.Gray);
            SendString("SERVER MESSAGE: " + GetClientIP(clients[id]) + " został wyrzuconu z serwera!");
            CloseClient(clients[id]);
        }

        private bool CheckBannedClient(CSServerClientField client)
        {
            foreach (var ban in bannedIP)
            {
                if (GetClientIP(client) == ban) return true;
            }
            return false;
        }

        public void ShowBannedClients()
        {
            AddDial("Banned Clients:", Colors.Orange);
            foreach (var val in bannedIP)
            {
                AddDial(val, Colors.Orange);
            }
        }

        public void UnbanClient(string str)
        {
            bannedIP.Remove(str);
            AddDial("unbanned client " + str, Colors.Gray);
        }

        //------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------
        //GETTERY I SETTERY

        public bool get_polaczenieaktywne() { return polaczenieAktywne; }
        public void set_polaczenieaktywne(bool val) { polaczenieAktywne = val; }
        public string get_adresip() { return adresIP; }
        public void set_adresip(string val) { adresIP = val; }
        public string get_port() { return port; }
        public void set_port(string val) { port = val; }

    }
}
