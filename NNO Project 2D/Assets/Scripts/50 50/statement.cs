using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatementUI : MonoBehaviour
{
    private fiftyFiftyManager manager;
    
    [HideInInspector] public Statement statement;
    [HideInInspector] public bool isCorrect;

    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    [SerializeField] private GameObject explanationWindow;
    [SerializeField] private GameObject correctVisual;
    [SerializeField] private GameObject incorrectVisual;
    [SerializeField] private TextMeshProUGUI explanationTMP;

    public void Awake() => manager = GetComponentInParent<fiftyFiftyManager>();

    public void Populate()
    {
        titleTMP.text = statement.title;
        descriptionTMP.text = statement.description;
    }

    public void ClickButton() => manager.ShowExplanations(isCorrect);

    public void ShowExplanation()
    {
        GetComponent<Button>().enabled = false;
        explanationWindow.SetActive(true);
        explanationTMP.text = statement.explanation;
        
        correctVisual.SetActive(isCorrect);
        incorrectVisual.SetActive(!isCorrect);
    }
}