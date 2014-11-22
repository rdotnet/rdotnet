using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using RDotNet.ProcessHost.RManagementService;
using RDotNet.Server;

namespace RDotNet.ProcessHost
{
    class Program
    {
        private static Uri _uri;
        private static int _id = -1;
        private static RServiceHost _host;
        private static bool _readyToExit;
        private static MonitorServiceHost _monitorServiceHost;
        private static bool _autostart;

        static int Main(string[] args)
        {
            if (!ParseOptions(args)) return -1;
            _monitorServiceHost = new MonitorServiceHost(_id, _uri, () => { _readyToExit = true; });

            //TODO: Check permissions (httpsys urlacl, etc..)
            UpdateServiceState(ServiceState.Initializing);
            _host = new RServiceHost(_uri, Console.Out);

            const int maxAttempts = 5;
            var started = RunServices(maxAttempts);

            if (started)
            {
                if (_autostart)
                {
                    var uri = new Uri(_uri + "RManagementService/" + _id);
                    var binding = CreateBinding(_uri.Scheme);
                    using (var client = new RManagementServiceClient(binding,
                                        new EndpointAddress(uri)))
                    {
                        Console.Write("Autostarting R Engine...");
                        client.Start(new StartupParameter { LoadInitFile = true, LoadSiteFile = true });
                        Console.WriteLine("Started");
                    }
                }
                UpdateServiceState(ServiceState.Ready);
                Console.WriteLine("Ready");
                
                WaitForExit();

                UpdateServiceState(ServiceState.Terminating);
                Console.Write("Terminating services and host process...");
                
                _host.Stop();

                UpdateServiceState(ServiceState.Closed);
                Console.WriteLine("terminated.");
                _monitorServiceHost.Dispose();
            }

            Console.WriteLine("The window should now close.");
            return started ? 0 : -1;
        }

        public static bool RunServices(int maxAttempts)
        {
            var attempts = maxAttempts;
            var totalAttempts = maxAttempts * 3;

            var lastAttempt = DateTime.Now;
            while (attempts-- != 0 && totalAttempts-- != 0)
            {
                UpdateServiceState(ServiceState.Initializing);
                Console.WriteLine("Initializing");
                Console.WriteLine("Attempting to startupParameter R services: {0}", _uri);

                try
                {
                    _host.Start(_id);
                    return true;
                }
                catch (Exception ex)
                {
                    UpdateServiceState(ServiceState.Faulted);
                    Console.WriteLine();
                    Console.Write(ex.Message);
                }

                if (DateTime.Now.Subtract(lastAttempt).TotalMinutes > 30)
                {
                    attempts = maxAttempts;
                }
            }

            return attempts != maxAttempts;
        }

        private static void WaitForExit()
        {
            while (!_readyToExit)
            {
                if (Console.KeyAvailable)
                {
                    var cki = Console.ReadKey();
                    if ((cki.Modifiers & ConsoleModifiers.Control) != 0 && cki.Key == ConsoleKey.X) break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private static bool ParseOptions(IEnumerable<string> args)
        {
            bool showHelp = false;

            var p = new OptionSet
            {
                {
                    "u|uri=",
                    "the {ROOTURI} for the services.",
                    u => Uri.TryCreate(u, UriKind.Absolute, out _uri)
                },
                {
                    "i|id=",
                    "the application generated {ID} for the host.",
                    (int i) => _id = i
                },
                {
                    "a|autostart",
                    "Autostart the R engine.",
                    a => _autostart = true
                },
                {
                    "h|help", "show this message and exit",
                    v => showHelp = v != null
                },
            };

            try
            {
                var extra = p.Parse(args);
                if (extra.Count > 0 || _uri == null || _id <= 0 || showHelp)
                {
                    if (_uri == null) Console.WriteLine("Invalid URI.");
                    if (_id <= 0) Console.WriteLine("Invalid Id.");
                    p.WriteOptionDescriptions(Console.Out);
                    {
                        return false;
                    }
                }
            }
            catch (OptionException ex)
            {
                Console.Write("{0}: {1} Try {0} --help for more information.", AppDomain.CurrentDomain.FriendlyName, ex.Message);
                {
                    return true;
                }
            }
            return true;
        }

        private static void UpdateServiceState(ServiceState state)
        {
            string text;
            switch (state)
            {
                case ServiceState.Initializing:
                    text = "Initializing";
                    break;

                case ServiceState.Ready:
                    text = "Ready";
                    break;

                case ServiceState.Terminating:
                    text = "Terminating";
                    break;

                case ServiceState.Closed:
                    text = "Closed";
                    break;

                default:
                    text = "Faulted";
                    break;
            }

            _monitorServiceHost.SetState(state);
            Console.Title = string.Format("State: {0} Instance: {1} RootURI: {2}", text, _id, _uri);
        }

        private static Binding CreateBinding(string bindingType)
        {
            switch (bindingType)
            {
                case "net.tcp":
                    return new NetTcpBinding();
                case "net.pipe":
                    return new NetNamedPipeBinding();
                case "https":
                    return new BasicHttpsBinding();
                default:
                    return new NetNamedPipeBinding();
            }
        }
    }
}
