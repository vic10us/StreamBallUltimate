#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    //It was the salmonmoose
    [SerializeField] public GameObject shopObject;
    [SerializeField] public Transform shopObjectFirstLocation;
    private MarbleList marbleList;
    private HashSet<GameObject> shopMarbles;
    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Settings.ResetShop.performed += OnResetShop;
        controls.Settings.Enable();
    }

    private void OnResetShop(InputAction.CallbackContext context)
    {
        ResetShop();
    }

    private void OnDisable()
    {
        controls.Settings.Disable();
        controls.Settings.ResetShop.performed -= OnResetShop;
    }

    private void Start()
    {
        marbleList = FindObjectOfType<MarbleList>();
        DisplayShopItems();
    }

    //Creates a shop Object -> sets its name cost and sprite from the Marble List
    public void DisplayShopItems(bool regenerate = true)
    {
        if (regenerate) GenerateShopItems();
        var counter = 0;
        foreach (var item in shopMarbles)
        {
            var shop = Instantiate(shopObject);
            shop.transform.SetParent(transform);
            var marble = item.GetComponent<Marble>();
            var commonName = marble.commonName;
            var cost = marble.cost;
            var sprite = marble.marbleSprite;

            var newShopObject = shop.GetComponent<ShopObject>();
            newShopObject.marbleName.text = commonName;
            newShopObject.marbleCost.text = $"${cost}";
            newShopObject.marbleSpriteRenderer.sprite = sprite;

            var offset  = counter * 1.5f;
            var posY = 3f - offset;
            var shopPos = shopObjectFirstLocation.position;
            shopPos.y = posY;
            shop.transform.position = shopPos;

            counter++;
        }
    }

    private void GenerateShopItems()
    {
        shopMarbles = marbleList.GetMarblesForShop(5);
    }

    public void ResetShop(bool regenerate = true)
    {
        DestroyShopObjectChildren();
        DisplayShopItems(regenerate);
    }

    public void DestroyShopObjectChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public bool MarbleNamesInShop(string checkedMarble)
    {
        return shopMarbles.Any(marble => marble.name.Equals(checkedMarble, StringComparison.InvariantCultureIgnoreCase));
    }

    public IEnumerable<Marble> MarblesInShop() {
        return shopMarbles.Select(m => m.GetComponent<Marble>());
    }
}
