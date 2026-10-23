using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour
{
    float rotSpeed = 0;
    bool isSpinning = false;

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip tickSound;     
    public AudioClip redSound;      
    public AudioClip yellowSound;
    public AudioClip orangeSound;   
    public AudioClip greenSound;    

    [Header("轉動與判定設定")]
    public int totalSlots = 7;      
    private float tickInterval = 360f / 7f; 
    private float accumulatedAngle = 0f;    
    
    [Header("減速設定 (減法)")]
    public float friction = 800f;

    void Start()
    {
        Application.targetFrameRate = 60;
        Random.InitState((int)System.DateTime.Now.Ticks);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSpinning)
        {
            this.rotSpeed = Random.Range(1200f, 1800f);
            accumulatedAngle = 0f; 
            isSpinning = true;
        }

        if (isSpinning)
        {
            float frameRotation = this.rotSpeed * Time.deltaTime;
            transform.Rotate(0, 0, frameRotation);

            accumulatedAngle += Mathf.Abs(frameRotation);

            if (accumulatedAngle >= tickInterval)
            {
                accumulatedAngle %= tickInterval; 
                if (tickSound != null)
                {
                    audioSource.PlayOneShot(tickSound);
                }
            }

            // 線性減速 (減法)
            rotSpeed -= friction * Time.deltaTime;

            if (rotSpeed <= 0.0f)
            {
                rotSpeed = 0;
                isSpinning = false;

                // 直接結算，保留最自然的物理慣性停下
                CheckColorAndPlaySound();
            }
        }
    }
    
    void CheckColorAndPlaySound()
    {
        float anglePerSlot = 360f / totalSlots;
        float currentZ = transform.eulerAngles.z % 360f;
        if (currentZ < 0) currentZ += 360f;

        // 使用四捨五入計算最接近的格子，徹底解決後半段格子的誤差 Bug
        int landedSlot = Mathf.RoundToInt(currentZ / anglePerSlot) % totalSlots;
        if (landedSlot < 0) landedSlot += totalSlots;

        if (landedSlot == 2 || landedSlot == 5)
        {
            Debug.Log($"停在第 {landedSlot + 1} 格：紅色！");
            if (redSound != null) audioSource.PlayOneShot(redSound);
        }
        else if (landedSlot == 0 || landedSlot == 3)
        {
            Debug.Log($"停在第 {landedSlot + 1} 格：黃色！");
            if (yellowSound != null) audioSource.PlayOneShot(yellowSound);
        }
        else if (landedSlot == 1 || landedSlot == 4)
        {
            Debug.Log($"停在第 {landedSlot + 1} 格：橘色！");
            if (orangeSound != null) audioSource.PlayOneShot(orangeSound);
        }
        else
        {
            Debug.Log($"停在第 {landedSlot + 1} 格：綠色！");
            if (greenSound != null) audioSource.PlayOneShot(greenSound);
        }
    }
}