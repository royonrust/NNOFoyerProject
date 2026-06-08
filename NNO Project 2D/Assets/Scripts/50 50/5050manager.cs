using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FiftyFiftyManager : MonoBehaviour
{
    [Header("Statements")]
    [SerializeField] private TextAsset allStatementsJSON;
    [SerializeField] private TextMeshProUGUI questionTMP;
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image sliderVisual;

    [Header("Feedback UI")]
    [SerializeField] private GameObject explanationUI;
    [SerializeField] private GameObject explanationClickToContinueUI;
    [SerializeField] private TextMeshProUGUI explanationTMP;
    [SerializeField] private GameObject correctUI;
    [SerializeField] private GameObject incorrectUI;
    [SerializeField] private GameObject correctUI1;
    [SerializeField] private GameObject incorrectUI1;
    [SerializeField] private GameObject timeIsUpText;

    [Header("Screens")]
    [SerializeField] private GameObject gamingScreen;
    [SerializeField] private GameObject gamingScreen2;
    [SerializeField] private GameObject idleScreen;
    [SerializeField] private GameObject idleScreen2;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameOverScreen2;

    [Header("Streak")]
    [SerializeField] private TextMeshProUGUI bestStreakCounter;
    [SerializeField] private TextMeshProUGUI secondScreenStreakCounter;
    [SerializeField] private TextMeshProUGUI gameOverTMP;

    [Header("Particles")]
    [SerializeField] private ParticleController particlesForeground;
    [SerializeField] private ParticleController particlesMiddleground;
    [SerializeField] private ParticleController particlesBackground;

    private Queue<TrueFalseStatement> randomStatementList;
    private TrueFalseStatement currentStatement;
    private StreakSave bestStreak;
    private int streak;
    private int lastStreak;
    private List<(int score, DateTime time)> recentScores = new();
    private Coroutine timerCoroutine;
    private Coroutine goToNextQuestionCoroutine;
    private Coroutine idleCoroutine;

    public void ResetIdleTimer()
    {
        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        idleCoroutine = StartCoroutine(IdleCoroutine());
    }

    private IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(30f);
        ShowIdleScreen();
    }

    private IEnumerator GoToNextQuestionCoroutine()
    {
        yield return new WaitForSeconds(15f);
        SpawnNewStatements();
    }

    private void Start()
    {
        streak = 0;
        CheckBest();
        ShowIdleScreen();
    }

    // ── Screen management ──────────────────────────────────────────

    public void StartGame()
    {
        SetScreens(true, false, false);
        SetStreakCounter();
        RefreshRandomList();
        SpawnNewStatements();
    }

    public void ShowGameOver()
    {
        SetScreens(false, false, true);

        recentScores.Insert(0, (lastStreak, DateTime.Now));
        if (recentScores.Count > 5) recentScores.RemoveAt(5);

        string recentLine = "";
        if (recentScores.Count > 0)
        {
            var lines = recentScores.Select(entry =>
            {
                TimeSpan elapsed = DateTime.Now - entry.time;
                string ago;
                if (elapsed.TotalSeconds < 20)
                    ago = "<size=52><color=#B3B3B3>zojuist</color></size>";
                else if (elapsed.TotalSeconds < 60)
                    ago = $"<size=52><color=#B3B3B3>{(int)elapsed.TotalSeconds} sec geleden</color></size>";
                else
                    ago = $"<size=52><color=#B3B3B3>{(int)elapsed.TotalMinutes} min geleden</color></size>";

                return $"{entry.score} {ago}";
            });
            recentLine = "\n\nRecente scores:\n" + string.Join("\n", lines);
        }

        gameOverTMP.text = $"<size=130>GAME OVER</size>\n\n" +
                           $"Jouw score: {lastStreak}\n" +
                           $"Beste vandaag: {bestStreak.streak}" +
                           recentLine;

        if (goToNextQuestionCoroutine != null) StopCoroutine(goToNextQuestionCoroutine);
        HideFeedbackUI();
    }

    public void ShowIdleScreen() => SetScreens(false, true, false);
    
    private void SetScreens(bool gaming, bool idle, bool gameOver)
    {
        gamingScreen?.SetActive(gaming);
        gamingScreen2?.SetActive(gaming);
        idleScreen?.SetActive(idle);
        idleScreen2?.SetActive(idle);
        gameOverScreen?.SetActive(gameOver);
        gameOverScreen2?.SetActive(gameOver);
        ResetIdleTimer();
    }

    // ── Statements ─────────────────────────────────────────────────

    private void SpawnNewStatements()
    {
        if (randomStatementList == null || randomStatementList.Count == 0)
            RefreshRandomList();

        if (randomStatementList == null || randomStatementList.Count == 0)
        {
            Debug.LogWarning("No statements found!");
            return;
        }

        if (goToNextQuestionCoroutine != null) StopCoroutine(goToNextQuestionCoroutine);
        HideFeedbackUI();

        currentStatement = randomStatementList.Dequeue();
        questionTMP.text = currentStatement.question;

        StartTimer();
        ResetIdleTimer();
    }

    private void RefreshRandomList()
    {
        List<TrueFalseStatement> temp = JsonUtility.FromJson<TrueFalseStatementList>(allStatementsJSON.text).statements;

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        randomStatementList = new Queue<TrueFalseStatement>(temp);
    }
    
    private void StartTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        timerSlider.gameObject.SetActive(true);
        float remaining = 10f;
        timerSlider.maxValue = remaining;
        
        while (remaining > 0f)
        {
            timerSlider.value = remaining;
            sliderVisual.color = Color.Lerp(new Color32(218, 79, 54, 255), new Color32(218, 169, 54, 255), remaining / 10f);
            timerTMP.text = Mathf.CeilToInt(remaining).ToString();
            remaining -= Time.deltaTime;
            yield return null;
        }

        timerTMP.text = "0";
        ShowExplanation(false, true);
    }

    // ── Feedback & streak ──────────────────────────────────────────

    private void ShowExplanation(bool isTrue, bool timeIsUp = false)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerTMP.text = "";
        timerSlider.gameObject.SetActive(false);
        timeIsUpText.SetActive(timeIsUp);

        bool wasCorrect = (currentStatement.isTrue == isTrue && timeIsUp == false);
        
        lastStreak = streak;
        streak = wasCorrect ? streak + 1 : 0;

        OnQuestionAnswer(wasCorrect);
        SetStreakCounter();

        explanationUI.SetActive(true);
        explanationClickToContinueUI.SetActive(true);
        explanationTMP.text = currentStatement.explanation;
        correctUI.SetActive(wasCorrect);
        correctUI1.SetActive(wasCorrect);
        incorrectUI.SetActive(!wasCorrect);
        incorrectUI1.SetActive(!wasCorrect);
        ResetIdleTimer();
        goToNextQuestionCoroutine = StartCoroutine(GoToNextQuestionCoroutine());
    }

    public void AnswerTrue() => ShowExplanation(true);
    public void AnswerFalse() => ShowExplanation(false);

    private void HideFeedbackUI()
    {
        explanationUI.SetActive(false);
        explanationClickToContinueUI.SetActive(false);
        correctUI.SetActive(false);
        correctUI1.SetActive(false);
        incorrectUI.SetActive(false);
        incorrectUI1.SetActive(false);
    }

    private void OnQuestionAnswer(bool wasCorrect)
    {
        particlesBackground.ChangeBasedOnStreak(streak, wasCorrect);
        particlesMiddleground.ChangeBasedOnStreak(streak - 1, wasCorrect);
        particlesForeground.ChangeBasedOnStreak(streak - 3, wasCorrect);
    }

    // ── Streak persistence ─────────────────────────────────────────

    private void SetStreakCounter()
    {
        secondScreenStreakCounter.fontSize = Mathf.Clamp(75f + streak * 15f, 0f, 350f);
        var c = secondScreenStreakCounter.color;
        c.a = Mathf.Clamp01(0.50f + streak * 0.05f);
        secondScreenStreakCounter.color = c;
        secondScreenStreakCounter.text = "Score: " + streak;

        CheckBest();
    }

    private void CheckBest()
    {
        string path = Application.persistentDataPath + "/bestStreak.json";
        bestStreak ??= new StreakSave();

        if (File.Exists(path))
            bestStreak = JsonUtility.FromJson<StreakSave>(File.ReadAllText(path));

        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (bestStreak.date != today || bestStreak.streak < streak)
            SaveBest();

        bestStreakCounter.text = "Beste Score vandaag: " + bestStreak.streak;
    }

    private void SaveBest()
    {
        bestStreak.date = DateTime.Now.ToString("yyyy-MM-dd");
        bestStreak.streak = streak;
        
        File.WriteAllText(Application.persistentDataPath + "/bestStreak.json", JsonUtility.ToJson(bestStreak));
    }
}

[Serializable]
public class StreakSave
{
    public string date;
    public int streak;
}