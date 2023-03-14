using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    [SerializeField] private FootstepScriptableObject[] footsteps;
    [SerializeField][Range(0f, 1f)] private float runstepLengthen = 0.7f;
    [SerializeField] private float stepInterval = 2.5f;

    private FirstPersonController player;
    private CharacterController playerController;
    private TerrainTextureCheck checkTerrain;
    private AudioSource audioSource;
    private float stepCycle;
    private float nextStep;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<FirstPersonController>();
        playerController = GetComponent<CharacterController>();
        checkTerrain = GetComponent<TerrainTextureCheck>();
        audioSource = GetComponent<AudioSource>();

        nextStep = stepCycle / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        ProgressStepCycle();
    }

    void ProgressStepCycle() {
        if (playerController.velocity.sqrMagnitude > 0f && player.Move != Vector3.zero) {
            stepCycle += (playerController.velocity.magnitude + (player.CurrentSpeed * (player.IsRunning ? runstepLengthen : 1f))) * Time.deltaTime;
        }

        if (!(stepCycle > nextStep)) return;

        nextStep = stepCycle + stepInterval;

        // Spherecast from FirstPersonController
        if (player.GetGroundRaycastHit() is RaycastHit hit) {

            foreach (var footstep in footsteps) {
                if (footstep.Surface == Surface.None) continue;

                if (hit.collider.CompareTag(footstep.GetSurfaceTag())) {
                    PlayFootStepAudio(footstep.Footsteps);
                    return;
                }
            }

            if (hit.collider is not TerrainCollider) return;
            if (checkTerrain is null) return;

            checkTerrain.GetTerrainTexture();

            for (int i = 0; i < checkTerrain.Length; i++) {
                if (checkTerrain[i] > 0f) {
                    foreach (var step in footsteps) {
                        if (step.TerrainLayer == checkTerrain.Layers[i]) {
                            PlayFootStepAudio(step.Footsteps);
                        }
                    }
                }
            }
        }
    }

    void PlayFootStepAudio(AudioClip[] footsteps) {
        if (!player.IsGrounded()) return;

        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, footsteps.Length);
        audioSource.clip = footsteps[n];
        audioSource.PlayOneShot(audioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        footsteps[n] = footsteps[0];
        footsteps[0] = audioSource.clip;
    }
}
