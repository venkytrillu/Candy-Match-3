using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    private GoalManager goalManager;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();

    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeColorChnage();
    }

    private void Update()
    {
        if(hitPoints<=0)
        {

            if (goalManager != null)
            {
                goalManager.CompareGoal(this.tag.ToString());
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    void MakeColorChnage()
    {
        Color color = sprite.color;
        float newAlpha = color.a*0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }


}//class


















































