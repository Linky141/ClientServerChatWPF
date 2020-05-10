using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ProgramInitialize();
        }

        private void ProgramInitialize()
        {
            //server
            srv_server = new CSServer(this);

            IPHostEntry tempadresyIP = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress pozycja in tempadresyIP.AddressList) srv_cbx_adress.Items.Add(pozycja.ToString());

            srv_btn_stop.IsEnabled = false;

            srv_tbx_port.Text = "20000";
            srv_tbx_dialhistorry.Document.Blocks.Clear();
            srv_cbx_adress.SelectedIndex = srv_cbx_adress.Items.Count - 1;

            //client

            cl_client = new CSClient(this);


            cl_ChangeStatusOfConnection(false);
            cl_client.set_polaczenieaktywne(false);
            cl_clientname = cl_tbx_nick.Text;


            cl_tbx_port.Text = "20000";
            cl_tbx_dialhistorry.Document.Blocks.Clear();
            cl_tbx_ip.Text = "192.168.1.63";

            object s = null;
            RoutedEventArgs x = null;
            CheckBox_Unchecked(s, x);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            backgroundofall.Background = Brushes.Black;
            header_client.Background = Brushes.Black;
            header_client.Foreground = Brushes.LightGray;
            
            header_server.Background = Brushes.Black;
            header_server.Foreground = Brushes.LightGray;
            foreground_client.Background = Brushes.Black;
            foreground_server.Background = Brushes.Black;
            cl_tbx_dialhistorry.Background = Brushes.DarkSlateGray;
            cl_tbx_dialhistorry.Foreground = Brushes.Gray;
            cl_tbx_command.Background = Brushes.DarkSlateGray;
            cl_tbx_command.Foreground = Brushes.Black;
            cl_lbl_ip.Foreground = Brushes.Gray;
            cl_tbx_ip.Background = Brushes.Black;
            cl_tbx_ip.Foreground = Brushes.Gray;
            cl_lbl_port.Foreground = Brushes.Gray;
            cl_tbx_port.Background = Brushes.Black;
            cl_tbx_port.Foreground = Brushes.Gray;
            cl_tbx_nick.Background = Brushes.Black;
            cl_tbx_nick.Foreground = Brushes.Gray;
            cl_btn_send.Foreground = Brushes.Gray;
            cl_btn_send.Background = Brushes.Black;
            cl_btn_send.BorderBrush = Brushes.Gray;
            cl_btn_changename.Foreground = Brushes.Gray;
            cl_btn_changename.Background = Brushes.Black;
            cl_btn_changename.BorderBrush = Brushes.Gray;
            cl_btn_ping.Foreground = Brushes.Gray;
            cl_btn_ping.Background = Brushes.Black;
            cl_btn_ping.BorderBrush = Brushes.Gray;
            cl_btn_clearchat.Foreground = Brushes.Gray;
            cl_btn_clearchat.Background = Brushes.Black;
            cl_btn_clearchat.BorderBrush = Brushes.Gray;
            cl_btn_disconnect.Foreground = Brushes.Gray;
            cl_btn_disconnect.Background = Brushes.Black;
            cl_btn_disconnect.BorderBrush = Brushes.Gray;
            cl_btn_connect.Foreground = Brushes.Gray;
            cl_btn_connect.Background = Brushes.Black;
            cl_btn_connect.BorderBrush = Brushes.Gray;
            cl_lbl_nick.Foreground = Brushes.Gray;
            cbx_nightmode.Foreground = Brushes.Gray;

            srv_lbx_users.Background = Brushes.DarkSeaGreen;
            srv_lbx_users.Foreground = Brushes.Black;

            srv_tbx_dialhistorry.Background = Brushes.DarkSlateGray;
            srv_tbx_dialhistorry.Foreground = Brushes.Black;

            srv_tbx_dialinput.Background = Brushes.DarkSlateGray;
            srv_tbx_dialinput.Foreground = Brushes.Gray;

            srv_tbx_ttl.Background = Brushes.Black;
            srv_tbx_ttl.Foreground = Brushes.Gray;

            srv_tbx_port.Background = Brushes.Black;
            srv_tbx_port.Foreground = Brushes.Gray;

            srv_cbx_adress.Background = Brushes.Black;
            srv_cbx_adress.Foreground = Brushes.Gray;

            srv_btn_dialsend.Foreground = Brushes.Gray;
            srv_btn_dialsend.Background = Brushes.Black;
            srv_btn_dialsend.BorderBrush = Brushes.Gray;

            srv_btn_start.Foreground = Brushes.Gray;
            srv_btn_start.Background = Brushes.DarkGreen;
            srv_btn_start.BorderBrush = Brushes.Gray;

            srv_btn_stop.Foreground = Brushes.Gray;
            srv_btn_stop.Background = Brushes.DarkRed;
            srv_btn_stop.BorderBrush = Brushes.Gray;

            srv_btn_cleardial.Foreground = Brushes.Gray;
            srv_btn_cleardial.Background = Brushes.Black;
            srv_btn_cleardial.BorderBrush = Brushes.Gray;

            srv_btn_pingall.Foreground = Brushes.Gray;
            srv_btn_pingall.Background = Brushes.Black;
            srv_btn_pingall.BorderBrush = Brushes.Gray;

            srv_lbl_ttl.Foreground = Brushes.Gray;
            srv_lbl_ip.Foreground = Brushes.Gray;
            srv_lbl_port.Foreground = Brushes.Gray;

            srv_commandinfo.Foreground = Brushes.DarkGray;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            backgroundofall.Background = Brushes.White;
            header_client.Background = Brushes.White;
            header_client.Foreground = Brushes.Black;
            header_server.Background = Brushes.White;
            header_server.Foreground = Brushes.Black;
            foreground_client.Background = Brushes.White;
            foreground_server.Background = Brushes.White;
            cl_tbx_dialhistorry.Background = Brushes.White;
            cl_tbx_dialhistorry.Foreground = Brushes.Black;
            cl_tbx_command.Background = Brushes.White;
            cl_tbx_command.Foreground = Brushes.Black;
            cl_lbl_ip.Foreground = Brushes.Black;
            cl_tbx_ip.Background = Brushes.White;
            cl_tbx_ip.Foreground = Brushes.Black;
            cl_lbl_port.Foreground = Brushes.Black;
            cl_tbx_port.Background = Brushes.White;
            cl_tbx_port.Foreground = Brushes.Black;
            cl_tbx_nick.Background = Brushes.White;
            cl_tbx_nick.Foreground = Brushes.Black;
            cl_btn_send.Foreground = Brushes.Black;
            cl_btn_send.Background = Brushes.White;
            cl_btn_send.BorderBrush = Brushes.Black;
            cl_btn_changename.Foreground = Brushes.Black;
            cl_btn_changename.Background = Brushes.White;
            cl_btn_changename.BorderBrush = Brushes.Black;
            cl_btn_ping.Foreground = Brushes.Black;
            cl_btn_ping.Background = Brushes.White;
            cl_btn_ping.BorderBrush = Brushes.Black;
            cl_btn_clearchat.Foreground = Brushes.Black;
            cl_btn_clearchat.Background = Brushes.White;
            cl_btn_clearchat.BorderBrush = Brushes.Black;
            cl_btn_disconnect.Foreground = Brushes.Black;
            cl_btn_disconnect.Background = Brushes.White;
            cl_btn_disconnect.BorderBrush = Brushes.Black;
            cl_btn_connect.Foreground = Brushes.Black;
            cl_btn_connect.Background = Brushes.White;
            cl_btn_connect.BorderBrush = Brushes.Black;
            cl_lbl_nick.Foreground = Brushes.Black;
            cbx_nightmode.Foreground = Brushes.Black;


            srv_lbx_users.Background = Brushes.White;
            srv_lbx_users.Foreground = Brushes.Black;

            srv_tbx_dialhistorry.Background = Brushes.White;
            srv_tbx_dialhistorry.Foreground = Brushes.Black;

            srv_tbx_dialinput.Background = Brushes.White;
            srv_tbx_dialinput.Foreground = Brushes.Black;

            srv_tbx_ttl.Background = Brushes.White;
            srv_tbx_ttl.Foreground = Brushes.Black;

            srv_tbx_port.Background = Brushes.White;
            srv_tbx_port.Foreground = Brushes.Black;

            srv_cbx_adress.Background = Brushes.White;
            srv_cbx_adress.Foreground = Brushes.Black;

            srv_btn_dialsend.Foreground = Brushes.Black;
            srv_btn_dialsend.Background = Brushes.White;
            srv_btn_dialsend.BorderBrush = Brushes.Black;

            srv_btn_start.Foreground = Brushes.Black;
            srv_btn_start.Background = Brushes.Green;
            srv_btn_start.BorderBrush = Brushes.Black;

            srv_btn_stop.Foreground = Brushes.Black;
            srv_btn_stop.Background = Brushes.Red;
            srv_btn_stop.BorderBrush = Brushes.Black;

            srv_btn_cleardial.Foreground = Brushes.Black;
            srv_btn_cleardial.Background = Brushes.White;
            srv_btn_cleardial.BorderBrush = Brushes.Black;

            srv_btn_pingall.Foreground = Brushes.Black;
            srv_btn_pingall.Background = Brushes.White;
            srv_btn_pingall.BorderBrush = Brushes.Black;

            srv_lbl_ttl.Foreground = Brushes.Black;
            srv_lbl_ip.Foreground = Brushes.Black;
            srv_lbl_port.Foreground = Brushes.Black;
            srv_commandinfo.Foreground = Brushes.Gray;


        }

        //#############################################################################
        //#############################################################################
        //#############################################################################
        //SERVER

        //ZMIENNE

        CSServer srv_server = null;
        private int srv_ttl = 0;

        bool srv_showerrors = false;

        //METODY

        public void srv_AddDial(string val, Color color)
        {
            if (val != "" && val != "\n")
            {
                try
                {
                    srv_tbx_dialhistorry.Dispatcher.Invoke(new Action(() =>
                    {
                        Run run = new Run(val);
                        run.Foreground = new SolidColorBrush(color);
                        Paragraph paragraph = new Paragraph(run);
                        paragraph.LineHeight = 1;
                        paragraph.FontSize = 12;
                        srv_tbx_dialhistorry.Document.Blocks.Add(paragraph);
                        srv_tbx_dialhistorry.ScrollToEnd();
                    }));
                }
                catch (Exception exc) { if (srv_showerrors) MessageBox.Show(exc.ToString()); }
            }
        }

        private void srv_ChangeStatusOfWorkServer(bool val)
        {
            if (val)
            {
                srv_btn_start.IsEnabled = false;
                srv_btn_stop.IsEnabled = true;
                srv_AddDial("#####SERVER UP#####", Colors.DarkMagenta);
                srv_cbx_adress.IsEnabled = false;
                srv_tbx_port.IsEnabled = false;
            }
            else
            {
                srv_btn_start.IsEnabled = true;
                srv_btn_stop.IsEnabled = false;
                srv_AddDial("#####SERVER DOWN#####", Colors.DarkMagenta);
                srv_cbx_adress.IsEnabled = true;
                srv_tbx_port.IsEnabled = true;
            }
        }

        //INTERAKCJA UŻYTKOWNIKA


        private void srv_Btn_dialsend_Click(object sender, RoutedEventArgs e)
        {
            string temp = srv_tbx_dialinput.Text;
            if (temp.Length == 5) //  /kick
                if (temp.Substring(0, 5) == "/kick")
                {
                    int id = srv_lbx_users.SelectedIndex;
                    srv_server.TryKickClient(id);
                    return;
                }
            if (temp.Length == 4) //  /ban
                if (temp.Substring(0, 4) == "/ban")
                {
                    int id = srv_lbx_users.SelectedIndex;
                    srv_server.TryBanClient(id);
                    return;
                }
            if (temp.Length > 6) //  /unban
                if (temp.Substring(0, 6) == "/unban")
                {
                    srv_server.UnbanClient(temp.Substring(7));
                    return;
                }
            if (temp.Length == 8) //  /showban
                if (temp.Substring(0, 8) == "/showban")
                {
                    srv_server.ShowBannedClients();
                    return;
                }

            temp = temp.Insert(0, "SERVER: ");
            srv_AddDial(temp, Colors.DarkCyan);
            if (srv_server.get_polaczenieaktywne()) srv_server.SendString(temp);

            srv_tbx_dialinput.Text = "";
        }

        private void srv_Btn_start_Click(object sender, RoutedEventArgs e)
        {
            srv_server.set_adresip(srv_cbx_adress.Text);
            srv_server.set_port(srv_tbx_port.Text);

            char x = srv_server.TryStartServer();
            if (x == 'S') srv_ChangeStatusOfWorkServer(true);
        }

        private void srv_Tbx_dialinput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) this.srv_Btn_dialsend_Click(sender, e);
        }

        private void srv_Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            char x = srv_server.TryStopServer();
            if (x == 'S') srv_ChangeStatusOfWorkServer(false);
        }

        private void srv_Btn_cleardial_Click(object sender, RoutedEventArgs e)
        {
            srv_tbx_dialhistorry.Document.Blocks.Clear();
        }

        private void srv_Btn_pingall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                srv_ttl = Int32.Parse(srv_tbx_ttl.Text);
                if (srv_ttl == 0)
                {
                    srv_ttl = 128;
                    srv_tbx_ttl.Text = srv_ttl + "";
                }
            }
            catch
            {
                srv_ttl = 128;
                srv_tbx_ttl.Text = srv_ttl + "";
            }

            string adresstemp = "";
            try
            {
                for (int clk = 0; clk < srv_lbx_users.Items.Count; clk++)
                {
                    if (srv_lbx_users.Items.GetItemAt(clk).ToString() != "")
                    {
                        adresstemp = srv_lbx_users.Items.GetItemAt(clk).ToString();
                        PingOptions opcje = new PingOptions();
                        opcje.Ttl = srv_ttl;
                        opcje.DontFragment = true;
                        string dane = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

                        byte[] bufor = Encoding.ASCII.GetBytes(dane);
                        int timeout = 120;
                        for (int i = 0; i < 5; i++)
                        {
                            srv_AddDial(srv_server.WyslijPing(adresstemp, timeout, bufor, opcje), Colors.Blue);
                        }
                        srv_AddDial("-----------------", Colors.DarkBlue);
                    }
                    else
                    {
                        MessageBox.Show("Nie wprowadzono żadnych adresów", "Błąd");
                    }
                }
            }
            catch (Exception exc)
            {
                srv_AddDial("Nie mozna pingowac z hostem " + adresstemp + ". Brak polaczenia!", Colors.DarkBlue);
                MessageBox.Show(exc.ToString());
            }
        }


        //#############################################################################
        //#############################################################################
        //#############################################################################
        //Client

        //ZMIENNE

        CSClient cl_client = null;
        string cl_clientname = "";

        bool cl_showerrors = false;


        //METODY


        public void cl_AddDial(string val, Color color)
        {
            if (val != "" && val != "\n")
            {
                try
                {
                    cl_tbx_dialhistorry.Dispatcher.Invoke(new Action(() =>
                    {
                        Run run = new Run(val);
                        run.Foreground = new SolidColorBrush(color);
                        Paragraph paragraph = new Paragraph(run);
                        paragraph.LineHeight = 1;
                        paragraph.FontSize = 12;
                        cl_tbx_dialhistorry.Document.Blocks.Add(paragraph);
                        cl_tbx_dialhistorry.ScrollToEnd();
                    }));
                }
                catch (Exception exc) { if (cl_showerrors) MessageBox.Show(exc.ToString()); }
            }
        }

        private void cl_ChangeStatusOfConnection(bool val)
        {
            if (val)
            {
                cl_client.set_polaczenieaktywne(true);
                cl_btn_connect.Dispatcher.Invoke(new Action(() => { cl_btn_connect.IsEnabled = false; }));
                cl_btn_disconnect.Dispatcher.Invoke(new Action(() => { cl_btn_disconnect.IsEnabled = true; }));
                cl_btn_send.Dispatcher.Invoke(new Action(() => { cl_btn_send.IsEnabled = true; }));
                cl_tbx_ip.Dispatcher.Invoke(new Action(() => { cl_tbx_ip.IsEnabled = false; }));
                cl_tbx_port.Dispatcher.Invoke(new Action(() => { cl_tbx_port.IsEnabled = false; }));
                cl_tbx_nick.Dispatcher.Invoke(new Action(() => { cl_tbx_nick.IsEnabled = true; }));
                cl_btn_changename.Dispatcher.Invoke(new Action(() => { cl_btn_changename.IsEnabled = true; }));

            }
            else
            {
                cl_client.set_polaczenieaktywne(false);
                cl_btn_connect.Dispatcher.Invoke(new Action(() => { cl_btn_connect.IsEnabled = true; }));
                cl_btn_disconnect.Dispatcher.Invoke(new Action(() => { cl_btn_disconnect.IsEnabled = false; }));
                cl_btn_send.Dispatcher.Invoke(new Action(() => { cl_btn_send.IsEnabled = false; }));
                cl_tbx_ip.Dispatcher.Invoke(new Action(() => { cl_tbx_ip.IsEnabled = true; }));
                cl_tbx_port.Dispatcher.Invoke(new Action(() => { cl_tbx_port.IsEnabled = true; }));
                cl_tbx_nick.Dispatcher.Invoke(new Action(() => { cl_tbx_nick.IsEnabled = false; }));
                cl_btn_changename.Dispatcher.Invoke(new Action(() => { cl_btn_changename.IsEnabled = false; }));
            }
        }







        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //INTERAKCJE UŻYTKOWNIKA

        private void cl_Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            cl_client.TryConnect();
        }

        private void cl_Btn_disconnect_Click(object sender, RoutedEventArgs e)
        {
            cl_client.TryDisconnect();
        }

        private void cl_Btn_send_Click(object sender, RoutedEventArgs e)
        {
            if (!cl_client.get_client_isnull())
            {
                if (cl_clientname != "") cl_client.SendString(cl_clientname + ": " + cl_tbx_command.Text);
                cl_tbx_command.Text = "";
            }
        }

        private void cl_Tbx_command_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && cl_btn_send.IsEnabled == true) this.cl_Btn_send_Click(sender, e);
        }



        private void cl_Btn_ping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cl_tbx_ip.Text != "")
                {
                    PingOptions opcje = new PingOptions();
                    opcje.Ttl = 128;
                    opcje.DontFragment = true;
                    string dane = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

                    byte[] bufor = Encoding.ASCII.GetBytes(dane);
                    int timeout = 120;
                    for (int i = 0; i < 5; i++)
                    {
                        cl_AddDial(cl_client.WyslijPing(cl_tbx_ip.Text, timeout, bufor, opcje), Colors.Blue);
                    }
                }
                else
                {
                    if (cl_showerrors) MessageBox.Show("Nie wprowadzono żadnych adresów", "Błąd");
                }
            }
            catch (Exception exc)
            {
                cl_AddDial("Nie mozna pingowac z hostem " + cl_tbx_ip.Text + ". Brak polaczenia!", Colors.Red);
                if (cl_showerrors) MessageBox.Show(exc.ToString());
            }
        }

        private void cl_Btn_clearchat_Click(object sender, RoutedEventArgs e)
        {
            cl_tbx_dialhistorry.Document.Blocks.Clear();
        }

        private void cl_Tbx_nick_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cl_Btn_changename_Click(object sender, RoutedEventArgs e)
        {
            bool zmieniono = false;
            string oldnick = "";
            if ((cl_tbx_nick.Text != "SERVER") && (cl_tbx_nick.Text != "SERVER MESSAGE"))
            {
                oldnick = cl_clientname;
                cl_clientname = cl_tbx_nick.Text;
                zmieniono = true;
            }
            else
            {
                cl_tbx_nick.Text = "Client";
                cl_clientname = cl_tbx_nick.Text;
                zmieniono = true;
            }

            if (zmieniono && cl_client != null)
            {
                cl_client.SendString("SERVER MESSAGE: " + oldnick + " zmienił nazwę na " + cl_clientname);
            }
        }


    }

}

