#pragma warning disable 649

using System.Collections;
using UnityEngine;

public class Null : MonoBehaviour
{
    //NULL is a charasmatic Lady bug who is the master of ceremony for the stream marble games!
    [SerializeField] GameController gameController;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void NullStartCutScene(int wait = 10)
    {
        gameObject.SetActive(true);
        StartCoroutine(StartingDialogue(wait));
    }

    IEnumerator StartingDialogue(int wait = 10)
    {
        yield return new WaitForSeconds(wait);
        gameController.TriggerGame();
    }

    public void HideCharacter()
    {
        gameObject.SetActive(false);
    }
}
