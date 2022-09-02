using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class FloorDeco : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] bool oncePlayParticle;
    [SerializeField] TextMeshProUGUI nameText;

    private bool canPlayParticle;
    public int height;

    private void OnEnable()
    {
        canPlayParticle = true;
    }

    private void OnDisable()
    {
        canPlayParticle = false;
    }

    public void SetHeight(int height)
    {
        this.height = height;

        //if (!oncePlayParticle)
        HeightCheck();
    }

    public void SetText(string name)
    {
        if (nameText == null)
            return;

        nameText.text = name;
    }

    void HeightCheck()
    {
        InGameManager.Instance.Height
            .Where(y => y == height)
            .Subscribe(y => PlayParticles())
            .AddTo(this.gameObject);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!canPlayParticle)
    //        return;

    //    // height °è»ê?
    //    if (collision.CompareTag(Values.Key_Player))
    //    {
    //        PlayParticles();
    //    }
    //}

    void PlayParticles()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }

        if (oncePlayParticle)
            canPlayParticle = false;
    }
}
