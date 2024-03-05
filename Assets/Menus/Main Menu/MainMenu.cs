using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverBeebe.UnityUtilities.Runtime;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField] private Scene loadScene;
    [Space]

    [SerializeField] private Wave3 positionWave, rotationWave;
    [SerializeField] private Transform waveTransform;
    [SerializeField] private VolumeProfile postProcessing;
    [SerializeField] private float dofSpeed;
    [SerializeField] private new Camera camera;

    [Header("Start Sequence")]
    [SerializeField] private SoundEffect startSound;
    [SerializeField] private float soundDelay;
    [SerializeField] private SmartCurve bloomIntensity;
    [SerializeField] private Image screenCover;
    [SerializeField] private SmartCurve screenCoverColor;

    private float dofVelocity;

    private void Start() {
        postProcessing.TryGet(out Bloom bloom);
        bloom.threshold.value = 1;
        bloom.intensity.value = 1;
        screenCover.enabled = false;
    }

    private void Update() {

        waveTransform.localPosition = positionWave.Evaluate();
        waveTransform.localEulerAngles = rotationWave.Evaluate();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, GameInfo.GroundMask) && postProcessing.TryGet(out DepthOfField dof)) {

            float dofTarget = (camera.transform.position - hit.point).magnitude;

            dof.focusDistance.value = Mathf.SmoothDamp(dof.focusDistance.value, dofTarget, ref dofVelocity, dofSpeed);
        }
    }

    public void StartGame() {

        StartCoroutine(LightningFlash());
        StartCoroutine(SoundDelay());

        IEnumerator LightningFlash() {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            postProcessing.TryGet(out Bloom bloom);

            bloom.threshold.value = 0;

            bloomIntensity.Start();
            while (!bloomIntensity.Done) {
                bloom.intensity.value = bloomIntensity.Evaluate();
                yield return null;
            }

            bloom.threshold.value = 1;
            bloom.intensity.value = 1;
            screenCover.enabled = true;

            screenCoverColor.Start();
            while (!screenCoverColor.Done) {
                screenCover.color = Color.Lerp(Color.white, Color.black, screenCoverColor.Evaluate());
                yield return null;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(loadScene);
        }

        IEnumerator SoundDelay() {

            yield return new WaitForSeconds(soundDelay);

            startSound.Play(this);
        }
    }
}
