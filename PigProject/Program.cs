using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PigProject
{

    class AI
    {
        Game inst;
        public string id;
        static int currentTurns = 0;
        static int saveAfter;

        public AI(Game game, int maximumSave)
        {
            inst = game;
            saveAfter = maximumSave;
            id = Guid.NewGuid().ToString();
        }

        public delegate void AITurnEventHandler(object sender, BroadcastArgs e);
        public void AITurn(object sender, BroadcastArgs e)
        {
            Console.WriteLine($"Captured {e.Player}");
            if (e.Player != id) { return; }

            // Run function
            inst.AITurn(id, 'r');
        }

        public void Run(Game instance)
        {

            // Add event subscription
            // Remove warning for incompatability of AITurn args && EventHandler<BroacastArgs>
#pragma warning disable CS8622
            instance.BroadcastTurn += AITurn;
#pragma warning restore CS8622
        }
    }

    class PigProject
    {

        static bool using_AI = false;
        static int ai_opponents = 2;
        static List<AI> opponents = new List<AI>();

        public static void _benchmark_ai(Game instance)
        {

            for (int i = 0; i < ai_opponents; i++)
            {
                AI opponent = new AI(instance, 3);
                opponent.Run(instance);
                opponents.Add(opponent);
                instance.players.Add(opponent.id, 0);
            }

            instance.Run();
        }

        public static void Main(string[] args)
        {

            //Game inst = new Game(true, 5000, false);
            //inst.Run();

            // For AI
            Game inst = new(true, 1000, true);
            _benchmark_ai(inst);

        }
    }

}