using UnityEngine;

public class VictoriaBoss : Boss
{
    protected override void FixedUpdate()
    {
        
    }
    
    
    protected override void Die()
    {
        Debug.Log("Victoria defeated!");
        base.Die();
    }
}