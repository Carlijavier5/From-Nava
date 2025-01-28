using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum ObjectState { Default, Burning, Frozen }

public class BaseObject : MonoBehaviour {

    public event System.Action<Vector2, float> OnBlow;
    public event System.Action<ObjectState, bool> OnHeatToggle;
    public event System.Action<int> OnDamage;
    public event System.Action OnCopy;

    [System.Serializable] public class ObjectAttributes {
        public bool isHeavy;
        public bool isCopyable;
    }[SerializeField] protected ObjectAttributes attributes;
    public ObjectAttributes Attributes => attributes;
    [SerializeField] protected ObjectState defaultState;

    [SerializeField] private SpriteRenderer[] spriteRenderers;
    public SpriteRenderer[] SpriteRenderers => spriteRenderers;
    private readonly Dictionary<SpriteRenderer, List<Material>> materialDict = new();

    public BaseObject ParentObject { get; protected set; }

    private readonly HashSet<MonoBehaviour> triggers = new();

    public ObjectState State { get; private set; }
    private Vector2 origin;

    /// <summary>
    /// Associate an object with another to migrate interaction calls;
    /// <br></br> Should be called on Awake;
    /// </summary>
    /// <param name="parentObject"> Object to link; </param>
    public void AttachTo(BaseObject parentObject) => ParentObject = parentObject;

    protected virtual void Awake() {
        SurveyBaseMaterials();
        origin = transform.position;
    }

    protected virtual void Start() => ObjectReset();

    protected void Ignite() => OnHeatToggle?.Invoke(State, true);
    public virtual void Ignite(MonoBehaviour trigger) { if (IsTriggerRelevant(trigger)) Ignite(); }
    protected void Freeze() => OnHeatToggle?.Invoke(State, false);
    public virtual void Freeze(MonoBehaviour trigger) { if (IsTriggerRelevant(trigger)) Freeze(); }
    public virtual void Blow(MonoBehaviour trigger, Vector2 dir, float strength) {
        if (IsTriggerRelevant(trigger)) OnBlow?.Invoke(dir, strength);
    }
    public void Damage(int damageAmount) => OnDamage?.Invoke(damageAmount);
    public virtual void Kill() { }

    public void SignalCopy() {
        OnCopy?.Invoke();
    }

    public virtual void SetState(ObjectState state) => State = state;
    public void ForceState(ObjectState state) {
        switch (state) {
            case ObjectState.Default:
                switch (State) {
                    case ObjectState.Burning:
                        Freeze();
                        break;
                    case ObjectState.Frozen:
                        Ignite();
                        break;
                } break;
            case ObjectState.Burning:
                switch (State) {
                    case ObjectState.Default:
                        Ignite();
                        break;
                    case ObjectState.Frozen:
                        Ignite();
                        Ignite();
                        break;
                } break;
            case ObjectState.Frozen:
                switch (State) {
                    case ObjectState.Default:
                        Freeze();
                        break;
                    case ObjectState.Burning:
                        Freeze();
                        Freeze();
                        break;
                } break;
        }
    }

    private void SurveyBaseMaterials() {
        foreach (SpriteRenderer renderer in SpriteRenderers) {
            materialDict[renderer] = new();
            materialDict[renderer].Add(renderer.material);
        }
    }

    public void ApplyMaterial(Material material, bool allowDuplicates = true) {
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.material = material;
            if (allowDuplicates
            || (!materialDict[renderer].Contains(material))) {
                materialDict[renderer].Add(material);
            }
        }
    }

    public void RemoveMaterial(Material material) {
        foreach (KeyValuePair<SpriteRenderer, List<Material>> kvp in materialDict) {
            List<Material> materialList = kvp.Value;
            for (int i = 0; i < materialList.Count; i++) {
                bool isLast = i == materialList.Count - 1;
                if (materialList[i] == material) {
                    materialList.RemoveAt(i);
                    if (isLast && materialList.Count > 0) {
                        foreach (SpriteRenderer renderer in spriteRenderers) {
                            renderer.material = materialList.Last();
                        }
                    }
                    break;
                }
            }
        }
    } 

    public virtual void ObjectReset() {
        transform.position = origin;
        ForceState(defaultState);
        SetState(defaultState);
    }

    private bool IsTriggerRelevant(MonoBehaviour trigger) {
        if (triggers.Contains(trigger)) return false;
        triggers.Add(trigger);
        QueueTriggerRemoval(trigger);
        return true;
    }

    private async void QueueTriggerRemoval(MonoBehaviour trigger) {
        await Task.Delay(500);
        triggers.Remove(trigger);
    }
}