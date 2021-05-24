using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using cli_life.config;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public readonly Config config;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        /// <summary>
        /// Продолжается симуляция из файла
        /// </summary>
        /// <param name="sb"></param>
        public Board(StateBoard sb) {
            if (sb != null) {
                config = sb.config;
                Cells = sb.GetCell();
                CellSize = config.cellSize;
                ConnectNeighbors();
            } else {
                config = Config.DefaultConfig;
                CellSize = config.cellSize;
                Cells = new Cell[config.width / config.cellSize, config.height / config.cellSize];
                CreateBoard();
            }
                
		}
        /// <summary>
        /// Создается новая симуляция
        /// </summary>
        /// <param name="config"></param>
        public Board(Config config) {
            if (config == null)
                config = Config.DefaultConfig;
            this.config = config;
            CellSize = config.cellSize;
            Cells = new Cell[config.width / config.cellSize, config.height / config.cellSize];
            CreateBoard();
        }
        private void CreateBoard() {
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
            ConnectNeighbors();
            Randomize(config.liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    
	class Program
    {
        public enum State {
            RUN,
            MENU
		}
        public static Dictionary<string, MenuState> menuStates;
        public static MenuState currentState;
        public static State state;
        static Board board;
        static Config config;
        static private void Reset()
        {
            board = new Board(config);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Pause(object sender, ConsoleCancelEventArgs args) {
            state = State.MENU;
            args.Cancel = true;
		}
        static void Menu() {
            Console.Clear();
            Console.WriteLine(currentState.text);
            string key = Console.ReadLine();
            currentState.actions[key](key);
        }
        public static void Save() {
            Console.WriteLine("Введите путь для сохранения");
            string path = Console.ReadLine();
            StateBoard.SaveBoard(board, path);
		}
        public static void Load() {
            Console.WriteLine("Введите путь загрузки");
            string path = Console.ReadLine();
            board = new Board(StateBoard.LoadBoard(path));
            state = State.RUN;
		}
        static void Main(string[] args)
        {
           currentState = new MenuState(
                "1 - Continue\n2 - Save\n3 - Load",
                new Dictionary<string, Action<string>> {
                    {"1",(str) =>  state = State.RUN},
                    {"2",(str) => Save() },
                    {"3",(str) => Load() }
				}
            );


            Console.CancelKeyPress += new ConsoleCancelEventHandler(Pause);
            Reset();
            while(true)
            {
				switch (state) {
                    case State.MENU:
                        Menu();
                        break;
                    case State.RUN:
                        Console.Clear();
                        Render();
                        board.Advance();
                        Thread.Sleep(1000);
                        break;
				}
            }
        }
    }
}
