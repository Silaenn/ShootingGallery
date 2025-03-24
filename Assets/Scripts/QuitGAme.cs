using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGAme : MonoBehaviour
{
    void OnMouseDown()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}
