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
        private bool logged;

        Socket socketFd;

        private FormLogIn loginForm;

        public Form1(Socket socketFd)
        {
            
            InitializeComponent();
            this.obj = this;
            
            this.socketFd = socketFd;
            logged = false;
            loginForm = new FormLogIn(socketFd);
            openChildFormLogin(loginForm);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private Form activeForm = null;
        private void openChildFormLogin(FormLogIn childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.LoginStateChanged += loginForm_LoginStateChanged;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

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

            }
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            socketFd.BeginSend(Encoding.ASCII.GetBytes("o;"), 0, "o;".Length, 0, new AsyncCallback(SendCallback), state);
            this.buttonLogOut.Visible = false;
            this.buttonTopcics.Visible = false;
            this.buttonSend.Visible = false;
            this.buttonRecieve.Visible = false;
            loginForm = new FormLogIn(socketFd);
            openChildFormLogin(loginForm);
            
        }

        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {
     
        }


        void loginForm_LoginStateChanged(bool newLoginState)
        {
            if(newLoginState==true)
            {
                this.buttonLogOut.Visible = true;
                this.buttonTopcics.Visible = true;
                this.buttonSend.Visible = true;
                this.buttonRecieve.Visible = true;
            }
            else
            {
                this.buttonLogOut.Visible = false;
                this.buttonTopcics.Visible = false;
                this.buttonSend.Visible = false;
                this.buttonRecieve.Visible = false;

            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            socketFd.BeginSend(Encoding.ASCII.GetBytes("?;"), 0, "?;".Length, 0, new AsyncCallback(SendCallback), state);
            /* shutdown and close socket */
            socketFd.Shutdown(SocketShutdown.Both);
            socketFd.Close();
            this.Close();
            System.Environment.Exit(1);
        }

        private void buttonTopcics_Click(object sender, EventArgs e)
        {
            openChildForm(new Form2(this.socketFd));

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void buttonRecieve_Click(object sender, EventArgs e)
        {
            openChildForm(new FormRecieve(this.socketFd));
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
