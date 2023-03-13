using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart, halfHeart, emptyHeart;
    
    private Image _heartImage;
    
    private void Awake()
    {
        _heartImage = GetComponent<Image>();
    }
    
    public void SetHeartState(HeartState state)
    {
        switch (state)
        {
            case HeartState.Full:
                _heartImage.sprite = fullHeart;
                break;
            case HeartState.Half:
                _heartImage.sprite = halfHeart;
                break;
            case HeartState.Empty:
                _heartImage.sprite = emptyHeart;
                break;
        }
    }
}

public enum HeartState
{
    Full = 2,
    Half = 1,
    Empty = 0
}
