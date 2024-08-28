using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Zenject;

class Target_TriggeringObject : TargetSO<IEvent>
{
    [SerializeField] private string _eventTypeName;
    private Type _eventType;
    private IEvent _lastTriggeredEvent;
    private IEventManager _eventManager;

    [Inject]
    public void Construct(IEventManager eventManager)
    {
        _eventManager = eventManager;

        _eventType = Type.GetType(_eventTypeName);
        if (_eventType != null)
        {
            var method = _eventManager.GetType().GetMethod("AddListener");
            var genericMethod = method.MakeGenericMethod(_eventType);
            genericMethod.Invoke(_eventManager, new object[] { this });
        }
    }

    ~Target_TriggeringObject()
    {
        if (_eventType != null)
        {
            var method = typeof(IEventManager).GetMethod("RemoveListener");
            var genericMethod = method.MakeGenericMethod(_eventType);
            genericMethod.Invoke(_eventManager, new object[] { this });
        }
    }

    public override IEvent Target
    {
        get => _lastTriggeredEvent;
        set => _lastTriggeredEvent = value;
    }

    public Type EventType
    {
        get { return _eventType; }
        set { _eventType = value; }
    }

    public IEnumerator OnEventRaised(IEvent eventData)
    {
        if (_eventType.IsInstanceOfType(eventData))
        {
            _lastTriggeredEvent = eventData;
        }
        yield return null;
    }

    public bool ShouldReceiveEvent(IEvent eventData)
    {
        return _eventType.IsInstanceOfType(eventData);
    }

    public void SetEventType(Type eventType)
    {
        _eventType = eventType;
        _eventTypeName = eventType.AssemblyQualifiedName; 
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Target_TriggeringObject))]
public class Target_TriggeringObjectEditor : StoneSOEditor
{
    private SerializedProperty _eventTypeNameProperty;

    private void OnEnable()
    {
        _eventTypeNameProperty = serializedObject.FindProperty("_eventTypeName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_eventTypeNameProperty);

        if (GUILayout.Button("Select Event Type"))
        {
            ShowSelectEventTypeMenu(_eventTypeNameProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif