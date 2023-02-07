using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField] private List<Sprite> idleSprites;
    [SerializeField] private List<Sprite> runningSprites;
    [SerializeField] private List<Sprite> fallingSprites;
    private int idleNum;
    [SerializeField] private SpriteRenderer rend;
    private Rigidbody2D rigidBody;

    [SerializeField] private float period = 1f;
    private float lastChanged;

    public animationState state = animationState.idle;
    public enum animationState {
        idle,
        running,
        falling,
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (state == animationState.idle)
            SwitchSprites(idleSprites);
        if (state == animationState.running)
            SwitchSprites(runningSprites);
        if (state == animationState.falling)
            SwitchSprites(fallingSprites);

        if (rigidBody.velocity.x > 0)//simple flip sprite based on velocity(to make it look the direction of movement)
            rend.flipX = false;
        else if (rigidBody.velocity.x < 0)
            rend.flipX = true;
    }

    private void SwitchSprites(List<Sprite> spriteList)
    {
        if ((Time.time - lastChanged) >= period)
        {
            if (idleNum < spriteList.Count)
            {
                rend.sprite = spriteList[idleNum];
                idleNum++;
            }
            else if (idleNum >= spriteList.Count)
            {
                idleNum = 0;
                rend.sprite = spriteList[idleNum];
            }

            lastChanged = Time.time;
        }
    }
}
