

using UDP_Listener;

public class Program
{

    static List<Listener> _listeners;

    static void Main(string[] args)
    {

        var ports = new int[] { 6001, 6002, 6004, 6005 };
        _listeners = new List<Listener>();

        foreach (var port in ports)
        {
            _listeners.Add(new Listener(port));
        }



        foreach (var listener in _listeners)
        {
            listener.Start();
        }
        Console.WriteLine("All Listeners Started");

        Console.ReadKey();

        foreach (var lister in _listeners)
        {
            lister.Stop();
        }
        Console.WriteLine("All Listeners Terminated");
        Console.ReadKey();
    }
}