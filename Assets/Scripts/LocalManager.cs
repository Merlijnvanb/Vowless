using UnityEngine;

public class LocalManager : MonoBehaviour
{
    public bool LimitFramerate = true;
    public int TargetFramerate = 60;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (LimitFramerate)
            Application.targetFrameRate = TargetFramerate;
    }
}
