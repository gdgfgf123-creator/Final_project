using System.Collections;
using UnityEngine;

public class GriffinAttack : BossAttack
{
    [SerializeField] private WaveBullet waveBulletPrefab;
    [SerializeField] private Transform griffinFirePoint;
    [SerializeField, Min(1)] private int waveCount = 5;
    [SerializeField, Range(0f, 180f)] private float waveSpread = 60f;

    public override IEnumerator Execute(Boss boss)
    {
        ShootFan(waveBulletPrefab, griffinFirePoint, waveCount, waveSpread);
        yield break;
    }
}