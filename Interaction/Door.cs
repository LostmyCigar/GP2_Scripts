using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Door : InteractableObject {
    public List<Lock> Locks;
    public Animator Anim;
    private Player _player;
    public void InvokeEvents() => events.ForEach(ev => ev?.Invoke());
    private bool _opened = false;

    protected override void InteractAction(Player player) {
        _player = player;
        OpenDoor();
    }

    public void OpenDoor() {
        Debug.Log("Called: door open");
        Anim = GetComponent<Animator>();
        if (CheckIfUnlocked() && !_opened) {
            Anim.SetTrigger("DoorOpen"); // doesn't exist
            AudioManager.Instance.Play("DoorOpen", transform.position);
            _opened = true;
        }
        else {
            Debug.Log("Tried to unlock but was stil log");
            // maybe text to screen, door is locked, or sound
        }
    }

    private bool CheckIfUnlocked() {
        if (Locks.Count < 1 || Locks == null) {
            InvokeEvents();
            return true;
        }
        else {
            bool output = false;
            foreach (var doorlock in Locks) {
                if (doorlock.TypeOfLock == LockType.Quest) {
                    doorlock.TestUnlock();
                }
                else {
                    if (_player.InventoryComponent.HeldItem == null)
                        continue;
                    doorlock.TestUnlock(_player.InventoryComponent, _player.InventoryComponent.HeldItem.gameObject);
                }
            }

            output = Locks.TrueForAll(doorlock => doorlock.Unlocked);

            if (output) {
                InvokeEvents();
                return true;
            }
            else {
                return false;
            }
        }
    }

    public void PlayBookShelfSound() {
        AudioManager.Instance.Play("BookToShelf", transform.position);
    }
}

[System.Serializable]
public class Lock {
    public Quest QuestToUnlock;
    public bool Unlocked;
    public LockType TypeOfLock;
    public GameObject Key;
    public List<UnityEvent> OnUnlockEvents;

    public void InvokeEvents() => OnUnlockEvents.ForEach(ev => ev?.Invoke());

    public void TestUnlock(PlayerInventory inventory = null, GameObject heldItem = null) {
        if (TypeOfLock == LockType.Quest || inventory == null) {
            if (QuestToUnlock.CurrentQuestState == QuestState.Completed) {
                Unlocked = true;
                InvokeEvents();
            }
        }
        else {
            if (heldItem == Key) {
                inventory.UseInventoryObject();
                Unlocked = true;
                InvokeEvents();
            }
        }
    }
}

public enum LockType {
    Quest,
    Key
}