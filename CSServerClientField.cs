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

namespace ClientServerChat
{
    class CSServerClientField
    {
        public TcpClient client = null;
        public BinaryReader reader = null;
        public BinaryWriter writer = null;
        public NetworkStream stream = null;

        public CSServerClientField(TcpClient tc, BinaryReader br, BinaryWriter bw, NetworkStream ns)
        {
            client = tc;
            reader = br;
            writer = bw;
            stream = ns;
        }

        public CSServerClientField()
        {
            client = null;
            reader = null;
            writer = null;
            stream = null;
        }

        public void Initialization(TcpListener serv)
        {
            this.NewTcpClient();
            this.TcpClientAcceptTcpClient(serv);
            this.NetworkStreamGetFromTcpClient();
            this.NewBinaryReader();
            this.NewBinaryWriter();
        }

        public void NewBinaryReader()
        {
            reader = new BinaryReader(stream);
        }

        public void NewBinaryWriter()
        {
            writer = new BinaryWriter(stream);
        }

        public void NewTcpClient()
        {
            client = new TcpClient();
        }

        public void TcpClientAcceptTcpClient(TcpListener serv)
        {
            client = serv.AcceptTcpClient();
        }

        public void NetworkStreamGetFromTcpClient()
        {
            stream = client.GetStream();
        }

        public void CloseAll()
        {
            this.reader.Close();
            this.reader.Dispose();
            this.writer.Close();
            this.writer.Dispose();
            this.stream.Close();
            this.stream.Dispose();
            this.client.Close();
            this.client.Dispose();

            this.client = null;
            this.reader = null;
            this.writer = null;
            this.stream = null;
        }



    }
}