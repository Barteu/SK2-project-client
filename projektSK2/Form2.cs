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
    public partial class Form2 : Form
    {
        Socket socketFd;
        private Form obj;
       

        delegate void setThreadedTextBoxCallback(String text);
        delegate void setThreadedCheckedListBoxCallback(string[] text);
        delegate void setThreadedCheckedListBoxBoxesCallback(string[] text);
        delegate void setThreadedTextBoxTopicCallback(string text);

        public Form2(Socket socketFD)
        {
            InitializeComponent();
            this.obj = this;
            this.socketFd = socketFD;
            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;
            socketFd.BeginSend(Encoding.ASCII.GetBytes("t;"), 0, "t;".Length, 0, new AsyncCallback(DoubleSendCallback), state);
        }
        
        private void setThreadedCheckedListBox(string [] text)
        {
            if (this.checkedListBox1.InvokeRequired)
            {
                setThreadedCheckedListBoxCallback CheckedListBoxCallback = new setThreadedCheckedListBoxCallback(setThreadedCheckedListBox);
                this.obj.Invoke(CheckedListBoxCallback, new object[] { text });
            }
            else
            {
                //text = text.substring(0, text.length - 2);
                this.checkedListBox1.Items.Clear();
                if(text.Length>0)
                {
                    this.checkedListBox1.Items.AddRange(text);
                }
            }
        }

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

        private void setThreadedTextBoxTopic(string text)
        {
            if (this.textBoxTopic.InvokeRequired)
            {
                setThreadedTextBoxTopicCallback TextBoxTopicCallback = new setThreadedTextBoxTopicCallback(setThreadedTextBoxTopic);
                this.obj.Invoke(TextBoxTopicCallback, new object[] { text });
            }
            else
            {
                //text = text.substring(0, text.length - 2);
                textBoxTopic.Text = text;
            }
        }

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
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveFullInfoCallback), state);

            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        private void DoubleSendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveTopicsCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }

        private void SendMyTopicsCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveMySubsCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }


        private void SendSubCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                // Complete sending the data to the remote device.  
                int bytesSent = socketFd.EndSend(ar);

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveInfoCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());

            }
        }



       

        private void ReceiveTopicsCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                state.m_StringBuilder.Clear();
                state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));
               
                /* all the data has arrived */
                if (state.m_StringBuilder.Length > 1)
                {

                    //MessageBox.Show(state.m_StringBuilder.ToString());
                    //  setThreadedTextBox(state.m_StringBuilder.ToString());
                    string[] topics;
                    topics = state.m_StringBuilder.ToString().Split('|');
               
                    string[] topicNames = new string[topics.Length/2];

                    for (int i=0; i< topics.Length;i++)
                    {
                        if (i % 2 == 0 && topics[i].Length > 0)
                            topicNames[Convert.ToInt32(topics[i+1])-1]= topics[i];
                    }
                    

                    setThreadedCheckedListBox(topicNames);

                    socketFd.BeginSend(Encoding.ASCII.GetBytes("m;"), 0, "m;".Length, 0, new AsyncCallback(SendMyTopicsCallback), state);
                    

                }
                //}
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        private void ReceiveMySubsCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                state.m_StringBuilder.Clear();
                state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));
                
                /* all the data has arrived */
                if (state.m_StringBuilder.Length > 1)
                {

                    string [] topics_ = state.m_StringBuilder.ToString().Split('|');

                    string[] topicNames = new string[topics_.Length / 2];

                    for (int i = 0; i < topics_.Length; i++)
                    {
                        if (i % 2 == 0 && topics_[i].Length > 0)
                            topicNames[i / 2] = topics_[i];
                    }

                    setThreadedCheckedListBoxBoxes(topicNames);
                }
         
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        
         private void ReceiveInfoCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                state.m_StringBuilder.Clear();
                state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                /* all the data has arrived */
                if (state.m_StringBuilder.Length > 1)
                {
                    MessageBox.Show(state.m_StringBuilder.ToString());
                }
                //}
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }


       
        private void ReceiveFullInfoCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                state.m_StringBuilder.Clear();
                state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                /* all the data has arrived */
                if (state.m_StringBuilder.Length > 1)
                {

                    //MessageBox.Show(state.m_StringBuilder.ToString());
                    //  setThreadedTextBox(state.m_StringBuilder.ToString());

                    MessageBox.Show(state.m_StringBuilder.ToString());
                    socketFd.BeginSend(Encoding.ASCII.GetBytes("t;"), 0, "t;".Length, 0, new AsyncCallback(DoubleSendCallback), state);
                }
                //}
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
            //MessageBox.Show(this.checkedListBox1.SelectedIndex.ToString());

            SocketStateObject state = new SocketStateObject();
            state.m_SocketFd = this.socketFd;

            string selectedTopic = "s|" + Convert.ToString(this.checkedListBox1.SelectedIndex + 1);

            socketFd.BeginSend(Encoding.ASCII.GetBytes(selectedTopic), 0, selectedTopic.Length, 0, new AsyncCallback(SendSubCallback), state);
            //  if(this.checkedListBox1.)
            // MessageBox.Show("Zmiana");
            /// if (e.NewValue == CheckState.Checked)
            //    MessageBox.Show("Zmiana");
        }

        private void buttonAddTopic_Click(object sender, EventArgs e)
        {
            

            if(this.textBoxTopic.Text.Length>2)
            {
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = this.socketFd;

                string topicToSend = "n|";
                topicToSend += textBoxTopic.Text.ToString();

                socketFd.BeginSend(Encoding.ASCII.GetBytes(topicToSend), 0, topicToSend.Length, 0, new AsyncCallback(SendTopicCallback), state);

            }

        }





        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }



    }
}
