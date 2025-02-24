using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PigProject
{
    public class Game
    {

        public Dictionary<string, int> players = new Dictionary<string, int>();
        public static Random rng = new Random();
        public static int turnCache = 0;    // Turn Total cache
        public static bool gameEnd = false;

        public static bool output;
        public static int delay;
        public static bool usingAI;
        
        public event EventHandler<BroadcastEventArgs> BroadcastTurn;
        public class BroadcastEventArgs : EventArgs
        {
            public required string Player { get; set; }
        }

        /// <summary>
        /// Constructor for Game object
        /// </summary>
        /// <param name="printOut">Option to output messages or not</param>
        /// <param name="timeout">Option for timeout after the end of a game before beginning a new one</param>
        /// <param name="onlyAI">Option for running a test with only AI</param>
        public Game(bool printOut, int timeout, bool onlyAI)
        {
            output = printOut;
            delay = timeout;
            usingAI = onlyAI;
        }

        /// <summary>
        /// Outputs the given string and checks if it can
        /// </summary>
        /// <param name="msg">String to be outputted</param>
        public void PrintOut(string msg)
        {
            if (!output) { return; }
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Processing 
        /// </summary>
        /// <returns>Inputted string</returns>
        public string TryReadLine()
        {
            string? input = Console.ReadLine();
            
            if (input == null)
            {
                return TryReadLine();
            }

            return input;
        }

        /// <summary>
        /// Convert input into Int32
        /// </summary>
        /// <returns>Int32 for players</returns>
        public Int32 ToInt()
        {
            PrintOut("Amount of players: ");
            string input = TryReadLine();
            Int32 num;

            // Break code if not an int
            try
            {
                num = Int32.Parse(input);
            }
            catch (Exception e)
            {
                PrintOut($"Invalid input: {e}");
                return ToInt();
            }

            return num;
        }

        /// <summary>
        /// Starting game sequence
        /// </summary>
        public void Start()
        {

            // Reset variables
            turnCache = 0;
            gameEnd = false;
            //players.Clear();

            // If using ai then stop here
            if (usingAI) { return; }
            players.Clear();

            // Output
            PrintOut("\nPigProject ~ ");
            int numPlayers = 0;

            // Get amount players
            numPlayers = ToInt();

            // Get Players
            PrintOut("\n");
            for (int i = 0; i < numPlayers; i++)
            {
                PrintOut($"\nEnter player name -> {i + 1}");
                string name = TryReadLine();
                players.Add(name, 0);
            }

            return;
        }

        /// <summary>
        /// Processes a players turn and handles input
        /// </summary>
        /// <param name="name">Name of the player to capture input for</param>
        public void PlayerTurn(string name)
        {

            PrintOut($"----------------------------------------");
            PrintOut($"\nPlayer {name}'s turn");
            PrintOut($"Would you like to hold or roll? ({name})");
            string input = TryReadLine().ToLower();

            // Player holds and doubles points
            if (input.Contains("hold") || input.Contains("h"))
            {
                players[name] += turnCache;

                // 100 points, end the game
                if (players[name] >= 100)
                {
                    gameEnd = true;
                    PrintOut($"Player {name} has reached 100 points!");
                    return;
                }

                PrintOut($"Player held. Points added: {turnCache}. Point total: {players[name]}");
                return;
            }

            // Player didn't write rolls
            if (input != "r" && input.Contains("roll") == false)
            {
                PlayerTurn(name);
                return;
            }

            int roll = rng.Next(1, 7);

            // Check if not 1
            if (roll == 1)
            {
                PrintOut($"Player rolled a 1. Turn over. ({name}). Point total: {players[name]}");
                return;
            }

            // Add points
            turnCache += roll;
            PrintOut($"Player rolled a {roll}. Points in current turn: {turnCache}");

            // Recurse the function
            PlayerTurn(name);
            return;
        }

        public void AITurn(char input)
        {
            if (input == 'h')
            {

            }
        }

        /// <summary>
        /// Initiates a game instance
        /// </summary>
        protected virtual void BeginGame()
        {
            // TODO: FIX UP STRUCTURE

            // Only for AI
            // Not very good structure
            List<string> keys = new List<string>(players.Keys);
            if (usingAI)
            {
                while (!gameEnd)
                {
                    foreach (string player in keys)
                    {
                        // Break loop if someone won
                        if (gameEnd) { break; }


                        BroadcastEventArgs args = new()
                        {
                            Player = player
                        };
                        BroadcastTurn?.Invoke(this, args);
                        turnCache = 0;
                    }
                }

                return;
            }

            while (!gameEnd)
            {

                foreach (string player in keys)
                {
                    // Break loop if someone won
                    if (gameEnd) { break; }

                    PlayerTurn(player);
                    turnCache = 0;
                }
            }
        }

        ///<summary>
        /// Sequence after a game instance was completed
        /// </summary>
        public void EndGame()
        {

            PrintOut("\n----------------------------------------");
            PrintOut($"~ Leaderboard ~\n");

            // Output winner
            foreach (KeyValuePair<string, int> player in players)
            {
                Console.WriteLine($"{player.Key} -- {player.Value}");
            }
            PrintOut("\n----------------------------------------\n");

            PrintOut("Rerunning game...\n");
            Thread.Sleep(delay);
        }

        public void Run()
        {
            Start();
            BeginGame();
            EndGame();
        }

    }
}