using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 8f;
    private bool isFacingRight = true;
    public TMP_Text winText;
    public TMP_Text failText;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            winText.gameObject.SetActive(true);

            Time.timeScale = 0f;

            CustomEvent levelStatistics = new CustomEvent("levelStatistics")
            {
                { "level_name", SceneManager.GetActiveScene().name },
                { "energy_used_count", PlayerPrefs.GetInt("EnergyUsedCount", 0) },
                { "death_count", PlayerPrefs.GetInt("DeathCount", 0) },
                { "restart_count", PlayerPrefs.GetInt("RestartCount", 0) },
            };

            AnalyticsService.Instance.RecordEvent(levelStatistics);
            AnalyticsService.Instance.Flush();
        }
        else if (collision.tag == "Enemy")
        {
            failText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("DeathCount", PlayerPrefs.GetInt("DeathCount", 0) + 1);

            Time.timeScale = 0f;

            CustomEvent deathToSpike = new CustomEvent("deathToSpike")
            {
                { "level_name", SceneManager.GetActiveScene().name },
                { "spike_name", collision.name },
            };
            AnalyticsService.Instance.RecordEvent(deathToSpike);
            AnalyticsService.Instance.Flush();
        }
    }

}