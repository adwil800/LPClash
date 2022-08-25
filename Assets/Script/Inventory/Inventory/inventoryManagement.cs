using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class inventoryManagement : MonoBehaviour
{



    [HideInInspector] public bool dragging = false;

    [Header("TOOLTIP")]
    public GameObject toolTip;
    public GameObject compToolTip;

    [Header("GENERAL VARS")]
    public GameObject inv;
    public GameObject character;
    public GameObject quest;

    [Header("SHIELD PREFABS")]
    public GameObject knightShield;
    public GameObject longShield;

    [Header("WEAPON PREFABS")]
    public GameObject knightSword;
    public GameObject shortSword;
    public GameObject evilSlasher;

    [Header("ARMOR PREFABS SET 01")]
    public GameObject knightHelmet;
    public GameObject knightTorso;
    public GameObject knightHips;
    public GameObject knightLegs;
    public GameObject knightShoulders;
    public GameObject knightArms;
    [Header("ARMOR PREFABS SET 02")]
    public GameObject kingHelmet;
    public GameObject kingTorso;
    public GameObject kingHips;
    public GameObject kingLegs;
    public GameObject kingShoulders;
    public GameObject kingArms;

    [Header("WEAPON AND SHIELD SLOTS")]
    public Transform weaponSlot;
    public Transform shieldSlot;
    public Transform weaponBackSlot;
    public Transform shieldBackSlot;


    public GameObject UIcam;

    playerController controllerScript;

    //Reference to playerStats
    [HideInInspector] public playerStats statsScript;
    //Character animator
    Animator charAnimator;


    bool openInv = false;
    [HideInInspector] public Inventory inventory;

    [HideInInspector] public Inventory charEquipment;
    void Start()
    {

        controllerScript = GetComponent<playerController>();
        statsScript = GetComponent<playerStats>();
        charAnimator = GetComponent<Animator>();

        //Inventory
        inventory = new Inventory();
        //Character equipment inventory
        charEquipment = new Inventory();

    }

    void Update()
    {

        //Press TAB to open inventory
        if (Input.GetKeyDown(KeyCode.Tab) && !openInv)
        {
            openInv = true;
            //Lock camera position
            controllerScript.LockCameraPosition = true;
            controllerScript.inventoryOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inventoryWindow();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && openInv && !dragging)
        {
            openInv = false;
            dragging = false;
            closeWindow();
        }




    }

    //UI inventory :: inv

    //Refresh inventory

    public void refreshInventoryItems()
    {
        //Disable all images
        for (int i = 0; i < 30; i++)
        {

            Transform slotParent = inv.transform.GetChild(0).GetChild(i);

            Image itemRenderer = slotParent.GetChild(0).GetComponent<Image>();
            itemRenderer.enabled = false;

        }



        //Get all item slots and enable only used images

        foreach (Item item in inventory.getItemList())
        {

            //Get parent gameobject
            Transform slotParent = inv.transform.GetChild(0).GetChild(item.slot);

            //Get sprite renderer (Image)
            Image itemRenderer = slotParent.GetChild(0).GetComponent<Image>();
            //Add existing item sprite
            itemRenderer.sprite = item.itemSprite;
            //Show
            itemRenderer.enabled = true;


        }
    }


    public int availableSlots()
    {

        List<Item> itemList = inventory.getItemList();
        bool taken = false;
        for (int i = 0; i < 30; i++)
        {
            taken = false;
            //Runs through all slots
            for (int j = 0; j < inventory.getListCount(); j++)
            {
                if (itemList[j].slot == i)
                {
                    taken = true;
                }

            }

            if (!taken)
            {
                return i;
            }

        }

        return -1;

    }


    public Item findItem(int slot, Inventory invList)
    {
        List<Item> itemList = invList.getItemList();


        for (int i = 0; i < invList.getListCount(); i++)
        {
            if (itemList[i].slot == slot)
            {
                return itemList[i];
            }
        }

        return null;

    }

    public GameObject getInvPrefab(int slot, Inventory invList)
    {

        //Debug.Log(findItem(slot).prefabName.ToLower());

        switch (findItem(slot, invList).itemName.ToLower())
        {
            default:
            case "knight shield": return knightShield;
            case "knight sword": return knightSword;
            case "short sword": return shortSword;
            case "evil slasher": return evilSlasher;
            case "long shield": return longShield;


            //Set 02
            case "knight helmet": return knightHelmet;
            case "knight torso": return knightTorso;
            case "knight legs": return knightHips;
            case "knight boots": return knightLegs;
            case "knight shoulders": return knightShoulders;
            case "knight arms": return knightArms;

            //Set 01
            case "king's helmet": return kingHelmet;
            case "king's torso": return kingTorso;
            case "king's hips": return kingHips;
            case "king's boots": return kingLegs;
            case "king's shoulders": return kingShoulders;
            case "king's arms": return kingArms;
        }

    }
    //THE INVENTORY IS DONE, START WORKING ON THE CHARACTER EQUIPMENT WINDOW

    //Tooltip events
    public void invToolTip(int slot)
    {

        //Hovered item, only for the inventory
        Item item = findItem(slot, inventory);

        //Check if this itemType is already equipped, if so, enable comparisonTooltip

        if (getCharSlot(item.itemType) != -1 && findItem(getCharSlot(item.itemType), charEquipment) != null)
        {
            comparisonToolTip(item);

        }


        string itemStats = "";
        //Item stats
        for (int i = 0; i < item.stats.Count; i++)
        {
            itemStats += item.stats[i] + "\n";
        }

        //Get tooltipText
        TextMeshProUGUI itemNameText = toolTip.transform.GetChild(1).GetComponent<TextMeshProUGUI>(),
                        itemStatsText = toolTip.transform.GetChild(2).GetComponent<TextMeshProUGUI>(),
                        rarity = toolTip.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        if(item.itemType != Item.ItemType.healthPotion || item.itemType != Item.ItemType.staminaPotion)
        {
            //Test textmeshpro gradient
            toolTip.transform.GetChild(3).transform.gameObject.SetActive(true);
            itemNameText.colorGradient = item.getRarityGradient(item.itemRarity);
            rarity.colorGradient = item.getRarityGradient(item.itemRarity);
        }
        else
        {
            toolTip.transform.GetChild(3).transform.gameObject.SetActive(false);
        }


        itemNameText.text = item.itemName;
        itemStatsText.text = itemStats;
        rarity.text = item.itemRarity.ToString();
        //Debug.Log("Item found in slot " + slot + " : " +item.itemName);

        //Relocate frame
        Transform slotParent = inv.transform.GetChild(0).GetChild(slot);
        slotParent.GetComponent<Image>().enabled = true;
        //Set slot as parent and reset position;

        toolTip.SetActive(true);


    }

    public void clearToolTip(Transform slotParent)
    {

        compToolTip.SetActive(false);
        toolTip.SetActive(false);
        if (!dragging) {
            slotParent.GetComponent<Image>().enabled = false;
        } 

    }


    //MAYBE SOMEDAY TRY AND FIX TOOLTIP OVER  ALL IMAGES
    //DRAG AND DROP INVENTORY THINGY, CHANGE POSITIONS AND DROP INTO ENVIRONMENT




    //CHAR EQUIPMENT SECTION
    public void charEquipmentTooltip(int slot)
    {

        //Hovered item, only for the charEquipment
        Item item = findItem(slot, charEquipment);
        string itemStats = "";
        //Item stats
        for (int i = 0; i < item.stats.Count; i++)
        {
            itemStats += item.stats[i] + "\n";
        }

        //Get tooltipText
        TextMeshProUGUI itemNameText = toolTip.transform.GetChild(1).GetComponent<TextMeshProUGUI>(),
                        itemStatsText = toolTip.transform.GetChild(2).GetComponent<TextMeshProUGUI>(),
                        rarity = toolTip.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        itemNameText.colorGradient = item.getRarityGradient(item.itemRarity);
        rarity.colorGradient = item.getRarityGradient(item.itemRarity);

        itemNameText.text = item.itemName;
        itemStatsText.text = itemStats;
        rarity.text = item.itemRarity.ToString();

        //Relocate frame
        Transform slotParent = character.transform.GetChild(0).GetChild(slot);
        slotParent.GetComponent<Image>().enabled = true;
        //Set slot as parent and reset position;

        toolTip.SetActive(true);

    }

    //Compares equipped and inventory items if their type match
    public void comparisonToolTip(Item invItem)
    {

        //ITEM SWITCHING MIXES COLORGRADIENT

        //Find the item in charEquipment
        Item charEqItem = findItem(getCharSlot(invItem.itemType), charEquipment);
        TextMeshProUGUI itemNameText = compToolTip.transform.GetChild(1).GetComponent<TextMeshProUGUI>(), 
                        itemStatsText = compToolTip.transform.GetChild(2).GetComponent<TextMeshProUGUI>(),
                        rarity = compToolTip.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Transform ghostSlot = compToolTip.transform.GetChild(5);

        
            //Get item sprite
            ghostSlot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = charEqItem.itemSprite;
            //Get item name and stats
            string itemStats = "";
            //Item stats
            for (int i = 0; i < charEqItem.stats.Count; i++)
            {
                itemStats += charEqItem.stats[i] + "\n";
            }



        itemNameText.colorGradient = charEqItem.getRarityGradient(charEqItem.itemRarity);
        rarity.colorGradient = charEqItem.getRarityGradient(charEqItem.itemRarity);

        itemNameText.text = charEqItem.itemName;
        itemStatsText.text = itemStats;
        rarity.text = charEqItem.itemRarity.ToString();

        //Enable tooltip
        compToolTip.SetActive(true);

    }

    //ADD arms TO SET 02
    //Correct char inventory slot
    int getCharSlot(Item.ItemType itemType)
    {

        switch (itemType)
        {
            case Item.ItemType.helmet: return 0;
            case Item.ItemType.shoulders: return 1;
            case Item.ItemType.torso: return 2;
            case Item.ItemType.hips: return 3;
            case Item.ItemType.boots: return 4;
            case Item.ItemType.arms: return 5;
            case Item.ItemType.weapon: return 6;
            case Item.ItemType.shield: return 7;
        }

        return -1;

    }

    public void refreshEquippedItems()
    {
        //Disable all images
        for (int i = 0; i < 8; i++)
        {

            Transform slotParent = character.transform.GetChild(0).GetChild(i);

            Image itemRenderer = slotParent.GetChild(0).GetComponent<Image>();
            itemRenderer.enabled = false;

        }


        //Get all item slots and enable only used images

        foreach (Item item in charEquipment.getItemList())
        {

            //Get parent gameobject
            Transform slotParent = character.transform.GetChild(0).GetChild(item.slot);

            //Get sprite renderer (Image)
            Image itemRenderer = slotParent.GetChild(0).GetComponent<Image>();
            //Add existing item sprite
            itemRenderer.sprite = item.itemSprite;
            //Show
            itemRenderer.enabled = true;

        }
    }
    //FIND A WAY TO RETARGET ONPOINTENTER TOOLTIP EVENT AFTER EQUIPPING AN ITEM













    //CHARACTER EQUIPMENT SECTION, DOUBLE CLICK TO DO SO FROM THE INVENTORY

    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;


    //Allows the player to equip an item when hovering and right clicking
    public void equipItem(int slot)
    {


        // Detecting double click
        clicked++;

        if (clicked == 1)
            clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            // Double click detected
            clicked = 0;
            clicktime = 0;







            //Check for item slot and find the item that is going to be equipped
            Item item = findItem(slot, inventory);

            //Check for item type and get slot 
            int itemSlot = getCharEquipmentSlot(item.itemType);

            //If the player wants to equip a new weapon then bring the other one to front
            if(itemSlot == 6 || itemSlot == 7)
                equippedWeaponsToFront();

            //Check if slot is full, if so, exchange items when clicked
            Item existingItemInSlot = findItem(itemSlot, charEquipment);
            if (existingItemInSlot != null)
            {
                //Exchange slots 
                int tooltipSlot = item.slot;
                existingItemInSlot.slot = item.slot; //Changes the existing slot in character to the slot in the inventory where it's coming from
                item.slot = itemSlot; //Set the item to be equipped slot to the character corresponding slot (itemSlot)

                //Move to the inventory
                inventory.addItem(existingItemInSlot);
                //Substract old item stats
                statsScript.sRefreshStats(existingItemInSlot);
                //Delete from the character equipment
                charEquipment.deleteItem(existingItemInSlot);
                //Equip new item
                charEquipment.addItem(item);
                //Add new item stats
                statsScript.refreshStats(item);
                //Delete from inventory
                inventory.deleteItem(item);

                //Retarget tooltip
                invToolTip(tooltipSlot);


                //Refresh inventories and stats
                refreshInventoryItems();
                refreshEquippedItems();

                //Re-run weapon hiding timeout
                if (itemSlot == 6 || itemSlot == 7)
                {
                    stopCoroutines();
                    StartCoroutine(equippedWeaponsToBack(20f));
                }

            }
            //If it's not full then just equip and remove from inv
            else
            {
                item.slot = itemSlot; //Set the item to be equipped slot to the character corresponding slot (itemSlot)

                //Equip new item
                charEquipment.addItem(item);

                //Add
                statsScript.refreshStats(item);
                //Delete from inventory
                inventory.deleteItem(item);

                //Refresh inventories and stats
                refreshInventoryItems();
                refreshEquippedItems();

                //Re-run weapon hiding timeout
                if (itemSlot == 6 || itemSlot == 7)
                {
                    stopCoroutines();
                    StartCoroutine(equippedWeaponsToBack(20f));
                }
            }


            //Alter hierarchy to enable and disable corresponding objects or well, in case of shield and sword just move them to their corresponding slots
            {
                /*  Original set pieces
                  *  Hair: Chr_Hair_38
                  *  Head: Chr_Head_Female_02
                  *  Shoulders: None
                  *  Torso: Chr_Torso_Female_03
                  *  Hips: Chr_Hips_Female_03
                  *  
                  *  UpperRightArm: Chr_ArmUpperRight_Female_03
                  *  LowerRightArm: Chr_ArmLowerRight_Female_03
                  *  RightHand: Chr_HandRight_Female_03
                  *  RightLeg: Chr_LegRight_Female_03
                  *  
                  *  UpperLeftArm: Chr_ArmUpperLeft_Female_03
                  *  LowerLeftArm: Chr_ArmLowerLeft_Female_03
                  *  LeftHand: Chr_HandLeft_Female_03
                  *  LeftLeg: Chr_LegLeft_Female_03
                  *  
                  *  HipsAttachment: Chr_HipsAttachment_03
                 */
            }
            switch (itemSlot)
            {
                //Is helmet
                case 0:
                    //Disable hair and head: Chr_Hair_38, Chr_Head_Female_02 and/or already existing one
                    if (existingItemInSlot != null)
                    {
                        GameObject oldItem = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        if (oldItem != null)
                        {
                            oldItem.SetActive(false);
                        }
                    }

                    //Equip new item
                    handleSingleArmorPiece(item.prefabName.Replace("_Static", ""), "Chr_Head_Female_02", "Chr_Hair_38");
                    break;

                //PrefabName only contains the RIGHT side of shoulders
                //Is shoulders
                case 1:
                    //Disable existing if there ever were
                    if (existingItemInSlot != null)
                    {
                        GameObject oldItemPiece1 = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        GameObject oldItemPiece2 = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Right", "Left"));
                        if (oldItemPiece1 != null && oldItemPiece2 != null)
                        {
                            oldItemPiece1.SetActive(false);
                            oldItemPiece2.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }
                    }
                    //Render new shoulders without specifying old ones because there were none
                    handleMultipleArmorPiece(item.prefabName.Replace("_Static", ""));

                    break;

                //Is torso
                case 2:
                    //Disable torso and/or already existing one

                    if (existingItemInSlot != null)
                    {
                        GameObject oldItem = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        if (oldItem != null)
                        {
                            oldItem.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }
                    }

                    //Equip new item
                    handleSingleArmorPiece(item.prefabName.Replace("_Static", ""), "Chr_Torso_Female_03");

                    break;

                //Is hips
                case 3:
                    //Disable hips and/or already existing one
                    if (existingItemInSlot != null)
                    {
                        GameObject oldItem = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        if (oldItem != null)
                        {
                            oldItem.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }
                    }

                    //Equip new item
                    handleSingleArmorPiece(item.prefabName.Replace("_Static", ""), "Chr_Hips_Female_03");
                    break;

                //PrefabName only contains the RIGHT side of boots
                //Is boots
                case 4:
                    //Disable existing if there ever were
                    if (existingItemInSlot != null)
                    {
                        GameObject oldItemPiece1 = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        GameObject oldItemPiece2 = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Right", "Left"));
                        if (oldItemPiece1 != null && oldItemPiece2 != null)
                        {
                            oldItemPiece1.SetActive(false);
                            oldItemPiece2.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }
                    }

                    //Render new boots
                    handleMultipleArmorPiece(item.prefabName.Replace("_Static", ""), "Chr_LegRight_Female_03");
                    break;
                    //Is arms
                    case 5:

                    //Custom lower and upper arm
                   

                    //Disable existing if there ever were
                    if (existingItemInSlot != null)
                    {
                            
                        GameObject currentGlovePieceR = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", ""));
                        GameObject currentGlovePieceL = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Right", "Left"));

                        GameObject currentLowerArmPieceR = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower"));
                        GameObject currentLowerArmPieceL = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower").Replace("Right", "Left"));

                        GameObject currentUpperArmPieceR = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper"));
                        GameObject currentUpperArmPieceL = FindObjectInChilds(this.gameObject, existingItemInSlot.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper").Replace("Right", "Left"));
                        if (currentGlovePieceR != null && currentGlovePieceL != null && currentLowerArmPieceR != null && currentLowerArmPieceL != null &&
                            currentUpperArmPieceR != null && currentUpperArmPieceL != null)
                        {
                            currentGlovePieceR.SetActive(false);
                            currentGlovePieceL.SetActive(false);
                            currentLowerArmPieceR.SetActive(false);
                            currentLowerArmPieceL.SetActive(false);
                            currentUpperArmPieceR.SetActive(false);
                            currentUpperArmPieceL.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }
                    }
                    //Disable base arms
                    else
                    { 
                        GameObject baseGlovePieceR = FindObjectInChilds(this.gameObject, "Chr_HandRight_Female_03");
                        GameObject baseGlovePieceL = FindObjectInChilds(this.gameObject, "Chr_HandLeft_Female_03");
                        GameObject baseLowerArmPieceR = FindObjectInChilds(this.gameObject, "Chr_ArmLowerRight_Female_03");
                        GameObject baseLowerArmPieceL = FindObjectInChilds(this.gameObject, "Chr_ArmLowerLeft_Female_03");
                        GameObject baseUpperArmPieceR = FindObjectInChilds(this.gameObject, "Chr_ArmUpperRight_Female_03");
                        GameObject baseUpperArmPieceL = FindObjectInChilds(this.gameObject, "Chr_ArmUpperLeft_Female_03");

                        if (baseGlovePieceR != null && baseGlovePieceL != null && baseLowerArmPieceR != null && baseLowerArmPieceL != null &&
                                baseUpperArmPieceR != null && baseUpperArmPieceL != null)
                        {
                            baseGlovePieceR.SetActive(false);
                            baseGlovePieceL.SetActive(false);
                            baseLowerArmPieceR.SetActive(false);
                            baseLowerArmPieceL.SetActive(false);
                            baseUpperArmPieceR.SetActive(false);
                            baseUpperArmPieceL.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                        }

                    }


                    //Render new arms
                    GameObject newArmPieceR = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", ""));
                    GameObject newArmPieceL = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", "").Replace("Right", "Left"));

                    GameObject newLowerArmPieceR = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower"));
                    GameObject newLowerArmPieceL = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower").Replace("Right", "Left"));

                    GameObject newUpperArmPieceR = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper"));
                    GameObject newUpperArmPieceL = FindObjectInChilds(this.gameObject, item.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper").Replace("Right", "Left"));
                    if (newArmPieceR != null && newArmPieceL != null && newLowerArmPieceR != null && newLowerArmPieceL != null &&
                        newUpperArmPieceR != null && newUpperArmPieceL != null)
                    {
                        newArmPieceR.SetActive(true);
                        newArmPieceL.SetActive(true);
                        newLowerArmPieceR.SetActive(true);
                        newLowerArmPieceL.SetActive(true);
                        newUpperArmPieceR.SetActive(true);
                        newUpperArmPieceL.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                    }



                    break;

                //Is weapon
                case 6:

                    //Remove existing weapon if there's one
                    if (weaponSlot.childCount > 0)
                    {
                        Destroy(weaponSlot.GetChild(0).gameObject);
                    }

                    //Get weapon prefab, instantiate and set corresponding parent
                    GameObject weapon = Instantiate(getInvPrefab(item.slot, charEquipment), transform.position, transform.rotation) as GameObject;
                    weapon.transform.SetParent(weaponSlot);
                    weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = Quaternion.identity;
                    weapon.layer = 6;
                    //SetTrigger for collider and kinematic rigidbody
                    
                    weapon.GetComponent<MeshCollider>().enabled = false;
                    
                    weapon.GetComponent<BoxCollider>().enabled = true;
                    weapon.GetComponent<BoxCollider>().isTrigger = true;
                    weapon.GetComponent<Rigidbody>().isKinematic = true;
                    //Enable has weapon animation set
                    transform.GetComponent<Animator>().SetBool("hasWeapon", true);

                    //Re-run weapon hiding timeout
                    stopCoroutines();
                    StartCoroutine(equippedWeaponsToBack(20f));

                    break;

                //Is shield
                case 7:

                    //Remove existing weapon if there's one
                    if (shieldSlot.childCount > 0)
                    {
                        Destroy(shieldSlot.GetChild(0).gameObject);
                    }

                    //Get shield prefab, instantiate and set corresponding parent
                    GameObject shield = Instantiate(getInvPrefab(item.slot, charEquipment), transform.position, transform.rotation) as GameObject;
                    shield.transform.SetParent(shieldSlot);
                    shield.transform.localPosition = Vector3.zero;
                    shield.transform.localRotation = Quaternion.identity;
                    shield.layer = 6;
                    //SetTrigger for collider and kinematic rigidbody

                    shield.GetComponent<MeshCollider>().enabled = false;

                    shield.GetComponent<BoxCollider>().enabled = true;
                    shield.GetComponent<BoxCollider>().isTrigger = true;
                    shield.GetComponent<Rigidbody>().isKinematic = true;
                    //Enable has shield animation set
                    transform.GetComponent<Animator>().SetBool("hasShield", true);

                    //Re-run weapon hiding timeout
                    stopCoroutines();
                    StartCoroutine(equippedWeaponsToBack(20f));
                    break;
            }


        }
        else if (clicked > 2 || Time.time - clicktime > 1)
            clicked = 0;

    }

    //Unequips old armor and equips new one, if something goes wrong then just equips old armor :: Handles Hips, Torso and Head
    void handleSingleArmorPiece(string newArmorPiece, string oldArmorPiece, string oldArmorPiece2 = "")
    {
        //Disable old armor piece
        FindObjectInChilds(this.gameObject, oldArmorPiece).SetActive(false);
        //If a 2nd piece is specified then disable
        if (oldArmorPiece2.Length > 0)
            FindObjectInChilds(this.gameObject, oldArmorPiece2).SetActive(false);

        //Render new torso 
        GameObject newPiece = FindObjectInChilds(this.gameObject, newArmorPiece);
        //Render new hips
        if (newPiece != null)
        {
            newPiece.SetActive(true);
        }
        //Render old hips
        else
        {
            FindObjectInChilds(this.gameObject, oldArmorPiece).SetActive(true);

            //If a 2nd piece is specified then enable
            if (oldArmorPiece2.Length > 0)
                FindObjectInChilds(this.gameObject, oldArmorPiece2).SetActive(true);
        }
    }


    //Unequips old armor and equips new one, if something goes wrong then just equips old armor :: Handles Shoulders and Boots
    void handleMultipleArmorPiece(string newArmorPiece, string oldArmorPiece = "")
    {
        //Disable old armor piece
        if (oldArmorPiece.Length > 0)
        {
            FindObjectInChilds(this.gameObject, oldArmorPiece).SetActive(false);
            FindObjectInChilds(this.gameObject, oldArmorPiece.Replace("Right", "Left")).SetActive(false);
        }
        //If a 2nd piece is specified then disable

        //Render new torso 
        GameObject newPieceR = FindObjectInChilds(this.gameObject, newArmorPiece);
        GameObject newPieceL = FindObjectInChilds(this.gameObject, newArmorPiece.Replace("Right", "Left"));
        //Render new hips
        if (newPieceR != null && newPieceL != null)
        {
            newPieceR.SetActive(true);
            newPieceL.SetActive(true);
        }
        //Render old multi-pieces
        else
        {
            //If multi old pieces are specified
            if (oldArmorPiece.Length > 0)
            {
                FindObjectInChilds(this.gameObject, oldArmorPiece).SetActive(true);
                FindObjectInChilds(this.gameObject, oldArmorPiece.Replace("Right", "Left")).SetActive(true);
            }
        }
    }

    public GameObject FindObjectInChilds(GameObject gameObject, string gameObjectName)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform item in children)
        {
            if (item.name == gameObjectName)
            {
                return item.gameObject;
            }
        }

        return null;
    }

    int getCharEquipmentSlot(Item.ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case Item.ItemType.helmet: return 0;
            case Item.ItemType.shoulders: return 1;
            case Item.ItemType.torso: return 2;
            case Item.ItemType.hips: return 3;
            case Item.ItemType.boots: return 4;
            case Item.ItemType.arms: return 5;
            case Item.ItemType.weapon: return 6;
            case Item.ItemType.shield: return 7;

        }
    }



    [HideInInspector] public bool swordToBack = false, shieldToBack = false;
    public IEnumerator equippedWeaponsToBack(float time)
    {
        yield return new WaitForSeconds(time);

        // weaponSlot; shieldSlot; weaponBackSlot; shieldBackSlot;
        if (charAnimator.GetBool("hasWeapon"))
        {
            Transform currentWeapon = weaponSlot.GetChild(0);

            //Set parent
            currentWeapon.SetParent(weaponBackSlot);
            //Reset transform
            currentWeapon.localPosition = Vector3.zero;
            currentWeapon.localRotation = Quaternion.identity;
            //Set animation
            charAnimator.SetBool("hasWeapon", false);
            swordToBack = true;
        }

        if (charAnimator.GetBool("hasShield"))
        {
            Transform currentShield = shieldSlot.GetChild(0);

            //Set parent
            currentShield.SetParent(shieldBackSlot);
            //Reset transform
            currentShield.localPosition = Vector3.zero;
            currentShield.localRotation = Quaternion.identity;
            //Set animation
            charAnimator.SetBool("hasShield", false);
            shieldToBack = true;
        }

    }

    public void equippedWeaponsToFront()
    {  

        // weaponSlot; shieldSlot; weaponBackSlot; shieldBackSlot;
        if (swordToBack)
        {
            Transform currentWeapon = weaponBackSlot.GetChild(0);

            //Set parent
            currentWeapon.SetParent(weaponSlot);
            //Reset transform
            currentWeapon.localPosition = Vector3.zero;
            currentWeapon.localRotation = Quaternion.identity;
            //Set animation
            charAnimator.SetBool("hasWeapon", true);
            swordToBack = false;
        }

        if (shieldToBack)
        {
            Transform currentWeapon = shieldBackSlot.GetChild(0);

            //Set parent
            currentWeapon.SetParent(shieldSlot);
            //Reset transform
            currentWeapon.localPosition = Vector3.zero;
            currentWeapon.localRotation = Quaternion.identity;
            //Set animation
            charAnimator.SetBool("hasShield", true);
            shieldToBack = false;
        }

    }

    public void stopCoroutines()
    {
        StopAllCoroutines();
    }



















    //BUTTON EVENTS
    public void closeWindow()
    {
        UIcam.SetActive(false);
        openInv = false;
        inv.SetActive(false);
        quest.SetActive(false);
        character.SetActive(false); 

        compToolTip.SetActive(false);
        toolTip.SetActive(false);

        //Lock camera position
        controllerScript.LockCameraPosition = false;
        controllerScript.inventoryOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    //SWITCH WINDOW
    }

    public void inventoryWindow()
    {

        //Disable UI camera 
        UIcam.SetActive(false);

        refreshInventoryItems();
        inv.SetActive(true);
        quest.SetActive(false);
        character.SetActive(false);
          
        compToolTip.SetActive(false);
        toolTip.SetActive(false);

    }

    public void characterWindow()
    {

        //Enable UI camera 
        UIcam.SetActive(true);
        
        character.SetActive(true);
        inv.SetActive(false);
        quest.SetActive(false);  

        compToolTip.SetActive(false);
        toolTip.SetActive(false);

    }

    public void questWindow()
    {

        //Disable UI camera 
        UIcam.SetActive(false);

        quest.SetActive(true);
        inv.SetActive(false);
        character.SetActive(false);

        compToolTip.SetActive(false);
        toolTip.SetActive(false);

    }




}

