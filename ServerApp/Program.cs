using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace ServerApp
{
    class ServerChat
    {
        const short port = 4040;
        const string JOIN_CMD = "$<join>";
        const string LEAVE_CMD = "$<leave>";
        UdpClient server;
        List<IPEndPoint> members;
        IPEndPoint clientEndPoint = null;
        public ServerChat()
        {
            server = new UdpClient(port);
            members = new List<IPEndPoint>();
        }

        private void AddMember()
        {
            members.Add(clientEndPoint);
            Console.WriteLine("Member was added!");
        }
        private void RemoveMember()
        {
            members.Remove(clientEndPoint);
            Console.WriteLine("Member was removed!");
        }
        private void SendToAll(byte[] data)
        {
            if (members.Contains(clientEndPoint))
            {
                foreach (var member in members)
                {
                    server.SendAsync(data, data.Length, member);
                }
            }
        }
        public void Start()
        {
            while (true)
            {
                byte[] data = server.Receive(ref clientEndPoint);
                string jsonString = Encoding.UTF8.GetString(data);
                var msgData = JsonSerializer.Deserialize<Tuple<string, string>>(jsonString);
                string message = msgData.Item1;
                Console.WriteLine($"{message} from {clientEndPoint}. Date : {DateTime.Now}");

                switch (message)
                {
                    case JOIN_CMD:
                        AddMember();
                        break;
                    case LEAVE_CMD:
                        RemoveMember();
                        break;
                    default:
                        SendToAll(data);
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
