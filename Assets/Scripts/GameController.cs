#pragma warning disable 649

using UnityEngine;
using TMPro;

public enum GameState { DownTime, CutScene, GameTime };
public enum GameMode { LongJump, HighJump, Race };
public class GameController : MonoBehaviour
{
    public DataManager dataManager;
    public GameState currentState;
    public GameMode currentGameMode;
    [SerializeField] TextMeshPro gameStateText;
    JumpManager jumpManager;
    public Shop gameShop;
    [SerializeField] Null nullCharacter;
    [SerializeField] Timer timer;
    [SerializeField] Shop shop;

    //The game has states Downtime, Cutscene, Gametime
    //Gametime can link to different game modes longJump, highJump, race 

    void Start()
    {
        currentGameMode = GameMode.LongJump;
        currentState = GameState.DownTime;
        jumpManager = FindObjectOfType<JumpManager>();
        UpdateGameStateText();
    }

    private void UpdateGameStateText()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case GameState.DownTime:
                gameStateText.text = "Down Time";
                break;
            case GameState.CutScene:
                gameStateText.text = "Cut Scene";
                break;
            case GameState.GameTime:
                gameStateText.text = "Game Time";
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(jumpManager.DestroyMarbles());
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            PlayNow();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            DowntimeNow();
        }
    }

    public void PlayNow() {
        timer.wasGameTime = false;
        TriggerCutscene(0);
    }

    public void DowntimeNow() {
        timer.wasGameTime = true;
        TriggerDowntime();
    }

    public void TriggerCutscene(int wait = 10)
    {
        currentState = GameState.CutScene;
        gameShop.gameObject.SetActive(false);
        nullCharacter.NullStartCutScene(wait);
        UpdateGameStateText();
    }

    public void TriggerGame()
    {
        currentState = GameState.GameTime;
        UpdateGameStateText();
        timer.ResetGameTimer();
    }

    public void TriggerDowntime()
    {
        shop.ResetShop();
        currentState = GameState.DownTime;
        UpdateGameStateText();
        timer.ResetDowntimeTimer();
        StartCoroutine(jumpManager.DestroyMarbles());
        nullCharacter.HideCharacter();
        gameShop.gameObject.SetActive(true);
    }

    public string FindGameState()
    {
        Debug.Log(currentGameMode);
        switch (currentGameMode)
        {
            case GameMode.LongJump:
                return "Long Jump";
            case GameMode.HighJump:
                Debug.Log("Launching High Jump");
                return "High Jump";
            case GameMode.Race:
                return "Race";
            default:
                return "YOUR MOM";
        }
    }
}
