using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{



    //Inventory list
    public List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        //addItem(new Item { itemType = Item.ItemType.weapon, amount = 1 });



    }

    public void addItem(Item item)
    {
        itemList.Add(item);
    }
    public void deleteItem(Item item)
    {
        itemList.Remove(item);
    }

    public List<Item> getItemList()
    {
        return itemList;
    }
    
    public int getListCount()
    {
        return itemList.Count;
    }




}

