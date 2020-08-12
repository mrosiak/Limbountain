using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine;
using Verlet;
using System;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] public float w = 0.01f;
    const string SHADER_PATH = "Hidden/Internal-Colored";

    protected Material lineMaterial;
    protected virtual void OnRenderObject()
    {
        CheckInit();

        lineMaterial.SetPass(0);
        lineMaterial.SetInt("_ZTest", (int)CompareFunction.Always);
    }

    protected void RenderConnection(List<Node> particles, Color color)
    {
        particles.ForEach(p => RenderConnection(p, color));
    }

    protected void RenderConnection(Node p, Color color)
    {
        p.Connection.ForEach(e =>
        {
            var other = e.Other(p);

            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(e.GetColour());

            DrawThickLine(p,other);

            GL.End();
            GL.PopMatrix();
        });
    }

    private void DrawThickLine(Node p, Node other)
    {
        GL.Vertex(p.position);
        GL.Vertex(other.position);

        GL.Vertex(new Vector3(p.position.x + w, p.position.y+w));
        GL.Vertex(new Vector3(other.position.x + w, other.position.y+w));

        GL.Vertex(new Vector3(p.position.x - w, p.position.y-w));
        GL.Vertex(new Vector3(other.position.x - w, other.position.y-w));
    }

    void CheckInit()
    {
        if (lineMaterial == null)
        {
            Shader shader = Shader.Find(SHADER_PATH);
            if (shader == null) return;
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}
