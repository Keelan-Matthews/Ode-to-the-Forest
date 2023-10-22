using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart, threeQuarterHeart, halfHeart, oneQuarterHeart, emptyHeart;
    
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
            case HeartState.ThreeQuarter:
                _heartImage.sprite = threeQuarterHeart;
                break;
            case HeartState.Half:
                _heartImage.sprite = halfHeart;
                break;
            case HeartState.OneQuarter:
                _heartImage.sprite = oneQuarterHeart;
                break;
            case HeartState.Empty:
                _heartImage.sprite = emptyHeart;
                break;
        }
    }
}

public enum HeartState
{
    Full = 4,
    ThreeQuarter = 3,
    Half = 2,
    OneQuarter = 1,
    Empty = 0
}
