#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Api.Models.Undocumented.Chatters;

public class TwitchAPI : MonoBehaviour
{

    public Api api;
    private readonly Client getClient;
    private TwitchClient twitchClient;

    // Start is called before the first frame update
    private void Start()
    {
        Application.runInBackground = true;
        api = new Api();
        api.Settings.AccessToken = GlobalConfiguration.bot_access_token;
        api.Settings.ClientId = GlobalConfiguration.client_id;
        var client = GameObject.Find("Client");
        twitchClient = client.GetComponent<TwitchClient>();
    }

    // Update is called once per frame
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            api.Invoke(api.Undocumented.GetChattersAsync
                (twitchClient.client.JoinedChannels[0].Channel), GetChattersListCallBack);
        }
        */
    }

    private void GetChattersListCallBack(List<ChatterFormatted> listOfChatters)
    {
        //Debug.Log("List of " + listOfChatters.Count + "Viewers: ");
        // foreach (var chatterObject in listOfChatters)
        // {
        //    // Debug.Log(chatterObject.Username);
        // }
    }
}
