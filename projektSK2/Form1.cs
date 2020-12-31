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
    public partial class Form1 : Form
    {
        private Form obj;
        delegate void setThreadedTextBoxCallback(String text);
        delegate void setThreadedStatusLabelCallback(String text);
        delegate void setThreadedButtonCallback(bool status);

        public Form1()
        {
            InitializeComponent();
            this.obj = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void setThreadedTextBox(String text)
        {
            if (this.textBoxReturnMsg.InvokeRequired)
            {
                setThreadedTextBoxCallback textBoxCallback = new setThreadedTextBoxCallback(setThreadedTextBox);
                this.obj.Invoke(textBoxCallback, text);
            }
            else
            {
                //text = text.Substring(0, text.Length - 2);
                this.textBoxReturnMsg.Text = text;
            }
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
            if (this.logInButton.InvokeRequired)
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, status);
            }
            else
            {
                this.logInButton.Enabled = status;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    /* all the data has arrived */
                    if (state.m_StringBuilder.Length > 1)
                    {
                        setThreadedTextBox(state.m_StringBuilder.ToString());
                        setThreadedStatusLabel("Done.");
                        setThreadedButton(true);

                        /* shutdown and close socket */
                        socketFd.Shutdown(SocketShutdown.Receive);
                        socketFd.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);
            }
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(this.textBoxPort.Text.ToString()));
                // powyzej powinno sie wybrac addresses[1] bo [0] to IPv6

                setThreadedStatusLabel("Wait! Connecting...");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
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

                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                setThreadedStatusLabel("Wait! Sending...");

                /* begin  SENDING the data */
                string loginPassword = textBoxLogin.Text.ToString() + ';' + textBoxPass.Text.ToString();
            

                socketFd.BeginSend(Encoding.ASCII.GetBytes(loginPassword), 0, loginPassword.Length, 0, new AsyncCallback(SendCallback), state);
                /* begin  receiving the data */
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);
            }
        }


        private void logInButton_Click(object sender, EventArgs e)
        {
            try
            {
                setThreadedButton(false);
                setThreadedTextBox("");
                setThreadedStatusLabel("Wait! DNS query...");

                if (this.textBoxAddr.Text.Length > 0 && this.textBoxPort.Text.Length > 0 && this.textBoxLogin.Text.Length > 0 && this.textBoxPass.Text.Length>0)
                {
                    /* get DNS host information */
                    Dns.BeginGetHostByName(this.textBoxAddr.Text.ToString(), new AsyncCallback(GetHostEntryCallback), null);
                }
                else
                {
                    if (this.textBoxAddr.Text.Length <= 0) MessageBox.Show("No server address!");
                    else if (this.textBoxPort.Text.Length <= 0) MessageBox.Show("No server port number!");
                    else if (this.textBoxLogin.Text.Length <= 0) MessageBox.Show("No login!");
                    else if (this.textBoxPass.Text.Length <= 0) MessageBox.Show("No password!");

                    setThreadedButton(true);
                    setThreadedStatusLabel("Check \"Server Info\" and try again!");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Check \"Server Info\" and try again!");
                setThreadedButton(true);
            }
        }

    }

    public class SocketStateObject
    {
        public const int BUF_SIZE = 1024;
        public byte[] m_DataBuf = new byte[BUF_SIZE];
        public StringBuilder m_StringBuilder = new StringBuilder();
        public Socket m_SocketFd = null;
    }


}
