using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperDiscordBot {
    class Program {
        private DiscordSocketClient client;
        private Dictionary<ulong, Communicator> communicators = new Dictionary<ulong, Communicator>();

        static void Main(string[] args) {
            new Program().AsyncMain().GetAwaiter().GetResult();
        }

        private async Task AsyncMain() {
            client = new DiscordSocketClient();

            if (File.Exists("token.txt") == false) {
                File.Create("token.txt");
                Console.WriteLine("Please place your token inside token.txt");
                Console.ReadKey();
                return;
            }
            string token = File.ReadAllText("token.txt");

            await client.LoginAsync(TokenType.Bot, token);

            client.Log += Log;
            client.MessageReceived += MessageRecieved;

            await client.StartAsync();

            // Run indefinately
            await Task.Delay(-1);
        }

        private Task Log(LogMessage log) {
            Console.WriteLine(log.Message);
            return Task.CompletedTask;
        }

        private Task MessageRecieved(SocketMessage message) {
            Communicator communicator;
            ulong id = message.Channel.Id;

            if (communicators.ContainsKey(message.Channel.Id)) {
                communicator = communicators[id];
            } else {
                communicator = new Communicator();
                communicators.Add(id, communicator);
            }

            communicator.HandleMessage(message);

            return Task.CompletedTask;
            
        }

        
    }
}
