using System;
using System.IO;
using System.Text;

namespace faasSzero
{
    internal enum ArgType {
        None,
        Gateway,
        User,
        Password,
        Time
    }

    public enum AppEnv {
        FAAS_SCALE_ZERO_GATEWAY,
        FAAS_SCALE_ZERO_USER,
        FAAS_SCALE_ZERO_PASSWORD,
        FAAS_SCALE_ZERO_INTERVAL
    }

    public enum AppCommand {
        Check,
        Install
    }

    public class PacketReader : StringReader {
        public PacketReader(string s)
            : base(s)
        { }

        public bool ReadNext<T>(ref T value) where T : IConvertible
        {
            var sb = new StringBuilder();
            var type = typeof(T);
            do {
                string current = ((char)Peek()).ToString();
                if (current[0] == '\uffff')
                    break;
                if (char.IsWhiteSpace(current[0]))
                    break;
                try {
                    T test = (T)((IConvertible)current).ToType(typeof(T), System.Globalization.CultureInfo.CurrentCulture);
                }
                catch {
                    break;
                }
                sb.Append(current);
                Read();

                if (type == typeof(char))
                    break;
            } while (true);

            if (sb.Length == 0)
                return false;
            value = (T)((IConvertible)sb.ToString()).ToType(typeof(T), System.Globalization.CultureInfo.CurrentCulture);
            return true;
        }

    }

    public class ArgParser
    {
        public AppCommand Command { get; set; }
        public string Gateway;
        public string User;
        public string Password;
        public string Interval;

        private void Validate()
        {
            if (Gateway.Length == 0)
                throw new System.Exception("flag --gateway required");
            if (User.Length == 0)
                throw new System.Exception("flag --user required");
            if (Password.Length == 0)
                throw new System.Exception("flag --password required");
            if (Interval.Length == 0)
                throw new System.Exception("flag --interval required");

            if (GetIntervalInSeconds() <= 0)
                throw new System.Exception("invalid --interval value");
        }

        private void ReadEnvironmentValue(string name, ref string field)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (value != null && value.Length > 0)
                field = value;
        }

        private void ReadArgs(string []args)
        {
            ArgType argType = ArgType.None;
            foreach (string arg in args) {
                if (argType != ArgType.None) {
                    switch (argType) {
                        case ArgType.Gateway:
                            Gateway = arg;
                            argType = ArgType.None;
                            continue;
                        case ArgType.User:
                            User = arg;
                            argType = ArgType.None;
                            continue;
                        case ArgType.Password:
                            Password = arg;
                            argType = ArgType.None;
                            continue;
                        case ArgType.Time:
                            Interval = arg;
                            argType = ArgType.None;
                            continue;
                    }
                }

                if (arg[0] == '-') {
                    if (arg == "--gateway" || arg == "-g")
                        argType = ArgType.Gateway;
                    else if (arg == "--user" || arg == "-u")
                        argType = ArgType.User;
                    else if (arg == "--password" || arg == "-p")
                        argType = ArgType.Password;
                    else if (arg == "--interval" || arg == "-i")
                        argType = ArgType.Time;
                    else throw new System.Exception("Invalid flag " + arg);
                } else {
                    try {
                        Command = Enum.Parse<AppCommand>(arg, true);
                    } catch {
                        throw new Exception("Invalid command " + arg);
                    }
                }
            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("OpenFaaS Scale Zero");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("  faasSzero [Command] Flags");
            Console.WriteLine("");
            Console.WriteLine("Available Commands:");
            Console.WriteLine("\tinstall\t- install service");
            Console.WriteLine("");
            Console.WriteLine("Flags:");
            Console.WriteLine("  -g, --gateway     Gateway URL starting with http(s):// (default \"http://127.0.0.1:8080\")");
            Console.WriteLine("      or env: FAAS_SCALE_ZERO_GATEWAY");
            Console.WriteLine("  -p, --password    Gateway password");
            Console.WriteLine("      or env:  FAAS_SCALE_ZERO_PASSWORD");
            Console.WriteLine("  -u, --username    Gateway username (default \"admin\")");
            Console.WriteLine("      or env:  FAAS_SCALE_ZERO_USER");
            Console.WriteLine("  -i, --interval    Time interval(eg: 30s, 15m, 1h)");
            Console.WriteLine("      or env:  FAAS_SCALE_ZERO_INTERVAL");
            Console.WriteLine("");
        }

        public int GetIntervalInSeconds()
        {
            PacketReader reader = new PacketReader(Interval);
            int intValue = -1;
            char typeValue = 'a';
            int intervalInt = 0;
            while (true) {
                if (!reader.ReadNext<int>(ref intValue))
                    break;
                if (reader.ReadNext<char>(ref typeValue)) {
                    switch (typeValue) {
                        case 's':
                            intervalInt += intValue;
                            break;
                        case 'm':
                            intervalInt += intValue * 60;
                            break;
                        case 'h':
                            intervalInt += intValue * 60 * 60;
                            break;
                        case 'd':
                            intervalInt += intValue * 60 * 60 * 24;
                            break;
                        default: return -1;
                    }
                }
                else {
                    intervalInt += intValue * 60;
                    break;
                }   
            }
            return intervalInt;
        }

        public ArgParser(string []args)
        {
            Interval = "30m";
            Gateway = "http://127.0.0.1:8080";
            User = "admin";
            Password = "";
            Command = AppCommand.Check;

            ReadEnvironmentValue(AppEnv.FAAS_SCALE_ZERO_GATEWAY.ToString(), ref Gateway);
            ReadEnvironmentValue(AppEnv.FAAS_SCALE_ZERO_USER.ToString(), ref User);
            ReadEnvironmentValue(AppEnv.FAAS_SCALE_ZERO_PASSWORD.ToString(), ref Password);
            ReadEnvironmentValue(AppEnv.FAAS_SCALE_ZERO_INTERVAL.ToString(), ref Interval);

            ReadArgs(args);
            Validate();
        }
    }
}
