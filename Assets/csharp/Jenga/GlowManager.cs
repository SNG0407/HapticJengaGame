using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowManager : MonoBehaviour
{
    public Material glow, nonglow;
    bool isGlowing = true;

    public void SetGlow()
    {
        if(isGlowing)
        {
            gameObject.GetComponent<MeshRenderer>().material = nonglow;
            isGlowing = false;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = glow;
            isGlowing = true;
        }
    }
}
