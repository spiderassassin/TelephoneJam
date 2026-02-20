using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HisaGames.TransformSetting;
using HisaGames.Cutscene;
using HisaGames.Props;
using HisaGames.Character;
using System;
using DG.Tweening;


namespace HisaGames.CutsceneManager
{
    public class EcCutsceneManager : MonoBehaviour
    {
        public static EcCutsceneManager instance;

        [Header("Character Settings")]
        [Tooltip("Array of character prefab objects used in the cutscene.")]
        [SerializeField] GameObject[] characterPrefabs;

        [HideInInspector]
        [Tooltip("Array of initialized character objects.")]
        EcCharacter[] characters;

        [Header("Properties Settings")]
        [Tooltip("Array of prop prefab objects used in the cutscene.")]
        [SerializeField] GameObject[] propPrefabs;

        [HideInInspector]
        [Tooltip("Array of initialized prop objects.")]
        EcProps[] props;

        [Header("Cutscene Settings")]
        [Tooltip("Panel that contains cutscene elements.")]
        [SerializeField] EcCutscene[] cutscenes;

        [Tooltip("Name of current active cutscene.")]
        [SerializeField] string currentCutscene;

        [Tooltip("Array of transform settings for character positions and movements.")]
        [SerializeField] EcTransformSetting[] transformSettings;

        [Header("Other Settings")]
        [Tooltip("Panel that contains gui elements such as character, props and others.")]
        [SerializeField] GameObject guiPanel;

        [Tooltip("Timer for auto-playing the next cutscene. Set to 0 to disable auto-play.")]
        public float autoplayTime;

        [Tooltip("Speed at which the character transitions between positions, rotations, or scales. Default value is 30")]
        public float characterTransitionSpeed;

        [Tooltip("Delay between each character in chat typing (in seconds).")]
        public float chatTypingDelay;
        
        public float tweenTime = 0.5f;

        private void Awake()
        {
            instance = this;
            InitCharacters();
            InitProps();
            InitCutscenes(currentCutscene);
        }

        /// <summary>
        /// Initializes characters by instantiating prefabs and storing them in the characters array.
        /// </summary>
        void InitCharacters()
        {
            characters = new EcCharacter[characterPrefabs.Length];
            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                GameObject temp = Instantiate(characterPrefabs[i]);
                temp.name = temp.name.Replace("(Clone)", "");
                // Debug.Log(temp.transform.position);
                // Debug.Log(temp.transform.localPosition);
                // Debug.Log(temp.GetComponent<RectTransform>().anchoredPosition);
                // Debug.Log("/////////////");
                temp.transform.SetParent(guiPanel.transform, false);
                temp.transform.SetAsFirstSibling();
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000,0);
                // Debug.Log(temp.transform.position);
                // Debug.Log(temp.transform.localPosition);
                // Debug.Log(temp.GetComponent<RectTransform>().anchoredPosition);
                characters[i] = temp.GetComponent<EcCharacter>();
            }
        }

        /// <summary>
        /// Initializes props by instantiating prefabs and storing them in the props array.
        /// </summary>
        void InitProps()
        {
            props = new EcProps[propPrefabs.Length];
            for (int i = 0; i < propPrefabs.Length; i++)
            {
                GameObject temp = Instantiate(propPrefabs[i]);
                temp.name = temp.name.Replace("(Clone)", "");
                temp.transform.SetParent(guiPanel.transform);
                props[i] = temp.GetComponent<EcProps>();
            }
        }

        /// <summary>
        /// Initializes props by instantiating prefabs and storing them in the props array.
        /// </summary>
        /// 
        public void closeCutscenesInstant()
        {
            guiPanel.GetComponent<CanvasGroup>().alpha = 0;
            guiPanel.SetActive(false);
            for (int i = 0; i < cutscenes.Length; i++)
            {
                cutscenes[i].gameObject.SetActive(false);
            }
        }
        public void closeCutscenes()
        {
            guiPanel.GetComponent<CanvasGroup>().DOFade(0, tweenTime).OnComplete(() =>
            {
                guiPanel.SetActive(false);
                for (int i = 0; i < cutscenes.Length; i++)
                {

                    cutscenes[i].gameObject.SetActive(false);
                }
            });
        }
        public void InitCutscenes(string cutsceneName)
        {
            closeCutscenesInstant();

            currentCutscene = cutsceneName;
            EcCutscene temp = getCutscenesObject(currentCutscene);
            if (temp != null)
            {
                guiPanel.SetActive(true);
                guiPanel.GetComponent<CanvasGroup>().DOFade(1, tweenTime).OnComplete(()=>
                {
                    temp.gameObject.SetActive(true);
                    temp.StartCutscene();
                });
            }
        }


        /// <summary>
        /// Retrieves a character object by its name.
        /// </summary>
        /// <param name="name">The name of the character to retrieve.</param>
        /// <returns>The character object if found, otherwise null.</returns>
        public EcCharacter getCharacterObject(string name)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (name == characters[i].name)
                    return characters[i];
            }

            return null;
        }

        /// <summary>
        /// Retrieves a prop object by its name.
        /// </summary>
        /// <param name="name">The name of the prop to retrieve.</param>
        /// <returns>The prop object if found, otherwise null.</returns>
        public EcProps getPropObject(string name)
        {
            for (int i = 0; i < props.Length; i++)
            {
                if (name == props[i].name)
                    return props[i];
            }

            return null;
        }

        /// <summary>
        /// Retrieves a transform setting for a character by its name.
        /// </summary>
        /// <param name="name">The name of the transform setting to retrieve.</param>
        /// <returns>The transform setting if found, otherwise null.</returns>
        public EcTransformSetting getCharaTransformSetting(string name)
        {
            for (int i = 0; i < transformSettings.Length; i++)
            {
                if (name == transformSettings[i].name)
                    return transformSettings[i];
            }

            return null;
        }

        /// <summary>
        /// Retrieves a cutscene object by its name.
        /// </summary>
        /// <param name="name">The name of the cutscene to retrieve.</param>
        /// <returns>The cutscene object if found, otherwise null.</returns>
        public EcCutscene getCutscenesObject(string name)
        {
            for (int i = 0; i < cutscenes.Length; i++)
            {
                if (name == cutscenes[i].name)
                    return cutscenes[i];
            }

            Debug.Log("cutscene with name " + name + " not found");
            return null;
        }

        public void PlayNextCutscene()
        {
            getCutscenesObject(currentCutscene).PlayNextCutscene();
        }
    }
}