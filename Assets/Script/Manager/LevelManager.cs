using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int CurrentStage { get; private set; } = 1;

    public IEnumerator WaitForStageOneClear(Boss boss)
    {
        if (CurrentStage != 1)
            yield break;

        // Enemy.Die() ทำลาย GameObject ของบอสเมื่อ HP หมด
        while (boss != null)
            yield return null;

        CurrentStage = 2;
        Debug.Log("Stage 2", this);
    }
}