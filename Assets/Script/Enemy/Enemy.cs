    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        public int CurrentHP => currentHP;
        [SerializeField, Min(1)] private int bulletCount = 3;
        [SerializeField, Range(0f, 180f)] private float spreadAngle = 40f;
        
        [Header("Health")]
        [SerializeField] protected int currentHP = 30;

        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 2f;
        [SerializeField] private float moveDistance = 3f;

        [Header("Attack")]
        [SerializeField] protected EnemyBullet bulletPrefab;
        [SerializeField] protected Transform firePoint;
        [SerializeField] private float fireInterval = 1f;

        protected Rigidbody2D rb;
        protected bool isDead;

        private float startX;
        private int moveDirection = 1;
        private float nextFireTime;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            startX = rb.position.x;
            nextFireTime = Time.time + fireInterval;
        }

        protected virtual void Update()
        {
            if (isDead || Time.timeScale == 0f) return;

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireInterval;
                Shoot();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (isDead) return;

            float leftX = startX - moveDistance;
            float rightX = startX + moveDistance;

            float nextX = rb.position.x +
                moveDirection * moveSpeed * Time.fixedDeltaTime;

            if (nextX >= rightX)
            {
                nextX = rightX;
                moveDirection = -1;
            }
            else if (nextX <= leftX)
            {
                nextX = leftX;
                moveDirection = 1;
            }

            rb.MovePosition(new Vector2(nextX, rb.position.y));
        }

        protected bool TryGetAimDirection(out Vector2 direction)
        {
            direction = Vector2.zero;

            if (firePoint == null)
                return false;

            Player target = FindFirstObjectByType<Player>();

            if (target == null || !target.isActiveAndEnabled)
                return false;

            direction = (Vector2)(target.transform.position - firePoint.position);
            return direction.sqrMagnitude > 0.001f;
        }

        protected virtual void Shoot()
        {
            if (bulletPrefab == null || !TryGetAimDirection(out Vector2 direction))
                return;

            EnemyBullet bullet = Instantiate(
                bulletPrefab, firePoint.position, Quaternion.identity
            );

            bullet.SetDirection(direction);
        }

        public virtual void TakeDamage(int damage)
        {
            if (isDead || damage <= 0)
                return;

            currentHP = Mathf.Max(0, currentHP - damage);

            if (ScoreTracker.Instance != null)
                ScoreTracker.Instance.RegisterHit();

            if (currentHP <= 0)
                Die();
        }
        protected virtual void Die()
        {
            if (isDead)
                return;

            isDead = true;

            if (TryGetComponent<RewardScoreSystem>(out var reward))
                reward.GiveReward();

            Destroy(gameObject);
        }
    }