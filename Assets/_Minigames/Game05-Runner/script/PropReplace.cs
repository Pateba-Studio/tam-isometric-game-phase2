using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropReplace : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<Sprite> _propSprites;

    private System.Random _rand;

    private void Start() 
    {
        _rand = new System.Random();
    }

    public void ReplaceProps()
    {
        int propId = _rand.Next(0, _propSprites.Count);
        _spriteRenderer.sprite = _propSprites[propId];
    }
}
