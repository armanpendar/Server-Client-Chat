using System;
using System.Collections;
/////////
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Server_Net
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        //*****************************************************************************
        ArrayList usedPorts;
        Socket listner;
        Socket handler;
        IPHostEntry ipHostInfo;
        IPAddress ipAdd;
        IPEndPoint localEndPoint;

        const int maxClient = 10;

        Thread th1;
        Thread th2;

        string data;

        byte[] msg;

        private delegate void setDisplay (string Text);//for lstMessage
        private delegate void EnableTrue(bool b);//for txtMsg & btnSend & lstMsg
        //----------------------------------------- Connect
        private void Connect()
        {
            try
            {
                btnConnectText("Waiting for Connect a Client...");
                ipHostInfo = Dns.Resolve(Dns.GetHostName());
                ipAdd = ipHostInfo.AddressList[0];
                int PortNumber = int.Parse(textPortNumber.Text);
                localEndPoint = new IPEndPoint(ipAdd,PortNumber);
                listner.Bind(localEndPoint);
                listner.Listen(maxClient);
                
                th1 = new Thread(new ThreadStart(AcceptStart));
                th1.Start();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------- DisConnect
        private void DisConnect()
        {
            try
            {
                handler.Disconnect(true);
                listner.Disconnect(true);
              //  handler.Shutdown(SocketShutdown.Both);
              //  listner.Shutdown(SocketShutdown.Both);
                th1.Abort();
                th2.Abort();
                btnConnectText("Connect ...");
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------- Accept
        private void AcceptStart()
        {
            try
            {
                handler = listner.Accept();
                th2 = new Thread(new ThreadStart(DataReceive));
                th2.Start();
                
                btnConnectText("DisConnect ...");
                EnableAfterConnect(true);
                System.Media.SystemSounds.Exclamation.Play();
                
                //String ClientIP = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
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
                        byteRec = handler.Receive(bytes);
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
            if (btnSendMsg.InvokeRequired == true)
            {
                EnableTrue et = new EnableTrue(EnableAfterConnect);
                this.Invoke(et, new object[] { b });
            }
            else
            {
                btnSendMsg.Enabled = b;
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
        //----------------------------------------- fill list Messeges
        private void FillLstMsg(string str)
        {
            try
            {
                str = str.Trim();
                if (lstMsg.InvokeRequired == true)
                {
                    setDisplay d = new setDisplay(FillLstMsg);
                    this.Invoke(d, new object[] { str });
                }
                else
                {
                    // check if str is an mathematic string
                    if(str.StartsWith("calc"))
                    {
                        str = str.Substring(4).Trim();
                        if (str != "")
                        {
                            double result = MathFolder.Math_Function.Evaluate(str);
                            lstMsg.Items.Add("Client: " + str + " = " + result);

                            msg = System.Text.Encoding.UTF8.GetBytes(str + " = " + result);
                            handler.Send(msg);
                        }
                        else
                        {
                            msg = System.Text.Encoding.UTF8.GetBytes(" Please Enter Correct Style Examle: Calc 2*99 " );
                            handler.Send(msg);
                        }
                    }
                    // else
                    else
                    {
                        lstMsg.Items.Add("Client: " + str );
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //-----------------------------------------
        //***************************************************************************** this_Load
        private void frmMain_Load(object sender, EventArgs e)
        {
            usedPorts = new ArrayList();
            ListAvailableTCPPort();

            IPHostEntry myHostInfo = Dns.Resolve(Dns.GetHostName());
            lblIP.Text = myHostInfo.AddressList[0].ToString();
            lblName.Text = Dns.GetHostName();

            listner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            lstMsg.Enabled = false;
            txtMsg.Enabled = false;
            btnSendMsg.Enabled = false;

        }
        //*****************************************************************************
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (handler==null || handler.Connected == false)
                Connect();
            else
                DisConnect();
        }
        //***************************************************************************** Exit
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                listner.Shutdown(SocketShutdown.Both);
                th1.Abort();
                th2.Abort();
            }
            catch
            {
            }
            Environment.Exit(0);
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                listner.Shutdown(SocketShutdown.Both);
                th1.Abort();
                th2.Abort();
            }
            catch
            {
            }
            Environment.Exit(0);
        }
        //***************************************************************************** Send Message
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMsg.Text != "")
                {
                    msg = System.Text.Encoding.UTF8.GetBytes(txtMsg.Text);
                    handler.Send(msg);
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
                btnSendMsg_Click(this, EventArgs.Empty);
        }
        //*****************************************************************************
        //List used tcp port
        private void ListAvailableTCPPort()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();

            while (myEnum.MoveNext())
            {
                TcpConnectionInformation TCPInfo = (TcpConnectionInformation)myEnum.Current;
                string Local_ip = TCPInfo.LocalEndPoint.Address.ToString();
                int Local_port = TCPInfo.LocalEndPoint.Port;
                string Remote_ip = TCPInfo.RemoteEndPoint.Address.ToString();
                int Remote_port = TCPInfo.RemoteEndPoint.Port;
                string State = TCPInfo.State.ToString();
                //              
                dataGridView1.Rows.Add(Local_ip, Local_port, Remote_ip, Remote_port, State);
              
               this.usedPorts.Add(TCPInfo.LocalEndPoint.Port);
            }
        }

        //****************************************************************************
        // Reafresh Used Port List
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            usedPorts.Clear();
            ListAvailableTCPPort();
        }

        private void lblIP_Click(object sender, EventArgs e)
        {

        }
    }
}