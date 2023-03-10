using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PowerSwitchManager : InteractableObject
{
    public int CurrentInput = 0; // keeps track of the current order of the activated switches??
    [SerializeField]private int[] _correctOrder = new int[8];
    public int[] InputOrder = new int[8];
    public Switch[] Switches = new Switch[16];
    
    
    public bool CheckIfCorrect() {
           
            for (int i = 0; i < _correctOrder.Length; i++) {
                if (_correctOrder[i] != InputOrder[i]) {
                    return false;
                }
            } 
            return true;
    }
    public void ResetPuzzle() {
        Debug.Log("Reset time");
        foreach (var switchObj in Switches)
        {
            if (switchObj.Activated)
            {
                switchObj.Deactivate();
            }
        }

        CurrentInput = 0;
    }
    
    public void Unlock() {
        _quest.CompleteQuest();
        Debug.Log(_quest._currentQuestState);

        foreach (var @switch in Switches) {
            HighlightManager.Instance.RemoveFromOutline(@switch.gameObject);
        }
        
        foreach (var codeEvents in events) {
            Debug.Log("Invoked Events");
            codeEvents?.Invoke();
        }
    }
}
