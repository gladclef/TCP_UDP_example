using TCP_UDP_example;

string transport = "udp"; // udp or tcp
string mode = "local"; // local, server, or client
string addr = Vars.local;

string[] clargs = System.Environment.GetCommandLineArgs();
if (clargs.Length > 1)
{
    mode = clargs[1];
}
if (clargs.Length > 2)
{
    addr = clargs[2];
}

Server server = new Server(transport, addr);
Client client = new Client(transport, addr);

try
{
    Console.WriteLine("Press enter to end program...");

    if (mode == "local" || mode == "server")
    {
        Thread threadServer = new Thread(new ThreadStart(server.run));
        threadServer.Start();
        Thread.Sleep(200);
    }

    if (mode == "local" || mode == "client")
    {
        Thread threadClient = new Thread(new ThreadStart(client.run));
        threadClient.Start();
    }

    Console.ReadLine();
}
finally
{
    server.stop();
    Thread.Sleep(200);
    client.stop();
    Thread.Sleep(200);
}

System.Environment.Exit(0);