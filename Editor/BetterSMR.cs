#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace CustomUnityTools
{
    [CustomEditor(typeof(SkinnedMeshRenderer))]
    [CanEditMultipleObjects]
    public class BetterSMR : Editor
    {
        private readonly Dictionary<string, string[]> m_CustomCategories = new Dictionary<string, string[]>
        {
            { "VRChat", new[] { "vrc.v_aa", "vrc.v_aa.ft", "vrc.v_ch", "vrc.v_dd", "vrc.v_e", "vrc.v_e.ft", "vrc.v_ff", "vrc.v_ih", "vrc.v_kk", "vrc.v_nn", "vrc.v_oh", "vrc.v_oh.ft", "vrc.v_ou", "vrc.v_ou.ft", "vrc.v_pp", "vrc.v_rr", "vrc.v_sil", "vrc.v_ss", "vrc.v_th",
                                "vrc.blink", "vrc.looking_up", "vrc.looking_down" } },

            { "MMD", new[] { "まばたき", "笑い", "ウィンク", "ウィンク右", "ウィンク２", "ｳｨﾝｸ２右", "なごみ", "はぅ", "びっくり", "じと目", "ｷﾘｯ", "はちゅ目", "星目", "はぁと", "瞳小", "瞳縦潰れ", "光下", "恐ろしい子！", "ハイライト消", "映り込み消", "喜び", "わぉ?!", "なごみω", "悲しむ", "敵意",
                             "あ", "い", "う", "え", "お", "あ２", "ん", "▲", "∧", "□", "ワ", "ω", "ω□", "にやり", "にやり２", "にっこり", "ぺろっ", "てへぺろ", "てへぺろ２", "口角上げ", "口角下げ", "口横広げ", "歯無し上", "歯無し下",
                             "真面目", "困る", "にこり", "怒り", "上", "下" } },

            { "Unified Expressions", new[] { "EyeLookOutRight", "EyeLookInRight", "EyeLookUpRight", "EyeLookDownRight", "EyeLookOutLeft", "EyeLookInLeft", "EyeLookUpLeft", "EyeLookDownLeft",
                                             "EyeClosedRight", "EyeClosedLeft", "EyeSquintRight", "EyeSquintLeft", "EyeWideRight", "EyeWideLeft", "EyeDilationRight", "EyeDilationLeft", "EyeConstrictRight", "EyeConstrictLeft",
                                             "BrowPinchRight", "BrowPinchLeft", "BrowLowererRight", "BrowLowererLeft", "BrowInnerUpRight", "BrowInnerUpLeft", "BrowOuterUpRight", "BrowOuterUpLeft",
                                             "NoseSneerRight", "NoseSneerLeft", "NasalDilationRight", "NasalDilationLeft", "NasalConstrictRight", "NasalConstrictLeft",
                                             "CheekSquintRight", "CheekSquintLeft", "CheekPuffRight", "CheekPuffLeft", "CheekSuckRight", "CheekSuckLeft",
                                             "JawOpen", "MouthClosed", "JawRight", "JawLeft", "JawForward", "JawBackward", "JawClench", "JawMandibleRaise",
                                             "LipSuckUpperRight", "LipSuckUpperLeft", "LipSuckLowerRight", "LipSuckLowerLeft", "LipSuckCornerRight", "LipSuckCornerLeft", "LipFunnelUpperRight", "LipFunnelUpperLeft", "LipFunnelLowerRight", "LipFunnelLowerLeft",
                                             "LipPuckerUpperRight", "LipPuckerUpperLeft", "LipPuckerLowerRight", "LipPuckerLowerLeft",
                                             "MouthUpperUpRight", "MouthUpperUpLeft", "MouthLowerDownRight", "MouthLowerDownLeft", "MouthUpperDeepenRight", "MouthUpperDeepenLeft", "MouthUpperRight", "MouthUpperLeft", "MouthLowerRight", "MouthLowerLeft",
                                             "MouthCornerPullRight", "MouthCornerPullLeft", "MouthCornerSlantRight", "MouthCornerSlantLeft", "MouthFrownRight", "MouthFrownLeft", "MouthStretchRight", "MouthStretchLeft",
                                             "MouthDimpleRight", "MouthDimpleLeft", "MouthRaiserUpper", "MouthRaiserLower", "MouthPressRight", "MouthPressLeft", "MouthTightenerRight", "MouthTightenerLeft",
                                             "TongueOut", "TongueUp", "TongueDown", "TongueRight", "TongueLeft", "TongueRoll", "TongueBendDown", "TongueCurlUp", "TongueSquish", "TongueFlat", "TongueTwistRight", "TongueTwistLeft",
                                             "SoftPalateClose", "ThroatSwallow", "NeckFlexRight", "NeckFlexLeft",

                                             "EyeClosed", "EyeWide", "EyeSquint", "EyeDilation", "EyeConstrict", "BrowDownRight", "BrowDownLeft", "BrowDown", "BrowInnerUp", "BrowUpRight", "BrowUpLeft", "BrowUp", "NoseSneer", "NasalDilation", "NasalConstrict",
                                             "CheekPuff", "CheekSuck", "CheekSquint", "LipSuckUpper", "LipSuckLower", "LipSuck", "LipFunnelUpper", "LipFunnelLower", "LipFunnel", "LipPuckerUpper", "LipPuckerLower", "LipPucker",
                                             "MouthUpperUp", "MouthLowerDown", "MouthOpen", "MouthRight", "MouthLeft", "MouthSmileRight", "MouthSmileLeft", "MouthSmile", "MouthSadRight", "MouthSadLeft", "MouthSad", "MouthStretch", "MouthDimple", "MouthTightener", "MouthPress" } },

            { "ARKit", new[] { "browDownLeft", "browDownRight", "browInnerUp", "browOuterUpLeft", "browOuterUpRight",
                               "eyeBlinkLeft", "eyeLookDownLeft", "eyeLookInLeft", "eyeLookOutLeft", "eyeLookUpLeft", "eyeSquintLeft", "eyeWideLeft",
                               "eyeBlinkRight", "eyeLookDownRight", "eyeLookInRight", "eyeLookOutRight", "eyeLookUpRight", "eyeSquintRight", "eyeWideRight",
                               "jawForward", "jawLeft", "jawRight", "jawOpen",
                               "mouthClose", "mouthFunnel", "mouthPucker", "mouthLeft", "mouthRight", "mouthSmileLeft", "mouthSmileRight", "mouthFrownLeft", "mouthFrownRight", "mouthDimpleLeft", "mouthDimpleRight", "mouthStretchLeft", "mouthStretchRight",
                               "mouthRollLower", "mouthRollUpper", "mouthShrugLower", "mouthShrugUpper", "mouthPressLeft", "mouthPressRight", "mouthLowerDownRight", "mouthLowerDownLeft", "mouthUpperUpLeft", "mouthUpperUpRight",
                               "cheekPuff", "cheekSquintLeft", "cheekSquintRight", "noseSneerLeft", "noseSneerRight"} },
        };

        private static readonly List<string> DefaultSeparators = new List<string>
        {
            "--", "//", "==", "**", "- -", "/ /", "——", "— —", "__", "_ _",
            "---", "///", "===", "***", "———", "___"
        };

        private Editor m_DefaultEditor;
        private MethodInfo m_LightingSettingsGUI;
        private MethodInfo m_RayTracingSettingsGUI;
        private MethodInfo m_OtherSettingsGUI;

        private SerializedProperty m_AABB;
        private SerializedProperty m_DirtyAABB;
        private SerializedProperty m_BlendShapeWeights;
        private SerializedProperty m_Quality;
        private SerializedProperty m_UpdateWhenOffscreen;
        private SerializedProperty m_Mesh;
        private SerializedProperty m_RootBone;

        private string m_NewGroupName = "";
        private string m_SearchText = "";
        private Dictionary<string, int> m_VertexCountCache = new Dictionary<string, int>();

        private Material m_HighlightMaterial;

        [SerializeField]
        private BlendShapeLayoutData m_LayoutData;

        private enum DragActionType { None, Shape, Group, Slider }
        private DragActionType m_CurrentDragAction = DragActionType.None;

        private List<string> m_SelectedShapes = new List<string>();
        private string m_LastClickedShape = null;
        private string m_DragCandidateShape = null;
        private bool m_DidDragOccur = false;
        private BlendShapeGroup m_DragCandidateGroup = null;
        private List<string> m_CurrentDisplayOrder = new List<string>();

        [Serializable]
        private class BlendShapeLayoutData
        {
            public bool isMainExpanded = true;
            public bool isSeparatorsExpanded = false;
            public bool hasInitializedSeparators = false;
            public int sortMode = 0;
            public List<string> separators = new List<string>();
            public List<BlendShapeGroup> groups = new List<BlendShapeGroup>();

            public bool useBuiltInCategorization = true;

            public List<int> activeHighlights = new List<int>();
            public Color highlightColor = new Color(0f, 1f, 1f, 0.4f);
            public bool showWireframe = true;
        }

        [Serializable]
        private class BlendShapeGroup
        {
            public string groupName;
            public bool isExpanded = true;

            [NonSerialized]
            public bool isRenaming = false;

            public List<string> shapes = new List<string>();

            [NonSerialized]
            public int organizeMode = 0; // 0 = Original, 1 = A-Z, 2 = Z-A
        }

        [Serializable]
        private class SeparatorClipboardWrapper
        {
            public List<string> list;
        }

        private BlendShapeLayoutData LoadLayoutData(Object targetObj)
        {
            string key = "SMR_BlendShapes_" + GlobalObjectId.GetGlobalObjectIdSlow(targetObj).ToString();
            string savedLayout = EditorPrefs.GetString(key, "");
            return !string.IsNullOrEmpty(savedLayout) ? JsonUtility.FromJson<BlendShapeLayoutData>(savedLayout) : new BlendShapeLayoutData();
        }

        private void SaveLayoutData(Object targetObj, BlendShapeLayoutData data)
        {
            string key = "SMR_BlendShapes_" + GlobalObjectId.GetGlobalObjectIdSlow(targetObj).ToString();
            EditorPrefs.SetString(key, JsonUtility.ToJson(data));
        }

        private void SaveLayoutData()
        {
            SaveLayoutData(target, m_LayoutData);
            EditorUtility.SetDirty(this);
        }

        public void OnEnable()
        {
            Type defaultEditorType = Type.GetType("UnityEditor.SkinnedMeshRendererEditor, UnityEditor");
            m_DefaultEditor = Editor.CreateEditor(targets, defaultEditorType);

            Type rendererBaseType = defaultEditorType.BaseType;
            m_LightingSettingsGUI = rendererBaseType.GetMethod("LightingSettingsGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            m_RayTracingSettingsGUI = rendererBaseType.GetMethod("RayTracingSettingsGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            m_OtherSettingsGUI = rendererBaseType.GetMethod("OtherSettingsGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            m_AABB = serializedObject.FindProperty("m_AABB");
            m_DirtyAABB = serializedObject.FindProperty("m_DirtyAABB");
            m_BlendShapeWeights = serializedObject.FindProperty("m_BlendShapeWeights");
            m_Quality = serializedObject.FindProperty("m_Quality");
            m_UpdateWhenOffscreen = serializedObject.FindProperty("m_UpdateWhenOffscreen");
            m_Mesh = serializedObject.FindProperty("m_Mesh");
            m_RootBone = serializedObject.FindProperty("m_RootBone");

            m_LayoutData = LoadLayoutData(target);

            if (!EditorPrefs.HasKey("SMR_BlendShapes_" + GlobalObjectId.GetGlobalObjectIdSlow(target).ToString()))
            {
                m_LayoutData.useBuiltInCategorization = true;
            }

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            if (m_DefaultEditor != null) DestroyImmediate(m_DefaultEditor);

            if (m_HighlightMaterial != null) DestroyImmediate(m_HighlightMaterial);
        }

        private void OnUndoRedo()
        {
            SaveLayoutData();
            Repaint();
        }

        private void SanitizeLayoutData(Mesh m)
        {
            if (m == null || m_LayoutData == null)
            {
                return;
            }

            bool changed = false;

            HashSet<string> validNames = new HashSet<string>();

            for (int i = 0; i < m.blendShapeCount; i++)
            {
                validNames.Add(m.GetBlendShapeName(i));
            }

            foreach (var group in m_LayoutData.groups)
            {
                int removedCount = group.shapes.RemoveAll(s => !validNames.Contains(s));

                if (removedCount > 0)
                {
                    changed = true;
                }
            }

            if (changed)
            {
                SaveLayoutData();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_DefaultEditor.serializedObject.Update();

            SkinnedMeshRenderer renderer = target as SkinnedMeshRenderer;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AABB, new GUIContent("Bounds"));
            if (EditorGUI.EndChangeCheck())
            {
                m_DirtyAABB.boolValue = false;
            }

            EditorGUILayout.PropertyField(m_Mesh, new GUIContent("Mesh"));
            DrawCustomMaterialList();
            OnBlendShapeUI(renderer);

            EditorGUILayout.PropertyField(m_Quality, new GUIContent("Quality"));
            EditorGUILayout.PropertyField(m_UpdateWhenOffscreen, new GUIContent("Update When Offscreen"));
            EditorGUILayout.PropertyField(m_RootBone, new GUIContent("Root Bone"));

            m_LightingSettingsGUI?.Invoke(m_DefaultEditor, new object[] { false });
            m_RayTracingSettingsGUI?.Invoke(m_DefaultEditor, null);
            m_OtherSettingsGUI?.Invoke(m_DefaultEditor, new object[] { false, true, false });

            m_DefaultEditor.serializedObject.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();

            if (Event.current.rawType == EventType.MouseUp)
            {
                if (m_CurrentDragAction == DragActionType.Shape && m_DragCandidateShape != null && !m_DidDragOccur && !Event.current.control && !Event.current.shift && !Event.current.command)
                {
                    m_SelectedShapes.Clear();
                    m_SelectedShapes.Add(m_DragCandidateShape);
                    Repaint();
                }

                m_CurrentDragAction = DragActionType.None;
                m_DragCandidateShape = null;
                m_DragCandidateGroup = null;
                m_DidDragOccur = false;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                m_SelectedShapes.Clear();
                GUI.FocusControl(null);
                Repaint();
            }
        }

        #region Custom Material Drawer & Scene Wireframe

        private void DrawCustomMaterialList()
        {
            SerializedProperty materialsProp = serializedObject.FindProperty("m_Materials");

            materialsProp.isExpanded = EditorGUILayout.Foldout(materialsProp.isExpanded, "Materials", true);

            if (materialsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                int newSize = EditorGUILayout.DelayedIntField("Size", materialsProp.arraySize);

                if (EditorGUI.EndChangeCheck())
                {
                    materialsProp.arraySize = Mathf.Max(0, newSize);
                }

                for (int i = 0; i < materialsProp.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty matProp = materialsProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(matProp, new GUIContent($"Element {i}"));

                    bool isHighlighted = m_LayoutData.activeHighlights.Contains(i);
                    Color defaultColor = GUI.backgroundColor;

                    if (isHighlighted) GUI.backgroundColor = new Color(0.2f, 0.8f, 1f, 1f);

                    if (GUILayout.Button(isHighlighted ? "Hide" : "Show", GUILayout.Width(50)))
                    {
                        foreach (var t in targets)
                        {
                            var d = (t == target) ? m_LayoutData : LoadLayoutData(t);

                            if (isHighlighted)
                            {
                                d.activeHighlights.Remove(i);
                            }

                            else if (!d.activeHighlights.Contains(i))
                            {
                                d.activeHighlights.Add(i);
                            }

                            if (t != target)
                            {
                                SaveLayoutData(t, d);
                            }
                        }

                        SaveLayoutData();
                        SceneView.RepaintAll();
                    }

                    GUI.backgroundColor = defaultColor;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            bool anyHighlights = false;

            foreach (var t in targets)
            {
                var d = (t == target) ? m_LayoutData : LoadLayoutData(t);

                if (d.activeHighlights.Count > 0)
                {
                    anyHighlights = true;
                    break;
                }
            }

            if (anyHighlights)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField("Highlight Settings", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("⚠️ Ensure Gizmos are enabled in the Scene view to see highlights.", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.Space(2);

                EditorGUI.BeginChangeCheck();

                Color newColor = EditorGUILayout.ColorField("Color", m_LayoutData.highlightColor);

                EditorGUILayout.BeginHorizontal();
                bool newWire = EditorGUILayout.Toggle("Wireframe Mode", m_LayoutData.showWireframe);

                if (GUILayout.Button("Reset", GUILayout.Width(60)))
                {
                    newColor = new Color(0f, 1f, 1f, 0.4f);
                    newWire = true;
                    GUI.FocusControl(null);
                }

                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this, "Change Highlight Settings");

                    foreach (var t in targets)
                    {
                        BlendShapeLayoutData d = (t == target) ? m_LayoutData : LoadLayoutData(t);

                        d.highlightColor = newColor;
                        d.showWireframe = newWire;

                        EditorUtility.SetDirty(t);

                        if (t != target)
                        {
                            SaveLayoutData(t, d);
                        }
                    }

                    m_LayoutData.highlightColor = newColor;
                    m_LayoutData.showWireframe = newWire;

                    SaveLayoutData();
                    SceneView.RepaintAll();
                }

                EditorGUILayout.Space(2);

                if (GUILayout.Button("Clear All Highlights"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Clear All Highlights");

                    foreach (var t in targets)
                    {
                        BlendShapeLayoutData d = (t == target) ? m_LayoutData : LoadLayoutData(t);
                        d.activeHighlights.Clear();

                        EditorUtility.SetDirty(t);

                        if (t != target)
                        {
                            SaveLayoutData(t, d);
                        }
                    }

                    m_LayoutData.activeHighlights.Clear();

                    SaveLayoutData();
                    SceneView.RepaintAll();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
        }

        public void OnSceneGUI()
        {
            if (m_DefaultEditor != null)
            {
                MethodInfo onSceneGUI = m_DefaultEditor.GetType().GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (onSceneGUI != null)
                {
                    onSceneGUI.Invoke(m_DefaultEditor, null);
                }
            }

            if (m_HighlightMaterial == null)
            {
                m_HighlightMaterial = new Material(Shader.Find("Sprites/Default"));
                m_HighlightMaterial.hideFlags = HideFlags.HideAndDontSave;
                m_HighlightMaterial.SetInt("_ZTest", (int)CompareFunction.Always);
            }

            // In an Editor script with [CanEditMultipleObjects], Unity automatically calls 
            // OnSceneGUI individually for EVERY selected object. 'target' is natively assigned.
            SkinnedMeshRenderer renderer = target as SkinnedMeshRenderer;
            if (renderer == null || renderer.sharedMesh == null) return;

            var data = LoadLayoutData(renderer);

            if (data.activeHighlights.Count > 0)
            {
                m_HighlightMaterial.color = data.highlightColor;

                bool previousWireframe = GL.wireframe;

                try
                {
                    if (data.showWireframe)
                    {
                        GL.wireframe = true;
                    }

                    // Execute CommandBuffer natively on GPU
                    CommandBuffer cb = new CommandBuffer();

                    foreach (int submeshIndex in data.activeHighlights)
                    {
                        if (submeshIndex >= 0 && submeshIndex < renderer.sharedMesh.subMeshCount)
                        {
                            cb.DrawRenderer(renderer, m_HighlightMaterial, submeshIndex, 0);
                        }
                    }

                    Graphics.ExecuteCommandBuffer(cb);
                    cb.Dispose();
                }
                finally
                {
                    GL.wireframe = previousWireframe;
                }
            }
        }

        #endregion Custom Material Drawer & Scene Wireframe

        private int GetAffectedVertexCount(Mesh mesh, int shapeIndex, string shapeName)
        {
            if (m_VertexCountCache.TryGetValue(shapeName, out int count))
            {
                return count;
            }

            if (!mesh.isReadable)
            {
                m_VertexCountCache[shapeName] = -1;
                return -1;
            }

            int frameCount = mesh.GetBlendShapeFrameCount(shapeIndex);

            if (frameCount == 0)
            {
                m_VertexCountCache[shapeName] = 0;
                return 0;
            }

            bool[] affected = new bool[mesh.vertexCount];
            Vector3[] deltas = new Vector3[mesh.vertexCount];
            Vector3[] normals = new Vector3[mesh.vertexCount];
            Vector3[] tangents = new Vector3[mesh.vertexCount];

            count = 0;
            float tinyThreshold = 1e-10f;

            for (int f = 0; f < frameCount; f++)
            {
                mesh.GetBlendShapeFrameVertices(shapeIndex, f, deltas, normals, tangents);
                for (int i = 0; i < deltas.Length; i++)
                {
                    if (!affected[i])
                    {
                        if (deltas[i].sqrMagnitude > tinyThreshold ||
                            normals[i].sqrMagnitude > tinyThreshold ||
                            tangents[i].sqrMagnitude > tinyThreshold)
                        {
                            affected[i] = true;
                            count++;
                        }
                    }
                }
            }

            m_VertexCountCache[shapeName] = count;

            return count;
        }

        private void DrawGripHandle(Rect rect)
        {
            if (Event.current.type != EventType.Repaint) return;
            Color color = GUI.color;
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            float yCenter = rect.y + rect.height / 2f;
            GUI.DrawTexture(new Rect(rect.x + 2, yCenter - 2, 10, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x + 2, yCenter + 2, 10, 2), Texture2D.whiteTexture);
            GUI.color = color;
        }

        private BlendShapeGroup GetOrCreateGroup(string name)
        {
            BlendShapeGroup group = m_LayoutData.groups.FirstOrDefault(x => x.groupName == name);
            if (group == null)
            {
                group = new BlendShapeGroup { groupName = name };
                m_LayoutData.groups.Add(group);
            }

            return group;
        }

        private void RemoveShapeFromAllGroups(string shapeName)
        {
            foreach (BlendShapeGroup group in m_LayoutData.groups)
            {
                group.shapes.Remove(shapeName);
            }
        }

        private void AutoCategorizeBlendShapes(Mesh mesh)
        {
            Undo.RecordObject(this, "Auto-Categorize BlendShapes");

            BlendShapeGroup currentBoundaryGroup = null;
            int fallbackCounter = 1;

            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string sName = mesh.GetBlendShapeName(i);
                int vCount = GetAffectedVertexCount(mesh, i, sName);

                bool isBoundary = false;

                foreach (string sep in m_LayoutData.separators)
                {
                    if (string.IsNullOrEmpty(sep))
                    {
                        continue;
                    }

                    if (sName.Contains(sep))
                    {
                        if (sep.Length >= 3 || vCount <= 0)
                        {
                            isBoundary = true;
                            break;
                        }
                    }
                }

                if (isBoundary)
                {
                    bool isFooter = !sName.Any(char.IsLetterOrDigit);

                    if (currentBoundaryGroup != null)
                    {
                        if (isFooter)
                        {
                            RemoveShapeFromAllGroups(sName);
                            currentBoundaryGroup.shapes.Add(sName);
                            currentBoundaryGroup = null;
                            continue;
                        }
                    }

                    string groupName = isFooter ? "Group " + fallbackCounter++ : sName;

                    currentBoundaryGroup = GetOrCreateGroup(groupName);
                    RemoveShapeFromAllGroups(sName);
                    currentBoundaryGroup.shapes.Add(sName);
                }
                else
                {
                    if (currentBoundaryGroup != null)
                    {
                        RemoveShapeFromAllGroups(sName);
                        currentBoundaryGroup.shapes.Add(sName);
                    }
                }
            }

            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string sName = mesh.GetBlendShapeName(i);

                if (m_LayoutData.groups.Any(g => g.shapes.Contains(sName)))
                {
                    continue;
                }

                if (m_LayoutData.useBuiltInCategorization)
                {
                    foreach (var category in m_CustomCategories)
                    {
                        bool match = category.Value.Any(keyword => sName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (match)
                        {
                            BlendShapeGroup group = GetOrCreateGroup(category.Key);
                            RemoveShapeFromAllGroups(sName);
                            group.shapes.Add(sName);
                            break;
                        }
                    }
                }
            }

            SaveLayoutData();
        }

        public void OnBlendShapeUI(SkinnedMeshRenderer renderer)
        {
            int blendShapeCount = renderer.sharedMesh == null ? 0 : renderer.sharedMesh.blendShapeCount;

            if (blendShapeCount == 0)
            {
                return;
            }

            if (m_BlendShapeWeights.arraySize != blendShapeCount)
            {
                m_BlendShapeWeights.arraySize = blendShapeCount;
                serializedObject.ApplyModifiedProperties();
            }

            if (!m_LayoutData.hasInitializedSeparators)
            {
                m_LayoutData.separators = new List<string>(DefaultSeparators);
                m_LayoutData.hasInitializedSeparators = true;

                SaveLayoutData();
            }

            Mesh m = renderer.sharedMesh;
            SanitizeLayoutData(m);

            bool prevExpanded = m_LayoutData.isMainExpanded;
            m_BlendShapeWeights.isExpanded = prevExpanded;
            EditorGUILayout.PropertyField(m_BlendShapeWeights, new GUIContent("BlendShapes"), false);

            if (m_BlendShapeWeights.isExpanded != prevExpanded)
            {
                m_LayoutData.isMainExpanded = m_BlendShapeWeights.isExpanded;
                SaveLayoutData();
            }

            if (!m_LayoutData.isMainExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            using (new TESUIHelpersBetterSMR.SqueezeScope((0, 0, TESUIHelpersBetterSMR.SqueezeScope.SqueezeScopeType.Horizontal)))
            {
                using (new TESUIHelpersBetterSMR.SetLabelWidth(140))
                {
                    m_SearchText = EditorGUILayout.TextField("Search Blendshapes", m_SearchText);
                }

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    m_SearchText = "";
                    GUI.FocusControl(null);
                }
            }

            EditorGUILayout.Space(12);

            if (m_LayoutData.groups.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Expand", GUILayout.Width(80)))
                {
                    foreach (var g in m_LayoutData.groups) g.isExpanded = true;
                    SaveLayoutData();
                }

                if (GUILayout.Button("Collapse", GUILayout.Width(80)))
                {
                    foreach (var g in m_LayoutData.groups) g.isExpanded = false;
                    SaveLayoutData();
                }

                if (GUILayout.Button("Reset", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Reset BlendShape Groups", "Are you sure you want to delete all groups and return all blendshapes to the Ungrouped section?", "Yes, Reset", "Cancel"))
                    {
                        Undo.RecordObject(this, "Reset Groups");
                        m_LayoutData.groups.Clear();
                        SaveLayoutData();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            m_NewGroupName = EditorGUILayout.TextField(m_NewGroupName);

            if (GUILayout.Button("Create Group", GUILayout.Width(120)) && !string.IsNullOrWhiteSpace(m_NewGroupName))
            {
                Undo.RecordObject(this, "Create Group");

                if (!m_LayoutData.groups.Any(g => g.groupName == m_NewGroupName))
                {
                    m_LayoutData.groups.Add(new BlendShapeGroup { groupName = m_NewGroupName });
                    SaveLayoutData();
                }

                m_NewGroupName = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(12);

            string searchLower = m_SearchText.ToLower();
            bool isSearching = !string.IsNullOrEmpty(searchLower);

            m_CurrentDisplayOrder.Clear();
            List<int> sortedIndices = new List<int>();
            for (int i = 0; i < blendShapeCount; i++) sortedIndices.Add(i);

            if (m_LayoutData.sortMode == 1) sortedIndices.Sort((a, b) => string.Compare(m.GetBlendShapeName(a), m.GetBlendShapeName(b)));
            else if (m_LayoutData.sortMode == 2) sortedIndices.Sort((a, b) => string.Compare(m.GetBlendShapeName(b), m.GetBlendShapeName(a)));

            List<int> ungroupedIndices = new List<int>(sortedIndices);

            foreach (var group in m_LayoutData.groups)
            {
                foreach (var s in group.shapes)
                {
                    int shapeIdx = m.GetBlendShapeIndex(s);
                    if (shapeIdx != -1) ungroupedIndices.Remove(shapeIdx);

                    if (group.isExpanded) m_CurrentDisplayOrder.Add(s);
                }
            }

            foreach (int index in ungroupedIndices)
            {
                string shapeName = m.GetBlendShapeName(index);

                if (!m_LayoutData.groups.Any(g => g.shapes.Contains(shapeName)) &&
                    (!isSearching || shapeName.ToLower().Contains(searchLower)))
                {
                    m_CurrentDisplayOrder.Add(shapeName);
                }
            }

            foreach (var group in m_LayoutData.groups.ToList())
            {
                Rect groupRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                Rect headerRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

                bool groupIsAnimated = false;
                bool groupHasMatch = isSearching && group.shapes.Any(s => s.ToLower().Contains(searchLower));

                if (AnimationMode.InAnimationMode())
                {
                    foreach (var shapeName in group.shapes)
                    {
                        int shapeIdx = m.GetBlendShapeIndex(shapeName);

                        if (shapeIdx == -1)
                        {
                            continue;
                        }

                        // Check both the Array path (Inspector) and the blendShape path (Animation Window)
                        string arrayPath = $"m_BlendShapeWeights.Array.data[{shapeIdx}]";
                        string animPath = "blendShape." + shapeName;

                        if (AnimationMode.IsPropertyAnimated(renderer, arrayPath) || AnimationMode.IsPropertyAnimated(renderer, animPath))
                        {
                            groupIsAnimated = true;
                        }
                    }
                }

                if (groupIsAnimated)
                {
                    float alpha = group.isExpanded ? 0.15f : 0.4f;
                    EditorGUI.DrawRect(headerRect, new Color(0.2f, 0.6f, 1f, alpha));
                }
                else if (groupHasMatch)
                {
                    Color highlightColor = group.isExpanded ? new Color(0.8f, 0.8f, 0.2f, 0.15f) : new Color(0.8f, 0.8f, 0.2f, 0.4f);
                    EditorGUI.DrawRect(headerRect, highlightColor);
                }

                Rect dragHandleRect = new Rect(headerRect.x, headerRect.y, 15, headerRect.height);
                DrawGripHandle(dragHandleRect);
                EditorGUIUtility.AddCursorRect(dragHandleRect, MouseCursor.Pan);

                Event evt = Event.current;

                if (evt.type == EventType.MouseDown && dragHandleRect.Contains(evt.mousePosition))
                {
                    m_CurrentDragAction = DragActionType.Group;
                    m_DragCandidateGroup = group;
                    evt.Use();
                }

                if (evt.type == EventType.MouseDrag && m_CurrentDragAction == DragActionType.Group && m_DragCandidateGroup == group)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("DraggedGroup", group);
                    DragAndDrop.StartDrag($"Move Group {group.groupName}");
                    evt.Use();
                }

                HandleGroupShapeDropArea(headerRect, group);
                HandleGroupReorderDropArea(headerRect, group);

                Rect foldoutRect = new Rect(headerRect.x + 15, headerRect.y, headerRect.width - 60, headerRect.height);

                EditorGUI.BeginChangeCheck();
                bool isExpanded = EditorGUI.Foldout(foldoutRect, group.isExpanded, group.groupName, true);

                if (EditorGUI.EndChangeCheck())
                {
                    group.isExpanded = isExpanded;
                    SaveLayoutData();
                }

                Rect btnRectRename = new Rect(headerRect.xMax - 45, headerRect.y, 20, headerRect.height);
                Rect btnRectX = new Rect(headerRect.xMax - 20, headerRect.y, 20, headerRect.height);

                if (GUI.Button(btnRectRename, new GUIContent("R", "Rename Group")))
                {
                    group.isRenaming = !group.isRenaming;
                }

                if (GUI.Button(btnRectX, "X"))
                {
                    Undo.RecordObject(this, "Delete Group");
                    m_LayoutData.groups.Remove(group);

                    SaveLayoutData();
                    EditorGUILayout.EndVertical();
                    GUIUtility.ExitGUI();
                }

                if (group.isRenaming)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    string renamedGroup = EditorGUILayout.TextField(group.groupName);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (renamedGroup.Trim() != "")
                        {
                            Undo.RecordObject(this, "Rename Group");
                            group.groupName = renamedGroup;
                            SaveLayoutData();
                        }
                    }

                    if (GUILayout.Button("Save", GUILayout.Width(50)))
                    {
                        group.isRenaming = false;
                        GUI.FocusControl(null);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(2);
                    EditorGUI.indentLevel--;
                }

                if (group.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    if (group.shapes.Count == 0)
                    {
                        Rect emptyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(emptyRect, "Group is empty.", EditorStyles.centeredGreyMiniLabel);
                        HandleGroupShapeDropArea(emptyRect, group);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Sort", GUILayout.Width(45)))
                        {
                            GenericMenu menu = new GenericMenu();

                            menu.AddItem(new GUIContent("Original"), false, () => { group.organizeMode = 0; OrganizeGroup(group, m); });
                            menu.AddItem(new GUIContent("A-Z"), false, () => { group.organizeMode = 1; OrganizeGroup(group, m); });
                            menu.AddItem(new GUIContent("Z-A"), false, () => { group.organizeMode = 2; OrganizeGroup(group, m); });

                            menu.ShowAsContext();
                        }

                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < group.shapes.Count; i++)
                        {
                            string shapeName = group.shapes[i];
                            int shapeIndex = m.GetBlendShapeIndex(shapeName);
                            if (shapeIndex == -1) continue;

                            Rect shapeRect = DrawBlendShapeSlider(m, shapeIndex, shapeName);
                            HandleShapeDropArea(shapeRect, group, shapeName);
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            if (ungroupedIndices.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                m_LayoutData.sortMode = GUILayout.Toolbar(m_LayoutData.sortMode, new string[] { "Original", "A-Z", "Z-A" });
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Change Sort Mode");
                    SaveLayoutData();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                bool newToggle = EditorGUILayout.ToggleLeft(new GUIContent("Built-in category detection", "VRChat\nMMD\nUnified Expressions\nARKit"), m_LayoutData.useBuiltInCategorization);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Toggle Auto Categorization");
                    m_LayoutData.useBuiltInCategorization = newToggle;
                    SaveLayoutData();
                }

                if (GUILayout.Button("Auto-Categorize BlendShapes", GUILayout.Height(25)))
                {
                    AutoCategorizeBlendShapes(m);

                    foreach (var g in m_LayoutData.groups) g.isExpanded = false;
                    SaveLayoutData();
                }

                EditorGUILayout.EndHorizontal();

                Rect sepRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

                if (Event.current.type == EventType.ContextClick && sepRect.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Copy Separators"), false, () => {
                        EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(new SeparatorClipboardWrapper { list = m_LayoutData.separators });
                    });

                    menu.AddItem(new GUIContent("Paste Separators"), false, () => {
                        try
                        {
                            SeparatorClipboardWrapper wrapper = JsonUtility.FromJson<SeparatorClipboardWrapper>(EditorGUIUtility.systemCopyBuffer);
                            if (wrapper != null && wrapper.list != null)
                            {
                                Undo.RecordObject(this, "Paste Separators");
                                m_LayoutData.separators = wrapper.list;
                                SaveLayoutData();
                            }
                        }
                        catch
                        {
                            Debug.LogWarning("Clipboard does not contain valid separator data.");
                        }
                    });

                    menu.ShowAsContext();
                    Event.current.Use();
                }

                EditorGUI.BeginChangeCheck();
                bool isSepExpanded = EditorGUI.Foldout(sepRect, m_LayoutData.isSeparatorsExpanded, "Boundary Separators (Right-Click to Copy/Paste)", true);

                if (EditorGUI.EndChangeCheck())
                {
                    m_LayoutData.isSeparatorsExpanded = isSepExpanded;
                    SaveLayoutData();
                }

                if (m_LayoutData.isSeparatorsExpanded)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < m_LayoutData.separators.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        string newSep = EditorGUILayout.TextField(m_LayoutData.separators[i]);

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(this, "Edit Separator");

                            m_LayoutData.separators[i] = newSep;
                            SaveLayoutData();
                        }

                        if (GUILayout.Button("X", GUILayout.Width(25)))
                        {
                            Undo.RecordObject(this, "Remove Separator");

                            m_LayoutData.separators.RemoveAt(i);
                            SaveLayoutData();
                            i--;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 15f);

                    if (GUILayout.Button("Add Custom Separator"))
                    {
                        Undo.RecordObject(this, "Add Separator");
                        m_LayoutData.separators.Add("");
                        SaveLayoutData();
                    }

                    if (GUILayout.Button("Reset to Defaults", GUILayout.Width(120)))
                    {
                        Undo.RecordObject(this, "Reset Separators");
                        m_LayoutData.separators = new List<string>(DefaultSeparators);
                        SaveLayoutData();
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(12);
            }

            Rect ungroupedRect = EditorGUILayout.BeginVertical();
            Rect ungroupedHeaderRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(ungroupedHeaderRect, "Ungrouped", EditorStyles.boldLabel);
            HandleGroupShapeDropArea(ungroupedHeaderRect, null);

            if (ungroupedIndices.Count == 0)
            {
                Rect emptyRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(emptyRect, isSearching ? "No matching ungrouped shapes." : "No ungrouped blendshapes.", EditorStyles.centeredGreyMiniLabel);
                HandleGroupShapeDropArea(emptyRect, null);
            }
            else
            {
                foreach (int index in ungroupedIndices)
                {
                    string shapeName = m.GetBlendShapeName(index);
                    if (isSearching && !shapeName.ToLower().Contains(searchLower))
                        continue;

                    Rect shapeRect = DrawBlendShapeSlider(m, index, shapeName);
                    HandleShapeDropArea(shapeRect, null, shapeName);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        private void OrganizeGroup(BlendShapeGroup group, Mesh mesh)
        {
            Undo.RecordObject(this, "Organize Group");

            Dictionary<string, int> meshOrder = new Dictionary<string, int>();
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string name = mesh.GetBlendShapeName(i);
                meshOrder[name] = i;
            }

            switch (group.organizeMode)
            {
                case 0: // Original
                    group.shapes.Sort((a, b) =>
                    {
                        int ia = meshOrder.ContainsKey(a) ? meshOrder[a] : int.MaxValue;
                        int ib = meshOrder.ContainsKey(b) ? meshOrder[b] : int.MaxValue;
                        return ia.CompareTo(ib);
                    });
                    break;

                case 1: // A-Z
                    group.shapes.Sort((a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
                    break;

                case 2: // Z-A
                    group.shapes.Sort((a, b) => string.Compare(b, a, StringComparison.OrdinalIgnoreCase));
                    break;
            }

            SaveLayoutData();
        }

        private Rect DrawBlendShapeSlider(Mesh m, int shapeIndex, string shapeName)
        {
            int arraySize = m_BlendShapeWeights.arraySize;
            int blendShapeCount = m.blendShapeCount;

            int vCount = GetAffectedVertexCount(m, shapeIndex, shapeName);
            GUIContent content = new GUIContent(shapeName, vCount >= 0 ? $"Vertices: {vCount}" : "Vertices: (Mesh unreadable)");

            float sliderMin = 0f, sliderMax = 0f;
            int frameCount = m.GetBlendShapeFrameCount(shapeIndex);
            for (int j = 0; j < frameCount; j++)
            {
                float frameWeight = m.GetBlendShapeFrameWeight(shapeIndex, j);
                sliderMin = Mathf.Min(frameWeight, sliderMin);
                sliderMax = Mathf.Max(frameWeight, sliderMax);
            }

            Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(controlRect.x, controlRect.y, labelWidth, controlRect.height);
            Rect sliderRect = new Rect(controlRect.x + labelWidth, controlRect.y, controlRect.width - labelWidth, controlRect.height);

            bool isSelected = m_SelectedShapes.Contains(shapeName);
            bool isSearchMatch = !string.IsNullOrEmpty(m_SearchText) && shapeName.ToLower().Contains(m_SearchText.ToLower());

            if (isSelected)
            {
                EditorGUI.DrawRect(labelRect, new Color(0.2f, 0.45f, 0.8f, 0.4f));
            }
            else if (isSearchMatch)
            {
                EditorGUI.DrawRect(labelRect, new Color(0.8f, 0.8f, 0.2f, 0.4f));
            }

            Event evt = Event.current;

            if (evt.type == EventType.MouseDown)
            {
                if (sliderRect.Contains(evt.mousePosition))
                {
                    m_CurrentDragAction = DragActionType.Slider;
                }
                else if (labelRect.Contains(evt.mousePosition))
                {
                    m_CurrentDragAction = DragActionType.Shape;

                    if (evt.control || evt.command)
                    {
                        if (isSelected) m_SelectedShapes.Remove(shapeName);
                        else m_SelectedShapes.Add(shapeName);
                        m_LastClickedShape = shapeName;
                    }
                    else if (evt.shift && m_LastClickedShape != null && m_CurrentDisplayOrder.Contains(m_LastClickedShape))
                    {
                        int startIndex = m_CurrentDisplayOrder.IndexOf(m_LastClickedShape);
                        int endIndex = m_CurrentDisplayOrder.IndexOf(shapeName);
                        int min = Mathf.Min(startIndex, endIndex);
                        int max = Mathf.Max(startIndex, endIndex);

                        m_SelectedShapes.Clear();

                        for (int i = min; i <= max; i++)
                        {
                            m_SelectedShapes.Add(m_CurrentDisplayOrder[i]);
                        }
                    }
                    else
                    {
                        if (!isSelected)
                        {
                            m_SelectedShapes.Clear();
                            m_SelectedShapes.Add(shapeName);
                        }
                        m_LastClickedShape = shapeName;
                    }

                    m_DragCandidateShape = shapeName;
                    m_DidDragOccur = false;
                    evt.Use();
                }
            }

            if (evt.type == EventType.MouseDrag && m_CurrentDragAction == DragActionType.Shape && m_DragCandidateShape == shapeName)
            {
                m_DidDragOccur = true;
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData("DraggedShapes", m_SelectedShapes.ToList());
                DragAndDrop.StartDrag(m_SelectedShapes.Count > 1 ? $"Move {m_SelectedShapes.Count} Shapes" : "Move Shape");
                evt.Use();
            }

            EditorGUI.BeginChangeCheck();
            float newSliderValue = 0f;

            if (shapeIndex < arraySize)
            {
                EditorGUI.Slider(controlRect, m_BlendShapeWeights.GetArrayElementAtIndex(shapeIndex), sliderMin, sliderMax, content);
            }
            else
            {
                newSliderValue = EditorGUI.Slider(controlRect, content, 0f, sliderMin, sliderMax);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change BlendShape Weight");
                if (shapeIndex >= arraySize || m_BlendShapeWeights.arraySize < blendShapeCount)
                {
                    m_BlendShapeWeights.arraySize = blendShapeCount;
                    if (shapeIndex >= arraySize)
                        m_BlendShapeWeights.GetArrayElementAtIndex(shapeIndex).floatValue = newSliderValue;
                }

                if (Event.current.shift && m_SelectedShapes.Contains(shapeName))
                {
                    float targetValue = m_BlendShapeWeights.GetArrayElementAtIndex(shapeIndex).floatValue;

                    foreach (string selShape in m_SelectedShapes)
                    {
                        if (selShape == shapeName) continue;
                        int selIdx = m.GetBlendShapeIndex(selShape);

                        if (selIdx != -1)
                        {
                            m_BlendShapeWeights.GetArrayElementAtIndex(selIdx).floatValue = targetValue;
                        }
                    }
                }
            }

            return controlRect;
        }

        private void HandleGroupShapeDropArea(Rect headerRect, BlendShapeGroup targetGroup)
        {
            Event evt = Event.current;
            List<string> draggedShapes = DragAndDrop.GetGenericData("DraggedShapes") as List<string>;

            if (draggedShapes != null && headerRect.Contains(evt.mousePosition))
            {
                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.Use();
                }
                else if (evt.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Move)
                {
                    EditorGUI.DrawRect(headerRect, new Color(0, 1f, 1f, 0.2f));
                }
                else if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    Undo.RecordObject(this, "Move Blendshapes to Group");
                    foreach (BlendShapeGroup g in m_LayoutData.groups) g.shapes.RemoveAll(s => draggedShapes.Contains(s));

                    if (targetGroup != null) targetGroup.shapes.AddRange(draggedShapes);

                    SaveLayoutData();
                    DragAndDrop.SetGenericData("DraggedShapes", null);
                    evt.Use();
                }
            }
        }

        private void HandleShapeDropArea(Rect dropRect, BlendShapeGroup targetGroup, string targetShapeName)
        {
            Event evt = Event.current;
            List<string> draggedShapes = DragAndDrop.GetGenericData("DraggedShapes") as List<string>;

            if (draggedShapes != null && dropRect.Contains(evt.mousePosition))
            {
                bool dropAbove = evt.mousePosition.y < dropRect.center.y;

                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.Use();
                }
                else if (evt.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Move)
                {
                    Rect lineRect = new Rect(dropRect.x, dropAbove ? dropRect.yMin : dropRect.yMax, dropRect.width, 2);
                    EditorGUI.DrawRect(lineRect, Color.cyan);
                }
                else if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    Undo.RecordObject(this, "Reorder Blendshapes");
                    foreach (BlendShapeGroup g in m_LayoutData.groups) g.shapes.RemoveAll(s => draggedShapes.Contains(s));

                    if (targetGroup != null)
                    {
                        int insertIndex = targetGroup.shapes.Count;
                        if (!string.IsNullOrEmpty(targetShapeName))
                        {
                            insertIndex = targetGroup.shapes.IndexOf(targetShapeName);
                            if (!dropAbove) insertIndex++;
                        }
                        insertIndex = Mathf.Clamp(insertIndex, 0, targetGroup.shapes.Count);
                        targetGroup.shapes.InsertRange(insertIndex, draggedShapes);
                    }

                    SaveLayoutData();
                    DragAndDrop.SetGenericData("DraggedShapes", null);
                    evt.Use();
                }
            }
        }

        private void HandleGroupReorderDropArea(Rect headerRect, BlendShapeGroup targetGroup)
        {
            Event evt = Event.current;
            BlendShapeGroup draggedGroup = DragAndDrop.GetGenericData("DraggedGroup") as BlendShapeGroup;

            if (draggedGroup != null && draggedGroup != targetGroup && headerRect.Contains(evt.mousePosition))
            {
                bool dropAbove = evt.mousePosition.y < headerRect.center.y;

                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.Use();
                }
                else if (evt.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Move)
                {
                    Rect lineRect = new Rect(headerRect.x, dropAbove ? headerRect.yMin : headerRect.yMax, headerRect.width, 2);
                    EditorGUI.DrawRect(lineRect, Color.cyan);
                }
                else if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    Undo.RecordObject(this, "Reorder Group");
                    m_LayoutData.groups.Remove(draggedGroup);
                    int newIndex = m_LayoutData.groups.IndexOf(targetGroup);
                    if (!dropAbove) newIndex++;
                    m_LayoutData.groups.Insert(newIndex, draggedGroup);

                    SaveLayoutData();
                    DragAndDrop.SetGenericData("DraggedGroup", null);
                    evt.Use();
                }
            }
        }

    }
}

#endif