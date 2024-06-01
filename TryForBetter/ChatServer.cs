using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TryForBetter
{
    // tao ra lop co kha nang su kien thay doi trang thai
    public class StatusChangedEventArgs : EventArgs
    {
        private string EventMsg;
        public string EventMessage
        {
            get{return EventMsg;}
            set{EventMsg = value;}
        }
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }
  
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    // thiet lap lop Server de khoi tao va duy tri ket noi 
    class ChatServer
    {
        public static Hashtable Users = new Hashtable(30);
        public static Hashtable Connections = new Hashtable(30);
        private IPAddress ipAddress;
        private TcpClient tcpClient;
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
       

        // ham truyen vao dia chi IP 
        public ChatServer(IPAddress address)
        {
            ipAddress = address;
        }
        private Thread thrListener;
        private TcpListener tlsClient;
        bool ServRunning = false;
        // khoi tao ham AddUser de add cac user connect toi server
        public static void AddUser(TcpClient tcpUser, string strUsername)
        {
            ChatServer.Users.Add(strUsername, tcpUser);
            ChatServer.Connections.Add(tcpUser, strUsername);
            SendAdminMessage(Connections[tcpUser] + " đã đăng nhập!");
            SendUserList();


        }
        // khi ma cac user huy ket noi toi server thi cac user out 
        public static void RemoveUser(TcpClient tcpUser)
        {
            if (Connections[tcpUser] != null)
            {
                string username = (string)Connections[tcpUser];
                SendAdminMessage(Connections[tcpUser] + " đã đăng xuất!");
                ChatServer.Users.Remove(ChatServer.Connections[tcpUser]);
                ChatServer.Connections.Remove(tcpUser);
                SendUserList();
            }
        }
    

        //  su ly su kien trang thai thay doi status
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e);
            }
        }

      


        // admin gui ve tin nhan thong bao len khung chat tong
        public static void SendAdminMessage(string Message)
        {
            StreamWriter swSender;
            e = new StatusChangedEventArgs("Admin: " + Message);
            OnStatusChanged(e);
            TcpClient[] tcpClients = new TcpClient[ChatServer.Users.Count];
            ChatServer.Users.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSender = new StreamWriter(tcpClients[i].GetStream());
                    swSender.WriteLine("Admin: " + Message);
                    swSender.Flush();
                    swSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }
        // ham xu ly su kien phan giai ten mien
        static string ResolveDns(string domainName)
        {
            int b = 0;
            string c = "";
            string d = "";
            string a = "";
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(domainName);
                    foreach (IPAddress ip in hostEntry.AddressList)
                {
                    b++;
                    if (b == 1)
                    {
                        c = ip.ToString();
                    }
                    if (b == 2)
                    {
                            d = "   IP address:  "+ip.ToString();  
                    }         
                }
                if (b == 1)
                {
                    a = "IP address:  " +c ;
                }
                else
                {
                    a = c + d;
                }
               
            }  
            catch (Exception ex)
            {
                a = "";
                MessageBox.Show("DNS name ko phu hop");
            }
            return a;
        }
        // ham goi viec ham phan giai ten mien va dien ra su kien server reply lai ten mien user gui
        public static void SendDNSMessage(string Message)
        {
            string a = ResolveDns(Message);
            StreamWriter swSender;
            e = new StatusChangedEventArgs("Admin: " + a);
            OnStatusChanged(e);
            TcpClient[] tcpClients = new TcpClient[ChatServer.Users.Count];
            ChatServer.Users.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (a.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSender = new StreamWriter(tcpClients[i].GetStream());
                    swSender.WriteLine("Admin: " + a);
                    swSender.Flush();
                    swSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }

        // xu ly danh sach nguoi dang nhap
        public static void SendUserList()
        {
            string[] userList = new string[ChatServer.Users.Count];
            ChatServer.Users.Keys.CopyTo(userList, 0);
            string userListMessage = "UserList:" + string.Join(",", userList);

            TcpClient[] tcpClients = new TcpClient[ChatServer.Users.Count];
            ChatServer.Users.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (tcpClients[i] == null)
                    {
                        continue;
                    }
                    StreamWriter swSender = new StreamWriter(tcpClients[i].GetStream());
                    swSender.WriteLine(userListMessage);
                    swSender.Flush();
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }

        // ham xu ly su kien khi ma cac user gui tin nhan den server
        public static void SendMessage(string From, string Message)
        {
            StreamWriter swSender;
            e = new StatusChangedEventArgs(From + " gửi: " + Message);
            OnStatusChanged(e);
            TcpClient[] tcpClients = new TcpClient[ChatServer.Users.Count];
            ChatServer.Users.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSender = new StreamWriter(tcpClients[i].GetStream());
                    swSender.WriteLine(From + " gửi: " + Message);
                    swSender.Flush();
                    swSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }
        // ham bat dau khoi tao lang nghe ket noi bang cac dia chi IP va port 1986 
        public void StartListening()
        {
            IPAddress ipaLocal = ipAddress;
            tlsClient = new TcpListener(ipaLocal, 1986);
            tlsClient.Start();
            ServRunning = true;
            thrListener = new Thread(KeepListening);
            thrListener.Start();
        }
        // Duy tri ket noi den voi cac client
        private void KeepListening()
        {
            while (ServRunning == true)
            {
                tcpClient = tlsClient.AcceptTcpClient();
                Connection newConnection = new Connection(tcpClient);
            }
        }
    }

    // lop ket noi voi cac client
    class Connection
    {
        TcpClient tcpClient;
        private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;
        private string currUser;
        private string strResponse;
        // ham ket noi 
        public Connection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }
        // ham dong ket noi
        private void CloseConnection()
        {
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
        }
        //ham chap nhan ket noi
        private void AcceptClient()
        {
            srReceiver = new System.IO.StreamReader(tcpClient.GetStream());
            swSender = new System.IO.StreamWriter(tcpClient.GetStream());
            currUser = srReceiver.ReadLine();
            if (currUser != "")
            {
                if (ChatServer.Users.Contains(currUser) == true)
                {
                    swSender.WriteLine("0|This username already exists.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currUser == "Administrator")
                {
                    swSender.WriteLine("0|This username is reserved.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else
                {
                    swSender.WriteLine("1");
                    swSender.Flush();

                    ChatServer.AddUser(tcpClient, currUser);
                }
            }
            else
            {
                CloseConnection();
                return;
            }
            try
            {
                while ((strResponse = srReceiver.ReadLine()) != "")
                {
                    if (strResponse == null)
                    {
                        ChatServer.RemoveUser(tcpClient);
                    }
                    else
                    {
                        if (IsValidDomain(strResponse))
                        {
                            ChatServer.SendDNSMessage(strResponse);
                        }
                        else
                        {
                            ChatServer.SendMessage(currUser, strResponse);
                        }
                    }

                }
            }
            catch
            {
                ChatServer.RemoveUser(tcpClient);
            }
        }
        // ham kiem tra tin nhan cua user co phai la ten mien hay khong
        static bool IsValidDomain(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            string domainPattern = @"^(?!-)[A-Za-z0-9-]{1,63}(?<!-)\.[A-Za-z]{2,6}$";
            return Regex.IsMatch(input, domainPattern);
        }
    }
}
