using UnityEngine;

namespace Assets.Scripts
{
    public static class GripHelper
    {
        //const string SHADER_PATH = "Hidden/Internal-Colored";
        const string SHADER_PATH = "Standard";
        static Color[] dificulty = new Color[] { Color.white, Color.yellow, Color.green, Color.blue, Color.magenta, Color.red, Color.gray, Color.black };
        static public void SetColour(Transform model, GripStat stat)
        {
            var material = GetMaterial();
            int i = stat.dificulty;
            if (i >= dificulty.Length)
            {
                i = Random.Range(0, dificulty.Length);
            }
            material.color = dificulty[i];
            var renderer = model.GetComponent<Renderer>();
            renderer.material = material;
        }
        static Material GetMaterial()
        {
            Shader shader = Shader.Find(SHADER_PATH);
            Material lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            return lineMaterial;
        }
    }
}
