using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Left,
        Right,
        Top,
        Bottom
    }
    
    public DoorType doorType;
    private GameObject _player;
    private const float WidthOffset = 7.5f; // Change based on width of player
    private bool _locked;
    private SpriteRenderer _renderer;
    
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        
        // Get the Sprite child and update the sprite to locked door
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    // Teleport the player to the next room to overcome the door being blocked
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player") || _locked) return;
        var position = _player.transform.position;
        position = doorType switch
        {
            DoorType.Bottom => new Vector2(position.x, position.y - WidthOffset),
            DoorType.Left => new Vector2(position.x - WidthOffset, position.y),
            DoorType.Right => new Vector2(position.x + WidthOffset, position.y),
            DoorType.Top => new Vector2(position.x, position.y + WidthOffset),
            _ => position
        };
        _player.transform.position = position;
    }


    public void LockDoor()
    {
        _locked = true;
        
        _renderer.sprite = lockedSprite;
    }
    
    public void UnlockDoor()
    {
        _locked = false;
        
        // Get the Sprite child and update the sprite to unlocked door
        _renderer.sprite = unlockedSprite;
    }
}
