using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimationAssetSource : MonoBehaviour
{
    [SerializeField] private List<Sprite> _stageClearSprites;
    [SerializeField] private Image _stageClearImage;

    public void ReplaceSprite(int idx)
    {
        if (idx < _stageClearSprites.Count)
            _stageClearImage.sprite = _stageClearSprites[idx];
    }
}
