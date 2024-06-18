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
    //public Sprite IdleSprite;
    //public Sprite AttackSprite;
    public float movementSpeed = 5;
    protected Rigidbody2D body;

    SpriteRenderer spriteRenderer;



    public virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //SetState(CharacterState.Idle);
    }

    //For later use
    /*
    public void SetState(CharacterState newState)
    {
        State = newState;///keep track of the new state
        if (State == CharacterState.Idle)
        {
            spriteRenderer.sprite = IdleSprite;
        }
        else if (State == CharacterState.Attack)
        {
            spriteRenderer.sprite = AttackSprite;
        }
    }*/
}
