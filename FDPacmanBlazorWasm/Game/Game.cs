using FDPacmanBlazorWasm.Game;

namespace FDPacmanConsole
{
    public class Game
    {
        public const int Width = 20;
        public const int Height = 15;
        private char[,] board = new char[Height, Width];
        private List<Entity> entities = new List<Entity>();
        private bool isRunning = true;
        private int dotsCollected = 0;
        private DateTime startTime;
        private int _totalDots;
        public int TotalDots => _totalDots;
        public bool IsGameOver { get; private set; } = false;
        public int DotsCollected => dotsCollected;
        public TimeSpan ElapsedTime => DateTime.Now - startTime;

        public Game()
        {
            Initialize();
            startTime = DateTime.Now;
        }

        private void Initialize()
        {
            // Create empty board
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    board[y, x] = ' ';
                }
            }

            // Add walls around the borders
            for (int x = 0; x < Width; x++)
            {
                board[0, x] = '#';
                board[Height - 1, x] = '#';
            }
            for (int y = 0; y < Height; y++)
            {
                board[y, 0] = '#';
                board[y, Width - 1] = '#';
            }

            // Add dots
            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        board[y, x] = '.';
                        _totalDots++;
                    }
                }
            }
            Console.WriteLine("Number of dots: " + _totalDots);

            // Add Pacman
            entities.Add(new Pacman(Height / 2, Width / 2));
            board[Height / 2, Width / 2] = 'P';

            // Add ghosts
            for (int i = 1; i <= 3; i++)
            {
                int ghostX = i * 4;
                int ghostY = i * 4;
                entities.Add(new Ghost(ghostY, ghostX));
                board[ghostY, ghostX] = 'G';
            }
        }

        public void Run()
        {
            while (isRunning)
            {
                HandleInput();
                Update();
                Render();
                Thread.Sleep(100);
            }
        }

        private void Render()
        {
            ClearConsole();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(board[y, x]);
                }
                Console.WriteLine();
            }
        }

        private void ClearConsole()
        {
            Console.Clear();
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        MovePacman(0, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        MovePacman(0, 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        MovePacman(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        MovePacman(1, 0);
                        break;
                }
            }
        }

        public void MovePacman(int xDelta, int yDelta)
        {
            Pacman pacman = entities[0] as Pacman;
            if (pacman != null)
            {
                int newX = pacman.X + xDelta;
                int newY = pacman.Y + yDelta;

                // Check collision with walls
                if (board[newY, newX] == '#' || newY < 0 || newY >= Height || newX < 0 || newX >= Width)
                {
                    return; // Can't move into a wall or out of bounds
                }

                // Check collision with dots
                if (board[newY, newX] == '.')
                {
                    EatDot(pacman);
                    dotsCollected++;
                    Console.WriteLine($"Dot collected. Total dots collected: {dotsCollected}");
                }

                // Update position
                board[pacman.Y, pacman.X] = ' ';
                board[newY, newX] = 'P';
                pacman.X = newX;
                pacman.Y = newY;
            }
        }

        private void EatDot(Pacman pacman)
        {
            // Simple dot eating logic
            // You can add scoring here if needed
            board[pacman.Y, pacman.X] = ' ';
        }

        public void Update()
        {
            foreach (Entity entity in entities)
            {
                if (entity is Ghost ghost)
                {
                    MoveGhost(ghost);
                }
            }

            CheckCollisions();
            Console.WriteLine($"Dots collected: {dotsCollected}, Total dots: {_totalDots}");
            if (dotsCollected == _totalDots)
            {
                GameOver();
            }
        }

        private void MoveGhost(Ghost ghost)
        {
            // Simple AI: move randomly
            Random random = new Random();
            int xDelta = random.Next(-1, 2); // -1, 0, or +1
            int yDelta = random.Next(-1, 2);

            int newX = ghost.X + xDelta;
            int newY = ghost.Y + yDelta;

            if (newX < 0 || newX >= Width || newY < 0 || newY >= Height || board[newY, newX] == '#')
            {
                return; // Can't move out of bounds or into walls
            }

            board[ghost.Y, ghost.X] = ' ';
            board[newY, newX] = 'G';
            ghost.X = newX;
            ghost.Y = newY;
        }

        private void CheckCollisions()
        {
            Pacman pacman = entities[0] as Pacman;
            if (pacman == null)
            {
                return;
            }

            foreach (Entity entity in entities)
            {
                if (entity is Ghost ghost && ghost.X == pacman.X && ghost.Y == pacman.Y)
                {
                    GameOver();
                }
            }
        }

        private void GameOver()
        {
            IsGameOver = true;
            isRunning = false;
            Console.WriteLine("Game Over!");
        }

        public char GetBoardChar(int y, int x)
        {
            return board[y, x];
        }

        public void PrintBoard()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(board[y, x]);
                }
                Console.WriteLine();
            }
        }
    }
}
