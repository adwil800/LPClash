using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item
{
    

    //Item = itemName, spriteId

    public enum ItemType
    {
        weapon,
        shield,
        healthPotion, 
        staminaPotion,
        
        
        torso,
        helmet,
        hips,
        shoulders,
        boots,
        arms

    }

    public ItemType itemType;


    public int amount;
    public int slot;
    public string itemName;
    public List<string> stats;
    public Sprite itemSprite;
    public string prefabName;
    


    public enum ItemRarity
    {
       
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Primal,

    }
    public ItemRarity itemRarity;

    public VertexGradient getRarityGradient(ItemRarity itemRarity)
    {
        switch (itemRarity)
        {
            default:
            case ItemRarity.Common:     return new VertexGradient(Color.white, Color.white, Color.white, Color.white);
            case ItemRarity.Uncommon:   return new VertexGradient(Color.green, Color.green, Color.white, Color.white);
            case ItemRarity.Rare:       return new VertexGradient(Color.blue, Color.blue, Color.white, Color.white);
            case ItemRarity.Epic:       return new VertexGradient(Color.magenta, Color.magenta, Color.white, Color.white);
            case ItemRarity.Legendary:  return new VertexGradient(Color.yellow, Color.yellow, Color.white, Color.white);
            case ItemRarity.Primal:     return new VertexGradient(Color.red, Color.red, Color.white, Color.white);

        }

    }

}
