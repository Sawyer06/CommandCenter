using UnityEngine;
using UnityEngine.UI;

public class SetSpriteStart : MonoBehaviour
{
    public Material mat;
    public Texture firstImg;

    private void Awake()
    {
        mat.mainTexture = firstImg; // Set player texture to forward at start.
    }
}
