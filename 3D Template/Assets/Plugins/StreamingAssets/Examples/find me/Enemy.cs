using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;


[System.Serializable]
public class AnimationBlend
{
    #region Variables
    [SerializeField] Enemy enemy;

    [Space(10)]

    public Animator animator;

    [Space(10)]

    float blend;
    [Tooltip("Animtion blend speed")] [Range(0, 1)] [SerializeField] float smoothing = 0.5f;
    #endregion
    #region Methods
    public void Update()
    {
        bool moving = enemy.agent.velocity.magnitude > 0.5f;
        blend = Mathf.Lerp(blend, moving ? 1 : 0, smoothing);

        animator.SetFloat("Movement", blend);
    }
    #endregion
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]

    public NavMeshAgent agent;
    float initialSpeed;

    [Range(0, 1)] public float health = 1;

    bool died;

    Transform plr;
    Health plrHealth;

    [Header("Attack")]

    [Range(0, 1)] [SerializeField] float damage = 0.125f;

    [Space(10)]

    public float attackRange = 1f;
    float range;
    bool inRange;

    [Tooltip("Time enemy is in range before affecting player")] [SerializeField] float attackTime = 0.15f;
    [Tooltip("Attack cooldown (Recommened to use animation length)")] [SerializeField] float attackSleepTime = 0.25f;
    bool attackDeb;

    [Space(10)]

    [SerializeField] bool explodeOnDeath;
    [Range(0, 1)] [SerializeField] float maxExplodeDamge;
    public float minExplodeRadius;
    public float maxExplodeRadius;


    [Header("Animation")]

    [SerializeField] AnimationBlend movementBlend;

    void Awake()
    {
        initialSpeed = agent.speed;

        plr = GameObject.Find("PLAYER").transform;
        plrHealth = plr.GetComponent<Health>();
    }

    void Update()
    {
        health = Mathf.Clamp01(health);

        range = (transform.position - plr.position).magnitude;
        inRange = (range < attackRange);

        //Follow

        agent.SetDestination(plr.position);
        agent.isStopped = inRange;

        if (health > 0)
            agent.speed = (!attackDeb) ? initialSpeed : initialSpeed / 2;


        //Attack

        if (inRange && !attackDeb)
        {
            attackDeb = true;

            movementBlend.animator.Play("Attack", 1);

            StartCoroutine(Tks.SetTimeout(() => {
                if (inRange) {
                    plrHealth.value -= damage;
                }
            } , attackTime * 1000));

            //deb
            StartCoroutine(Tks.SetTimeout(() => attackDeb = false, attackSleepTime * 1000));
        }

        //death
        if (health <= 0 && !died) {
            died = true;

            agent.speed = 0;
            movementBlend.animator.Play("Death");

            StartCoroutine(Tks.SetTimeout(() => 
            LeanTween.move(gameObject, transform.position - Vector3.up * 1.25f, 1f)
            .setOnComplete(() => Destroy(gameObject))
            , 1000));


            if (!explodeOnDeath) return;

            float distance = (transform.position - plr.transform.position).magnitude;
            distance = (distance < 0.01f) ? 0 : distance;

            //damage based on distance
            plr.GetComponent<Health>().value -= Mathf.Clamp01(maxExplodeDamge * (1 - (distance - minExplodeRadius) / (maxExplodeRadius - minExplodeRadius)));
        }

        #region ANIMATION
        movementBlend.Update();
        #endregion
    }
}
//alex this breaks the build
//no it don't... I just made a typo, i dunno what u talkin abt fool?!
#region UNITY_EDITOR
[CustomEditor(typeof(Enemy))]
class EnemyEditor : Editor
{
    void OnSceneGUI()
    {
        Enemy enemy = (Enemy)target;

        Handles.color = Color.red;
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy.attackRange);

        Handles.color = Color.red / 2;
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy.maxExplodeRadius);
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy.minExplodeRadius);
    }
}
#endregion