using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PickUpable : InteractableObject
{
    private MeshRenderer mesh;
    [SerializeField] private Sound _soundOnPickup;
    

    private void Start() //Start to not overried base awake (interactableobject was checked out when i did this and i didnt want to bother anyone)
    {
        mesh = GetComponent<MeshRenderer>();
    }

    protected override void InteractAction(Player player)
    {
        TryPickUp(player);
    }

    public void TryPickUp(Player player)
    {
        if (player.InventoryComponent.TryAddItemToinventory(this)) {
            GameManager.Instance.ActivateBook(player._type);
        }
    }

    public virtual void PickUp() {
        if(_soundOnPickup != null)
            AudioManager.Instance.Play(_soundOnPickup.name, transform.position);
        
        HideAndDisable();
    }

    public override string ToString() {
        return gameObject.name;
    }

    public void HideAndDisable() {
        StopHighlight();
        StopOutline();
        mesh.enabled = false;
        _collider.enabled = false;
    }
    public void ShowAndEnable() {
        mesh.enabled = true;
        _collider.enabled = true;
    }
    public void ShowMesh() {
        mesh.enabled = true;
    }

}
