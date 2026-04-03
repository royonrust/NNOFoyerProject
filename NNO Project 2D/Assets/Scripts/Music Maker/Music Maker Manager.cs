using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicMakerManager : MonoBehaviour
{
    [HideInInspector] public int bpm;
    public bool isLooped;

    [SerializeField] private Transform playerParent;
    [SerializeField] private GameObject player;
    
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject abortButton;
    
    [SerializeField] private GameObject endOfStaff;

    [HideInInspector] public MusicMakerPlayer currentPlayer;
    public Action onPlayerStart;

    [SerializeField] private TMP_InputField bpmInput;
    
    [SerializeField] private Transform staffSection;

    void Start() => ChangeBPM();

    public void ChangeBPM()
    {
        if (int.TryParse(bpmInput.text, out int parsedBpm))
        {
            parsedBpm = Mathf.Clamp(parsedBpm, 10, 240);
            bpmInput.text = parsedBpm.ToString();
            bpm = parsedBpm;
        }
    }

    public void ClearAllNotes()
    {
        GridSlot[] slots = staffSection.GetComponentsInChildren<GridSlot>(true);

        foreach (GridSlot slot in slots)
            foreach (Transform note in slot.transform)
                Destroy(note.gameObject);
    }

    public void StartPlay()
    {
        if (currentPlayer != null) currentPlayer.isPaused = false;
        else SpawnPlayer();
        
        playButton.SetActive(false);
        pauseButton.SetActive(true);
        abortButton.SetActive(true);
        onPlayerStart?.Invoke();
    }

    void Update()
    {
        if (currentPlayer == null) return;

        if (currentPlayer.transform.position.x >= endOfStaff.transform.position.x)
        {
            if (isLooped)
            {
                currentPlayer.transform.position = playerParent.position;
                onPlayerStart?.Invoke();
            }
            else Destroy(currentPlayer.gameObject);
        }
    }

    public void PausePlay()
    {
        if (currentPlayer == null) return;
        
        currentPlayer.isPaused = true;
        
        playButton.SetActive(true);
        pauseButton.SetActive(false);
    }
    
    public void StopPlay()
    {
        abortButton.SetActive(false);
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        
        if (currentPlayer != null)
            Destroy(currentPlayer.gameObject);
    } 

    public void SpawnPlayer()
    {
        currentPlayer = Instantiate(player, playerParent).GetComponent<MusicMakerPlayer>();
        currentPlayer.manager = this;
    }
}