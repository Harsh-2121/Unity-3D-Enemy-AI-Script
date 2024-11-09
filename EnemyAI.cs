//Some stupid enemy AI by GizmoWizard

using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] Transform playerTarget;
    [SerializeField] float activationRange = 10f;
    [SerializeField] float shootingRange = 5f;

    [Header("Movement Settings")]
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float rotationSpeed = 5f;

    [Header("Shooting Settings")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform gunPoint;
    [SerializeField] float bulletLifeTime = 3f;
    [SerializeField] float timeBetweenShots = 1.5f;
    bool canShoot = true;

    [Header("Enemy Attributes")]
    [SerializeField] int health = 3;

    void Update()
    {
        if (health <= 0) return;
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= activationRange)
        {
            EngagePlayer(distanceToPlayer);
        }
    }

    private void EngagePlayer(float distanceToPlayer)
    {
        LookAtPlayer();

        if (distanceToPlayer > shootingRange)
        {
            MoveTowardsPlayer();
        }
        else if (canShoot)
        {
            StartCoroutine(ShootPlayer());
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    IEnumerator ShootPlayer()
    {
        canShoot = false;
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = gunPoint.forward * 10f;
        }
        Destroy(bullet, bulletLifeTime);
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject); // Destroy the player on collision
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy enemy when health reaches zero
        }
    }
}