using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tks
{

    public static CursorLockMode cursorState = CursorLockMode.Locked;

    public static IEnumerator SetTimeout(System.Action action, float delay, bool realTime = false)
    {
        if (!realTime)
            yield return new WaitForSeconds(delay / 1000);
        else
            yield return new WaitForSecondsRealtime(delay / 1000);
        action();
    }

    //

    public enum WeaponType
    {
        Revolver,
        MicroSMG,
        Shotgun,
    }

    [System.Serializable]
    public class Collectable
    {
        public WeaponType type;
        public Sprite ico;
        public bool enabled;

        [Space(10)]

        public Animate animateTopic;
        public Fire fireTopic;
        public Ammunation ammoTopic;
        public Audio audioTopic;

        #region --class containers
        [System.Serializable]
        public class Animate
        {
            public Animator animator;
            [Tooltip("Animation name for camera animation")] public string shake = "shake";
            [Tooltip("Animation name for gun reload")] public string[] reload = new string[2];

            public float pullBack;
            [Range(0, 1)] public float pullBackSmoothing = 0.05f;
        }

        [System.Serializable]
        public class Fire
        {
            [Range(0, 1)] public float damage = 0.01f;
            [Tooltip("Piercing for enemies only")] public bool piercing;
            [Tooltip("Damange reduction each piercing for enemies")][Range(0, 1)] public float breakDamageReduction = 0.25f;

            [Space(10)]

            [Range(1, 10)] public int bullets = 1;

            [Space(10)]

            [Range(0, 1)] public float spread = 0.1f;

            [Space(10)]

            [Range(0, 90)] public float recoil = 5;
            [Range(0, 1)] public float recoilSmoothing = 0.025f;

            [Header("Cooldowns")]

            [Tooltip("Reload cooldown, ammo restores after this timer ends")] [Range(0, 10)] public float reloadTime;
            [Tooltip("Fire cooldown time")] [Range(0, 10)] public float fireRate = 0.25f;
        }
        
        [System.Serializable]
        public class Ammunation
        {
            public int ammo;
            [Range(0, 999)] public int ammoLimit;
        }

        [System.Serializable]
        public class Audio
        {
            public AudioSource audioSrc;

            [Space(10)]

            public AudioClip clip;
            
            [Space(10)]

            [Range(0, 1)] public float volume = 0.225f;
            [Range(0, 1)] [SerializeField] public float pitchDifferRange = 0.1f;
        }
        #endregion
    }

    //run everyframe
    static Dictionary<GameObject, bool> flickerDebFlags = new Dictionary<GameObject, bool>();

    public static IEnumerator FlickerImg(GameObject img, float timeout = 50)
    {
        if (!flickerDebFlags.ContainsKey(img))
            flickerDebFlags.Add(img, false);

        if (flickerDebFlags[img]) yield break;
        flickerDebFlags[img] = true;

        img.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeout / 1000);
        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeout / 1000);

        flickerDebFlags[img] = false;
    }


    //neg or pos

    public static int GetRandomSign()
    {
        int random = Random.Range(0, 2);

        int result = (random == 0) ? 1 : -1;

        return result;
    }

    //Changed listener

    static Dictionary<string, float> current = new Dictionary<string, float>();
    static Dictionary<string, float> late = new Dictionary<string, float>();

    public static void OnValueChanged(System.Action<bool> action, float real, string key)
    {
        if (!current.ContainsKey(key))
        {
            current.Add(key, 0);
            late.Add(key, 0);
        }

        current[key] = real;

        if (current[key] != late[key])
            action.Invoke(current[key] > late[key]);


        late[key] = real;
    }

    #region --random percentange chance
    public static bool TestChance(float inputChance)
    {
        float randomPercent = Random.Range(0f, 1f);

        bool result = (randomPercent <= Mathf.Clamp01(inputChance));

        return result;
    }
    #endregion
}
