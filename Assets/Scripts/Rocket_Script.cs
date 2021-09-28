using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket_Script : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] int LevelLoadDelay;

    [SerializeField] AudioClip MainEngine;
    [SerializeField] AudioClip Death;
    [SerializeField] AudioClip Success;

    [SerializeField] ParticleSystem MainEngineParticle;
    [SerializeField] ParticleSystem DeathParticle;
    [SerializeField] ParticleSystem SuccessParticle;

    [SerializeField] MeshRenderer RB;
    [SerializeField] MeshRenderer RLB;
    [SerializeField] MeshRenderer RRB;
    [SerializeField] MeshRenderer RN;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool CollisionDisabled = false;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (state != State.Alive || CollisionDisabled) 
        {
            return;                  // ignore collisions when dead
        }
        else if(coll.gameObject.tag == "Friendly")
        {
            //Debug.Log("Its Friendly");
        }
        else if(coll.gameObject.tag == "Finish")
        {
            StartSuccessSequence();
        }
        else
        {
            StartDeathSequence();
        }
    }
    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            CollisionDisabled = !CollisionDisabled;
        }
    }
    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(Success);
        SuccessParticle.Play();
        Invoke("LoadNextLevel", LevelLoadDelay);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        DestroyingRocket();
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        MainEngineParticle.Stop();
        DeathParticle.Play();
        Invoke("LoadSameLevel", LevelLoadDelay);
    }
    private void DestroyingRocket()
    {
        RB.enabled = false;
        RLB.enabled = false;
        RRB.enabled = false;
        RN.enabled = false;
    }
    private void LoadNextLevel()
    {
        int CurrentScene = SceneManager.GetActiveScene().buildIndex;
        int NextSceneIndex = CurrentScene + 1;
        if(NextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            NextSceneIndex = 0;
        }
        SceneManager.LoadScene(NextSceneIndex); 
    }
    private void LoadSameLevel()
    {
        int CurrentLoadScene = SceneManager.GetActiveScene().buildIndex;
        int LoadScene = CurrentLoadScene;
        SceneManager.LoadScene(LoadScene);
    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            MainEngineParticle.Stop();
        }
    }
    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(MainEngine);
        }
        MainEngineParticle.Play();
    }
    private void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.freezeRotation = true; // take manual control of rotation
            transform.Rotate(Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false; // resume physics control of rotation
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true; // take manual control of rotation
            transform.Rotate(-Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false; // resume physics control of rotation
        }
    }
}