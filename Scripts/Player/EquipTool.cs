using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    //components
    private Animator anim;
    private Camera cam;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking) {
            attacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    
    public void OnCanAttack() 
    {
        attacking = false;
    }

    void OnHit() 
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance)) {
            if (doesGatherResources && hit.collider.GetComponent<Resource>()) {
                hit.collider.GetComponent<Resource>().Gather(hit.point, hit.normal);
            }
            if (doesDealDamage && hit.collider.GetComponent<IDamagable>() != null) {
                hit.collider.GetComponent<IDamagable>().TakeDamage(damage);
            }
        }
    }
}
