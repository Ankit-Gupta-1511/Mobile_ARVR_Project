using UnityEngine;

public class GazeAnimationTrigger : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;

    public string animationStateName = "Unbox"; // Use your actual animation clip name
    public float gazeDuration = 2f;

    private float gazeTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartGaze()
    {
        gazeTimer = 0f;
        hasPlayed = false;
    }

    public void OnGazeStay()
    {
        if (hasPlayed) return;

        gazeTimer += Time.deltaTime;
        Debug.Log("Checking for Animation...");

        
        // if (gazeTimer >= gazeDuration)
        // {
        Debug.Log("Playing Animation on all siblings...");

        // Play animation on this object
        if (animator != null)
            animator.Play(animationStateName);

        // Play animation on all siblings with an Animator
        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform sibling in parent)
            {
                Animator siblingAnimator = sibling.GetComponent<Animator>();
                GazeAnimTarget target = sibling.GetComponent<GazeAnimTarget>();
                if (siblingAnimator != null && siblingAnimator != animator && !string.IsNullOrEmpty(target.animationStateName))
                {
                    siblingAnimator.Play(target.animationStateName);
                }
            }
        }

        hasPlayed = true;
        // }
    }

    public void EndGaze()
    {
        gazeTimer = 0f;
    }
}
