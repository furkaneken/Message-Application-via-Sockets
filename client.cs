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

namespace client_new
{
    public partial class Form1 : Form
    {
        static Socket Clientsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);    // burda clientımızın socketini oluşturuyoruz bu bizim servera baglanmamızı saglicak
        // biz şimdi GUI uzerinden calıstıgımız için bizim main fonksiyonumuz burası kafan  // karısmasın bu ne boyle partial class'ı gorunce ben oyle olmustum :D
        static bool connected = false;    //clientimizn baglanıp baglanmadıgını bu boola gore yoneticez            
        string ipadress1;                // buda kullanıcının açılan pencerede ip bölümüne girdiği rakamları atıyacağımız variable , string olması lazım cunku ip yazarken noktalar var mesela , 192.168.1.1
        int portnumbers1;                   // buda aynı şekilde kullanıcın girdiği port u atıyacagımız variable
        string nickname;                  // kullancının girdiği nick..
        static bool newuser = true;          // buda yeni bir cleint mı degil mi boolu






        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Connectbutton_Click(object sender, EventArgs e)                    // şimdi kullanıcı geldi gerekli kutulara bilgilerini girdi connect buttonuna bastığı an olan şeyler bunlar, sen bir button olusturup iki kere ustune tıklarsan sana bu şekilde bir fonksiyon açar ama içi boş olur
        {
            ipadress1 = Ipnum.Text;                     // burdaki ipnum dediğigmiz biz textboxt olusturduk ya onun ismini ben ipnum koydum ipnum.text diyincede içindeki yazıyı gostermis oluyoruz
            portnumbers1 = int.Parse(Portnum.Text);     // bizim burda inte çevirmemizin sebebi sen bir textbox olusturdugun içine yazıcagın herşey string olarak degerlendirilir ama port int olmalı int.parse bu işe yarıyor
            nickname = usernickbox.Text;               // yukarıdakilerin aynısı

            try
            {
                Clientsocket.Connect(ipadress1, portnumbers1);           // burdada daha onceden olusturdumuz socketin nerey baglanıcagını parametre seklinde veriyoruz // Connect socket kutuphanesinin bir member fonksiyonu
                connected = true;
                Chattextbox.SelectedText = "Client Connected to Server" + Environment.NewLine;       // burdada buyuk chat box 'ın içine yazı yazıyor baglandı diye 
                Thread recievethread = new Thread(new ThreadStart(recieve));       // thread dediğimiz şey aslında aynı anda başka birşey yaparken kullandıgmız şey yani biz servera baglandık ama bize serverdan sonucta mesaj gelicek bize ve ne zaman geliceğini bilmiyoruz o yuzden bu fonksiyon durmadan çalışmak zorunda bu işe yarıyor
                recievethread.Start();            // trehadlerin bu sekilde baslatılması lazım birde , treadhin içine recieve yazmısım gordun dimi , o işte threadin hangi fonksiyonu çalıştırcagını soyluyorum orda
                if(connected && newuser)          // threadi su sekilde dusun biz c++'da sadece bir bir main fonksiyonumuz vardı ve bir tek onu calıstırıyorduk ama burdaa aynı anda birden fazla main calıstırmamız lazım gibi dusunebilirsin
                {                             
                    Clientsocket.Send(Encoding.Default.GetBytes(nickname));      // burdada tamam baglandık sorun yok diye if'den geciyoruz sonrasında servera adımızı gonderiyoruz 
                    newuser = false;                                               // yine send fonksiyonu bir socket fonksiyonu , encode ediyoruz çunku biz direk string şeklinde bir yerden baska bir yere birşey gonderemeyiz onu byte'lara çeviyoruz:(0101010100101) böyle oldu atıyorum bunu gonderiyoruz
                }                                                                 // serve bu bytelerıda aynı sekilde atıyorum encoding.default.getstring(buffer) şeklinde bunu bizim anlaycagımız bir stringe donusturucek server kendi dosyasında(yani burası sadece client işte :D )



            }
            catch
            {
                Chattextbox.SelectedText = " there is a connection problem ";
            }
        }

        private void recieve()         // yukarıda threadin içinde kullandımız fonksiyonu burda tanımlıyoruz
        {
            while(connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];           // bak yukarda dedim ya server bunu encode ederek stringe doonusturcek diye burdada sw'den gelen bytelerı strine donusturcez o yuzden bir Byte[] yaratıyoruz gelenide bunn içine atıyoruz.
                    Clientsocket.Receive(buffer);
                    //Chattextbox.SelectedText = "Server : " + Encoding.Default.GetString(buffer) + Environment.NewLine;
                    abc1(buffer);           // bunu yazarak anlatamam herhalde konusuruz ama burda yaptıgmız işte byte'ı stirnge cevirip chatboxda yazdırmak
                }
                catch
                {
                    //Chattextbox.SelectedText = " there is a promblem with connection " + Environment.NewLine;
                    //connected = false;
                    //Clientsocket.Close();

                }
            }
        }

        private void Sendbutton_Click(object sender, EventArgs e)      // send butonuna bastıgmızda sendin yanında kutucaga yazdıgmız yazıyı gondericez burda
        {
            if(Sendtextbox.Text != "quit")           //senndtextbox'da işte ben buna isim verdim boyle 
            {
                try
                {
                    abc2(Sendtextbox.Text);           // bunu konusuruz ama temel olarak yine byte a cevirip gonderiyoruz
                    Clientsocket.Send(Encoding.Default.GetBytes(Sendtextbox.Text));
                    Sendtextbox.Clear();
                    Sendtextbox.Focus();
                    //Sendbutton.Focus();



                }
                catch
                {
                    Chattextbox.SelectedText = "There is a problem with connection again " + Environment.NewLine;
                }
            }

            if(Sendtextbox.Text == "quit")           //işte bunu yazarsada program kapanıyor
            {
                MessageBox.Show("Client is closing", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Clientsocket.Close();
            }
        }
        private void abc1(Byte[] buffer)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { abc1(buffer); });
                return;
            }
            Chattextbox.SelectedText = Environment.NewLine + "Server : " + Encoding.Default.GetString(buffer);
        }
        private void abc2(string a)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { abc2(a); });
                return;
            }
            Chattextbox.SelectedText = Environment.NewLine + nickname + " : " + a ;
        }
    }
}
