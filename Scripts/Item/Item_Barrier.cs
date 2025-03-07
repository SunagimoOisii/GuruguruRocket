using UnityEngine;

public class Item_Barrier : MonoBehaviour, ICollide
{
    [SerializeField] private SoundParameter soundParam;
    private SoundSystem soundSystem;
    private string soundSystemTag = "SoundSystem";

    [Header("ロケットに付与するバリア")]
    [SerializeField] private GameObject barrierPrefab;

    private void Start()
    {
        soundSystem = GameObject.FindWithTag(soundSystemTag).GetComponent<SoundSystem>();
    }

    public void OnCollide(params object[] args)
    {
        foreach (var arg in args)
        {
            if (arg is Rocket rocket)
            {
                if(rocket.param.HasBarrier == false)
                {
                    rocket.EnableBarrier(barrierPrefab);
                }
                GetComponent<Collider2D>().enabled = false;
                _ = soundSystem.SE.Play(soundParam.AddressSEBarrierGet);
                Destroy(gameObject);
            }
        }
    }
}