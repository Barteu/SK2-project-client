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

        public event LoginStateChangeHandler LoginStateChanged;
        public delegate void LoginStateChangeHandler(bool newLoginState);

        Socket socketFd;
   

        public FormLogIn(Socket socketFd)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFd;
           
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
                this.textBoxReturnMsg.Text = text;
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

                /* begin  receiving the data */
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
           
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

                state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                /* all the data has arrived */
                if (state.m_StringBuilder.Length > 1)
                {
                    setThreadedTextBox(state.m_StringBuilder.ToString());
               
                    setThreadedButton(true);
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
             
                setThreadedButton(true);
            }
        }


        private void logInButton_Click(object sender, EventArgs e)
        {
            try
            {
                setThreadedButton(false);
           
                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;


                if (this.textBoxLogin.Text.Length > 0 && this.textBoxPass.Text.Length > 0)
                {
                   
                    /* begin  SENDING the data */
                    string loginPassword = textBoxLogin.Text.ToString() + '|' + textBoxPass.Text.ToString();

                    socketFd.BeginSend(Encoding.ASCII.GetBytes(loginPassword), 0, loginPassword.Length, 0, new AsyncCallback(SendCallback), state);

                    Application.DoEvents();
                    
                    if (this.textBoxReturnMsg.Text=="Poprawnie zalogowano")
                    {
                        this.LoginStateChanged(true);
                        this.Close();
                    }
                }
                else
                {

                    if (this.textBoxLogin.Text.Length <= 0) MessageBox.Show("No login!");
                    else if (this.textBoxPass.Text.Length <= 0) MessageBox.Show("No password!");

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
