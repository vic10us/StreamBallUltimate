﻿using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Api.Models.Undocumented.Chatters;

public class TwitchAPI : MonoBehaviour
{

    public Api api;
    Client getClient;
    TwitchClient twitchClient;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        api = new Api();
        api.Settings.AccessToken = GlobalConfiguration.bot_access_token;
        api.Settings.ClientId = GlobalConfiguration.client_id;
        GameObject client = GameObject.Find("Client");
        twitchClient = client.GetComponent<TwitchClient>();
    }

    // Update is called once per frame
    void Update()
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
        foreach (var chatterObject in listOfChatters)
        {
           // Debug.Log(chatterObject.Username);
        }
    }
}
