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

        delegate void setThreadedButtonCallback(Button button, bool state);

        Socket socketFd;

        private FormLogIn loginForm;
        
        // formularz aktualnie wyswietlany
        private Form activeForm = null;
        
        public Form1(Socket socketFd)
        {      
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFd;
            loginForm = new FormLogIn(socketFd);
            openChildFormLogin(loginForm);
        }

        //ustawia widoczność danego przycisku
        private void setThreadedButton(Button  button, bool state)
        { 
            if (button.InvokeRequired)
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, new object[] { button},new object[] { state });
            }
            else
            {
                button.Visible = state;
            }
        }


        // otwiera na środku formularz logowania
        private void openChildFormLogin(FormLogIn childForm)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
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

        // otwiera w przestrzeni roboczej dany formularz
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }  
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

        }

        // callback wysyłki wiadomości do servera
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

        // zamyka gniazda i wylacza aplikacje
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

        // wysyla informacje o wylogowaniu do servera
        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            setThreadedButton(this.buttonLogOut, false);
            setThreadedButton(this.buttonTopcics, false);
            setThreadedButton(this.buttonSend, false);
            setThreadedButton(this.buttonReceive, false);

            socketFd.BeginSend(Encoding.ASCII.GetBytes("o|~"), 0, "o|~".Length, 0, new AsyncCallback(SendCallback), state);

            loginForm = new FormLogIn(socketFd);
            openChildFormLogin(loginForm);
        }

        // reaguje na zmiane stanu zalogowania uktywając bądź pokazując przyciski
        void loginForm_LoginStateChanged(bool newLoginState)
        {
            if(newLoginState==true)
            {
                setThreadedButton(this.buttonLogOut, true);
                setThreadedButton(this.buttonTopcics, true);
                setThreadedButton(this.buttonSend, true);
                setThreadedButton(this.buttonReceive, true);
            }
            else
            {
                setThreadedButton(this.buttonLogOut, false);
                setThreadedButton(this.buttonTopcics, false);
                setThreadedButton(this.buttonSend, false);
                setThreadedButton(this.buttonReceive, false);
            }
        }

        // poniższe funkcje otwierają poszczególne formularze w głównej przestrzeni roboczej
        private void buttonExit_Click(object sender, EventArgs e)
        {
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            socketFd.BeginSend(Encoding.ASCII.GetBytes("?|~"), 0, "?|~".Length, 0, new AsyncCallback(SendExitCallback), state);
        
        }

        private void buttonTopcics_Click(object sender, EventArgs e)
        {
            openChildForm(new FormTopics(this.socketFd));

        }

        private void buttonRecieve_Click(object sender, EventArgs e)
        {
            openChildForm(new FormReceive(this.socketFd));
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;
                socketFd.BeginSend(Encoding.ASCII.GetBytes("?|~"), 0, "?|~".Length, 0, new AsyncCallback(SendExitCallback), state);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openChildForm(new FormSendMsg(this.socketFd));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {
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
