using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    public int damage;
    public float damageRate;
    private List<IDamagable> thigsToDamage = new List<IDamagable>();


void Start()
{
    StartCoroutine(DealDamage());
}
    IEnumerator DealDamage() 
    {
        while (true) {
            for (int i = 0; i < thigsToDamage.Count; i++) {
                thigsToDamage[i].TakeDamage(damage);
            }

            yield return new WaitForSeconds(damageRate);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null) {
            thigsToDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null) {
            thigsToDamage.Remove(collision.gameObject.GetComponent<IDamagable>());
        }
    }
}
