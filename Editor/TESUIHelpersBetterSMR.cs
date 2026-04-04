#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public static class TESUIHelpersBetterSMR
{
    public class SetLabelWidth : IDisposable
    {
        float originalLabelWidth;

        public SetLabelWidth(float x)
        {
            originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = x;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }
    }

    public class UIDisableEnableScope : IDisposable
    {
        bool originalUIState;

        public UIDisableEnableScope(bool x)
        {
            originalUIState = GUI.enabled;
            GUI.enabled = x;
        }

        public void Dispose()
        {
            GUI.enabled = originalUIState;
        }
    }

    public class ChangeCheckScope : IDisposable
    {
        public Action callBack;

        public ChangeCheckScope(Action callBack)
        {
            this.callBack = callBack;
            EditorGUI.BeginChangeCheck();
        }

        public void Dispose()
        {
            if (EditorGUI.EndChangeCheck())
            {
                callBack();
            }
        }
    }

    public class SqueezeScope : IDisposable
    {
        private readonly SqueezeSettings[] settings;

        public enum SqueezeScopeType
        {
            Horizontal,
            Vertical,
            EditorH,
            EditorV
        }

        public SqueezeScope(SqueezeSettings input) : this(new[] { input }) { }

        public SqueezeScope(params SqueezeSettings[] input)
        {
            settings = input;

            foreach (var squeezeSettings in input)
            {
                BeginSqueeze(squeezeSettings);
            }
        }

        private void BeginSqueeze(SqueezeSettings squeezeSettings)
        {
            switch (squeezeSettings.type)
            {
                case SqueezeScopeType.Horizontal:
                    GUILayout.BeginHorizontal(squeezeSettings.style);
                    break;
                case SqueezeScopeType.Vertical:
                    GUILayout.BeginVertical(squeezeSettings.style);
                    break;
                case SqueezeScopeType.EditorH:
                    EditorGUILayout.BeginHorizontal(squeezeSettings.style);
                    break;
                case SqueezeScopeType.EditorV:
                    EditorGUILayout.BeginVertical(squeezeSettings.style);
                    break;
            }

            GUILayout.Space(squeezeSettings.width1);
        }

        public void Dispose()
        {
            foreach (var squeezeSettings in settings.Reverse())
            {
                GUILayout.Space(squeezeSettings.width2);

                switch (squeezeSettings.type)
                {
                    case SqueezeScopeType.Horizontal:
                        GUILayout.EndHorizontal();
                        break;
                    case SqueezeScopeType.Vertical:
                        GUILayout.EndVertical();
                        break;
                    case SqueezeScopeType.EditorH:
                        EditorGUILayout.EndHorizontal();
                        break;
                    case SqueezeScopeType.EditorV:
                        EditorGUILayout.EndVertical();
                        break;
                }
            }
        }

    }

    public struct SqueezeSettings
    {
        public int width1;
        public int width2;
        public SqueezeScope.SqueezeScopeType type;
        public GUIStyle style;

        public static implicit operator SqueezeSettings((int, int) val)
        {
            return new SqueezeSettings
            {
                width1 = val.Item1,
                width2 = val.Item2,
                type = SqueezeScope.SqueezeScopeType.Horizontal,
                style = GUIStyle.none
            };
        }

        public static implicit operator SqueezeSettings((int, int, SqueezeScope.SqueezeScopeType) val)
        {
            return new SqueezeSettings
            {
                width1 = val.Item1,
                width2 = val.Item2,
                type = val.Item3,
                style = GUIStyle.none
            };
        }

        public static implicit operator SqueezeSettings((int, int, SqueezeScope.SqueezeScopeType, GUIStyle) val)
        {
            return new SqueezeSettings
            {
                width1 = val.Item1,
                width2 = val.Item2,
                type = val.Item3,
                style = val.Item4
            };
        }

    }

    //https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/#post-7191802
    //https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/#post-7229248
    //Thanks to JelleJurre for help, and for JeTeeS for sharing this class

}

#endif