using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class fiftyFiftyManager : MonoBehaviour
{
    [SerializeField] private StatementPair[] allStatements;
    private Queue<StatementPair> randomStatementList;
    
    [SerializeField] private GameObject statementUI;
    [SerializeField] private Transform choiceLeftParent;
    [SerializeField] private Transform choiceRightParent;

    private int streak;
    [SerializeField] private TextMeshProUGUI streakCounter;

    [SerializeField] private GameObject nextQuestionButton;

    private void Start()
    {
        SpawnNewStatements();
        SetStreaKCounter();
    }

    private void RefreshRandomList()
    {
        //Shuffle allStatements into a temporary list first
        List<StatementPair> temp = new List<StatementPair>(allStatements);
    
        for (int i = temp.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        randomStatementList = new Queue<StatementPair>(temp);
    }

    public void SpawnNewStatements()
    {
        if (randomStatementList == null || randomStatementList.Count == 0)
            RefreshRandomList();

        if (randomStatementList == null || randomStatementList.Count == 0)
        {
            Debug.LogWarning("No statements found!");
            return;
        }
        
        foreach (var statement in GetComponentsInChildren<StatementUI>())
            Destroy(statement.gameObject);
        
        nextQuestionButton.SetActive(false);

        StatementPair newPair = randomStatementList.Dequeue();

        int coinFlip = Random.Range(0, 2);

        if (coinFlip == 0)
        {
            SpawnStatement(newPair.correctStatement, true, choiceLeftParent);
            SpawnStatement(newPair.falseStatement, false, choiceRightParent);
        }
        else
        {
            SpawnStatement(newPair.correctStatement, true, choiceRightParent);
            SpawnStatement(newPair.falseStatement, false, choiceLeftParent);
        }
    }

    private void SpawnStatement(Statement statement, bool correct, Transform parent)
    {
        GameObject s = Instantiate(statementUI, parent);
        StatementUI script = s.GetComponent<StatementUI>();
        script.statement = statement;
        script.isCorrect = correct;
        script.Populate();
    }
    
    public void ShowExplanations(bool wasCorrect)
    {
        if (wasCorrect) streak++;
        else streak = 0;

        SetStreaKCounter();
        nextQuestionButton.SetActive(true);
        
        foreach (var statement in GetComponentsInChildren<StatementUI>())
            statement.ShowExplanation();
    }
    
    private void SetStreaKCounter()
    {
        if (streak < 1) streakCounter.text = "";
        else streakCounter.text = $"Streak: {streak}";
    }
}

[CreateAssetMenu(fileName = "NewStatementPair", menuName = "FiftyFifty/StatementPair")]
public class StatementPair : ScriptableObject
{
    [Space(20)]
    public Statement correctStatement;
    [Space(40)]
    public Statement falseStatement;
}

[Serializable]
public class Statement
{
    [TextArea(1, 1)] public string title;
    [TextArea(1, 3)] public string description;
    [Space]
    [TextArea(3, 15)] public string explanation;
}