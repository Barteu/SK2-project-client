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
        delegate void setThreadedButtonNextCallback(bool state);

        public FormRecieve(Socket socketFd)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFd;

            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            // po utworzeniu tego formularza, wysyła do serwera zapytanie o wiadomości
            socketFd.BeginSend(Encoding.ASCII.GetBytes("r|~"), 0, "r|~".Length, 0, new AsyncCallback(SendCallback), state);
        }

        // ustawia pole liczby wiadomości do odczytania
        private void setThreadedTextToRead(String text)
        {
            if (this.textToRead.InvokeRequired)
            {
                setThreadedTextToReadCallback textBoxCallback = new setThreadedTextToReadCallback(setThreadedTextToRead);
                this.obj.Invoke(textBoxCallback, text);
            }
            else
            {
                this.textToRead.Text = text;
            }
        }

        // ustawia tekst w polu kolejki
        private void setThreadedTextTopic(String text)
        {
            if (this.textTopic.InvokeRequired)
            {
                setThreadedTextTopicCallback textBoxTopicCallback = new setThreadedTextTopicCallback(setThreadedTextTopic);
                this.obj.Invoke(textBoxTopicCallback, text);
            }
            else
            {
                this.textTopic.Text = text;
            }
        }

        // ustawia tekst w polu tematu wiadomości
        private void setThreadedTextTitle(String text)
        {
            if (this.textTitle.InvokeRequired)
            {
                setThreadedTextTitleCallback textBoxTitleCallback = new setThreadedTextTitleCallback(setThreadedTextTitle);
                this.obj.Invoke(textBoxTitleCallback, text);
            }
            else
            {
                this.textTitle.Text = text;
            }
        }


        //ustawia tekst w polu wiadomości
        private void setThreadedTextBoxText(String text)
        {
            if (this.textBoxText.InvokeRequired)
            {
                setThreadedTextBoxTextCallback textBoxTextCallback = new setThreadedTextBoxTextCallback(setThreadedTextBoxText);
                this.obj.Invoke(textBoxTextCallback, text);
            }
            else
            {
                this.textBoxText.Text = text;
            }
        }
        
        // uaktywnia/dezaktywuje przyisk odbioru następnej wiadomości
        private void setThreadedButtonNext(bool state)
        {
            if (this.buttonNext.InvokeRequired)
            {
                setThreadedButtonNextCallback buttonNextCallback = new setThreadedButtonNextCallback(setThreadedButtonNext);
                this.obj.Invoke(buttonNextCallback, state);
            }
            else
            {
                this.buttonNext.Enabled = state;
            }
        }


        // po wysłaniu prośby do serwera, odbiera wiadomość
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                state.m_StringBuilder.Clear();
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        // po odebraniu wiadomości, jej składowe zostają wyświetlone w odpowiednich polach formularza
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
                if (size > 0) 
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    // jeżeli ostatnim odebranym znakiem jest '~' to kończy odbieranie w przeciwnym przypadku odbiera reszte danych
                    if (state.m_StringBuilder.ToString().Contains("~")) 
                    {
                          
                            string[] message = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                            if (message[0] != "0")
                            {
                                setThreadedTextToRead(Convert.ToString(Convert.ToInt32(message[0]) - 1));
                                setThreadedTextTopic(message[1]);
                                setThreadedTextTitle(message[3]);
                                setThreadedTextBoxText(message[4]);

                                setThreadedButtonNext(true);
                            }
                            else 
                            {
                                MessageBox.Show("No messages to read");

                                setThreadedButtonNext(true);
                            }
    
                    }
                    else // jezeli nie odczytano calej wiadomosci
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }
                else
                {
                    /* all the data has arrived */
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
                        string[] message = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                        if (message[0] != "0")
                        {
                            setThreadedTextToRead(Convert.ToString(Convert.ToInt32(message[0]) - 1));
                            setThreadedTextTopic(message[1]);
                            setThreadedTextTitle(message[3]);
                            setThreadedTextBoxText(message[4]);

                            setThreadedButtonNext(true);
                        }
                        else
                        {
                            MessageBox.Show("No messages to read");
                            setThreadedButtonNext(true);
                        }

                    }
                }


                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        // wysyła prośbe o kolejną wiadomość do servera
        private void buttonNext_Click(object sender, EventArgs e)
        {

            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            setThreadedButtonNext(false);
            socketFd.BeginSend(Encoding.ASCII.GetBytes("r|~"), 0, "r|~".Length, 0, new AsyncCallback(SendCallback), state);
    

        }
    }
}
