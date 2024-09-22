using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    #region Singleton
    public static InventoryUI instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);

        instance = this;
    }

    #endregion

    #region NummericKeyCode
    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };
    #endregion
    public ToolTip toolTip;
    [HideInInspector] public bool isInventoryOpen;
    public GameObject inventoryPanel;
    [HideInInspector] public InventorySlot[] inventorySlots;
    [HideInInspector] public InventoryManager playerInventory;

    public GameObject craftingGridPanel;
    [SerializeField] private GameObject craftingAndEquipmentPanel;
    [HideInInspector] public InventoryManager craftingInventory;
    [HideInInspector] public CraftingSlot[] craftingSlots;

    public GameObject craftingOutputPanel;
    public GameObject craftingOutputWindow;
    public GameObject outputButtomPrefab;
    [SerializeField] private Button craftButton;
    [HideInInspector] public CraftingManager craftingManager;
    [HideInInspector] public OutputItemSlot[] outputItemSlots;

    private int selectedSlotIndex = 0;
    public GameObject quickAcessPanel;
    [HideInInspector] public QuickAssetSlotUI[] quickAssetSlots;

    public GameObject equipmentPanel;
    [HideInInspector] public EquipmentSlot[] equipmentSlots;
    [HideInInspector] EquipmentManager equipment;

    public GameObject chestPanel;
    [HideInInspector] public InventorySlot[] chestSlots;
    [HideInInspector] public InventoryManager chestInventory;

    public RectTransform parentTransform;

    private void Start()
    {
        AssignPlayerInventoryManagerReferences();
        InitializeCraftingOutputPanel();
        AssignUIArrayReferences();
        AssignInventoryCallback();
        InitializeUIPanels();

        craftButton.onClick.AddListener(craftingManager.CraftAndAdd);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (chestPanel.activeSelf)
                ToggleChestInventory(null);
            else if (craftingGridPanel.activeSelf)
                ToggleCraftingPanel();
            else
                ToggleInventory();
        }

        QuickAccessControl();     
    }

    private void AssignPlayerInventoryManagerReferences()
    {
        playerInventory = PlayerManager.Instance.player.GetComponentInChildren<InventoryManager>();
        equipment = PlayerManager.Instance.player.GetComponentInChildren<EquipmentManager>();
        craftingInventory = SetupSingleton.Instance.gameManagers.GetComponentInChildren<InventoryManager>();
        craftingManager = SetupSingleton.Instance.gameManagers.GetComponentInChildren<CraftingManager>();
    }

    private void AssignUIArrayReferences()
    {
        inventorySlots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        quickAssetSlots = quickAcessPanel.GetComponentsInChildren<QuickAssetSlotUI>();
        chestSlots = chestPanel.GetComponentsInChildren<InventorySlot>();
        craftingSlots = craftingAndEquipmentPanel.GetComponentsInChildren<CraftingSlot>();
        equipmentSlots = equipmentPanel.GetComponentsInChildren<EquipmentSlot>(true);
        outputItemSlots = craftingOutputPanel.GetComponentsInChildren<OutputItemSlot>(true);

        inventorySlots = inventorySlots.Concat(quickAssetSlots).ToArray();
        inventorySlots = inventorySlots.Concat(chestSlots).ToArray();
        inventorySlots = inventorySlots.Concat(craftingSlots).ToArray();

        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].index = i;
        for (int i = 0; i < playerInventory.inventorySize; i++)
            inventorySlots[i].inventoryManager = playerInventory;
        for (int i = 0; i < craftingInventory.inventorySize; i++)
        {
            craftingSlots[i].inventoryManager = craftingInventory;
            craftingSlots[i].craftingManager = craftingManager;
        }
        foreach (OutputItemSlot outputItemSlot in outputItemSlots)
            outputItemSlot.craftingManager = craftingManager;
    }

    private void AssignInventoryCallback()
    {
        playerInventory.inventoryAddCallBack += UpdateInventoryAdd;
        playerInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
        craftingInventory.inventoryAddCallBack += UpdateInventoryAdd;
        craftingInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
        craftingManager.inputItemChangeCallback += ResetOutputItemUI;
        craftingManager.outputItemChangeCallback += ResetOutputItemUI;
        equipment.equipmentChangeCallBack += UpdateEquipment;

        for (int i = 0; i < quickAssetSlots.Length; i++)
            quickAssetSlots[i].slotUpdatedCallback += equipment.CallEquipMainHand;
    }

        
    private void InitializeCraftingOutputPanel() {
        foreach (CraftingRecipe recipe in craftingManager.recipeList)
        {
            OutputItemSlot newOutputItemSlot = Instantiate(outputButtomPrefab, craftingOutputWindow.transform).GetComponent<OutputItemSlot>();
            newOutputItemSlot.SetRecipe(recipe);
        }
    }

    private void InitializeUIPanels()
    {
        craftingGridPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        chestPanel.SetActive(false);
        craftingOutputPanel.SetActive(false);

        for (int i = 0; i < playerInventory.inventorySize; i++)
        {
            inventorySlots[i].AddItem(playerInventory.itemList[i]);
        }
    }

    private void ToggleInventory()
    {
        toolTip.HideTooltip();
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        craftingAndEquipmentPanel.SetActive(inventoryPanel.activeSelf);
        equipmentPanel.SetActive(inventoryPanel.activeSelf);
        ToggleCraftingOutputPanel();

        if (!inventoryPanel.activeSelf)              // Return item when inventory is closed
            craftingManager.ReturnItemToInventory();

        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    public void ToggleCraftingPanel()
    {
        toolTip.HideTooltip();
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        craftingAndEquipmentPanel.SetActive(inventoryPanel.activeSelf);
        craftingGridPanel.SetActive(inventoryPanel.activeSelf);
        ToggleCraftingOutputPanel();

        if (!craftingGridPanel.activeSelf)              // Return item when inventory is closed
            craftingManager.ReturnItemToInventory();

        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    private void ToggleCraftingOutputPanel()
    {
        craftingOutputPanel.SetActive(inventoryPanel.activeSelf);
        craftingManager.RemoveOutoutItem();
        ResetOutputItemUI();
    }
    public void ToggleChestInventory(InventoryManager chestInventory)
    {
        if (chestInventory != null)
        {
            this.chestInventory = chestInventory;
            OpenChest(chestInventory);
        }
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        chestPanel.SetActive(inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    private void OpenChest(InventoryManager chestInventory)
    {
        if (chestInventory == null)
            return;
        for (int i = 0; i < chestSlots.Length; i++)
        {
            chestSlots[i].inventoryManager = chestInventory;
            chestSlots[i].AddItem(chestInventory.itemList[i]);
        }

        if (chestInventory.inventoryAddCallBack == null)
            chestInventory.inventoryAddCallBack += UpdateInventoryAdd;
        if (chestInventory.inventoryRemoveCallBack == null)
            chestInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
    }
    private void QuickAccessControl()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                selectedSlotIndex = i;
                quickAssetSlots[i].GetSelect();
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (i != selectedSlotIndex)
                quickAssetSlots[i].GetUnselect();
        }
    }

    #region UpdateInventoryUI
    private void UpdateInventoryAdd(Item item, int index)
    {
        if (item != null)
        {
            //Debug.Log(item.name + " added to inventory");
            if (index >= 0)
                inventorySlots[index].AddItem(item);
        }
        else
        {
            UpdateInventoryRemove(item, index);
        }
    }
        


    private void UpdateInventoryRemove(Item item, int index)
    {
        //Debug.Log("Removed from inventory");
        if (index >= 0)
            inventorySlots[index].RemoveItem();             
    }
    #endregion
    private void UpdateEquipment(Equipment newEquipment, Equipment oldEquipment)
    {
        int index;
       
        if (newEquipment != null && newEquipment.GetType() != typeof(Tool))
        {
            index = (int)newEquipment.equipSlot;
            equipmentSlots[index].AddEquipment(newEquipment);
        }
        else if (oldEquipment != null && oldEquipment.GetType() != typeof(Tool))
        {
            index = (int)oldEquipment.equipSlot;
            equipmentSlots[index].RemoveEquipment();
        }
    }

    private void ResetOutputItemUI()
    {
        //Debug.Log("Output UI reseted");
        foreach (OutputItemSlot outputItemButton in outputItemSlots)
        {
            //if (craftingManager.TryCraft(outputItemButton.item))
            if (craftingManager.TryCraftRecipe(outputItemButton.recipe))
                outputItemButton.gameObject.SetActive(true);
            else
                outputItemButton.gameObject.SetActive(false);

            if (outputItemButton.recipe != craftingManager.outputRecipe)
                outputItemButton.GetUnselect();
        }
    }    
}
