using UnityEngine;
using UnityEngine.UI;

public class MudarBackground : MonoBehaviour
{
    [Header("Referência da UI")]
    public Image panel;

    [Header("Nova imagem")]
    public Sprite bgSprite;

    public void TrocarBrackground(Sprite novoBg)
    {
        bgSprite = novoBg;

        if (panel != null && bgSprite != null) {
            panel.sprite = bgSprite;
        }
    }
}
