using UnityEngine;

public class LocalManager : MonoBehaviour
{
    public bool LimitFramerate = true;
    public int TargetFramerate = 60;
    
    void Start()
    {
        if (LimitFramerate)
            Application.targetFrameRate = TargetFramerate;
    }
}
