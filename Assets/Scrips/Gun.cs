using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [Header("Technical")]
    [SerializeField] private bool bloom = true;
    [SerializeField] private Vector3 bloomVariance = new Vector3(0f, 0f, 0f);
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private LayerMask mask;

    [Header("Effects")]
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;
    [SerializeField] private TrailRenderer bulletTrail;

    private Animator animator;
    private float lastShootTime;

    private void Awake()
    {
        animator = GameObject.Find("PistolArms").GetComponent<Animator>();
    }

    public void Shoot()
    {
        if (lastShootTime + fireRate < Time.time)
        {
            //object pooling much better
            animator.Play("PistolShoot");
            shootingSystem.Play();
            Vector3 direction = GetDirection();

            if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
            {
                TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, hit));
                Debug.Log(hit.collider.transform.name);
                lastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = -transform.right;

        if (bloom)
        {
            direction += new Vector3(
                Random.Range(-bloomVariance.x, bloomVariance.x),
                Random.Range(-bloomVariance.y, bloomVariance.y),
                Random.Range(-bloomVariance.z, bloomVariance.z)
                );
            direction.Normalize();

        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0f;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        animator.SetBool("isShooting", false);
        trail.transform.position = hit.point;
        Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

}
