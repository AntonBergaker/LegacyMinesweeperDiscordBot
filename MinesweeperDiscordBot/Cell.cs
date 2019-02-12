using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperDiscordBot {
    class Cell {
        public bool Flagged { get; protected set; }
        public bool HasMine { get; protected set; }
        public int MinesNear { get; protected set;}
        public bool Cleared { get; protected set; }


        public Cell(bool hasMine, int near) {
            MinesNear = near;
            Flagged = false;
            Cleared = false;
            HasMine = hasMine;
        }

        public void Flag() {
            Flagged = !Flagged;
        }

        public void Clear() {
            Cleared = true;
        }
    }
}
