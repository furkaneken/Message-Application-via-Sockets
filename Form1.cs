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
using System.Threading;
using System.Threading.Tasks;
namespace gui_and_serverclient
{                                               // bu server ilk başta client' a bak sonra buna bakarsın orda bazı şeyleri daha ayrıntılı açıkladım .
    public partial class Form1 : Form
    {
        static Socket Serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   
        static List<Socket> Clientsockets = new List<Socket>();      // bagalanan client socketlerini bu listeye atıcaz 
        bool stop = false;
        bool start = true;
        bool acceptbool = true;
        string portnumberstring;
        int portnum;
        bool newuser = true;
        string username;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
            //Chatbox.Text = portnumberstring;
        }

        private void Portnumberbox_TextChanged(object sender, EventArgs e)
        {
            portnumberstring = Portnumberbox.Text;
            portnum = Int32.Parse(portnumberstring);     // client dosyasında açıkladım burayı
        }

        private void Chatbox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portnum);     // burdada işte server'ı açıyoruz içine yazdıgmız portnumberdan server dinlemeye baslıyor bu endpoint dediğimiz şey nereyi dinliyoruz o muhabbet bizde içine yazıyoruz işte
            Serversocket.Bind(endPoint);         // bununla bak bu endpointi dinle işte diyoruz
            Serversocket.Listen(4); // içindeki numaraya takılmaz açıklarım sonra
            Chatbox.SelectedText = "Server is listening";
            Chatbox.SelectedText = Environment.NewLine;
            Thread Acceptthread = new Thread(new ThreadStart(Accept1));        // bu threadte servera heran bir client baglanabilir o yuzden bunun surekli bir şekilde çalışması lazım
            Acceptthread.Start();
           
        }

        private void abc()      // burayı sonra anlatıcam 
        {
            if(InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { abc(); });
                return;
            }
            Chatbox.SelectedText = "New Client is connected";
            Chatbox.SelectedText = Environment.NewLine;
        }
        

        private void Accept1()     // threadin içine yazdımız fonksiyonu burda tanımlıyoruz 
        {
            while (acceptbool)
            {

                Socket newclient = Serversocket.Accept();       // acceptr socketin bir member fonskiyonu biz yaratmadık kafan karısmasın bu birisi geldiğinde triggerlanıyor diye dusun 
                Clientsockets.Add(newclient);           // triggerlanınca başta yarattığımız client socket listine ekliyor onu
                //Chatbox.SelectedText = "New Client is connected"
                //Chatbox.SelectedText = Environment.NewLine;
                abc();                                             // sonra analtıcam
                Thread recievethread = new Thread(recievefunc);       // buda işte clientlardan gelen datayı işlicek 
                recievethread.Start();
                if(stop)
                {
                    acceptbool = false;
                }





            }
        }
        private void abc2(Byte[] buffer)   // sonraaaaaaaaaaaa
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { abc2(buffer); });
                return;
            }

            
            Chatbox.SelectedText = Environment.NewLine + username + " : " + Encoding.Default.GetString(buffer);
            
        }

        private void recievefunc()            
        {
            
            bool connected = true;
            int lengclientsockets = Clientsockets.Count();
            Socket thisclient = Clientsockets[lengclientsockets - 1];        // işte kacıncı olucagnı gosteriyor burda clien listteki sonucta içinde onceden baska clientlarda olabilir bu sayede sonuna eklicez
            while(connected && !stop)
            {
                try
                {
                    Byte[] buffer = new byte[64];
                    thisclient.Receive(buffer);                        // recieve yine socket fonksiyonu gelen datayı bir parametredeki byte şeklinde varibla içine koyuyor
                    //Chatbox.SelectedText = Environment.NewLine + Encoding.Default.GetString(buffer);
                    if(newuser)
                    {
                        username = Encoding.Default.GetString(buffer);     // biliyorsun artık buraları :D
                        newuser = false;
                    }
                    abc2(buffer);
                }
                catch
                {
                    connected = false;
                }
            }
            
        }
        private void abc1()       // sonraaaaaaa
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { abc1(); });
                return;
            }
            Chatbox.SelectedText = Environment.NewLine + "Broadcast :  " + Chatsendbox.Text + Environment.NewLine ;
            
        }




        private void SendButton_Click(object sender , EventArgs e)      // client'ta anlattığım send'in aynısı
        {
            
            if(Chatsendbox.Text != "quit" && !stop)
            {
                //Chatbox.SelectedText =  "Broadcast :  ";
                //Chatbox.SelectedText =  Chatsendbox.Text ;
                abc1();
                Chatbox.SelectedText = Environment.NewLine;
                Byte[] buffer = Encoding.Default.GetBytes(Chatsendbox.Text);
                foreach (Socket client in Clientsockets)
                {
                    client.Send(buffer);
                }

                Chatsendbox.Clear();
                Chatsendbox.Focus();
            }
            if(Chatsendbox.Text == "quit")
            {
                stop = true;
                MessageBox.Show("server is turning off", "exit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();

            }
            
        }
    }
}
