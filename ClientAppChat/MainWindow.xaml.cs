using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAppChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint ServerEndPoint;
        //const string serverAddress = "127.0.0.1";
        //const short serverPort = 4040;
        UdpClient client;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        private string username = null;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            client = new UdpClient();
            string address = ConfigurationManager.AppSettings["ServerAddress"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Listen();
        }

        private void LeaveBtnClick(object sender, RoutedEventArgs e)
        {
            string message = "$<leave>";
            SendMessage(message);
            messages.Clear();
            username = null;
        }

        private void JoinBtnClick(object sender, RoutedEventArgs e)
        {
            string message = "$<join>";
            var input = new Input();
            if (input.ShowDialog() == true)
            {
                username = input.InputText;
                SendMessage(message);
            }
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string message = msgText.Text;
            SendMessage(message);
        }

        private async void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, ServerEndPoint);
        }

        private async void Listen()
        {
            while (true)
            {
                var data = await client.ReceiveAsync();
                string message = Encoding.UTF8.GetString(data.Buffer);
                messages.Add(new MessageInfo(username, message, DateTime.Now));
            }
        }
    }
}