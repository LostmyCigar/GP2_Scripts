using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Quest")]public class Quest : ScriptableObject {
    public QuestState _currentQuestState = QuestState.Inactive;
    public QuestState CurrentQuestState {
        get => _currentQuestState;
        set {
            if (value == QuestState.Completed && PlayTextOnComplete && value != _currentQuestState) {
                GameManager.Instance.StartQuestTimer(this);
            }
            _currentQuestState = value;
            if (value == QuestState.Active) {
                GameManager.Instance.StartQuestTimer(this);
            }

        }
    }
    [TextArea] public string Hint;
    public Quest LinkedQuest;
    public List<Quest> QuestPrerequisites;
    public float HintTime;
    public InteractableObject Interactable;
    public bool PlayTextOnComplete;
   

    public void Initialize(InteractableObject interactable) => Interactable = interactable;

    public void UpdateQuestState() {
        if (QuestPrerequisites != null && QuestPrerequisites.Count > 0) {
            if (QuestPrerequisites.TrueForAll(quest => quest.CurrentQuestState == QuestState.Completed)) {
                if(CurrentQuestState == QuestState.Inactive)
                    CurrentQuestState = QuestState.Active;
            }
        }
    }
    
    public void CompleteQuest() { // this method needs to be called at correct time...
        if (CurrentQuestState == QuestState.Active) {
            CurrentQuestState = QuestState.Waiting;
        }
        
        if (LinkedQuest != null) {
            if (LinkedQuest.CurrentQuestState == QuestState.Waiting) {
                LinkedQuest.CurrentQuestState = QuestState.Completed;
                CurrentQuestState = QuestState.Completed;
            }
        }
        else CurrentQuestState = QuestState.Completed;

        GameManager.Instance.UpdateQuests();
    }
}

public enum QuestState {
    Inactive, Active, Completed, NotCompleted, Waiting
}