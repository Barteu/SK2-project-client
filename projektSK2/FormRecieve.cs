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
  
    
    public partial class FormRecieve : Form
    {
        Socket socketFd;
        private Form obj;

        delegate void setThreadedTextToReadCallback(String text);
        delegate void setThreadedTextTopicCallback(String text);
        delegate void setThreadedTextTitleCallback(String text);
        delegate void setThreadedTextBoxTextCallback(String text);

        public FormRecieve(Socket socketFd)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFd;

            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            socketFd.BeginSend(Encoding.ASCII.GetBytes("r;"), 0, "r;".Length, 0, new AsyncCallback(SendCallback), state);

        }

        
        private void setThreadedTextToRead(String text)
        {
            if (this.textToRead.InvokeRequired)
            {
                setThreadedTextToReadCallback textBoxCallback = new setThreadedTextToReadCallback(setThreadedTextToRead);
                this.obj.Invoke(textBoxCallback, text);
            }
            else
            {
                //text = text.Substring(0, text.Length - 2);
                this.textToRead.Text = text;
            }
        }

        private void setThreadedTextTopic(String text)
        {
            if (this.textTopic.InvokeRequired)
            {
                setThreadedTextTopicCallback textBoxTopicCallback = new setThreadedTextTopicCallback(setThreadedTextTopic);
                this.obj.Invoke(textBoxTopicCallback, text);
            }
            else
            {
                //text = text.Substring(0, text.Length - 2);
                this.textTopic.Text = text;
            }
        }

        private void setThreadedTextTitle(String text)
        {
            if (this.textTitle.InvokeRequired)
            {
                setThreadedTextTitleCallback textBoxTitleCallback = new setThreadedTextTitleCallback(setThreadedTextTitle);
                this.obj.Invoke(textBoxTitleCallback, text);
            }
            else
            {
                //text = text.Substring(0, text.Length - 2);
                this.textTitle.Text = text;
            }
        }



        private void setThreadedTextBoxText(String text)
        {
            if (this.textBoxText.InvokeRequired)
            {
                setThreadedTextBoxTextCallback textBoxTextCallback = new setThreadedTextBoxTextCallback(setThreadedTextBoxText);
                this.obj.Invoke(textBoxTextCallback, text);
            }
            else
            {
                //text = text.Substring(0, text.Length - 2);
                this.textBoxText.Text = text;
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

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

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
                    string[] message= state.m_StringBuilder.ToString().Split('|');

                    if(message[0]!="0")
                    {
                        setThreadedTextToRead(Convert.ToString(Convert.ToInt32(message[0])-1));
                        setThreadedTextTopic(message[1]);
                        setThreadedTextTitle(message[3]);
                        setThreadedTextBoxText(message[4]);
                    }
                    else
                    {
                        MessageBox.Show("No messages to read");
                    }
       
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {

            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            socketFd.BeginSend(Encoding.ASCII.GetBytes("r;"), 0, "r;".Length, 0, new AsyncCallback(SendCallback), state);
            socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);

        }
    }
}
