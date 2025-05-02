using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;  // Important to use the EventSystem for input focus

public class BlinkingPlaceholderAsCaret : MonoBehaviour
{
    public TMP_InputField inputField;    // Reference to the TMP_InputField
    public string placeholderText = "_"; // The underscore placeholder text

    private bool isTyping = false;       // Flag to check if the user is typing
    private Coroutine blinkCoroutine;    // Coroutine to manage the blinking
    private TextMeshProUGUI placeholderTextComponent; // Reference to the placeholder text component

    void Start()
    {
        inputField.selectionColor = new Color(50, 50, 50);

        // Get the placeholder TextMeshProUGUI component
        placeholderTextComponent = inputField.placeholder.GetComponent<TextMeshProUGUI>();

        // Set the initial placeholder text to an underscore
        placeholderTextComponent.text = placeholderText;

        // Hide the default caret by setting caretWidth to 0
        inputField.caretWidth = 0;

        // Set caret color to transparent to ensure it's hidden
        inputField.caretColor = new Color(0, 0, 0, 0);  // Fully transparent caret

        // Set caret blink rate to 0 to stop the blinking caret behavior
        inputField.caretBlinkRate = 0f;

        // Start the blinking underscore coroutine
        blinkCoroutine = StartCoroutine(BlinkUnderscoreCaret());

        // Detect when the user starts typing
        inputField.onValueChanged.AddListener(OnInputChanged);

        // Detect when the input field is deselected (clicked out)
        inputField.onDeselect.AddListener(OnFieldDeselected);

        // Automatically select and focus on the input field after a brief delay to ensure UI is initialized
        StartCoroutine(FocusOnInputFieldAfterDelay());
    }

    // Coroutine to automatically focus the input field with a brief delay to ensure UI is initialized
    private IEnumerator FocusOnInputFieldAfterDelay()
    {
        // Wait until the next frame to ensure the UI is fully initialized
        yield return new WaitForEndOfFrame();

        // Ensure that the input field is selected and activated by the EventSystem
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.Select();
        inputField.ActivateInputField();
    }

    // Coroutine to make the underscore blink as a caret
    private IEnumerator BlinkUnderscoreCaret()
    {
        while (true)
        {
            // Toggle the placeholder between "_" and an empty string to simulate blinking
            placeholderTextComponent.text = placeholderText;
            yield return new WaitForSeconds(0.5f);  // Blink interval

            placeholderTextComponent.text = "";
            yield return new WaitForSeconds(0.5f);  // Blink interval
        }
    }

    // Called whenever the input field's text changes
    private void OnInputChanged(string text)
    {
        // If the user starts typing, stop the blinking underscore
        if (!isTyping && text.Length > 0)
        {
            StopCoroutine(blinkCoroutine);
            isTyping = true;

            // Clear the placeholder text (no underscore)
            placeholderTextComponent.text = text;

            // Restore the blinking underscore as the caret after the user finishes typing
            placeholderTextComponent.text += "_";
        }

        // If the text is empty again, restart blinking underscore
        if (isTyping && text.Length == 0)
        {
            isTyping = false;
            blinkCoroutine = StartCoroutine(BlinkUnderscoreCaret());
        }
    }

    // Called when the input field is deselected (clicked out of)
    private void OnFieldDeselected(string text)
    {
        // Restart blinking underscore when the input field is deselected and text is empty
        if (string.IsNullOrEmpty(inputField.text))
        {
            blinkCoroutine = StartCoroutine(BlinkUnderscoreCaret());
        }
    }

    // Ensure the blinking stops when the object is destroyed
    private void OnDestroy()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
    }
}
