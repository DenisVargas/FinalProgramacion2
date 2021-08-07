using UnityEngine;

public class BS_AnimEventListener : MonoBehaviour
{
    Boss _owner;
    [SerializeField] Collider hitBox;

    private void Awake()
    {
        _owner = GetComponentInParent<Boss>();
    }

    //Combate --> StartUp, Active, Recovery, EndOfAttackAnim
    void StartUp()
    {
        _owner.LookTowardsPlayer = true;
    }

    void Active()
    {
        hitBox.enabled = true;
        _owner.LookTowardsPlayer = false;
    }

    void Recovery()
    {
        hitBox.enabled = false;
        _owner.EvaluateTarget();
    }

    void EndOfAttackAnim()
    {
        _owner.ExecuteEvaluateAction();
        _owner.animator.SetBool("BasicAttack", false);
    }

    void RecoverFromStunn()
    {
        _owner.EvaluateTarget();
    }

    void EndRecoverAnimation()
    {
        _owner.ExecuteEvaluateAction();
    }
}
