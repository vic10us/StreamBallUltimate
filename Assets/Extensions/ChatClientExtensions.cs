using System.Collections;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace StreamBallUltimate.Assets.Extensions
{
    public static class ChatClientExtensions
    {
        public static IEnumerator SendMessages(this Client client, JoinedChannel channel, string message)
        {
            var messages = message.SplitToLines(500);
            foreach (var m in messages)
            {
                client.SendMessage(channel, m);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
