using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Added Animator reference
    private Coroutine currentCoroutine;
    private bool isMoving = false;
    private bool isJuiceEffectPlaying = false; // Track if JuiceEffect is already playing

    public Sprite danceLeftSprite;
    public Sprite danceRightSprite;
    public Sprite danceUpSprite;
    public Sprite danceDownSprite;
    public Sprite idleSprite;
    public float idleDelay = 0.5f; // Delay in seconds before switching to idle sprite
    public float animatorReenableDelay = 0.25f; // Delay before re-enabling animator
    public AnimationCurve scaleCurve; // Curve for scaling effect
    public AnimationCurve bobCurve; // Curve for bobbing effect
    public float scaleDuration = 0.5f; // Duration of the scaling effect
    public float bobDuration = 0.5f; // Duration of the bobbing effect
    public float bobAmount = 0.5f; // Amount of vertical bobbing

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>
    {
        { KeyCode.LeftArrow, false },
        { KeyCode.RightArrow, false },
        { KeyCode.UpArrow, false },
        { KeyCode.DownArrow, false }
    };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Initialize Animator
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        bool isKeyPressed = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            HandleKeyPress(KeyCode.LeftArrow, danceLeftSprite);
            isKeyPressed = true;
        }
        else
        {
            HandleKeyRelease(KeyCode.LeftArrow);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            HandleKeyPress(KeyCode.RightArrow, danceRightSprite);
            isKeyPressed = true;
        }
        else
        {
            HandleKeyRelease(KeyCode.RightArrow);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            HandleKeyPress(KeyCode.UpArrow, danceUpSprite);
            isKeyPressed = true;
        }
        else
        {
            HandleKeyRelease(KeyCode.UpArrow);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            HandleKeyPress(KeyCode.DownArrow, danceDownSprite);
            isKeyPressed = true;
        }
        else
        {
            HandleKeyRelease(KeyCode.DownArrow);
        }

        if (isKeyPressed)
        {
            isMoving = true;
            if (!isJuiceEffectPlaying)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                if (animator != null)
                {
                    animator.enabled = false; // Disable animator
                }
                currentCoroutine = StartCoroutine(JuiceEffect(false)); // Movement effect
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(SetIdleAfterDelay());
        }
        else
        {
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }
    }

    void HandleKeyPress(KeyCode key, Sprite sprite)
    {
        if (!keyStates[key])
        {
            spriteRenderer.sprite = sprite;
            AudioManager.Instance.PlayDancestepSFX();
            keyStates[key] = true;
        }
    }

    void HandleKeyRelease(KeyCode key)
    {
        if (keyStates[key])
        {
            keyStates[key] = false;
        }
    }

    IEnumerator SetIdleAfterDelay()
    {
        yield return new WaitForSeconds(idleDelay); // Wait before switching to idle sprite
        spriteRenderer.sprite = idleSprite;
        currentCoroutine = StartCoroutine(JuiceEffect(true)); // Idle effect
        yield return new WaitForSeconds(animatorReenableDelay); // Wait before re-enabling the animator
        if (animator != null)
        {
            animator.enabled = true; // Re-enable animator when idle
        }
    }

    IEnumerator JuiceEffect(bool isIdle)
    {
        isJuiceEffectPlaying = true; // Indicate that JuiceEffect is playing
        float elapsedTime = 0f;

        float scaleDurationActual = isIdle ? scaleDuration : 0;
        float bobDurationActual = isIdle ? bobDuration : 0;

        while (elapsedTime < Mathf.Max(scaleDurationActual, bobDurationActual))
        {
            if (isIdle)
            {
                float scaleY = scaleCurve.Evaluate(Mathf.Clamp01(elapsedTime / scaleDuration)) * (originalScale.y - 1) + 1;
                transform.localScale = new Vector3(originalScale.x, scaleY, originalScale.z);

                float bob = bobCurve.Evaluate(Mathf.Clamp01(elapsedTime / bobDuration)) * bobAmount;
                transform.position = new Vector3(originalPosition.x, originalPosition.y + bob, originalPosition.z);
            }
            else
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isIdle)
        {
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }

        isJuiceEffectPlaying = false; // Indicate that JuiceEffect has finished
    }
}
