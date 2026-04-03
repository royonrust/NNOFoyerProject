using UnityEngine;

public class MusicMakerPlayer : MonoBehaviour
{
    public bool isPaused;
    public MusicMakerManager manager;
    
    void Update()
    {
        if (isPaused) return;
        
        float moveAmount = manager.bpm / 60f * 200f * Time.deltaTime;
        transform.Translate(Vector3.right * moveAmount);
    }
}