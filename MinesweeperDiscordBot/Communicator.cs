using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperDiscordBot {
    class Communicator {

        private Board board;

        private string[] numbers = { "0️⃣", "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9️⃣" };
        private string[] formattedNumbers = { "0️⃣", "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣" };

        public Communicator() {
            board = null;
        }

        public void HandleMessage(SocketMessage message) {
            string content = message.Content.ToLower();

            string[] args = content.Split(" ");
            int[] numbersArgs = new int[args.Length];

            for (int i = 0; i < args.Length; i++) {
                int.TryParse(args[i], out int number);
                numbersArgs[i] = number;
            }

            if (args.Length > 0) {
                switch (args[0]) {
                    case "mine":
                        if (args.Length == 5) {
                            if (args[1] == "init") {
                                board = new Board(numbersArgs[2], numbersArgs[3], numbersArgs[4]);

                                message.Channel.SendMessageAsync(PrintBoard());
                            }
                        }
                        break;
                    case "flag":
                        if (args.Length == 3) {
                            if (board == null) {
                                message.Channel.SendMessageAsync("There is no board for this channel! Initialize with `mine init WIDTH HEIGHT MINES`");
                            }
                            else {
                                board.Flag(numbersArgs[1] - 1, numbersArgs[2] - 1);

                                message.Channel.SendMessageAsync(PrintBoard());
                            }
                        }
                        break;
                    case "sweep":
                    case "clear":
                        if (args.Length == 3) {
                            if (board == null) {
                                message.Channel.SendMessageAsync("There is no board for this channel! Initialize with `mine init WIDTH HEIGHT MINES`");
                            }
                            else {

                                PlaceResult result = board.Clear(numbersArgs[1] - 1, numbersArgs[2] - 1);

                                string reply = "";

                                if (result == PlaceResult.Win) {
                                    reply = "You win!\n";
                                }
                                else if (result == PlaceResult.Loss) {
                                    reply = "You lose!\n";
                                }
                                else if (result == PlaceResult.Invalid) {
                                    reply = "Invalid placement!";
                                    break;
                                }

                                message.Channel.SendMessageAsync(reply + PrintBoard());
                            }
                        }
                        break;
                }
            }
        }

        private string PrintBoard() {
            Cell[,] cells = board.GetCells();

            StringBuilder sb = new StringBuilder();
            for (int x = 0; x <= board.Width; x++) {
                sb.Append(formattedNumbers[x % 10] + " ");
            }

            sb.Append("\n");

            for (int y = 0; y < board.Height; y++) {
                sb.Append(formattedNumbers[(y + 1) % 10]);

                for (int x = 0; x < board.Width; x++) {
                    if (board == null || board.Initialized == false) {
                        sb.Append("◻️");
                    }
                    else {
                        sb.Append(PrintCell(cells[x, y]));
                    }
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }

        private string PrintCell(Cell cell) {
            if (board.State == BoardState.Lost && cell.HasMine) {
                return "💥";
            }

            if (cell.Flagged) {
                return "🚩";
            }

            if (cell.Cleared == false) {
                return "◻️";
            }

            if (cell.MinesNear > 0) {
                return numbers[cell.MinesNear];
            }

            return "◼️";
        }
    }
}
