using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAwakening
{
    public Image blackScreen;
    public AudioSource bellSound;
    public AudioSource rainSound;
    public GameObject player;

    public float fadeDuration = 3f;

    public PlayerAwakening()
    {
        GameObject blackScreenObject = GameObject.Find("Black Cover");
        GameObject bellSoundObject = GameObject.Find("Church Bell Sound");
        GameObject ambientSoundObject = GameObject.Find("Rain Sound");
        player = GameObject.Find("Player");

        if (blackScreenObject != null)
        {
            blackScreen = blackScreenObject.GetComponent<Image>();
        }
        if (bellSoundObject != null)
        {
            bellSound = bellSoundObject.GetComponent<AudioSource>();
        }
        if (ambientSoundObject != null)
        {
            rainSound = ambientSoundObject.GetComponent<AudioSource>();
        }
    }

    public IEnumerator AwakeningStart()
    {
        if (blackScreen == null || bellSound == null || player == null)
        {
            Debug.LogError("PlayerAwakening: One or more of the required objects are null");
            yield break;
        }

        yield return new WaitForSecondsRealtime(2f);

        bellSound.Play();

        yield return new WaitForSecondsRealtime(bellSound.clip.length / 16);

        Color startColor = blackScreen.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) 
        {
            float percentFaded = Mathf.Clamp01(elapsedTime / fadeDuration);
            float nextLoopDistance = 0.005f;
            elapsedTime += nextLoopDistance;

            // interpolate from the start color to percentFaded% towards the end color
            blackScreen.color = Color.Lerp(startColor, endColor, percentFaded);

            // second half drags a bit, so making it faster
            yield return new WaitForSecondsRealtime(0.005f);
        }

        StateManager.Instance.SetState(StateManager.GameState.Playing);
    }
}
