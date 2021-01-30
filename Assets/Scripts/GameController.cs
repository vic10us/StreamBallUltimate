#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField] public TextMeshPro _gameStateText;
    [SerializeField] public Null _nullCharacter;
    [SerializeField] public Timer _timer;
    [SerializeField] public Shop _shop;

    public GameState currentState;
    public GameMode currentGameMode;
    private JumpManager _jumpManager;
    public Shop gameShop;
    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable() {
        controls.Settings.PlayGame.performed += OnPlayGame;
        controls.Settings.ClearMarbles.performed += OnClearMarbles;
        controls.Settings.StopGame.performed += OnStopGame;
        controls.Settings.Enable();
    }

    private void OnDisable()
    {
        controls.Settings.Disable();
        controls.Settings.PlayGame.performed -= OnPlayGame;
        controls.Settings.ClearMarbles.performed -= OnClearMarbles;
        controls.Settings.StopGame.performed -= OnStopGame;
    }

    private void OnStopGame(InputAction.CallbackContext obj)
    {
        DowntimeNow();
    }

    private void OnClearMarbles(InputAction.CallbackContext context)
    {
        StartCoroutine(_jumpManager.DestroyMarbles());
    }
    
    private void OnPlayGame(InputAction.CallbackContext context)
    {
        Debug.Log("Play Game Called");
        PlayNow();
    }

    void Start()
    {
        currentGameMode = GameMode.LongJump;
        currentState = GameState.DownTime;
        _jumpManager = FindObjectOfType<JumpManager>();
        UpdateGameStateText();
    }

    private void UpdateGameStateText()
    {
        // Debug.Log(currentState);
        switch (currentState)
        {
            case GameState.DownTime:
                _gameStateText.text = "Down Time";
                break;
            case GameState.CutScene:
                _gameStateText.text = "Cut Scene";
                break;
            case GameState.GameTime:
                _gameStateText.text = "Game Time";
                break;
            default:
                _gameStateText.text = _gameStateText.text;
                break;
        }
    }

    public void PlayNow() {
        _timer.wasGameTime = false;
        TriggerCutscene(0);
    }

    public void DowntimeNow() {
        _timer.wasGameTime = true;
        TriggerDowntime();
    }

    public void TriggerCutscene(int wait = 10)
    {
        currentState = GameState.CutScene;
        gameShop.gameObject.SetActive(false);
        _nullCharacter.NullStartCutScene(wait);
        UpdateGameStateText();
    }

    public void TriggerGame()
    {
        currentState = GameState.GameTime;
        UpdateGameStateText();
        _timer.ResetGameTimer();
    }

    public void TriggerDowntime()
    {
        _shop.ResetShop();
        currentState = GameState.DownTime;
        UpdateGameStateText();
        _timer.ResetDowntimeTimer();
        StartCoroutine(_jumpManager.DestroyMarbles());
        _nullCharacter.HideCharacter();
        gameShop.gameObject.SetActive(true);
    }

    public string FindGameState()
    {
        switch (currentGameMode)
        {
            case GameMode.LongJump:
                return "Long Jump";
            case GameMode.HighJump:
                return "High Jump";
            case GameMode.Race:
                return "Race";
            default:
                return "YOUR MOM";
        }
    }
}
