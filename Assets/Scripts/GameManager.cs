using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private const string TUTORIAL_KEY = "HasCompletedTutorial";
    private const string HIGH_SCORE_KEY = "HighScore";

    void Awake()
    {
        ResetTutorial();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inisialisasi PlayerPrefs untuk tutorial dan high score
            if (!PlayerPrefs.HasKey(TUTORIAL_KEY))
            {
                PlayerPrefs.SetInt(TUTORIAL_KEY, 0);
                PlayerPrefs.Save();
            }
            if (!PlayerPrefs.HasKey(HIGH_SCORE_KEY))
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, 0);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool HasCompletedTutorial
    {
        get { return PlayerPrefs.GetInt(TUTORIAL_KEY, 0) == 1; }
        set 
        { 
            PlayerPrefs.SetInt(TUTORIAL_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int HighScore
    {
        get { return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0); }
        set 
        { 
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_KEY);
        PlayerPrefs.SetInt(TUTORIAL_KEY, 0);
        PlayerPrefs.Save();
    }
}