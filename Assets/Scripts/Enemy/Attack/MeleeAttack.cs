public class MeleeAttack : AttackDefault
{
    protected override void StartAttack()
    {
        //GetComponent<Animator>().Play("MeleeAttack");
        enabled = false;
    }
}
