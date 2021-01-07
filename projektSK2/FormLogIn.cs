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
    public partial class FormLogIn : Form
    {


        private Form obj;
        delegate void setThreadedTextBoxCallback(String text);
        delegate void setThreadedStatusLabelCallback(String text);
        delegate void setThreadedButtonCallback(bool status);

        //reakcja na zmiane statusu zalogowania
        public event LoginStateChangeHandler LoginStateChanged;
        public delegate void LoginStateChangeHandler(bool newLoginState);

        Socket socketFd;
   

        public FormLogIn(Socket socketFd)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFd;
        }

        // ustawia wiadomość o poprawności zalogowania
        private void setThreadedTextBox(String text)
        {
            if (this.textBoxReturnMsg.InvokeRequired)
            {
                setThreadedTextBoxCallback textBoxCallback = new setThreadedTextBoxCallback(setThreadedTextBox);
                this.obj.Invoke(textBoxCallback, text);
            }
            else
            {
                this.textBoxReturnMsg.Text = text;
            }
        }

        // uaktywnia/dezaktywuje przycisk do zalogowania
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

        // po wysłaniu danych logowania odbiera od serwera decyzje o zalogowaniu
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                /* begin  receiving the data */
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
           
                setThreadedButton(true);
            }
        }

        // odbiór decyzji o zalogowaniu od serwera
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;
               
                /* read data */
                int size = socketFd.EndReceive(ar);

                // jeżeli coś odebrano może nastąpić próba dalszego odbioru
                if ( size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    // jeżeli ostatnim odebranym znakiem jest '~' to kończy odbieranie w przeciwnym przypadku odbiera reszte danych
                    if (state.m_StringBuilder.ToString().Contains("~")) 
                    {      
                        setThreadedTextBox(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
                        setThreadedButton(true);
                    }
                    else
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }
                else 
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
                        setThreadedTextBox(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
                        setThreadedButton(true);
                    } 
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
             
                setThreadedButton(true);
            }
        }




        //wysyła dane logowania
        private void logInButton_Click(object sender, EventArgs e)
        {
            try
            {
                setThreadedButton(false);
           
                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;


                if (this.textBoxLogin.Text.Length > 2 && this.textBoxPass.Text.Length > 2)
                {
                   
                    /* begin  SENDING the data */
                    string loginPassword = textBoxLogin.Text.ToString().Replace('|', ' ') + '|' + textBoxPass.Text.ToString().Replace('|', ' ')+'~';

                    socketFd.BeginSend(Encoding.ASCII.GetBytes(loginPassword), 0, loginPassword.Length, 0, new AsyncCallback(SendCallback), state);

                    Application.DoEvents();
                    
                    if (this.textBoxReturnMsg.Text.Contains("Logged in"))
                    {
                        this.LoginStateChanged(true);
                        this.Close();
                    }
                }
                else
                {

                    if (this.textBoxLogin.Text.Length <= 2) MessageBox.Show("Login is too short");
                    else if (this.textBoxPass.Text.Length <= 2) MessageBox.Show("Password is too short!");

                    setThreadedButton(true);
                 
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
             
                setThreadedButton(true);
            }
        }

        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
        }
    }






}
