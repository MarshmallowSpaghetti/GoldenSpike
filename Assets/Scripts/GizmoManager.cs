using System.Collections.Generic;
using UnityEngine;

//Must be attached to a camera.
public class GizmoManager : MonoBehaviour
{
    public struct GizmoLine
    {
        public Vector3 a;
        public Vector3 b;
        public Color color;

        public GizmoLine(Vector3 a, Vector3 b, Color color)
        {
            this.a = a;
            this.b = b;
            this.color = color;
        }
    }

    public bool showGizmos = true;
    internal static List<GizmoLine> lines = new List<GizmoLine>();
    public Material lineMaterial;

    public static bool Show { get; private set; }

    private static GizmoManager m_instance;
    public static GizmoManager Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<GizmoManager>();
                if (m_instance)
                    m_instance = Camera.main.gameObject.AddComponent<GizmoManager>();
            }
            return m_instance;
        }
    }

    private void Update()
    {
        Show = showGizmos;
    }

    void OnPostRender()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);

        for (int i = 0; i < lines.Count; i++)
        {
            GL.Color(lines[i].color);
            GL.Vertex(lines[i].a);
            GL.Vertex(lines[i].b);
        }

        GL.End();
        lines.Clear();
    }
}