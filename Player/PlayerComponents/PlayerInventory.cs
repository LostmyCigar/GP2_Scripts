using UnityEngine;


public class PlayerInventory : PlayerComponent
{
    public PickUpable HeldItem;
    private Transform _handPos;
    private Player _player;
    private InputHandler _inputHandler;
    public bool HoldInventoryItem;

    public PlayerInventory(Transform handPosition, Player player, InputHandler inputHandler)
    {
        _handPos = handPosition;
        _player = player;
        _inputHandler = inputHandler;   
    }


    public override void LogicUpdate()
    {
        if (_inputHandler._dropOff)
            DropHeldItem();

        if (!HoldInventoryItem) return;

        //Tie to hand or something

    }
    public bool TryAddItemToinventory(PickUpable item)
    {
        if (HeldItem == null)
        {
            HeldItem = item;
            item.PickUp();
            return true;
        }
        else {
            return false;
        }
    }
    public void DropHeldItem()
    {
        if (HeldItem == null)
            return;

        Ray ray = new Ray(_player.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            HeldItem.ShowAndEnable();
            HeldItem.transform.position = hit.point;
            UseInventoryObject();
        }
    }
    public void UseInventoryObject()
    {
        HeldItem = null;
        GameManager.Instance.DeactivateBook(_player._type);
    }
}
