using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace projektSK2
{
    public partial class FormConnect : Form
    {
        Socket socketFd;
        private Form obj;
        protected bool connected;

        delegate void setThreadedStatusLabelCallback(String text);
        delegate void setThreadedButtonCallback(bool status);
        delegate void setThreadedButtonOpenWindowCallback(bool status);

        public FormConnect()
        {
            this.obj = this;
            /* create a socket */
            socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InitializeComponent();
        }
       
        // ustawia tekst w polu statusu
        private void setThreadedStatusLabel(String text)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                setThreadedStatusLabelCallback statusLabelCallback = new setThreadedStatusLabelCallback(setThreadedStatusLabel);
                this.obj.Invoke(statusLabelCallback, text);
            }
            else
            {
                this.toolStripStatusLabel1.Text = text;

            }
        }

        // uaktywnia/dezaktywuje przycisk nawiązania połączenia
        private void setThreadedButton(bool status)
        {
            if (this.buttonConnect.InvokeRequired)
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, status);
            }
            else
            {
                this.buttonConnect.Enabled = status;
            }
        }

        // uaktywnia/dezaktywuje przycisk uruchomienia właściwego okna klienta
        private void setThreadedButtonOpenWindow(bool status)
        {
            if (this.buttonOpenWindow.InvokeRequired)
            {
                setThreadedButtonOpenWindowCallback buttonOpenWindowCallback = new setThreadedButtonOpenWindowCallback(setThreadedButtonOpenWindow);
                this.obj.Invoke(buttonOpenWindowCallback, status);
            }
            else
            {
                this.buttonOpenWindow.Enabled = status;
            }
        }

     
        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(this.textBoxPort.Text.ToString()));
                // powyzej  addresses[1] to IPv4, [0] to IPv6

                setThreadedStatusLabel("Wait! Connecting...");
                
                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), this.socketFd);
                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);

            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the state object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);
                setThreadedStatusLabel("Connected to the server");

                setThreadedButton(false);
                setThreadedButtonOpenWindow(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);

            }
        }

        // ustala połączenie z serwerem
        private void buttonConnect_Click_1(object sender, EventArgs e)
        {
            try
            {

                if (this.textBoxAddr.Text.Length > 0 && this.textBoxPort.Text.Length > 0)
                {
                    /* get DNS host information */
                    setThreadedButton(false);
                    Dns.BeginGetHostByName(this.textBoxAddr.Text.ToString(), new AsyncCallback(GetHostEntryCallback), null);
                }
                else
                {
                    if (this.textBoxAddr.Text.Length <= 0) MessageBox.Show("No server address!");
                    else if (this.textBoxPort.Text.Length <= 0) MessageBox.Show("No server port number!");
                    //setThreadedButton(true);
                    setThreadedStatusLabel("Check \"Server Info\" and try again!");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }

        // uruchamia główne okno klienta
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1(this.socketFd);
            f1.Show();
        }

        // po wysyłce serwerowi informacji o zamknięciu klienta zamyka gniazda i wyłącza aplikacje
        private void SendExitCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                /* shutdown and close socket */
                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();
                System.Environment.Exit(1);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        // jeżeli ustanowiono połączenie z serwerem a klient jest zamykany to wcześniej wysyła tą informacje serwerowi
        private void FormConnect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.buttonOpenWindow.Enabled)
            {
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;
                socketFd.BeginSend(Encoding.ASCII.GetBytes("?|~"), 0, "?|~".Length, 0, new AsyncCallback(SendExitCallback), state);
            }

        }
    }
 
}
