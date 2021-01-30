#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManager : MonoBehaviour
{
    // I have syntactic sugar
    [SerializeField] public GameObject marbleObject;
    [SerializeField] public Transform longJumpLocation;

    public int orderInLayer;
    public int costToReRoll = 50;

    private MarbleList marbleList;
    private MarbleObject playerMarble;
    private GameData gameData;
    private List<string> longJumpedPlayers = new List<string>();
    private List<string> highJumpedPlayers = new List<string>();
    private readonly SpriteRenderer playerSpriteRenderer;

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        marbleList = FindObjectOfType<MarbleList>();
        orderInLayer = 0;
    }
    public void ResetLongJumpedPlayers()
    {
        longJumpedPlayers = new List<string>();
    }
    
    public void ResetHighJumpedPlayers()
    {
        highJumpedPlayers = new List<string>();
    }

    public void CreateMarbleAndJump(Arrrgs e)
    {
        var userID = e.userID;
        var displayName = e.displayName;

        //if the player has not jumped yet
        if (!longJumpedPlayers.Contains(userID))
        {
            longJumpedPlayers.Add(userID);
            var mb = Instantiate(marbleObject, longJumpLocation.position, transform.rotation);
            mb.transform.SetParent(transform);
            mb.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderInLayer;
            orderInLayer++;
            playerMarble = mb.GetComponentInChildren<MarbleObject>();
            playerMarble.playerName.text = displayName;
            var marbleName = gameData.GetPlayerEquipSkin(userID);
            var marbleGameObject = marbleList.GetMarbleGameObject(marbleName);
            playerMarble.gameMarbleSprite.sprite = marbleGameObject.GetComponent<Marble>().marbleSprite;
            playerMarble.playerID = userID;
        }
        else //if player has already rolled
        {
            if (gameData.CheckPlayerMoney(userID) <= costToReRoll) return;
            var allMarbles = GetComponentsInChildren<MarbleObject>();
            foreach (var marble in allMarbles)
            {
                if (marble.playerID != userID || marble.isRolling) continue;
                Destroy(marble.transform.parent.gameObject);
                gameData.SubtractMoneyFromPlayerID(costToReRoll, userID);
                longJumpedPlayers.Remove(userID);
                CreateMarbleAndJump(e);
            }
            // if marble object isRolling do nothing
            // if marble object is not Rolling then re-roll and charge player 50 monies
        }
    }

    public void CreateMarbleAndHighJump(Arrrgs e)
    {
        var userID = e.userID;
        var displayName = e.displayName;
        if (!(highJumpedPlayers.Contains(userID)))
        {
            highJumpedPlayers.Add(userID);
            var mb = Instantiate(marbleObject, longJumpLocation.position, transform.rotation);
            mb.transform.SetParent(transform);
            mb.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderInLayer;
            orderInLayer++;
            playerMarble = mb.GetComponentInChildren<MarbleObject>();
            playerMarble.playerName.text = displayName;

            var marbleName = gameData.GetPlayerEquipSkin(userID);
            var marbleGameObject = marbleList.GetMarbleGameObject(marbleName);

            playerMarble.gameMarbleSprite.sprite = marbleGameObject.GetComponent<Marble>().marbleSprite;
            playerMarble.playerID = userID;
        }
    }

    public IEnumerator DestroyMarbles()
    {
        var allMarbles = GetComponentsInChildren<MarbleObject>();

        foreach (var marble in allMarbles)
        {
            while (marble.isRolling)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // yield return new WaitForSeconds(5f);
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        longJumpedPlayers = new List<string>();
        orderInLayer = 0;
    }
}

