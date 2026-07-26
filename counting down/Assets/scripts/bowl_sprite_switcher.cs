using System.Reflection;
using UnityEngine;

public class bowl_sprite_switcher : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the GameObject that has the CatMinigameManager component (or the component itself).")]
    public CatMinigameManager catManager;

    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite halfSprite;
    public Sprite fullSprite;

    private SpriteRenderer _renderer;
    private Sprite _currentSprite;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        // start with empty if available
        if (_renderer != null && emptySprite != null)
        {
            _renderer.sprite = emptySprite;
            _currentSprite = emptySprite;
        }
    }

    void Update()
    {
        if (catManager == null || _renderer == null)
            return;

        // read public foodCaught
        int caught = catManager.foodCaught;

        // try to read private serialized field foodNeededToWin via reflection
        int needed = 0;
        FieldInfo fi = typeof(CatMinigameManager).GetField("foodNeededToWin", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi != null)
        {
            object val = fi.GetValue(catManager);
            if (val is int)
                needed = (int)val;
        }

        // if reflection failed or value is 0, avoid division by zero and skip
        if (needed <= 0)
            return;

        float ratio = (float)caught / (float)needed;

        Sprite target = emptySprite;
        if (ratio >= 0.8f && fullSprite != null)
        {
            target = fullSprite;
        }
        else if (ratio >= 0.25f && halfSprite != null)
        {
            target = halfSprite;
        }

        if (target != _currentSprite)
        {
            _renderer.sprite = target;
            _currentSprite = target;
        }
    }
}
