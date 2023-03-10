using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class GameManager : GenericSingleton<GameManager> {
    // hints and objectives.. 
    public List<Quest> Quests;
    private TextMeshProUGUI textBox;
    public GameObject ZhiBookImage;
    public GameObject DogBookImage;
    public GameObject TextCanvas;
    private Queue<Quest> QuestTextToDisplay = new Queue<Quest>();
    public bool _isDisplayingText;

    private void Awake() {
        Quests = Resources.LoadAll<Quest>("QuestSO").ToList();
        if (textBox == null) textBox = GameObject.Find("Textbox").GetComponent<TextMeshProUGUI>();
        if (TextCanvas == null) TextCanvas = GameObject.Find("Screentext").transform.Find("TextPanel").gameObject;

        TextCanvas.SetActive(false);

        ZhiBookImage.SetActive(false);
        DogBookImage.SetActive(false);
        //Quests.ForEach(EditorUtility.SetDirty);
    }

    public void ActivateBook(PlayerType type) {
        if (type == PlayerType.Human) {
            ZhiBookImage.SetActive(true);
        }else if (type == PlayerType.Ghost) {
            DogBookImage.SetActive(true);
        }
    }
    public void DeactivateBook(PlayerType type) {
        if (type == PlayerType.Human) {
            ZhiBookImage.SetActive(false);
        }else if (type == PlayerType.Ghost) {
            DogBookImage.SetActive(false);
        }
    }

    public void UpdateQuests() {
        // this should be called on quest Complete
        foreach (var quest in Quests) {
            quest.UpdateQuestState();
        }
    }

    public void StartQuestTimer(Quest quest) {
        if (quest.HintTime == 0) return;


        StartCoroutine(QuestTimer(quest)); // nej det måste köras på
    }

    private IEnumerator QuestTimer(Quest quest) { // try to queue them 
        yield return new WaitForSeconds(quest.HintTime * 60);
        QuestTextToDisplay.Enqueue(quest);
        if (!_isDisplayingText) {
            if (quest.CurrentQuestState == QuestState.Active || quest.PlayTextOnComplete) {
                StartCoroutine(printHint(QuestTextToDisplay.Dequeue().Hint));
            }
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.J) && Input.GetKeyDown(KeyCode.K)) {
            List<Quest> QuestsToSetActive = new List<Quest>();
            List<Quest> QuestsDialogue = new List<Quest>();
            foreach (var quest in Quests) {
                quest.CurrentQuestState = QuestState.Inactive;
                if (quest.name == "FronDoorButtonHuman" || quest.name == "FrontDoorButtonGhost" ||
                    quest.PlayTextOnComplete) {
                    quest.CurrentQuestState = QuestState.Active;
                }
            }
        }
    }

    private IEnumerator printHint(string hint) { // could be made smoother.
        _isDisplayingText = true;
        TextCanvas.SetActive(true);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < hint.Length; i++) {
            builder.Append(hint[i]);
            textBox.text = builder.ToString();
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3); // hardcoded value
        textBox.text = "";

        _isDisplayingText = false;
        if (QuestTextToDisplay.Count == 0) TextCanvas.SetActive(false);
        else StartQuestTimer(QuestTextToDisplay.Dequeue());
    }
}

public enum GameState {
    NormalState,
    PuzzleState,
    TransitionToPuzzle,
    TransitionToNormal
}