using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueSystem : MonoBehaviour
{
    // These variables cache the UI elements for the dialogue
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject continueUI;
    
    GameLogic gameLogic;
    Animator animator;
    AudioManager audioManager;

    string currentText;
    bool closeEnough;
    int pressed = 1; // This is set to one so it skips over the first element. The unity GUI doesn't display element 0 well for string in an array


    private GenereationGameJam2022 playerInputAction;

    private void Awake()
    {
        playerInputAction = new GenereationGameJam2022();
        animator = FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Animator>();
        gameLogic = FindObjectOfType<GameLogic>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnEnable()
    {
        // this just subscribes to an event to check if they pressed the interact key
        playerInputAction.Player.Interact.performed += NextText;
        playerInputAction.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        // when this object is disabled this disables the subscription to the interact key
        playerInputAction.Player.Interact.performed -= NextText;
        playerInputAction.Player.Interact.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this checks if the player is close enough to the NPC and if it is, it changes closeEnough to true
        if (collision.CompareTag("Player"))
        {
            closeEnough = true;
            continueUI.SetActive(true);
            currentText = dialogue.GetDialogueText()[pressed].Replace("(current number)", gameLogic.GetNumberOfCollectables().ToString());
            currentText = currentText.Replace("(collected number)", gameLogic.GetCollected().ToString());
            currentText = currentText.Replace("(total number)", gameLogic.GetTotalCollectables().ToString());
            currentText = currentText.Replace("(total to win)", gameLogic.GetTotalToWin().ToString());
            currentText = currentText.Replace("(current to win)", gameLogic.GetCurrentToWin().ToString());
            dialogueText.text = currentText;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // this checks if the player is close enough to the NPC and if it isn't, it changes closeEnough to false
        if (collision.CompareTag("Player"))
        {
            closeEnough = false;
        }
    }

    // Fixed Update runs 60 times a second
    private void FixedUpdate()
    {
        // this checks if the dialogue UI should be shown
        if (closeEnough)
        {
            canvas.SetActive(true);
        }
        else
        {
            if (dialogue == null)
            {
                Debug.LogWarning("There is no dialogue in " + gameObject.transform.parent.name);
                return;
            }

            dialogueText.text = dialogue.GetDialogueText()[pressed];
            canvas.SetActive(false);
        }
    }

    private void NextText(InputAction.CallbackContext obj)
    {
        // this checks if the player is close enough. If not then the rest of the code doesn't run
        if (!closeEnough) { return; }
        if (dialogue == null) { return; }
        
        continueUI.SetActive(true);

        // this code block determines what text to show in the UI and dynamically updates the text to put in the correct values
        if (pressed < dialogue.GetDialogueText().Length - 1)
        {
            // Plays UI sound
            audioManager.PlaySound("next");
            
            // Iterates pressed to go to the next piece of dialogue
            pressed++;

            // This block is the dynamic dialogue syntax system (DDSS)
            currentText = dialogue.GetDialogueText()[pressed].Replace("(current number)", gameLogic.GetNumberOfCollectables().ToString());
            currentText = currentText.Replace("(collected number)", gameLogic.GetCollected().ToString());
            currentText = currentText.Replace("(total number)", gameLogic.GetTotalCollectables().ToString());
            currentText = currentText.Replace("(total to win)", gameLogic.GetTotalToWin().ToString());
            currentText = currentText.Replace("(current to win)", gameLogic.GetCurrentToWin().ToString());

            // This plays the animation only if the player is talking
            if (currentText.Contains("You:")) { animator.SetTrigger("talked"); }

            // This actually changes the text to the correct text
            dialogueText.text = currentText;
        } else if (!gameLogic.HaveCollected()) // this section checks if the player has collected any collectable and if they have it adds the more in the string
        {
            // This turns off the continue UI so players don't think there's more text
            continueUI.SetActive(false);

            // DDSS ^^^ Read line 111
            currentText = dialogue.GetRepeatingText().Replace("(total number)", gameLogic.GetTotalCollectables().ToString());
            currentText = currentText.Replace("(total to win)", gameLogic.GetTotalToWin().ToString());
            currentText = currentText.Replace("(current to win)", gameLogic.GetCurrentToWin().ToString());
            currentText = currentText.Replace("(collected number)", gameLogic.GetCollected().ToString());
            currentText = currentText.Replace("(current number)", gameLogic.GetNumberOfCollectables().ToString());
            if (dialogueText.text == currentText) { return; }
            audioManager.PlaySound("next");
            dialogueText.text = currentText;

        } else
        {
            continueUI.SetActive(false);
            currentText = dialogue.GetRepeatingText().Replace("(total number)", gameLogic.GetTotalCollectables().ToString());
            currentText = currentText.Replace("(collected number)", gameLogic.GetCollected().ToString());
            currentText = currentText.Replace("(total to win)", gameLogic.GetTotalToWin().ToString());
            currentText = currentText.Replace("(current to win)", gameLogic.GetCurrentToWin().ToString() + " more");
            currentText = currentText.Replace("(current number)", gameLogic.GetNumberOfCollectables().ToString());
            if (dialogueText.text == currentText) { return; }
            audioManager.PlaySound("next");
            dialogueText.text = currentText;
        }

        // if the player has collected all the collectables it shows the success text stored in the dialogue scriptable object
        if (gameLogic.HasWon() && pressed == dialogue.GetDialogueText().Length - 1)
        {
            continueUI.SetActive(false);
            currentText = dialogue.GetSuccessText().Replace("(current number)", gameLogic.GetNumberOfCollectables().ToString());
            currentText = currentText.Replace("(total to win)", gameLogic.GetTotalToWin().ToString());
            currentText = currentText.Replace("(current to win)", gameLogic.GetCurrentToWin().ToString());
            currentText = currentText.Replace("(collected number)", gameLogic.GetCollected().ToString());
            currentText = currentText.Replace("(total number)", gameLogic.GetTotalCollectables().ToString());
            if (dialogueText.text == currentText) { return; }
            audioManager.PlaySound("next");
            dialogueText.text = currentText;
        }
    }

}
