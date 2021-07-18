using System.Diagnostics;

namespace faasSzero
{
    public class InstallHelper
    {
        private static string destinationPath = "/usr/local/bin/faasSzero";
        private static string serviceFileName = "faasSzero.service";
        private static string servicePath = "/etc/systemd/system/" + serviceFileName;
        private static string ServiceScript(string gateway, string user, string password, string interval)
        {
            return
                "[Unit]\n" +
                "Description=faasSzero\n" +
                "\n" +
                "[Service]\n" +
                "WorkingDirectory=/usr/local/\n" +
                "ExecStart=/usr/local/bin/faasSzero\n" +
                "Restart=always\n" +
                "RestartSec=10\n" +
                "KillSignal=SIGINT\n" +
                $"Environment={AppEnv.FAAS_SCALE_ZERO_GATEWAY.ToString()}={gateway}\n" +
                $"Environment={AppEnv.FAAS_SCALE_ZERO_USER.ToString()}={user}\n" +
                $"Environment={AppEnv.FAAS_SCALE_ZERO_PASSWORD}={password}\n" +
                $"Environment={AppEnv.FAAS_SCALE_ZERO_INTERVAL.ToString()}={interval}\n" +
                "\n" +
                "[Install]\n" +
                "WantedBy=multi-user.target\n";
        }

        private static bool RunCommand(string command, string arguments)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = command;
            cmd.StartInfo.Arguments = arguments;
            cmd.StartInfo.UseShellExecute = true;
            cmd.Start();
            cmd.WaitForExit();
            return cmd.ExitCode == 0;
        }

        public static void Install(string gateway, string user, string password, string interval)
        {
            RunCommand("systemctl", $"stop {serviceFileName}");
            string exePath = Process.GetCurrentProcess().MainModule.FileName!;

            if (exePath != destinationPath)
                System.IO.File.Copy(exePath, destinationPath, true);

            using (var stream = System.IO.File.CreateText(servicePath))
                stream.Write(ServiceScript(gateway, user, password, interval));

            RunCommand("systemctl", "daemon-reload");
            if (!RunCommand("systemctl", $"enable {serviceFileName}"))
                throw new System.Exception("Unable to run systemctl");
            if (!RunCommand("systemctl", $"start {serviceFileName}"))
                throw new System.Exception("Unable to run systemctl");
        }
    }
}
