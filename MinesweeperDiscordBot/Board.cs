using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperDiscordBot {
    public enum PlaceResult {
        Success,
        Invalid,
        Loss,
        Win
    }

    public enum BoardState {
        Won,
        Lost,
        Playing,
        Started
    }

    class Board {
        private Cell[,] cells;
        public int Width { protected set; get; }
        public int Height { protected set; get; }
        public int MineCount { protected set; get; }

        public BoardState State {protected set; get; }
        public bool Initialized { protected set; get; }


        public Board(int width, int height, int mineCount) {
            this.Width = width;
            this.Height = height;
            this.MineCount = mineCount;
            Initialized = false;
            State = BoardState.Started;
        }

        private void Initialize(int x, int y) {
            State = BoardState.Playing;
            bool[,] mines = GenerateMines(x, y);
            int[,] nearGrid = CalculateNearGrid(mines);


            cells = new Cell[Width, Height];
            for (int xx = 0; xx < Width; xx++) {
                for (int yy = 0; yy < Height; yy++) {
                    cells[xx, yy] = new Cell(mines[xx, yy], nearGrid[xx, yy]);
                }
            }

            Initialized = true;
        }

        public PlaceResult Clear(int x, int y) {
            if (Initialized == false) {
                Initialize(x, y);
            }

            if (x < 0 || x >= Width || y < 0 ||y >= Height) {
                return PlaceResult.Invalid;
            }

            Cell cell = cells[x, y];

            if (cell.Flagged || cell.Cleared) {
                return PlaceResult.Invalid;
            }

            if (cell.HasMine) {
                State = BoardState.Lost;
                return PlaceResult.Loss;
            }

            cell.Clear();

            if (cell.MinesNear == 0) {
                for (int xx = x - 1; xx <= x + 1; xx++) {
                    for (int yy = y - 1; yy <= y + 1; yy++) {
                        if (xx != x || yy != y) {
                            Clear(xx, yy);
                        }
                    }
                }
            }

            if (CheckWin()) {
                return PlaceResult.Win;
            }

            return PlaceResult.Success;
        }

        public void Flag(int x, int y) {
            if (Initialized == false) {
                return;
            }

            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                return;
            }

            Cell cell = cells[x, y];

            if (cell.Cleared) {
                return;
            }

            cell.Flag();
        }

        private bool CheckWin() {
            for (int x=0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (cells[x, y].Cleared == false && cells[x,y].HasMine == false) {
                        return false;
                    }
                }
            }

            return true;
        }

        private int[,] CalculateNearGrid(bool[,] mines) {
            int[,] near = new int[Width, Height];

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (mines[x, y] == true) {
                        continue;
                    }

                    near[x, y] = CalculateNear(mines, x, y);
                }
            }

            return near;
        }

        private int CalculateNear(bool[,] mines, int x, int y) {
            int count = 0;
            int x0 = Math.Clamp(x - 1, 0, Width - 1);
            int x1 = Math.Clamp(x + 1, 0, Width - 1);
            int y0 = Math.Clamp(y - 1, 0, Height - 1);
            int y1 = Math.Clamp(y + 1, 0, Height - 1);

            for (int xx = x0; xx <= x1; xx++) {
                for (int yy = y0; yy <= y1; yy++) {
                    if (mines[xx, yy]) {
                        count++;
                    }
                }
            }

            return count;
        }


        private bool[,] GenerateMines(int startX, int startY) {
            bool[,] mines = new bool[Width, Height];
            Random rnd = new Random();

            int minesToPlace = MineCount;
            for (int i=0;  minesToPlace > 0; i++) {
                if (i > 1000) {
                    break;
                }

                int x = rnd.Next(Width);
                int y = rnd.Next(Height);

                if (Math.Abs(x-startX) < 2 && Math.Abs(y-startY) < 2) {
                    continue;
                }

                if (mines[x, y] == true) {
                    continue;
                }

                mines[x, y] = true;
                minesToPlace--;
                i = 0;
            }

            return mines;
        }

        public Cell[,] GetCells() {
            return cells;
        }
    }
}
