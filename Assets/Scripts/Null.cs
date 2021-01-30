#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections;
using UnityEngine;

public class Null : MonoBehaviour
{
    //NULL is a charismatic Lady bug who is the master of ceremony for the stream marble games!
    [SerializeField] public GameController gameController;
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void NullStartCutScene(int wait = 10)
    {
        gameObject.SetActive(true);
        StartCoroutine(StartingDialogue(wait));
    }

    private IEnumerator StartingDialogue(int wait = 10)
    {
        yield return new WaitForSeconds(wait);
        gameController.TriggerGame();
    }

    public void HideCharacter()
    {
        gameObject.SetActive(false);
    }
}
