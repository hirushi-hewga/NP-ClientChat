using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace ServerApp
{
    class ServerChat
    {
        const short port = 4040;
        const string address = "127.0.0.1";
        const int client_max_count = 5;
        const string DISCONNECT = "$<disconnect>";
        const string CONNECT = "$<connect>";

        TcpClient client = null;
        NetworkStream ns = null;
        StreamReader reader = null;
        StreamWriter writer = null;

        List<string> usernames = new List<string>();

        TcpListener listener = null;

        IPEndPoint clientEndPoint = null;
        public ServerChat()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse(address), port));
        }
        public void Disconnect(string username)
        {
            usernames.Remove(username);
            Console.WriteLine("Disconnected!!!!!!");
        }
        public void Connect(string username)
        {
            if (usernames.Count() >= 5)
                throw new Exception("Chat is full.");
            if (!usernames.Contains(username))
                usernames.Add(username);
            else throw new Exception("Username is exist.");
            Console.WriteLine("Connected!!!!!!");
        }
        public void WriteText(string jsonString, StreamWriter writer)
        {
            writer.WriteLine(jsonString);
            writer.Flush();
        }
        public void Start()
        {
            listener.Start();
            Console.WriteLine("Waiting for connection.....");
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);
            while (true)
            {
                string jsonMessage = reader.ReadLine();
                var msgData = JsonSerializer.Deserialize<Tuple<string, string>>(jsonMessage);
                string message = msgData.Item1;
                string username = msgData.Item2;
                Console.WriteLine($"{message} from {client.Client.LocalEndPoint}. Date : {DateTime.Now}");
        
                switch (message)
                {
                    case CONNECT:
                        Connect(username);
                        break;
                    case DISCONNECT:
                        Disconnect(username);
                        break;
                    default:
                        WriteText(jsonMessage, writer);
                        break;
                }
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerChat chat = new ServerChat();
            chat.Start();
        }
    }
}
