using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform mainCamera = null;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask interactiveLayers;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject bulletHolePrefab;
    private Animator gunAnimator = null;
    private Animator enemyAnimator = null;
    private Player player = null;
    private ParticleSystem muzzleFlash;

    private void Start()
    {
        player = GetComponent<Player>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && player.HasGun())
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        gunAnimator = player.gun.GetComponentInChildren<Animator>();
        muzzleFlash = player.gun.GetComponentInChildren<ParticleSystem>().GetComponentInChildren<ParticleSystem>();

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, player.gun.range, enemyLayers))
        {
            Enemy shotEnemy = hit.collider.GetComponent<Enemy>();
            if(shotEnemy != null)
            {
                enemyAnimator = shotEnemy.GetComponentInChildren<Animator>();
                shotEnemy.TakeGunDamage(player);

                if (shotEnemy.isDead)
                {
                    enemyAnimator.SetBool("isDead", true);
                }
            }
            

            Debug.Log("Shot the following enemy: " + hit.collider.name);
            
        }
        else if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, player.gun.range, wallLayers))
        {

            // TODO: Make last parameter of Instantiate random so bullet hole looks random every time
            Instantiate(bulletHolePrefab, hit.point + (0.01f * hit.normal), Quaternion.LookRotation(-1 * hit.normal,hit.transform.up));
            Debug.Log("Shot the following object: " + hit.collider.name);
        }
        else if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, player.gun.range, interactiveLayers))
        {
            // if is a pattern block (pattern game)
                // change interactive color
                // set it active in the array of interactives
            // if is moving platform
                // activate Coroutine to stop movement / fully stop
            Debug.Log("Shot the following interactive object: " + hit.collider.name);
        }

        gunAnimator.SetTrigger("Fire");

        player.gun.shotSoundEffect.Play();

        if(muzzleFlash != null)
        {
            Debug.Log("Bruh");
            muzzleFlash.Play();
        }
        
    }
}
