using Mirror;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // INTERACTABLE AREA CLASS
    #if _iMMO2D
    [RequireComponent(typeof(Collider2D))]
    #else
    [RequireComponent(typeof(Collider))]
    #endif
    public partial class Tools_InteractableArea : Tools_Interactable
    {
        public Color gizmoColor = new Color(0, 1, 1, 0.25f);
        public Color gizmoWireColor = new Color(1, 1, 1, 0.8f);
    
        // -----------------------------------------------------------------------------------
        // OnDrawGizmos
        // @Editor
        // -----------------------------------------------------------------------------------
        private void OnDrawGizmos()
        {
            // V�rifie si un collider compatible est pr�sent avant de tenter d'acc�der
            Gizmos.color = gizmoColor;
    
    #if _iMMO2D
            if (TryGetComponent(out BoxCollider2D box2D))
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(box2D.offset, box2D.size);
                Gizmos.color = gizmoWireColor;
                Gizmos.DrawWireCube(box2D.offset, box2D.size);
            }
            else if (TryGetComponent(out CircleCollider2D circle2D))
            {
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawSphere(transform.position + (Vector3)circle2D.offset, circle2D.radius);
                Gizmos.color = gizmoWireColor;
                Gizmos.DrawWireSphere(transform.position + (Vector3)circle2D.offset, circle2D.radius);
            }
    #else
            if (TryGetComponent(out BoxCollider box3D))
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(box3D.center, box3D.size);
                Gizmos.color = gizmoWireColor;
                Gizmos.DrawWireCube(box3D.center, box3D.size);
            }
            else if (TryGetComponent(out SphereCollider sphere))
            {
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
                Gizmos.color = gizmoWireColor;
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }
            else if (TryGetComponent(out CapsuleCollider capsule))
            {
                DrawFilledCapsule(capsule);
            }
            else if (TryGetComponent(out MeshCollider mesh))
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawMesh(mesh.sharedMesh);
            }
    #endif
    
            Gizmos.matrix = Matrix4x4.identity;
        }
    
    
        private void DrawWireCapsule(CapsuleCollider capsule)
        {
            Vector3 center = transform.position + capsule.center;
            float height = Mathf.Max(0, capsule.height - capsule.radius * 2);
            Vector3 up = transform.up * height / 2;
    
            Gizmos.DrawWireSphere(center + up, capsule.radius);
            Gizmos.DrawWireSphere(center - up, capsule.radius);
            Gizmos.DrawLine(center + up + transform.right * capsule.radius, center - up + transform.right * capsule.radius);
            Gizmos.DrawLine(center + up - transform.right * capsule.radius, center - up - transform.right * capsule.radius);
            Gizmos.DrawLine(center + up + transform.forward * capsule.radius, center - up + transform.forward * capsule.radius);
            Gizmos.DrawLine(center + up - transform.forward * capsule.radius, center - up - transform.forward * capsule.radius);
        }
    
        private void DrawFilledCapsule(CapsuleCollider capsule)
        {
            Vector3 center = transform.position + capsule.center;
            float height = Mathf.Max(0, capsule.height - 2 * capsule.radius); // Longueur du cylindre central
            Vector3 up = transform.up * height / 2; // Direction pour espacer les sph�res
            int segments = 20; // Nombre de segments pour simuler la courbure
    
            Gizmos.color = gizmoColor;
    
            // Dessiner l'h�misph�re sup�rieure
            for (int i = 0; i < segments / 2; i++) // Seulement la moiti� sup�rieure
            {
                float angleStart = Mathf.PI * i / segments;
                float angleEnd = Mathf.PI * (i + 1) / segments;
    
                for (int j = 0; j < segments; j++)
                {
                    float thetaStart = 2 * Mathf.PI * j / segments;
                    float thetaEnd = 2 * Mathf.PI * (j + 1) / segments;
    
                    // Points de l'h�misph�re sup�rieure
                    Vector3 p1 = center + up + new Vector3(
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Cos(thetaStart),
                        capsule.radius * Mathf.Cos(angleStart),
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Sin(thetaStart)
                    );
    
                    Vector3 p2 = center + up + new Vector3(
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Cos(thetaStart),
                        capsule.radius * Mathf.Cos(angleEnd),
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Sin(thetaStart)
                    );
    
                    Vector3 p3 = center + up + new Vector3(
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Cos(thetaEnd),
                        capsule.radius * Mathf.Cos(angleEnd),
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Sin(thetaEnd)
                    );
    
                    Vector3 p4 = center + up + new Vector3(
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Cos(thetaEnd),
                        capsule.radius * Mathf.Cos(angleStart),
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Sin(thetaEnd)
                    );
    
                    // Dessiner les triangles pour remplir la surface
                    Gizmos.DrawLine(p1, p2);
                    Gizmos.DrawLine(p2, p3);
                    Gizmos.DrawLine(p3, p4);
                    Gizmos.DrawLine(p4, p1);
                }
            }
    
            // Dessiner l'h�misph�re inf�rieure
            for (int i = 0; i < segments / 2; i++) // Seulement la moiti� inf�rieure
            {
                float angleStart = Mathf.PI * i / segments;
                float angleEnd = Mathf.PI * (i + 1) / segments;
    
                for (int j = 0; j < segments; j++)
                {
                    float thetaStart = 2 * Mathf.PI * j / segments;
                    float thetaEnd = 2 * Mathf.PI * (j + 1) / segments;
    
                    // Points de l'h�misph�re inf�rieure
                    Vector3 p1 = center - up + new Vector3(
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Cos(thetaStart),
                        -capsule.radius * Mathf.Cos(angleStart),
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Sin(thetaStart)
                    );
    
                    Vector3 p2 = center - up + new Vector3(
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Cos(thetaStart),
                        -capsule.radius * Mathf.Cos(angleEnd),
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Sin(thetaStart)
                    );
    
                    Vector3 p3 = center - up + new Vector3(
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Cos(thetaEnd),
                        -capsule.radius * Mathf.Cos(angleEnd),
                        capsule.radius * Mathf.Sin(angleEnd) * Mathf.Sin(thetaEnd)
                    );
    
                    Vector3 p4 = center - up + new Vector3(
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Cos(thetaEnd),
                        -capsule.radius * Mathf.Cos(angleStart),
                        capsule.radius * Mathf.Sin(angleStart) * Mathf.Sin(thetaEnd)
                    );
    
                    // Dessiner les triangles pour remplir la surface
                    Gizmos.DrawLine(p1, p2);
                    Gizmos.DrawLine(p2, p3);
                    Gizmos.DrawLine(p3, p4);
                    Gizmos.DrawLine(p4, p1);
                }
            }
    
            // Dessiner le cylindre central
            for (int j = 0; j < segments; j++)
            {
                float thetaStart = 2 * Mathf.PI * j / segments;
                float thetaEnd = 2 * Mathf.PI * (j + 1) / segments;
    
                Vector3 topStart = center + up + new Vector3(
                    capsule.radius * Mathf.Cos(thetaStart),
                    0,
                    capsule.radius * Mathf.Sin(thetaStart)
                );
    
                Vector3 topEnd = center + up + new Vector3(
                    capsule.radius * Mathf.Cos(thetaEnd),
                    0,
                    capsule.radius * Mathf.Sin(thetaEnd)
                );
    
                Vector3 bottomStart = center - up + new Vector3(
                    capsule.radius * Mathf.Cos(thetaStart),
                    0,
                    capsule.radius * Mathf.Sin(thetaStart)
                );
    
                Vector3 bottomEnd = center - up + new Vector3(
                    capsule.radius * Mathf.Cos(thetaEnd),
                    0,
                    capsule.radius * Mathf.Sin(thetaEnd)
                );
    
                // Dessiner les faces du cylindre
                Gizmos.DrawLine(topStart, bottomStart);
                Gizmos.DrawLine(bottomStart, bottomEnd);
                Gizmos.DrawLine(bottomEnd, topEnd);
                Gizmos.DrawLine(topEnd, topStart);
            }
        }
    
    
    
    
        // -----------------------------------------------------------------------------------
        // OnTriggerEnter
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
    #if _iMMO2D
        private void OnTriggerEnter2D(Collider2D _collider)
    #else
        private void OnTriggerEnter(Collider _collider)
    #endif
        {
            Player player = _collider.GetComponentInParent<Player>();
            if (player != null && player == Player.localPlayer)
            {
                if ((!interactionRequirements.hasRequirements() && !interactionRequirements.requierementCost.HasCosts()) || automaticActivation)
                {
                    ConfirmAccess();
                }
                else
                {
                    ShowAccessRequirementsUI();
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // OnTriggerExit
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
    #if _iMMO2D
        private void OnTriggerExit2D(Collider2D _collider)
    #else
        private void OnTriggerExit(Collider _collider)
    #endif
        {
            Player player = _collider.GetComponentInParent<Player>();
            if (player != null && player == Player.localPlayer)
            {
                HideAccessRequirementsUI();
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Update
        // @Client
        // -----------------------------------------------------------------------------------
        /*[ClientCallback]
        private void Update()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            // -- check for interaction Distance
            if (IsWorthUpdating())
                this.GetComponentInChildren<SpriteRenderer>().enabled = UMMO_Tools.Tools_CheckSelectionHandling(this.gameObject);
        }*/
    }
    
}
    #endif
