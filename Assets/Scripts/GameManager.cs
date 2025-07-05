using CrazyGames;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private const string TUTORIAL_KEY = "HasCompletedTutorial";
    private const string HIGH_SCORE_KEY = "HighScore";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CrazySDK.Init(() =>
            {
                Debug.Log("CrazyGames SDK Initialized");
                
                if (!CrazySDK.Data.HasKey(TUTORIAL_KEY))
                {
                    CrazySDK.Data.SetInt(TUTORIAL_KEY, 0);
                }
                if (!CrazySDK.Data.HasKey(HIGH_SCORE_KEY))
                {
                    CrazySDK.Data.SetInt(HIGH_SCORE_KEY, 0);
                }
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool HasCompletedTutorial
    {
        get { return CrazySDK.Data.GetInt(TUTORIAL_KEY, 0) == 1; }
        set { CrazySDK.Data.SetInt(TUTORIAL_KEY, value ? 1 : 0); }
    }

    public static int HighScore
    {
          get { return CrazySDK.Data.GetInt(HIGH_SCORE_KEY, 0); }
        set { CrazySDK.Data.SetInt(HIGH_SCORE_KEY, value); }
    }

    public void ResetTutorial()
    {
        CrazySDK.Data.DeleteKey(TUTORIAL_KEY);
        CrazySDK.Data.SetInt(TUTORIAL_KEY, 0);
        Debug.Log("Tutorial status direset!");
    }
}