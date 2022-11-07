using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ShooterPlayer : MonoBehaviourPun
{
    public float speed = 5f;
    public int health = 100;
    public int maxHealth = 100;
    public int damageValue = 10;
    public int restoreValue = 5;
    public ShooterBullet bulletPrefab;
    public TMP_Text playerName;
    public Animator animator;

    public Texture[] avatarTexture;
    private Renderer rend;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    private void Start()
    {
        maxHealth = health;
        playerName.text = photonView.Owner.NickName + $"({health})";
        rend = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody2D>();

        if (photonView.Owner.CustomProperties.TryGetValue(PropertyNames.Player.AvatarIndex, out var avatarIndex))
        {
            rend.material.mainTexture = avatarTexture[(int)avatarIndex];
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var healthValue))
        {
            health = (int)healthValue;
            maxHealth = (int)healthValue;
            playerName.text = photonView.Owner.NickName + $" ({health})";
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.DamageValue, out var damageValue))
        {
            this.damageValue = (int)damageValue;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.RestoreValue, out var restoreValue))
        {
            this.restoreValue = (int)restoreValue;
        }
    }

    private Vector3 mouseWorldPos, directionVector;

    private void Update()
    {
        if (!photonView.IsMine) return;

        moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        animator.SetBool("IsMove", (moveDir != Vector2.zero));

        if (Input.GetMouseButtonDown(0))
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
            directionVector = mouseWorldPos - transform.position;
            photonView.RPC("Shoot", RpcTarget.AllViaServer, transform.position, directionVector.normalized);
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 1);
        //}
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        rb.velocity = moveDir * speed;
    }

    [PunRPC]
    public void Shoot(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);

        if (photonView.IsMine) lag = 0;

        ShooterBullet bulletGO = Instantiate(bulletPrefab);
        bulletGO.Set(this, position, direction, lag);
    }

    [PunRPC]
    public void TakeDamage()
    {
        health -= damageValue;
        health = Mathf.Clamp(health, 0, maxHealth);
        playerName.text = photonView.Owner.NickName + $"({health})";
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.red, .2f).SetLoops(1, LoopType.Yoyo));
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }

    public void RestoreHealth()
    {
        if (photonView.IsMine)
            photonView.RPC("RestoreHealthRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RestoreHealthRPC()
    {
        health += restoreValue;
        health = Mathf.Clamp(health, 0, maxHealth);
        playerName.text = photonView.Owner.NickName + $"({health})";
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.green, .2f).SetLoops(1, LoopType.Yoyo));
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (photonView.IsMine)
            {
                photonView.RPC("TakeDamage", RpcTarget.AllViaServer);
            }
        }
    }
}
