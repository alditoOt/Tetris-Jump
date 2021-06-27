using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateParticle : MonoBehaviour
{
    public Grid grid;

    public List<ParticleSystem> lineParticle;

    private void Start()
    {
        grid.LinesDeleted.AddListener(CreateParticles);
        grid.LinesChecked.AddListener(ShakeCamera);
    }

    void CreateParticles(List<int> lines)
    {
        for(int i = 0; i < lines.Count; i ++)
        {
            lineParticle[i].transform.position = new Vector2(grid.transform.position.x, grid.transform.position.y + lines[i]);
            lineParticle[i].Play();
        }
    }
    void ShakeCamera(int lines)
    {
        CameraManager.Instance.Shake(lines/4f, 0.3f);
    }
}
