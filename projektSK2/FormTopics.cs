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
    public partial class FormTopics : Form
    {
        Socket socketFd;
        private Form obj;
       

        delegate void setThreadedTextBoxCallback(String text);
        delegate void setThreadedCheckedListBoxCallback(string[] text);
        delegate void setThreadedCheckedListBoxBoxesCallback(string[] text);
        delegate void setThreadedTextBoxTopicCallback(string text);
        delegate void setThreadedButtonAddTopicCallback(bool state);

        // po otworzeniu formularza tematow wysylane jest zapytanie o liste wszystkich tematow
        public FormTopics(Socket socketFD)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFD;
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            socketFd.BeginSend(Encoding.ASCII.GetBytes("t|~"), 0, "t|~".Length, 0, new AsyncCallback(DoubleSendCallback), state);
        }
        
        // ustawia liste tematow na liscie
        private void setThreadedCheckedListBox(string [] text)
        {
            if (this.checkedListBox1.InvokeRequired)
            {
                setThreadedCheckedListBoxCallback CheckedListBoxCallback = new setThreadedCheckedListBoxCallback(setThreadedCheckedListBox);
                this.obj.Invoke(CheckedListBoxCallback, new object[] { text });
            }
            else
            {
                this.checkedListBox1.Items.Clear();
                if(text.Length>0)
                {
                    this.checkedListBox1.Items.AddRange(text);
                }
            }
        }

        // ustawia checkBoxy na liscie tematow zaznaczajac subskrybcje
        private void setThreadedCheckedListBoxBoxes(string[] text)
        {
            if (this.checkedListBox1.InvokeRequired)
            {
                setThreadedCheckedListBoxBoxesCallback CheckedListBoxBoxesCallback = new setThreadedCheckedListBoxBoxesCallback(setThreadedCheckedListBoxBoxes);
                this.obj.Invoke(CheckedListBoxBoxesCallback, new object[] { text });
            }
            else
            {

                for(int i=0; i<this.checkedListBox1.Items.Count;i++)
                {
                    if (text.Contains(this.checkedListBox1.Items[i].ToString()))
                    {
                        this.checkedListBox1.SetItemChecked(i, true);

                    }
                }
            }
        }

        // ustawia pole tekstowe nazwy tematu do dodania
        private void setThreadedTextBoxTopic(string text)
        {
            if (this.textBoxTopic.InvokeRequired)
            {
                setThreadedTextBoxTopicCallback TextBoxTopicCallback = new setThreadedTextBoxTopicCallback(setThreadedTextBoxTopic);
                this.obj.Invoke(TextBoxTopicCallback, new object[] { text });
            }
            else
            {
                textBoxTopic.Text = text;
            }
        }

        // aktywuje/dezaktywuje przycisk dodania tematu
        private void setThreadedButtonAddTopic(bool state)
        {
            if (this.buttonAddTopic.InvokeRequired)
            {
                setThreadedButtonAddTopicCallback buttonAddTopicCallback = new setThreadedButtonAddTopicCallback(setThreadedButtonAddTopic);
                this.obj.Invoke(buttonAddTopicCallback, new object[] { state });
            }
            else
            {
                buttonAddTopic.Enabled = state;
            }
        }
        
        // po wyslaniu tematu do dodania, rozpoczyna odbiór informacji zwrotnej oraz listy wszystkich tematów
        private void SendTopicCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);
                setThreadedTextBoxTopic("");
                state.m_StringBuilder.Clear();
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveFullInfoCallback), state);

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        // "double" w nazwie oznacza, że wysłane zostaną dwa zapytania do serwera: o wszystkie oraz subskrybowane tematy
        private void DoubleSendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                state.m_StringBuilder.Clear();
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveTopicsCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        // po wyslaniu prosby o liste subskrybcji nastepuje jej odbior
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

        //po wysylce informacji o zasubskrybowaniu tematu, nastepuje odbior informacji zwrotnej
        private void SendSubCallback(IAsyncResult ar)
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

        // po odbiorze listy tematow nastepuje prosba o liste subskrybcji
        private void ReceiveTopicsCallback(IAsyncResult ar)
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
                        //MessageBox.Show(state.m_StringBuilder.ToString());
                        //  setThreadedTextBox(state.m_StringBuilder.ToString());
                        string[] topics;
                        topics = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                        string[] topicNames = new string[topics.Length / 2];

                        for (int i = 0; i < topics.Length; i++)
                        {
                            if (i % 2 == 0 && topics[i].Length > 0)
                                topicNames[Convert.ToInt32(topics[i + 1]) - 1] = topics[i];
                        }


                        setThreadedCheckedListBox(topicNames);

                        socketFd.BeginSend(Encoding.ASCII.GetBytes("m|~"), 0, "m|~".Length, 0, new AsyncCallback(SendMyTopicsCallback), state);
                    }
                    else
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveTopicsCallback), state);
                    }
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
   
                        string[] topics;
                        topics = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                        string[] topicNames = new string[topics.Length / 2];

                        for (int i = 0; i < topics.Length; i++)
                        {
                            if (i % 2 == 0 && topics[i].Length > 0)
                                topicNames[Convert.ToInt32(topics[i + 1]) - 1] = topics[i];
                        }


                        setThreadedCheckedListBox(topicNames);

                        socketFd.BeginSend(Encoding.ASCII.GetBytes("m|~"), 0, "m|~".Length, 0, new AsyncCallback(SendMyTopicsCallback), state);
                    }
                }



             
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }


        // po odbiorze subskrybcji aktalizuje liste subskrybcji 
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

                        string[] topicNames = new string[topics_.Length / 2];

                        for (int i = 0; i < topics_.Length; i++)
                        {
                            if (i % 2 == 0 && topics_[i].Length > 0)
                                topicNames[i / 2] = topics_[i];
                        }

                        setThreadedCheckedListBoxBoxes(topicNames);
                        setThreadedButtonAddTopic(true);
                    }
                    else
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveMySubsCallback), state);
                    }
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
                        string[] topics_ = state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1).Split('|');

                        string[] topicNames = new string[topics_.Length / 2];

                        for (int i = 0; i < topics_.Length; i++)
                        {
                            if (i % 2 == 0 && topics_[i].Length > 0)
                                topicNames[i / 2] = topics_[i];
                        }

                        setThreadedCheckedListBoxBoxes(topicNames);
                        setThreadedButtonAddTopic(true);
                    }
                }
         
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }


        // wyswietla informacje zwrotna od serwera
         private void ReceiveInfoCallback(IAsyncResult ar)
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
                        MessageBox.Show(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
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
                    }
                }

        
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }


        //wyswietla informacje zwrotna po dodaniu tematu a nastepnie wysyla zapytania o wszystkei tematy oraz subskrybcje
        //w celu aktualizacji widoku listy subskrybcji
        private void ReceiveFullInfoCallback(IAsyncResult ar)
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
                        socketFd.BeginSend(Encoding.ASCII.GetBytes("t|~"), 0, "t|~".Length, 0, new AsyncCallback(DoubleSendCallback), state);
                    }
                    else
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveFullInfoCallback), state);
                    }
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1 && state.m_StringBuilder.ToString().Contains("~"))
                    {
                        MessageBox.Show(state.m_StringBuilder.ToString().Substring(0, state.m_StringBuilder.ToString().Length - 1));
                        socketFd.BeginSend(Encoding.ASCII.GetBytes("t|~"), 0, "t|~".Length, 0, new AsyncCallback(DoubleSendCallback), state);
                    }
                }


            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        // po naciśnięciu na nazwę danego tematu w celu subskrybowania/odsubskrybowania, wysyla ta informacje serwerowi
        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            if(this.checkedListBox1.SelectedIndex>=0)
            {
                string selectedTopic = "s|" + Convert.ToString(this.checkedListBox1.SelectedIndex + 1)+"~";
                socketFd.BeginSend(Encoding.ASCII.GetBytes(selectedTopic), 0, selectedTopic.Length, 0, new AsyncCallback(SendSubCallback), state);

            }

        }

        // wysyla serwerowi informacje o checi dodana nowego tematu
        private void buttonAddTopic_Click(object sender, EventArgs e)
        {
            

            if(this.textBoxTopic.Text.Length>2)
            {
                setThreadedButtonAddTopic(false);
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;

                string topicToSend = "n|";
                topicToSend += textBoxTopic.Text.ToString().Replace('|', ' ').Replace('~', ' ');
                topicToSend += "~";
                
                socketFd.BeginSend(Encoding.ASCII.GetBytes(topicToSend), 0, topicToSend.Length, 0, new AsyncCallback(SendTopicCallback), state);
            }
            else
            {
                setThreadedButtonAddTopic(false);
                MessageBox.Show("Topic name is too short");
                setThreadedButtonAddTopic(true);
            }

        }


        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }


        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBoxTopic_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
