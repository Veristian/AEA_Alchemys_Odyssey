using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SliderAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;

    [Header("Volume")]
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float fadeOutSpeed = 4f;

    [Header("Interaction")]
    [SerializeField] private float stopDelay = 0.1f;
    [SerializeField] private float minLerpSpeed = 2f;
    [SerializeField] private float maxLerpSpeed = 20f;
    [SerializeField] private float speedMultiplier = 40f;

    private float lastValue;
    private float lastChangeTime = -999f;

    private void Reset()
    {
        slider = GetComponent<Slider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.Stop();
        audioSource.volume = 0f;

        lastValue = slider.value;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        lastChangeTime = Time.time;
    }

    private void Update()
    {
        float currentValue = slider.value;

        // Slider value change per second
        float valueSpeed = Mathf.Abs(currentValue - lastValue) / Mathf.Max(Time.deltaTime, 0.0001f);
        lastValue = currentValue;

        bool interacting = Time.time - lastChangeTime < stopDelay;

        if (interacting)
        {
            // Start the audio only when the slider is moved
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Faster movement = faster fade in
            float lerpSpeed = Mathf.Lerp(
                minLerpSpeed,
                maxLerpSpeed,
                Mathf.Clamp01(valueSpeed * speedMultiplier)
            );

            audioSource.volume = Mathf.Lerp(
                audioSource.volume,
                maxVolume,
                lerpSpeed * Time.deltaTime
            );
        }
        else
        {
            // Fade out
            audioSource.volume = Mathf.Lerp(
                audioSource.volume,
                0f,
                fadeOutSpeed * Time.deltaTime
            );

            // Stop once effectively silent
            if (audioSource.volume <= 0.01f)
            {
                audioSource.Stop();
                audioSource.volume = 0f;
            }
        }
    }
}