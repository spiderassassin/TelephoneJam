using System.Collections.Generic;
using UnityEngine;
using HisaGames.CutsceneManager;
using UnityEngine.UI;


namespace HisaGames.Character
{
    [System.Serializable]
    public class EcCharacter : MonoBehaviour
    {
        public enum CharacterState
        {
            StayInScene,
            Moving
        }

        [Header("State Settings")]
        [HideInInspector]
        [Tooltip("Current state of the character (StayInScene, MoveIn, MoveOut).")]
        public CharacterState characterState;

        [Tooltip("Target position for the character when moving.")]
        private Vector2 targetMovePosition;

        [Header("Sprite Settings")]
        [HideInInspector]
        [Tooltip("Image component for displaying character sprites.")]
        public Image image;

        [Tooltip("Array of sprites used for the character.")]
        public Sprite[] spriteImages;

        [Tooltip("Dictionary to map sprite names to their corresponding sprites.")]
        private Dictionary<string, Sprite> spriteDictionary;

        void Awake()
        {
            // Initialize the sprite dictionary & Populate the dictionary with sprites
            spriteDictionary = new Dictionary<string, Sprite>();
            foreach (var sprite in spriteImages)
            {
                spriteDictionary[sprite.name] = sprite;
            }

            // Get the Image component
            image = GetComponentInChildren<Image>();
        }

        /// <summary>
        /// Change the sprite of the character by name.
        /// </summary>
        /// <param name="spriteName">Name of the sprite to change to.</param>
        public void ChangeSpriteByName(string spriteName)
        {
            if (image != null)
            {
                if (spriteDictionary.TryGetValue(spriteName, out var newSprite))
                {
                    image.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning($"Sprite with name {spriteName} not found.");
                }
            }
            else
            {
                Debug.LogWarning("image is null.");
            }
        }

        /// <summary>
        /// Check and update the character's state during runtime.
        /// </summary>
        public void CheckingCharacterState()
        {
            var step = EcCutsceneManager.instance.characterTransitionSpeed * Time.deltaTime;

            switch (characterState)
            {
                case CharacterState.Moving:
                    Debug.Log("Supposed to move");
                    GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(GetComponent<RectTransform>().anchoredPosition, targetMovePosition, step);

                    if (targetMovePosition == GetComponent<RectTransform>().anchoredPosition)
                    {
                        characterState = CharacterState.StayInScene;
                        Debug.Log("Play StayInScene");
                    }

                    break;
                case CharacterState.StayInScene:
                    break;
            }
        }

        /// <summary>
        /// Set the character to move into the scene with a specific position and movement type.
        /// </summary>
        /// <param name="targetPosition">Target position for the character.</param>
        /// <param name="transformType">Movement type (e.g., LeftIn, RightIn).</param>
        public void SetCharacterMove(Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale)
        {
            targetMovePosition = new Vector2(targetPosition.x, targetPosition.y);
            if (transform.GetComponent<RectTransform>().anchoredPosition != targetMovePosition)
            {
                characterState = CharacterState.Moving;
                Debug.Log("Play Moving");
            }
        }
    }
}