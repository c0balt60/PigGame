using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection.Metadata.Ecma335;

namespace PigProject
{

    class AI
    {
        Game inst;
        public string id;
        static int currentTurns = 0;
        static int saveAfterTurn;
        static int saveAfterPoints = 25;

        public AI(Game game, int? maximumSave)
        {
            inst = game;
            saveAfterTurn = maximumSave!=null ? (int)maximumSave : 10;
            id = Guid.NewGuid().ToString();

            // Add event subscription
            // Remove warning for incompatability of AITurn args && EventHandler<BroacastArgs>
            game.BroadcastTurn += AITurn;
        }

        public void AITurn(object sender, BroadcastArgs e)
        {
            
            if (e.Player != id) { return; }
            Console.WriteLine($"\nCaptured ({e.Player});");

            Tuple<bool, int> turn;

            // Logic
            if (inst.turnNumber >= saveAfterTurn || inst.turnCache >= saveAfterPoints)

            // AI rolled more or scored more than threshold, save points
            turn = inst.AITurn(id, 'h');
            

            // Run function default
            else turn = inst.AITurn(id, 'r');

            Console.WriteLine($"Turn Total: {inst.turnCache}; Turn Number: {inst.turnNumber}");

            // Check if turn isn't over
            if (turn.Item1 == true)
            {
                AITurn(sender, e);
                return;
            }
            else
            {

                // Change bounds depending on game result
                saveAfterTurn += inst.turnNumber >= saveAfterTurn ? 1 : -1;
                saveAfterPoints += inst.turnCache >= saveAfterPoints ? 1 : -1;

                Console.WriteLine("END TURN\n");
            }
            return;
        }
    }

    class PigProject
    {

        //static bool using_AI = false;
        static int ai_opponents = 4;
        static List<AI> opponents = new List<AI>();

        /// <summary>
        /// Slightly misleading name
        /// Runs the AI's to battle each other
        /// </summary>
        /// <param name="instance">Active game instance</param>
        public static void _benchmark_ai(Game instance)
        {

            for (int i = 0; i < ai_opponents; i++)
            {
                AI opponent = new AI(instance, 3);

                // Cache AI object reference to save in memory
                opponents.Add(opponent);

                instance.players.Add(opponent.id, 0);
                Console.WriteLine($"New: {i}");
            }

            Console.WriteLine($"Events: {instance.Count()}");
            instance.Run();
        }

        public static void Main(string[] args)
        {

            // Example PVP battle
            Game inst = new(
                true,
                5000,
                false
             );

            // Example AI battle
            //Game inst = new(
            //    true,       // Output messages to console
            //    500,         // Delay between turns
            //    true        // Uses only ai
            // );
            //_benchmark_ai(inst);

        }
    }

}