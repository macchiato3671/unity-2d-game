using UnityEngine;

public class AfterImageSlash : MonoBehaviour
{
    public Animator bodyAnimator;
    public Animator weaponAnimator;
    float speed;
    float maxDistance;
    int damage;
    private Vector3 startPos;
    private Vector2 direction;
    public AudioClip afterimage;

    public float preDelay = 0.5f; // 멈춰 있는 시간
    private bool hasStartedMoving = false;
    private bool startaudio = false;
    private float delayTimer = 0f;

    public void SetStats(
    Vector2 dir,
    int final_damage,
    float final_speed,
    float final_maxDistance)
    {
        damage = final_damage;
        speed = final_speed;
        maxDistance = final_maxDistance;
        Init(dir);
    }
    void Init(Vector2 dir)
    {
        direction = dir.normalized;
        startPos = transform.position;

        hasStartedMoving = false;
        delayTimer = 0f;

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.flipX = (direction.x < 0);
            sr.color = new Color(0.2f, 0.9f, 1f, 1.0f); // 파란빛
            sr.sortingOrder = -1;
        }

        gameObject.SetActive(true);
        bodyAnimator.Play("AfterImageBody");
        weaponAnimator.Play("AfterImageWeapon");
    }

    void Update()
    {
        if (!hasStartedMoving)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= preDelay)
            {
                hasStartedMoving = true;
                startaudio = true;
            }
            return; // 아직 이동 안 함
        }

        if (startaudio)
        {
            if (afterimage == null)
            {
                Debug.LogWarning("attackSFX is null!");
            }
            else
            {
                Debug.Log($"attackSFX name: {afterimage}");
                SoundManager.inst.PlaySFX(afterimage);
            }
            startaudio = false;
        }

        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("적을 공격함! 데미지: " + damage);
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemy.Damaged(damage, knockbackDir);
            }
        }
    }
}