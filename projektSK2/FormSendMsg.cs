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
    public partial class FormSendMsg : Form
    {

        Socket socketFd;
        private Form obj;

        delegate void setThreadedComboBoxTopicCallback(List<Topic> listOfTopics);
        delegate void setThreadedTextBoxTitleCallback(String text);
        delegate void setThreadedTextBoxTextCallback(String text);
        delegate void setThreadedButtonSendCallback(bool state);

        protected List<Topic> topics;

       // po otworzeniu formularza wysylki wiadomości, wysyłane jest zapytanie o liste subskrybowanych przez użytkownika tematów
        public FormSendMsg(Socket socketFD)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFD;
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            socketFd.BeginSend(Encoding.ASCII.GetBytes("m|~"), 0, "m|~".Length, 0, new AsyncCallback(SendMyTopicsCallback), state);
        }

        // ustawia liste subskrybowanych tematów w ComboBoxie
        private void setThreadedComboBoxTopic(List<Topic> listOfTopics)
        {
            if (this.comboBoxTopic.InvokeRequired)
            {
                setThreadedComboBoxTopicCallback ComboBoxTopicCallback = new setThreadedComboBoxTopicCallback(setThreadedComboBoxTopic);
                this.obj.Invoke(ComboBoxTopicCallback, new object[] { listOfTopics });
            }
            else
            {
                this.comboBoxTopic.Items.Clear();
                for( int i=0; i< listOfTopics.Count;i++)
                {
                    this.comboBoxTopic.Items.Add(listOfTopics[i].Name);
                }
            }
        }
        
        // ustawia pole tytulu wiadomosci
        private void setThreadedTextBoxTitle(String text)
        {
            if (this.textTitle.InvokeRequired)
            {
                setThreadedTextBoxTitleCallback CTextBoxTitleCallback = new setThreadedTextBoxTitleCallback(setThreadedTextBoxTitle);
                this.obj.Invoke(CTextBoxTitleCallback, text );
            }
            else
            {
                this.textTitle.Text = text;
            }
        }
        // ustawia tekst pola wiadomości
        private void setThreadedTextBoxText(String text)
        {
            if (this.textBoxText.InvokeRequired)
            {
                setThreadedTextBoxTextCallback TextBoxTexteCallback = new setThreadedTextBoxTextCallback(setThreadedTextBoxText);
                this.obj.Invoke(TextBoxTexteCallback,  text );
            }
            else
            {
                this.textBoxText.Text = text;
            }
        }

        // uaktywnia/dezaktywuje przycisk do wysylki wiadomosci
        private void setThreadedButtonSend(bool state)
        {
            if (this.buttonSend.InvokeRequired)
            {
                setThreadedButtonSendCallback buttonSendCallback = new setThreadedButtonSendCallback(setThreadedButtonSend);
                this.obj.Invoke(buttonSendCallback, state);
            }
            else
            {
                this.buttonSend.Enabled = state;
            }
        }

    
        // po wysłaniu zapytania o liste subskrybcji odbiera ją
        private void SendMyTopicsCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                state.m_StringBuilder.Clear();

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveMySubsCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        // po odbiorze subskrybcji ustawia je w comboBoxie
        private void ReceiveMySubsCallback(IAsyncResult ar)
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
           
                            string[] topics_ = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                            topics = new List<Topic>();


                            string[] topicNames = new string[topics_.Length / 2];

                            for (int i = 0; i < topics_.Length; i++)
                            {
                                if (i % 2 == 0 && topics_[i].Length > 0)
                                {
                                    topics.Add(new Topic() { ID = topics_[i + 1], Name = topics_[i] });

                                }
                            }
                            setThreadedComboBoxTopic(topics);
                        
                    }
                    else// jezeli nie odczytano calej wiadomosc
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveMySubsCallback), state);
                    }
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {

                            string[] topics_ = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                            topics = new List<Topic>();


                            string[] topicNames = new string[topics_.Length / 2];

                            for (int i = 0; i < topics_.Length; i++)
                            {
                                if (i % 2 == 0 && topics_[i].Length > 0)
                                {
                                    topics.Add(new Topic() { ID = topics_[i + 1], Name = topics_[i] });

                                }
                            }
                            setThreadedComboBoxTopic(topics);
                        
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        // po próbie wysłania wiadomości odbiera potwierdzenie od serwera
        private void SendMsgCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                state.m_StringBuilder.Clear();

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveInfoCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        //wyświetla potwierdzenie wysłania wiadomości po odebraniu informacji od serwera
        private void ReceiveInfoCallback(IAsyncResult ar)
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

                    if (state.m_StringBuilder.ToString().Contains("~"))
                    {
    
                            MessageBox.Show(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
                            if (state.m_StringBuilder.ToString().Contains("Message sent"))
                            {
                                setThreadedTextBoxTitle(" ");
                                setThreadedTextBoxText(" ");
                            }
                            setThreadedButtonSend(true);
                    }
                    else
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveInfoCallback), state);
                    }
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
                        MessageBox.Show(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
                        if (state.m_StringBuilder.ToString().Contains("Message sent"))
                        {
                            setThreadedTextBoxTitle(" ");
                            setThreadedTextBoxText(" ");
                        }
                        setThreadedButtonSend(true);
                    }

                }

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }


        // przygotowuje (głównie konkatenuje) i wysyła wiadomość
        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                setThreadedButtonSend(false);
                if (this.comboBoxTopic.SelectedItem!=null)
                {
                    if ((this.topics.Find(x => x.Name.Contains(this.comboBoxTopic.SelectedItem.ToString())).ID.Length > 0) && this.comboBoxTopic.Text.Length > 2 && this.textBoxText.Text.Length > 2)
                    {
                        
                        string msgToSend = "e|";
                        msgToSend += this.topics.Find(x => x.Name.Contains(this.comboBoxTopic.SelectedItem.ToString())).ID;
                        msgToSend += "|";
                        msgToSend += this.textTitle.Text.Replace('|', ' ').Replace('~', ' ');
                        msgToSend += "|";
                        msgToSend += this.textBoxText.Text.Replace('|', ' ').Replace('~', ' ');
                        msgToSend += "~";
                        SocketStateObject state = new SocketStateObject();
                        state.m_SocketFd = this.socketFd;
                        socketFd.BeginSend(Encoding.ASCII.GetBytes(msgToSend), 0, msgToSend.Length, 0, new AsyncCallback(SendMsgCallback), state);
                    }
                    else
                    {
                        MessageBox.Show("Please fill in all fields");
                        setThreadedButtonSend(true);
                    }
                }
                else
                {
                    MessageBox.Show("Choose a topic!");
                    setThreadedButtonSend(true);
                }
                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }


        }


        private void FormSendMsg_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }
    }

    // klasa do przechowania ID i nazwy wiadomości
    public class Topic
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
