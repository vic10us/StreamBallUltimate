#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections;
using UnityEngine;
using TMPro;

public class MarbleObject : MonoBehaviour
{
    //Instantiate object that can do a task, collide with boundaries, provide locational information for points scoring
    //Ball needs to be tied to a player chat ID
    //Can only have one marble per player, should not instantiate if player has ball in play

    //FOR EVENTS:  -go to appropriate starting location 
    //                    -give instructions on where to go (should have random factor)
    //                    -De-spawn Object

    [SerializeField] public float highJumpForce = 1.0f;
    [SerializeField] public TextFollow marbleText;
    public string playerID;
    public bool isRolling;

    public SpriteRenderer gameMarbleSprite;
    public TextMeshPro playerName;
    private Rigidbody2D rb;
    private GameData gameData;
    private Commands commands;
    private GameController gameController;
    private string jumpDistance;
    private string gameState;
    private float longJumpForce;
    private float speed;

    private void Start()
    {
        isRolling = true;
        rb = GetComponent<Rigidbody2D>();
        FreeRotation();
        gameData = FindObjectOfType<GameData>();
        gameController = FindObjectOfType<GameController>();
        gameState = gameController.FindGameState();
        ActivateGameState(gameState);
    }

    private void ActivateGameState(string state)
    {
        switch (state)
        {
            case "Long Jump":
                LongJump();
                break;
            case "High Jump":
                HighJump();
                break;
        }
    }

    public void LongJump()
    {
        //How far player jumps
        var percentage = Random.Range(0, 10001);
        if (percentage == 10000)
        {
            commands = FindObjectOfType<Commands>();
            commands.AkaiEasterEgg(playerName.text);
        }
        //Force acting on the marble
        var range = 4.8f + (((7.9f / 10000f) * percentage));
        var distance = ((100f / 10000f) * percentage);
        jumpDistance = $"{distance:0.00}";
        var score = Mathf.RoundToInt(distance);
        longJumpForce = range;
        rb.AddForce(new Vector2(longJumpForce, 0), ForceMode2D.Impulse);
        StartCoroutine(WaitUntilMovementStops(playerID,score));

    }
    public void HighJump()
    {
        //How far player jumps
        var percentage = Random.Range(0, 10001);
        if (percentage == 10000)
        {
            commands = FindObjectOfType<Commands>();
            commands.AkaiEasterEgg(playerName.text);
        }
        rb.AddForce(new Vector2(0, -1*(highJumpForce)), ForceMode2D.Impulse);
    }

    private IEnumerator WaitUntilMovementStops(string ID, int money)
    {
        do
        {
            speed = rb.velocity.magnitude;
            yield return new WaitForSeconds(0.5f);
        }
        while (speed > 0);
        marbleText.TriggerAnimation();
        gameData.AddMoneyToPlayerID(money, ID);
        isRolling = false;
    }

    public void LockRotation()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void FreeRotation()
    {
        rb.constraints = RigidbodyConstraints2D.None;
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        var otherGameObject = otherCollider.gameObject;
        if (otherGameObject.layer == 9)
        {
            LockRotation();
        } 
    }

    public void TransitionToScoreText()
    {
        playerName.text += $"\n{jumpDistance}";
    }
}
