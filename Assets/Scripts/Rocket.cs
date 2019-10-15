using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip winSFX;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;


    private Rigidbody rigidBody;
    private AudioSource audioSource;
    
    enum State { ALIVE, DYING, TRANSCENDING};
    State currentState = State.ALIVE;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == State.ALIVE)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentState != State.ALIVE)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly": // do nothing;
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }

    }

    private void StartDeathSequence()
    {
        currentState = State.DYING;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSFX);
        deathParticles.Play();
        Invoke("ReloadLevel", 2f);
    }

    private void StartSuccessSequence()
    {
        currentState = State.TRANSCENDING;
        audioSource.Stop();
        audioSource.PlayOneShot(winSFX);
        winParticles.Play();
        Invoke("LoadNextLevel", 2f);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput()
    {
        // Thrusting
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        // Rotating Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        // Rotating Right
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    
}
