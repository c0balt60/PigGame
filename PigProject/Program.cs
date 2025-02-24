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

        public string id;
        static int currentTurns = 0;
        static int saveAfter;

        public AI(int maximumSave)
        {
            saveAfter = maximumSave;
            id = Guid.NewGuid().ToString();
            Console.WriteLine(id);
        }

        public static void AITurn(object sender,  e)
        {
            Console.WriteLine("Captured", e.Player);
        }

        public void Run(Game instance)
        {

            // Event
            instance.BroadcastTurn += AITurn;
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
                AI opponent = new AI(3);
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
            Game inst = new(true, 0, true);
            _benchmark_ai(inst);

        }
    }

}