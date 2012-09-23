using System;

namespace BabbleEngine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Engine game = Engine.Instance)
            {
                game.Run();
            }
        }
    }
#endif
}

