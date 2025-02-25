using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PigProject
{
    /// <summary>
    /// Interface for BroadcastTurn event
    /// </summary>
    class BroadcastArgs : EventArgs
    {
        public string? Player { get; set; }
        public int? Turn { get; set; }      // A little bit useless, subbed out for tuple return

    }

    public class Game
    {
        // player cache
        public Dictionary<string, int> players = new Dictionary<string, int>();

        // input vars
        public static Random rng = new Random();
        public int turnNumber = 0;          // Tracking the turn number for a player
        public int turnCache = 0;           // Turn Total cache
        public bool infinite = true;       // Infinitely recurses the Run method
        public Tuple<string, int> winner;   // Winner of the game

        // static vars
        public static bool gameEnd = false;
        public static bool output;
        public static int delay;
        public static bool usingAI;

        /// <summary>
        /// Event that handles broadcasting turns to AI for decision making.
        /// </summary>
        internal event EventHandler<BroadcastArgs> BroadcastTurn;

        /// <summary>
        /// Count the number of subscriptions in BroadcastTurn
        /// </summary>
        /// <returns>number of listeners</returns>
        public int Count()
        {
            return BroadcastTurn.GetInvocationList().Length;
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
        public static void PrintOut(string msg)
        {
            if (!output) { return; }
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Processing user input
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
            winner = Tuple.Create("", 0);
            foreach (KeyValuePair<string, int> player in players) players[player.Key] = 0;

            // If using ai then stop here
            if (usingAI) { return; }
            players.Clear();

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

        /// <summary>
        /// Processes a turn for AI
        /// Returns an Tuple(false,0) if the turn ended 
        /// </summary>
        /// <param name="name">GUID for the AI</param>
        /// <param name="input">The provided input of the AI</param>
        /// <returns>bool: is players turn?, int: current turn</returns>
        public Tuple<bool,int> AITurn(string name, char input)
        {
            if (input == 'h')
            {
                players[name] += turnCache;

                // 100 points, end the game
                if (players[name] >= 100)
                {
                    gameEnd = true;
                    PrintOut($"\t➤ Player {name} has reached 100 points!");
                    winner = Tuple.Create(name, players[name]);
                    return Tuple.Create(false, 0);
                }

                PrintOut($"\t➤ Player held. Points added: {turnCache}. Point total: {players[name]}");
                return Tuple.Create(false, 0);
            }
            else if (input == 'r')
            {
                int roll = rng.Next(1, 7);

                // Check if not 1
                if (roll == 1)
                {
                    PrintOut($"\t➤ Player rolled a 1. Turn over. ({name}). Point total: {players[name]}");
                    return Tuple.Create(false, 0);
                }

                // Add points
                turnCache += roll;
                PrintOut($"\t➜ Player rolled a {roll}. Points in current turn: {turnCache}. Overall: {players[name]}");

                // Fire another event
                turnNumber += 1;

                //_broadcastTurn?.Invoke(this, new BroadcastArgs() { Player = name });
                return Tuple.Create(true, turnNumber - 1);
            }
            return Tuple.Create(false, 0);
        }
        
        /// <summary>
        /// Initiates a game instance
        /// </summary>
        protected virtual void BeginGame()
        {
            // TODO: FIX UP STRUCTURE
            
            // Only for AI
            // Not very good structure
            List<string> keys = [.. players.Keys];
            if (usingAI)
            {
                while (!gameEnd)
                {
                    foreach (string player in keys)
                    {
                        // Break loop if someone won
                        if (gameEnd) { break; }
                        Thread.Sleep(delay);

                        BroadcastArgs args = new();
                        args.Player = player;
                        BroadcastTurn?.Invoke(this, args);

                        turnNumber = 0;
                        turnCache = 0;
                        Console.WriteLine($"Reset: {player}");
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
            PrintOut($"\nWinner: {winner.Item1}!!!\nScore: {winner.Item2}\n\n");
            PrintOut($"~ Leaderboard ~\n");

            // Output winner
            foreach (KeyValuePair<string, int> player in players)
            {
                Console.WriteLine($"{player.Key} -- {player.Value}");
            }
            PrintOut("\n----------------------------------------\n");

            if (infinite) PrintOut("Rerunning game...\n");
            Thread.Sleep(delay);
        }

        public void Run()
        {
            Start();
            BeginGame();
            EndGame();

            // Infinite option
            if (infinite) Run();
        }

    }
}