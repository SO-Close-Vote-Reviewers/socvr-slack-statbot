using MargieBot;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SOCVR.Slack.StatBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");

            bot.Aliases = new List<string>() { "sc" };
            bot.Responders.Add(new Responders.StatsReponder());
            bot.Responders.Add(new Responders.HelpResponder());
            bot.Connect(botAPIKey);

            Console.CancelKeyPress += delegate
            {
                bot.Disconnect();
                Console.WriteLine("Got signal to shut down.");
                exitMre.Set();
            };

#if MONO
            var unixSignalManager = new UnixExitSignal();
            unixSignalManager.Exit += UnixSignalManager_Exit;
#endif

            // Probably best to use waithandles. 
            exitMre.WaitOne();
        }

#if MONO
        private static void UnixSignalManager_Exit(object sender, EventArgs e)
        {
            bot.Disconnect();
            Console.WriteLine("Got exit command from linux.");
            exitMre.Set();
        }
#endif
    }

#if MONO
    //http://stackoverflow.com/a/32716784/1043380
    //no idea if this is going to work

    public interface IExitSignal
    {
        event EventHandler Exit;
    }

    public class UnixExitSignal : IExitSignal
    {
        public event EventHandler Exit;

        var signals = new UnixSignal[]
        {
            new UnixSignal(Mono.Unix.Native.Signum.SIGTERM),
            new UnixSignal(Mono.Unix.Native.Signum.SIGINT),
            new UnixSignal(Mono.Unix.Native.Signum.SIGUSR1)
        };

        public UnixExitSignal()
        {
            Task.Factory.StartNew(() =>
            {
                // blocking call to wait for any kill signal
                int index = UnixSignal.WaitAny(signals, -1);

                if (Exit != null)
                {
                    Exit(null, EventArgs.Empty);
                }
            });
        }
    }
#endif

}
