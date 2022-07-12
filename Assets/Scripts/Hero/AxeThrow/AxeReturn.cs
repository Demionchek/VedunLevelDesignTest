using StarterAssets;
using System.Collections;
using UnityEngine;

public class AxeReturn : MonoBehaviour
{
    [SerializeField] private PlayerAbilitiesConfigs configs;
    [SerializeField] private float timeToRelocateAfterCollision;
    [SerializeField] private Transform playersHand;
    [SerializeField] private int enemyLayer;
    [SerializeField] private float timeToReturn;

    private Rigidbody rigidBody;
    public bool isActive { get; set; }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isActive)
            StartCoroutine(ReturnCoroutine());
    }

    private IEnumerator ReturnCoroutine()
    {
        yield return new WaitForSeconds(timeToReturn);
        StartCoroutine(ReturnAxe());
    }

    private IEnumerator ReturnAxe()
    {
        yield return new WaitForSeconds(timeToRelocateAfterCollision);
        gameObject.SetActive(false);
        gameObject.transform.parent = playersHand;
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        rigidBody.velocity = Vector3.zero;
        isActive = false;
    }    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                gameObject.transform.parent = collision.transform;
                damageable.TakeDamage((int)configs.axeThrowDmg, configs.enemyLayer);
            }
        }
        StartCoroutine(ReturnAxe());
    }
}
