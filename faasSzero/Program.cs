using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace faasSzero
{
    class Program
    {
        static async Task<int> Main(string []args)
        {
            ArgParser arg;
            try {
                arg = new ArgParser(args);
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                ArgParser.PrintHelp();
                return -1;
            }

            if (arg.Command == AppCommand.Install) {
                OpenFaaSApi.Api api = new OpenFaaSApi.Api(arg.Gateway, arg.User, arg.Password);
                try {
                    await api.GetFunctionsAsync();
                    InstallHelper.Install(arg.Gateway, arg.User, arg.Password, arg.Interval);
                    return 0;
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    return -1;
                }
            }

            FunctionWatcher watcher = new FunctionWatcher(arg.Gateway, arg.User, arg.Password);
            int interval = arg.GetIntervalInSeconds();
            Console.WriteLine("Interval: {0}", arg.Interval);
            Console.WriteLine("interval sec: {0}", interval);
            while(true) {
                Console.WriteLine("Check {0}", DateTime.Now);
                await watcher.Check();

                Thread.Sleep(1000 * interval);
            }
        }
    }
}
