using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject waterBlob;
    [SerializeField] private GameObject goldenBlob;
    [SerializeField] private GameObject waterBlobParticles;
    [SerializeField] private GameObject goldenBlobParticles;

    [SerializeField] private float waterBlobChance = 35f;
    [SerializeField] private float goldenBlobChance = 5f;

    private float range = 1f;

    private bool enteredOnce;

    private void Start()
    {
        waterBlobParticles.GetComponent<ParticleSystem>().Stop();
        goldenBlobParticles.GetComponent<ParticleSystem>().Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !enteredOnce)
        {
            if (Random.value > (100 - goldenBlobChance) / 100f)
            {
                Vector3 pos = this.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0f);
                goldenBlobParticles.transform.position = pos;
                goldenBlobParticles.GetComponent<ParticleSystem>().Play();
                AudioManager.Instance.Play("GoldenBlobSpawn");
                Instantiate(goldenBlob, pos, Quaternion.identity);
            }


            else if (Random.value > (100 - waterBlobChance) / 100f)
            {
                Vector3 pos = this.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0f);
                waterBlobParticles.transform.position = pos;
                waterBlobParticles.GetComponent<ParticleSystem>().Play();
                AudioManager.Instance.Play("WaterBlobSpawn");
                Instantiate(waterBlob, pos, Quaternion.identity);
            }

            enteredOnce = true;
        }
    }
}
