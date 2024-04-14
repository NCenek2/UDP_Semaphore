namespace UDP_Limited
{
    public class Program
    {
        static List<Socket> _connections;
        static void Main(string[] args)
        {

            Console.WriteLine("Setting Up Socket");
            _connections = new List<Socket>();
            var ipAddress = "127.0.0.1";
            var ports = new int[] {6001, 6002, 6004, 6005 };

            foreach (var  port in ports)
            {
                _connections.Add(new Socket(ipAddress, port));
            }

            foreach (var connection in _connections)
            {
                connection.StartSending();
            }

            Console.WriteLine("Connections Set Up");
            Console.ReadKey();

            foreach (var connection in _connections)
            {
                connection.StopSending();
            }

            Console.WriteLine("Connections Disconnected");
            Console.ReadKey();
        }
    }
}