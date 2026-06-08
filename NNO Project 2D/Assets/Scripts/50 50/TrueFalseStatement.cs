using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrueFalseStatement
{
    public string question;
    public bool isTrue;
    public string explanation;
}

[Serializable]
public class TrueFalseStatementList
{
    public List<TrueFalseStatement> statements;
}