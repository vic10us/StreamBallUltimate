#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;
using TMPro;

public class ShopObject : MonoBehaviour
{
    [SerializeField] public TMP_Text marbleName;
    [SerializeField] public TextMeshPro marbleCost;
    [SerializeField] public SpriteRenderer marbleSpriteRenderer;
}
