using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum CharacterState
{
    Idle,
    Run
}

public class Character : MonoBehaviour
{
    public CharacterState State;
    public Sprite IdleSprite;
    public Sprite RunSprite;
    public float movementSpeed = 7f;//Player movement speed
    protected Rigidbody2D body;
    public Animator animator;//Used for both enemy and player

    SpriteRenderer spriteRenderer;

    public virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetState(CharacterState.Idle);
    }

    
    
    public void SetState(CharacterState newState)
    {
        State = newState;///keep track of the new state
        if (State == CharacterState.Idle)
        {
            spriteRenderer.sprite  = IdleSprite;
        }
        else if (State == CharacterState.Run)
        {
            spriteRenderer.sprite = RunSprite;
        }

        
    }
}
