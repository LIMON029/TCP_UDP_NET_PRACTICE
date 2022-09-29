using System;
using System.Collections.Generic;
using System.Text;

namespace GameServerPractice
{
    class GameLogic
    {
        // 유니티는 필요없음
        public static void Update()
        {
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    _client.player.Update();
                }
            }
            ThreadManager.UpdateMain();
        }
    }
}
