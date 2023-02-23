using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField] private List<Sprite> _idleSprites;
    [SerializeField] private List<Sprite> _runningSprites;
    [SerializeField] private List<Sprite> _fallingSprites;
    private int _idleNum;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidBody;

    [SerializeField] private float _period = 1f;
    private float _lastChanged;

    public AnimationState _state = AnimationState.Idle;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (_state)
        {
            case AnimationState.Idle:
                SwitchSprites(_idleSprites);
                break;
            case AnimationState.Running:
                SwitchSprites(_runningSprites);
                break;
            case AnimationState.Falling:
                SwitchSprites(_fallingSprites);
                break;
        }
        _renderer.flipX = (_rigidBody.velocity.x < 0);
        //simple flip sprite based on velocity(to make it look the direction of movement)
    }

    private void SwitchSprites(List<Sprite> spriteList)
    {
        if ((Time.time - _lastChanged) >= _period)
        {
            if (_idleNum < spriteList.Count)
            {
                _renderer.sprite = spriteList[_idleNum];
                _idleNum++;
            }
            else if (_idleNum >= spriteList.Count)
            {
                _idleNum = 0;
                _renderer.sprite = spriteList[_idleNum];
            }

            _lastChanged = Time.time;
        }
    }
}

[System.Serializable]
public enum AnimationState
{
    Idle,
    Running,
    Falling,
}
