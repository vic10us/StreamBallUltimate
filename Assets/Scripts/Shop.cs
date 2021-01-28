#pragma warning disable 649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //It was the salmonmoose
    [SerializeField] GameObject shopObject;
    [SerializeField] Transform[] shopObjectLocations;
    MarbleList marbleList;
    HashSet<GameObject> shopMarbles;

    void Start()
    {
        marbleList = FindObjectOfType<MarbleList>();
        DisplayShopItems();
    }

    //Creates a shop Object -> sets its name cost and sprite from the Marble List
    public void DisplayShopItems(bool regenerate = true)
    {
        if (regenerate) GenerateShopItems();
        int counter = 0;
        foreach (var item in shopMarbles)
        {
            GameObject shop = Instantiate(shopObject);
            shop.transform.SetParent(transform);
            Marble marble = item.GetComponent<Marble>();
            string name = marble.commonName;
            int cost = marble.cost;
            Sprite sprite = marble.marbleSprite;

            ShopObject newShopObject = shop.GetComponent<ShopObject>();
            newShopObject.marbleName.text = name;
            newShopObject.marbleCost.text = $"${cost}";
            newShopObject.marbleSpriteRenderer.sprite = sprite;

            var offset  = counter * 1.5f;
            var posY = 3f - offset;
            var shopPos = shopObjectLocations[0].position;
            shopPos.y = posY;
            shop.transform.position = shopPos;

            // if (counter < shopObjectLocations.Length)
            // {
            //     shop.transform.position = shopObjectLocations[counter].position;
            // }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetShop();
        }
    }

    public void DestroyShopObjectChildren()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public bool MarbleNamesInShop(string checkedMarble)
    {
        bool marbleInShop = false;
        foreach (var marble in shopMarbles)
        {
            Debug.Log(marble.name);
            Debug.Log(checkedMarble);
            if (marble.name.ToLower() == checkedMarble)
            {
                marbleInShop = true;
            }
        }
        return marbleInShop;
    }

    public IEnumerable<Marble> MarblesInShop() {
        return shopMarbles.Select(m => m.GetComponent<Marble>());
    }
}
