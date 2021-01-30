#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;

public class TextFollow : MonoBehaviour
{
    [SerializeField] public Transform marblePos;
    [SerializeField] public MarbleObject myMarbleObject;

    private readonly Vector3 offset = new Vector3(0, .9f, 0);
    private Transform textPos;
    private Animator textAnimator;

    private void Start()
    {
        textPos = GetComponent<Transform>();
        textAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        textPos.position = marblePos.position + offset;
    }
    public void DisplayScore()
    {
        myMarbleObject.TransitionToScoreText();
    }

    public void TriggerAnimation()
    {
        textAnimator.SetTrigger("showScore");
    }
}
