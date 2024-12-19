using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
        NetworkStream ns = null;
        StreamWriter writer = null;
        StreamReader reader = null;
        //const string serverAddress = "127.0.0.1";
        //const short serverPort = 4040;
        TcpClient client;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        private string username = null;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            client = new TcpClient();
            string address = ConfigurationManager.AppSettings["ServerAddress"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        private void DisconnectBtnClick(object sender, RoutedEventArgs e)
        {
            if (username == null)
            {
                MessageBox.Show("To disconnect you need to connect");
                return;
            }
            string message = "$<disconnect>";
            SendMessage(message);

            ns.Close();
            client.Close();
            username = null;
        }

        private void ConnectBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect(ServerEndPoint);
                ns = client.GetStream();
                writer = new StreamWriter(ns);
                reader = new StreamReader(ns);
                Listen();

                string message = "$<connect>";
                var input = new Input();
                bool? dialog = input.ShowDialog();
                if (dialog != true || string.IsNullOrWhiteSpace(input.InputText))
                {
                    throw new Exception("Username is not valid");
                }
                username = input.InputText;
                SendMessage(message);
            }
            catch (Exception ex)
            {
                ns.Close();
                client.Close();
                username = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string message = msgText.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Message is not valid");
                return;
            }
            SendMessage(message);
        }

        private async void SendMessage(string message)
        {
            var messageData = new Tuple<string, string>(message, username);
            string jsonString = JsonSerializer.Serialize(messageData);
            writer.WriteLine(jsonString);
            writer.Flush();
        }

        private async void Listen()
        {
            while (true)
            {
                string? jsonMessage = await reader.ReadLineAsync();
                var msgData = JsonSerializer.Deserialize<Tuple<string, string>>(jsonMessage);
                messages.Add(new MessageInfo(msgData.Item2, msgData.Item1, DateTime.Now));
            }
        }
    }
}