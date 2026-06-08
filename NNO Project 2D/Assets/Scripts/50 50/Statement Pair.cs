using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatementPair", menuName = "FiftyFifty/StatementPair")]
public class StatementPair : ScriptableObject
{
    [Space(20)]
    public Statement Question;
    [Space(40)]
    public Statement falseStatement;
    [Space(40)]
    [TextArea(3, 15)] public string explanation;
}

[Serializable]
public class Statement
{
    [TextArea(1, 1)] public string title;
    [TextArea(1, 3)] public string description;
}