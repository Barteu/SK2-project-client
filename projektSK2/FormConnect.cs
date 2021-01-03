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

        public FormConnect()
        {
            
            this.obj = this;
            /* create a socket */
            socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InitializeComponent();

        }

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


        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                /// Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(this.textBoxPort.Text.ToString()));
                // powyzej powinno sie wybrac addresses[1] bo [0] to IPv6

                setThreadedStatusLabel("Wait! Connecting...");
                
                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), this.socketFd);

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
            
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

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
              
            }
        }

        private void buttonConnect_Click_1(object sender, EventArgs e)
        {
            try
            {
                //setThreadedButton(false);
                //setThreadedTextBox("");
               // setThreadedStatusLabel("Wait! DNS query...");

                if (this.textBoxAddr.Text.Length > 0 && this.textBoxPort.Text.Length > 0)
                {
                    /* get DNS host information */
                    Dns.BeginGetHostByName(this.textBoxAddr.Text.ToString(), new AsyncCallback(GetHostEntryCallback), null);

                    MessageBox.Show("Connecting...");

                    if (this.toolStripStatusLabel1.Text == "Connected to the server")
                    {   
                        this.Hide();
                        Form1 f1 = new Form1(this.socketFd);
                        f1.Show();
                    }
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



    }
 
}
