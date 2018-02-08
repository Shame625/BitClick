using System;
using System.Collections.Generic;
using System.Text;

namespace BitClickServer
{
    public static class LevelManager
    {
        public static void StartChecking(Object src, System.Timers.ElapsedEventArgs e)
        {
            foreach (KeyValuePair<uint, BlockRoom> obj in Program._blockRooms)
            {
                //notify game manager to start timer for next restarted of this level
                if (obj.Value.TimeForReset() && obj.Value.isRestarting == false)
                {
                    
                }
            }
        }
    }
}
