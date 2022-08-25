using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemPickup : MonoBehaviour
{

    //To update coins
    playerStats statsScript;

    referenceCarrier reference;
    inventoryManagement invScript;

    Weapon weaponScript;
    Shield shieldScript;
    Armor armorScript;
    Potion potionScript;
    Coin coinScript;

    public float pickupRange = 6;

    void Start()
    {

        statsScript = GetComponent<playerStats>();
        invScript = GetComponent<inventoryManagement>();
        reference = invScript.FindObjectInChilds(this.gameObject, "Player").GetComponent<referenceCarrier>();

    }


    void Update()
    {

        handlePickup();

    }




    void handlePickup()
    {
        RaycastHit hit;

        if (Physics.Raycast(reference.thirdPersonCamera.transform.position, reference.thirdPersonCamera.transform.forward, out hit, pickupRange))
        {


            if (hit.collider.gameObject.layer == 3)
            {

                if (Input.GetKeyDown(KeyCode.E) && invScript.inventory.getListCount() < 30)
                {

                    //Verify if weapon
                    if (hit.transform.tag == "weapon")
                    {
                        weaponScript = hit.transform.GetComponent<Weapon>();
                    }
                    //Verify if shield
                    else if (hit.transform.tag == "shield")
                    {
                        // Debug.Log("is shield");
                        shieldScript = hit.transform.GetComponent<Shield>();
                    }
                    else if (hit.transform.tag == "armor")
                    {
                        // Debug.Log("is shield");
                        armorScript = hit.transform.GetComponent<Armor>();
                    }
                    else if (hit.transform.tag == "potion")
                    {
                        potionScript = hit.transform.GetComponent<Potion>();


                    }
                    else if (hit.transform.tag == "coin")
                    {
                        coinScript = hit.transform.GetComponent<Coin>();

                        statsScript.currentCoins = coinScript.randomize();
                        return;
                    }

                    pickUpInv(hit.collider.gameObject);
                }







            }



        }



    }

    void pickUpInv(GameObject pickable)
    {

        List<string> itemStats = new List<string>();
        //Check for available slot
        int useSlot = 0;
        if (invScript.inventory.getListCount() > 0)
        {
            useSlot = invScript.availableSlots();
        }

        //Item rarity
        Item.ItemRarity rarity;
        if (rarityRandomizer() == "common")
            rarity = Item.ItemRarity.Common;
        else if (rarityRandomizer() == "uncommon")
            rarity = Item.ItemRarity.Uncommon;
        else if (rarityRandomizer() == "rare")
            rarity = Item.ItemRarity.Rare;
        else if (rarityRandomizer() == "epic")
            rarity = Item.ItemRarity.Epic;
        else if (rarityRandomizer() == "legendary")
            rarity = Item.ItemRarity.Legendary;
        else if (rarityRandomizer() == "primal")
            rarity = Item.ItemRarity.Primal;
        else
            rarity = Item.ItemRarity.Common;

        Debug.Log("Current rarity: " + rarity);



        //IF POTION HANDLE TYPE
        Item.ItemType potionType = Item.ItemType.healthPotion;
        if (potionScript)
        {
            //Item rarity
            if (potionScript.potionType.ToLower() == "health")
                potionType = Item.ItemType.healthPotion;
            else if (potionScript.potionType.ToLower() == "stamina")
                potionType = Item.ItemType.staminaPotion;

        }





        //Generate stat array
        if (weaponScript)
        {
            handleWPRarityMultiplier(rarity, Item.ItemType.weapon);

            itemStats.Add("Damage: " + weaponScript.damage);
            itemStats.Add("Attack speed: " + weaponScript.attackSpeed);
            itemStats.Add("Durability: " + weaponScript.durability + "/" + weaponScript.maxDurability);
            itemStats.Add("Sword");

            invScript.inventory.addItem(new Item
            {
                itemType = Item.ItemType.weapon,
                amount = 1,
                itemSprite = pickable.transform.GetChild(0).GetComponent<Image>().sprite,
                slot = useSlot,
                stats = itemStats,
                itemName = pickable.name,
                prefabName = pickable.name,
                itemRarity = rarity,
            });
        }
        else if (shieldScript)
        {
            handleSHRarityMultiplier(rarity, Item.ItemType.shield);

            itemStats.Add("Block damage: " + shieldScript.damageBlock);
            itemStats.Add("Durability: " + shieldScript.durability + "/" +shieldScript.maxDurability);
            itemStats.Add("Shield");


            invScript.inventory.addItem(new Item
            {
                itemType = Item.ItemType.shield,
                amount = 1,
                itemSprite = pickable.transform.GetChild(0).GetComponent<Image>().sprite,
                slot = useSlot,
                stats = itemStats,
                itemName = pickable.name,
                prefabName = pickable.name, 
                itemRarity = rarity,
            });

        }
        else if (armorScript)
        {
            handleAMRarityMultiplier(rarity, getArmorType(armorScript.equipmentType));

            itemStats.Add("Defense: " + armorScript.defense);

                if (armorScript.hp > 0)
                {
                    itemStats.Add("Hp: " + armorScript.hp);
                }

            itemStats.Add("Durability: " + armorScript.durability + "/" + armorScript.maxDurability);
            itemStats.Add(armorScript.equipmentType);

            //Check for itemType
            invScript.inventory.addItem(new Item
            {
                itemType = getArmorType(armorScript.equipmentType),
                amount = 1,
                itemSprite = pickable.transform.GetChild(0).GetComponent<Image>().sprite,
                slot = useSlot,
                stats = itemStats,
                itemName = armorCustomName(pickable.name),
                prefabName = pickable.name,
                itemRarity = rarity,
            });

        }
        else if (potionScript)
        {

            itemStats.Add("Healing: " + potionScript.amount);


            itemStats.Add($"{potionScript.potionType} potion");

            //Check for itemType
            invScript.inventory.addItem(new Item
            {
                itemType = potionType,
                amount = 1,
                itemSprite = pickable.transform.GetChild(0).GetComponent<Image>().sprite,
                slot = useSlot,
                stats = itemStats,
                itemName = armorCustomName(pickable.name),
                prefabName = pickable.name, 
            });

        }

        //Add item and refresh
        invScript.refreshInventoryItems();
        invScript.refreshEquippedItems();


        Destroy(pickable);

    }

    string armorCustomName(string originalName)
    {
        switch (originalName)
        {
            default:
            //Set 01
            case "Chr_Head_No_Elements_Female_06_Static": return "King's helmet";
            case "Chr_Torso_Female_19_Static": return "King's torso";
            case "Chr_Hips_Female_20_Static": return "King's hips";
            case "Chr_LegRight_Female_12_Static": return "King's boots";
            case "Chr_ShoulderAttachRight_03_Static": return "King's shoulders";
            case "Chr_HandRight_Female_12_Static": return "King's arms";
            //Set 02
            case "Chr_Head_No_Elements_Female_02_Static": return "Knight helmet";
            case "Chr_Torso_Female_21_Static": return "Knight torso";
            case "Chr_Hips_Female_02_Static": return "Knight legs";
            case "Chr_LegRight_Female_02_Static": return "Knight boots";
            case "Chr_ShoulderAttachRight_05_Static": return "Knight shoulders";
            case "Chr_HandRight_Female_06_Static": return "Knight arms";
            
        }
    }


    Item.ItemType getArmorType(string originalName)
    {
        switch (originalName)
        {
            default:
            //Set 02
            case "helmet": return Item.ItemType.helmet;
            case "boots": return Item.ItemType.boots;
            case "torso": return Item.ItemType.torso;
            case "hips": return Item.ItemType.hips;
            case "shoulders": return Item.ItemType.shoulders;
            case "arms": return Item.ItemType.arms;
        }
    }






    /*
     * RarityRandomizer : Generate rarity
     * getRarityGradient : Get the color when necessary at the tooltip
     * rarityAdvantageMultiplier : Get advantage and the multiplier when necessary
     */

    void handleWPRarityMultiplier(Item.ItemRarity itemRarity, Item.ItemType itemType)
    {

        string [] advantageMultiplier = rarityAdvantageMultiplier(itemRarity, itemType).Split(':');

        //Check for stat and get multiplier

        switch (advantageMultiplier[0])
        {
            case "damage":
                    weaponScript.damage = Mathf.Ceil(weaponScript.damage * float.Parse(advantageMultiplier[1]));
                break;

            case "attSpeed":
                    weaponScript.attackSpeed = Mathf.Ceil(weaponScript.attackSpeed * float.Parse(advantageMultiplier[1]));
                break;

            case "durability":
                    weaponScript.durability = Mathf.Ceil(weaponScript.durability * float.Parse(advantageMultiplier[1]));
                    weaponScript.maxDurability = Mathf.Ceil(weaponScript.maxDurability * float.Parse(advantageMultiplier[1]));
                break;
        }



    }
    void handleSHRarityMultiplier(Item.ItemRarity itemRarity, Item.ItemType itemType)
    {
        string[] advantageMultiplier = rarityAdvantageMultiplier(itemRarity, itemType).Split(':');

        //Check for stat and get multiplier

        switch (advantageMultiplier[0])
        {
            case "damageBlock":
                shieldScript.damageBlock = Mathf.Ceil(shieldScript.damageBlock * float.Parse(advantageMultiplier[1]));
                break;

            case "durability":
                shieldScript.durability = Mathf.Ceil(shieldScript.durability * float.Parse(advantageMultiplier[1]));
                shieldScript.maxDurability = Mathf.Ceil(shieldScript.maxDurability * float.Parse(advantageMultiplier[1]));
                break;
        }

    }
    void handleAMRarityMultiplier(Item.ItemRarity itemRarity, Item.ItemType itemType) 
    {
        //defense hp durability
        string[] advantageMultiplier = rarityAdvantageMultiplier(itemRarity, itemType).Split(':');

        //Check for stat and get multiplier

        switch (advantageMultiplier[0])
        {
            case "defense":
                armorScript.defense = Mathf.Ceil(armorScript.defense * float.Parse(advantageMultiplier[1]));
                break;

            case "hp":
                armorScript.hp = Mathf.Ceil(armorScript.hp * float.Parse(advantageMultiplier[1]));
                break;

            case "durability":
                armorScript.durability = Mathf.Ceil(armorScript.durability * float.Parse(advantageMultiplier[1]));
                armorScript.maxDurability = Mathf.Ceil(armorScript.maxDurability * float.Parse(advantageMultiplier[1]));
                break;
        }

    }



    public string rarityAdvantageMultiplier(Item.ItemRarity itemRarity, Item.ItemType itemType)
    {
        switch (itemRarity)
        {
            default:
            case Item.ItemRarity.Common: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";
            case Item.ItemRarity.Uncommon: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";
            case Item.ItemRarity.Rare: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";
            case Item.ItemRarity.Epic: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";
            case Item.ItemRarity.Legendary: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";
            case Item.ItemRarity.Primal: return $"{rarityAdvantage(itemType)}:{rarityMultiplier(itemRarity)}";

        }

    }

    public string rarityMultiplier(Item.ItemRarity itemRarity)
    {
        float probability = Random.Range(0f, 1f);


        switch (itemRarity)
        {
            case Item.ItemRarity.Common:

                if (probability > 0 && probability < 0.33f)
                {
                    return "1.1";
                }
                else if (probability >= 0.33f)
                {
                    return "1";
                }

                break;

            case Item.ItemRarity.Uncommon:

                if (probability > 0 && probability < 0.33f)
                {
                    return "1.2";
                }
                else if (probability >= 0.33f)
                {
                    return "1.1";
                }

                break;


            case Item.ItemRarity.Rare:

                if (probability > 0 && probability < 0.33f)
                {
                    return "1.3";
                }
                else if (probability >= 0.33f)
                {
                    return "1.2";
                }

                break;

            case Item.ItemRarity.Epic:

                if (probability > 0 && probability < 0.2f)
                {
                    return "1.5";
                }
                else if (probability >= 0.22f)
                {
                    return "1.3";
                }

                break;

            case Item.ItemRarity.Legendary:

                if (probability > 0 && probability < 0.2f)
                {
                    return "1.8";
                }
                else if (probability >= 0.22f)
                {
                    return "1.5";
                }

                break;

            case Item.ItemRarity.Primal:

                if (probability > 0 && probability < 0.1f)
                {
                    return "1.8";
                }
                else if (probability >= 0.1f)
                {
                    return "1.5";
                }

                break;

        }


        return "1";

    }

    public string rarityAdvantage(Item.ItemType itemType)
    {
        float probability = Random.Range(0f, 1f);

        switch (itemType)
        {
            case Item.ItemType.weapon:

                //Damage, attspeed, durability                    
                if (probability > 0 && probability < 0.33f)//Damage
                {
                    return "damage";
                }
                else if (probability > 0.33f && probability < 0.66f)//AttSpeed
                {
                    return "attSpeed";
                }
                else if (probability > 0.66f)//Durability
                {
                    return "durability";
                }

                break; 
            case Item.ItemType.shield:

                //Damage block, durability                    
                if (probability > 0 && probability < 0.5f)//Damage block
                {
                    return "damageBlock";
                }
                else if (probability > 0.5f)//durability
                {
                    return "durability";
                }

                break;


            case Item.ItemType.torso:
            case Item.ItemType.helmet:
            case Item.ItemType.hips:
            case Item.ItemType.shoulders:
            case Item.ItemType.boots:
            case Item.ItemType.arms:

                //Defense, Hp, Durability                    
                if (probability > 0 && probability < 0.33f)//Defense
                {
                    return "defense";
                }
                else if (probability > 0.33f && probability < 0.66f)//Hp
                {
                    return "hp";
                }
                else if (probability > 0.66f)//Durability
                {
                    return "durability";
                }

                break;


        }


        return "durability";

    }


    public string rarityRandomizer()
    {
        float probability = Random.Range(0f, 1f);

        if (probability >= 0f && probability < 0.47f)//47%
            return "common";
        else if (probability >= 0.47f && probability < 0.67f) //20%
            return "uncommon";
        else if (probability >= 0.67f && probability < 0.82f) //15%
            return "rare";
        else if (probability >= 0.82f && probability < 0.92f)//10%
            return "epic";
        else if (probability >= 0.92f && probability < 0.97f)//5%
            return "legendary";
        else if (probability >= 0.97f)//3%
            return "primal";


        return "common";

    }


}
