using FDPacmanBlazorWasm.Game;
using System;

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
        // Add a single Random instance at the class level
        private readonly Random random = new Random();

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

            // Add random walls
            GenerateWalls(10); // Generate a fixed number of walls

            // Add dots
            GenerateDots(10); // Generate a fixed number of dots
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

        private void GenerateWalls(int numberOfWalls)
        {
            // Random random = new Random();
            int wallsPlaced = 0;

            while (wallsPlaced < numberOfWalls)
            {
                int x = random.Next(1, Width - 1);
                int y = random.Next(1, Height - 1);

                if (board[y, x] == ' ' && !IsCornerBlocked(x, y))
                {
                    board[y, x] = '#';
                    wallsPlaced++;
                }
            }
        }

        private bool IsCornerBlocked(int x, int y)
        {
            // Check if placing a wall here would block a dot in a corner
            if (board[y, x] == '.')
            {
                return true;
            }

            // Check surrounding cells to ensure no corner is blocked
            if (x > 1 && y > 1 && board[y - 1, x] == '.' && board[y, x - 1] == '.')
            {
                return true;
            }
            if (x < Width - 2 && y > 1 && board[y - 1, x] == '.' && board[y, x + 1] == '.')
            {
                return true;
            }
            if (x > 1 && y < Height - 2 && board[y + 1, x] == '.' && board[y, x - 1] == '.')
            {
                return true;
            }
            if (x < Width - 2 && y < Height - 2 && board[y + 1, x] == '.' && board[y, x + 1] == '.')
            {
                return true;
            }

            return false;
        }

        private void GenerateDots(int numberOfDots)
        {
            //Random random = new Random();
            int dotsPlaced = 0;

            while (dotsPlaced < numberOfDots)
            {
                int x = random.Next(1, Width - 1);
                int y = random.Next(1, Height - 1);

                if (board[y, x] == ' ')
                {
                    board[y, x] = '.';
                    dotsPlaced++;
                    _totalDots++;
                }
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
                    EatDot(newY, newX);
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

        private void EatDot(int y, int x)
        {
            // Simple dot eating logic
            board[y, x] = ' ';
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
            // Random random = new Random();
            int xDelta = random.Next(-1, 2); // -1, 0, or +1
            int yDelta = random.Next(-1, 2);

            int newX = ghost.X + xDelta;
            int newY = ghost.Y + yDelta;

            // Don't move if hitting bounds or walls
            if (newX < 0 || newX >= Width || newY < 0 || newY >= Height || board[newY, newX] == '#')
            {
                return;
            }

            // Store what's currently at the new position
            char newPositionChar = board[newY, newX];

            // Restore the previous position with what was under the ghost
            board[ghost.Y, ghost.X] = ghost.UnderChar;

            // Update ghost's position
            ghost.X = newX;
            ghost.Y = newY;

            // If moving to a position with a dot, keep the dot for later restoration
            if (newPositionChar == '.')
            {
                ghost.UnderChar = '.';
            }
            else if (newPositionChar == 'G' || newPositionChar == 'P')
            {
                // If moving to a position with another entity, the underlying space is empty
                ghost.UnderChar = ' ';
            }
            else
            {
                ghost.UnderChar = newPositionChar;
            }

            // Place ghost at new position
            board[newY, newX] = 'G';
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




