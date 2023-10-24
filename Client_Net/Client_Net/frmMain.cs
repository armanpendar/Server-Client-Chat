using System;
/////////
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Client_Net
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        //*****************************************************************************
        Socket client;
        IPHostEntry ipHostInfo;
        IPAddress ipAdd;
        IPEndPoint remoteEndPoint;

        Thread th1;

        string data;

        byte[] msg;

        private delegate void setDisplay(string Text);//for lstMessage
        private delegate void EnableTrue(bool b);//for txtMsg & btnSend & lstMsg
        //----------------------------------------- Connect
        private void Connect()
        {
            try
            {
                ipHostInfo = Dns.Resolve(txtServerIP.Text);                
                ipAdd = ipHostInfo.AddressList[0];
                int PortNumber = int.Parse(textPortNumber.Text);                
                remoteEndPoint = new IPEndPoint(ipAdd, PortNumber);
                client.Connect(remoteEndPoint);

                th1 = new Thread(new ThreadStart(DataReceive));
                th1.Start();

                EnableAfterConnect(true);
                btnConnectText("Connected!!");
                System.Media.SystemSounds.Exclamation.Play();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------- Data Receive
        private void DataReceive()
        {
            try
            {
                byte[] bytes = new byte[1000];
                int byteRec;

                while (true)
                {
                    while (true)
                    {
                        byteRec = client.Receive(bytes);
                        if (byteRec > 0)
                        {
                            data = System.Text.Encoding.UTF8.GetString(bytes, 0, byteRec);
                            break;
                        }
                    }

                    FillLstMsg(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------- fill list Messeges
        private void FillLstMsg(string str)
        {
            try
            {
                if (lstMsg.InvokeRequired == true)
                {
                    setDisplay d = new setDisplay(FillLstMsg);
                    this.Invoke(d, new object[] { str });
                }
                else
                {
                    lstMsg.Items.Add("Server: " + str);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------- Enable after Connect
        private void EnableAfterConnect(bool b)
        {
            if (txtMsg.InvokeRequired == true)
            {
                EnableTrue et = new EnableTrue(EnableAfterConnect);
                this.Invoke(et, new object[] { b });
            }
            else
            {
                txtMsg.Enabled = b;
            }
            //----------
            if (btnSend.InvokeRequired == true)
            {
                EnableTrue et = new EnableTrue(EnableAfterConnect);
                this.Invoke(et, new object[] { b });
            }
            else
            {
                btnSend.Enabled = b;
            }
            //----------
            if (lstMsg.InvokeRequired == true)
            {
                EnableTrue et = new EnableTrue(EnableAfterConnect);
                this.Invoke(et, new object[] { b });
            }
            else
            {
                lstMsg.Enabled = b;
            }
        }
        //-----------------------------------------
        //----------------------------------------- Change btnConnect.Text
        private void btnConnectText(string btnText)
        {
            if (btnConnect.InvokeRequired == true)
            {
                setDisplay d = new setDisplay(btnConnectText);
                this.Invoke(d, new object[] { btnText });
            }
            else
            {
                btnConnect.Text = btnText;
            }
        }
        //***************************************************************************** this_load
        private void frmMain_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            txtMsg.Enabled = false;
            lstMsg.Enabled = false;

            IPHostEntry myHostInfo = Dns.Resolve(Dns.GetHostName());
            lblIP.Text = myHostInfo.AddressList[0].ToString();
            lblName.Text = Dns.GetHostName();
            txtServerIP.Text = lblIP.Text;

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        //***************************************************************************** Connect to Server
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }
        //***************************************************************************** Exit
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                th1.Abort();
            }
            catch
            {
            }
            Environment.Exit(0);
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                th1.Abort();
            }
            catch
            {
            }
            Environment.Exit(0);
        }
        //*****************************************************************************
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMsg.Text != "")
                {
                    msg = System.Text.Encoding.UTF8.GetBytes(txtMsg.Text);
                    client.Send(msg);
                    lstMsg.Items.Add("My: " + txtMsg.Text);
                    txtMsg.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //*****************************************************************************
        private void txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                btnSend_Click(this, EventArgs.Empty);
        }

        private void textPortNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblIP_Click(object sender, EventArgs e)
        {

        }
        //*****************************************************************************
    }
}