using UnityEngine;

public class EnemyHackable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isStunned = false;
    private float stunTimer = 0f;

    private bool isInfected = false;
    private float infectionDuration = 5f;
    private float infectionTimer = 0f;
    private float tickTimer = 0f;
    private float infectionTickInterval = 1f;
    private float tickDamage = 5f;
    private float infectionRadius = 6f;

    private bool isSuiciding = false;
    private float suicideTimer = 0f;
    private float blinkInterval = 0.2f;
    private float blinkTimer = 0f;
    private bool blinkState = false;

    private bool isBlinded = false;
    private float blindTimer = 0f;

    private bool isVulnerable = false;
    private float vulnerableTimer = 0f;

    private bool isTurncoat = false;
    private float turncoatTimer = 0f;

    public LayerMask enemyLayer;

    [SerializeField] private AudioClip explosionSound;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        HandleStun();
        HandleInfection();
        HandleSuicide();
        HandleBlind();
        HandleVulnerable();
        HandleTurncoat();
    }

    // -------- 해킹 스킬 함수들 --------

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        spriteRenderer.color = Color.cyan;
        Debug.Log($"{name} ❄️ 스턴 상태!");
    }

    public void ApplyInfection()
    {
        if (isInfected) return;
        isInfected = true;
        infectionTimer = infectionDuration;
        tickTimer = 0f;
        spriteRenderer.color = Color.yellow;
        Debug.Log($"{name} 💉 감염 상태!");
    }

    public void ApplySuicide()
    {
        if (isSuiciding) return;

        isStunned = true;
        isSuiciding = true;
        suicideTimer = 2f;
        blinkTimer = 0f;
        blinkState = false;

        Debug.Log($"{name} 💥 자폭 카운트 시작");
    }

    public void ApplyBlind(float duration)
    {
        isBlinded = true;
        blindTimer = duration;
        spriteRenderer.color = Color.gray;
        Debug.Log($"{name} 👁️ 실명 상태!");
    }

    public void ApplyVulnerable(float duration)
    {
        isVulnerable = true;
        vulnerableTimer = duration;
        spriteRenderer.color = Color.magenta;
        Debug.Log($"{name} 💔 취약 상태!");
    }

    public void ApplyTurncoat(float duration)
    {
        isTurncoat = true;
        turncoatTimer = duration;
        spriteRenderer.color = new Color(0.5f, 1f, 0.5f);
        Debug.Log($"{name} 전향 상태!");

        BombEnemySpawner spawner = GetComponent<BombEnemySpawner>();
        if (spawner != null)
        {
            spawner.SetTurncoat();
        }
    }


    public static Transform currentLureTarget = null;

    public void ApplyLure(float duration)
    {
        currentLureTarget = transform;
        ApplyStun(duration);
        Invoke(nameof(ClearLureTarget), duration);

        Debug.Log($"{name} 유인 타겟 설정됨 (duration: {duration}s)");
    }

    void ClearLureTarget()
    {
        if (currentLureTarget == transform)
            currentLureTarget = null;
    }



    // -------- 상태 처리 핸들러들 --------

    void HandleStun()
    {
        if (!isStunned) return;
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            isStunned = false;
            ResetColor();
        }
    }

    void HandleInfection()
    {
        if (!isInfected) return;

        infectionTimer -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= infectionTickInterval)
        {
            tickTimer = 0f;
            ApplyTickDamage();
            TrySpreadInfection();
        }

        if (infectionTimer <= 0f)
        {
            isInfected = false;
            ResetColor();
        }
    }

    void HandleSuicide()
    {
        if (!isSuiciding) return;

        suicideTimer -= Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            blinkState = !blinkState;
            spriteRenderer.color = blinkState ? Color.red : originalColor;
        }

        if (suicideTimer <= 0f)
        {
            Debug.Log($"{name} 자폭!");
            if(explosionSound) SoundManager.inst.PlaySFX(explosionSound);
            Transform explosion = GameManager.inst.pool.GetRange(5).transform;
            explosion.localPosition = transform.position + new Vector3(0, 0.5f, 0);
            explosion.localRotation = Quaternion.identity;
            gameObject.SetActive(false); // 풀링 대응
        }
    }

    void HandleBlind()
    {
        if (!isBlinded) return;

        blindTimer -= Time.deltaTime;
        if (blindTimer <= 0f)
        {
            isBlinded = false;
            ResetColor();
        }
    }

    void HandleVulnerable()
    {
        if (!isVulnerable) return;

        vulnerableTimer -= Time.deltaTime;
        if (vulnerableTimer <= 0f)
        {
            isVulnerable = false;
            ResetColor();
        }
    }

    void HandleTurncoat()
    {
        if (!isTurncoat) return;

        turncoatTimer -= Time.deltaTime;
        if (turncoatTimer <= 0f)
        {
            isTurncoat = false;
            GetComponent<Enemy>().target = GameManager.inst.player.transform;
            ResetColor();
        }
    }

    // -------- 유틸리티 --------

    void ApplyTickDamage()
    {
        GetComponent<Enemy>()?.Damaged((int)tickDamage,Vector2.up);
    }

    void TrySpreadInfection()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, infectionRadius, enemyLayer);
        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject == gameObject) continue;
            EnemyHackable other = col.GetComponent<EnemyHackable>();
            if (other != null && !other.isInfected)
            {
                other.ApplyInfection();
            }
        }
    }

    public bool IsStunned() => isStunned;

    void ResetColor()
    {
        if (!isInfected && !isStunned && !isBlinded && !isVulnerable && !isTurncoat && !isSuiciding)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public bool IsTurncoat() => isTurncoat;

}
