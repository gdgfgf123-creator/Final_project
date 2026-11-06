using System.Collections;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] protected int damage = 10;
    [SerializeField] private bool isHoldingClick;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 0.2f;

    private Camera mainCamera;
    private float nextFireTime;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
    }
    public abstract class BossAttack : MonoBehaviour
    {
        public abstract IEnumerator Execute(Boss boss);
    }
    public void OnClick()
    {
        Shoot();
    }

    public void OnClickHold()
    {
        if (isHoldingClick)
            Shoot();
    }

    public void SetHoldingClick(bool holding)
    {
        isHoldingClick = holding;
    }

    private void Shoot()
    {
        if (Time.timeScale == 0f || Time.time < nextFireTime)
            return;

        if (bulletPrefab == null ||
            firePoint == null ||
            mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(
            Vector3.forward,
            firePoint.position
        );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector2 mousePosition = ray.GetPoint(distance);
        Vector2 direction =
            mousePosition - (Vector2)firePoint.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        nextFireTime = Time.time + fireInterval;

        Bullet bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }

    public void ApplyDamage()
    {
        // เพิ่มเมื่อทำระบบศัตรู
    }
}