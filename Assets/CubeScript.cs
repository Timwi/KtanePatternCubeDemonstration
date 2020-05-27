using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public GameObject[] Faces;
    public Camera Camera;
    public GameObject AllFaces;

    public Material[] ColoredMaterials;
    public Material SymbolMaterial;
    public Texture[] Symbols;

    enum Face { Front, Back, Top, Bottom, Left, Right }
    enum Rotation { X, MX, Y, MY, Z, MZ }

    struct TransformInfo
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static TransformInfo From(Transform t) { return new TransformInfo { Position = t.localPosition, Rotation = t.localRotation, Scale = t.localScale }; }
        public static TransformInfo From(GameObject obj) { return From(obj.transform); }
        public void Set(GameObject obj) { Set(obj.transform); }
        public void Set(Transform t) { t.localPosition = Position; t.localRotation = Rotation; t.localScale = Scale; }
    }

    TransformInfo[] FaceInfos;
    TransformInfo CameraInfo;
    TransformInfo AllFacesInfo;
    GameObject AllFacesCopy;

    int state;
    Coroutine coroutine;
    Queue<IEnumerator> queue = new Queue<IEnumerator>();

    T[] newArray<T>(params T[] array) { return array; }

    void OnMouseDown()
    {
        setState(state + 1);
    }

    void Start()
    {
        AllFacesInfo = TransformInfo.From(AllFaces);
        FaceInfos = Faces.Select(TransformInfo.From).ToArray();
        CameraInfo = TransformInfo.From(Camera.transform);
        setState(73-59);
    }

    void setState(int newState)
    {
        var stateMachine = newArray<Action>(
            () => { restoreEverything(null); },
            () => { startOrQueueAnimation(unfolding("T3F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T4L", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T1K", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T2R,R-B", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("R3B", noCamera: true, duration: .7f)); },
            () => { animateCamera(new TransformInfo { Position = new Vector3(1, 10, 0), Rotation = Quaternion.Euler(90, 90, 0), Scale = new Vector3(1, 1, 1) }); },
            () => { restoreEverything(null); },
            () => { startOrQueueAnimation(unfolding("R4F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T2R,R-F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T4L,L-K,L-B", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("L3B,B-K", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("B1K", noCamera: true, duration: .7f)); },
            () => { animateCamera(new TransformInfo { Position = new Vector3(-1, 10, 0), Rotation = Quaternion.Euler(90, 90, 0), Scale = new Vector3(1, 1, 1) }); },
            () => { restoreEverything(new[] { 2, 25, 9, 12, 4, 34 }); },
            () => { startOrQueueAnimation(unfolding("T3F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T4L", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T1K", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T2R,R-B", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("R3B", noCamera: true, duration: .7f)); },
            () => { animateCamera(new TransformInfo { Position = new Vector3(1, 10, 0), Rotation = Quaternion.Euler(90, 90, 0), Scale = new Vector3(1, 1, 1) }); },
            () => { takeCopy(); },
            () => { restoreEverything(new[] { 2, 25, 9, 12, 4, 34 }, keepCopy: true); },
            () => { startOrQueueAnimation(unfolding("R4F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T2R,R-F", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("T4L,L-K,L-B", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("L3B,B-K", noCamera: true, duration: .7f)); },
            () => { startOrQueueAnimation(unfolding("B1K", noCamera: true, duration: .7f)); },
            () => { animateCamera(new TransformInfo { Position = new Vector3(-1, 10, 0), Rotation = Quaternion.Euler(90, 90, 0), Scale = new Vector3(1, 1, 1) }); },
            () => { showWithCopy(); },
            () => { restoreEverything(null); },
            () => { startOrQueueAnimation(unfolding("T2R,T4L,L2F,L3B,L4K", 0, 1, -90)); },
            () => { setFaceSymbol(1, 91); },
            () => { startOrQueueAnimation(unfolding("T2R,T4L,L2F,L3B,L4K", backwards: true)); },
            () => { setFaceSymbol(0, 112); },
            () => { setFaceSymbol(4, 110); },
            () => { startOrQueueAnimation(unfolding("T2R,T4L,L2F,L3B,L4K", 0, 1, -90)); },
            () => { setFaceSymbol(3, 102); },
            () => { startOrQueueAnimation(unfolding("T2R,T4L,L2F,L3B,L4K", backwards: true)); },
            () => { startOrQueueAnimation(animateTransform(AllFaces.transform, new TransformInfo { Position = new Vector3(0, 0, 0), Rotation = Quaternion.Euler(-90, -90, -90), Scale = new Vector3(1, 1, 1) }, 1.2f)); },
            () => { startOrQueueAnimation(animateTransform(AllFaces.transform, new TransformInfo { Position = new Vector3(0, 0, 0), Rotation = Quaternion.Euler(0, 180, 90), Scale = new Vector3(1, 1, 1) }, 1.2f)); },
            () => { startOrQueueAnimation(animateTransform(AllFaces.transform, new TransformInfo { Position = new Vector3(0, 0, 0), Rotation = Quaternion.Euler(180, 90, 0), Scale = new Vector3(1, 1, 1) }, 1.2f)); },
            () => { setFaceSymbol(3, 1022); },
            () => { setFaceSymbol(2, 101); },
            () => { setFaceSymbol(5, 103); },
            () => { startOrQueueAnimation(unfolding("T2R,T4L,L2F,L3B,L4K", 0, 1, -90)); },
            null
        );
        state = newState % stateMachine.Length;
        if (stateMachine[state] != null)
            stateMachine[state]();
    }

    private void setFaceSymbol(int face, int symbol)
    {
        Faces[face].GetComponent<MeshRenderer>().sharedMaterial = SymbolMaterial;
        Faces[face].GetComponent<MeshRenderer>().material.mainTexture = Symbols.First(s => s.name == "Symbol" + symbol);
    }

    private IEnumerator unfolding(string str, int moveX = 0, int moveY = 0, int rotate = 0, bool backwards = false, Transform[] additionalTransforms = null, TransformInfo[] additionalTransformInfos = null, bool noCamera = false, float duration = 1.8f)
    {
        yield return null;
        var objsToDestroy = new List<GameObject>();
        var objsToRotate = new List<Transform>();
        var rotationInfos = new List<TransformInfo>();
        foreach (var ins in str.Split(','))
        {
            var anchorFace = Faces["TFRKLB".IndexOf(ins[0])].transform;
            var rotatingFace = Faces["TFRKLB".IndexOf(ins[2])].transform;

            var rotatorPlacer = new GameObject("Rotator Placer");
            rotatorPlacer.transform.parent = anchorFace;
            new TransformInfo { Position = new Vector3(0, 0, 0), Rotation = Quaternion.Euler(0, 0, ins[1] == '-' ? 0 : -90 * (ins[1] - '1')), Scale = new Vector3(.5f, .5f, .5f) }.Set(rotatorPlacer);
            objsToDestroy.Add(rotatorPlacer);

            var rotator = new GameObject("Rotator");
            rotator.transform.parent = rotatorPlacer.transform;
            new TransformInfo { Position = new Vector3(0, 1, 0), Rotation = Quaternion.identity, Scale = new Vector3(1, 1, 1) }.Set(rotator);
            objsToDestroy.Add(rotator);
            objsToRotate.Add(rotator.transform);
            rotationInfos.Add(new TransformInfo { Position = new Vector3(0, 1, 0), Rotation = Quaternion.Euler(ins[1] == '-' ? 0 : backwards ? 90 : -90, 0, 0), Scale = new Vector3(1, 1, 1) });

            rotatingFace.parent = rotator.transform;
        }

        if (!noCamera)
        {
            objsToRotate.Add(Camera.transform);
            rotationInfos.Add(backwards ? CameraInfo : new TransformInfo { Position = new Vector3(0, 10, 0), Rotation = Quaternion.Euler(90, 0, 0), Scale = new Vector3(1, 1, 1) });
        }

        objsToRotate.Add(AllFaces.transform);
        rotationInfos.Add(backwards ? AllFacesInfo : new TransformInfo { Position = new Vector3(moveX, 0, moveY), Rotation = Quaternion.Euler(0, rotate, 0), Scale = new Vector3(1, 1, 1) });

        if (additionalTransforms != null)
        {
            objsToRotate.AddRange(additionalTransforms);
            rotationInfos.AddRange(additionalTransformInfos);
        }

        yield return animateTransforms(objsToRotate.ToArray(), rotationInfos.ToArray(), duration: duration, cleanup: () =>
        {
            foreach (var f in Faces)
                f.transform.parent = AllFaces.transform;
            foreach (var obj in objsToDestroy)
                Destroy(obj);
        });
    }

    private void takeCopy()
    {
        AllFacesCopy = Instantiate(AllFaces);
        AllFacesCopy.transform.parent = transform;
        startOrQueueAnimation(animateTransform(AllFacesCopy.transform, new TransformInfo { Position = new Vector3(2.88f, -1, -5.45f), Rotation = Quaternion.identity, Scale = new Vector3(.2f, .2f, .2f) }, duration: .7f));
    }

    private void showWithCopy()
    {
        startOrQueueAnimation(animateTransforms(new[] { AllFaces.transform, AllFacesCopy.transform, Camera.transform },
            newArray(
                new TransformInfo { Position = new Vector3(1, 0, 3), Rotation = Quaternion.identity, Scale = new Vector3(1, 1, 1) },
                new TransformInfo { Position = new Vector3(-1, 0, -3), Rotation = Quaternion.identity, Scale = new Vector3(1, 1, 1) },
                new TransformInfo { Position = new Vector3(0, 12, 0), Rotation = Quaternion.Euler(90, 90, 0), Scale = new Vector3(1, 1, 1) }), duration: .7f));
    }

    private void restoreEverything(int[] symbols, bool keepCopy = false)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        queue.Clear();
        coroutine = null;
        AllFacesInfo.Set(AllFaces);
        for (var f = 0; f < Faces.Length; f++)
        {
            Faces[f].transform.parent = AllFaces.transform;
            //Debug.LogFormat(@"{0}, {1}, {2}", FaceInfos == null, Faces == null, Faces[f] == null);
            FaceInfos[f].Set(Faces[f]);
            if (symbols == null)
                Faces[f].GetComponent<MeshRenderer>().sharedMaterial = ColoredMaterials[f];
            else
            {
                Faces[f].GetComponent<MeshRenderer>().sharedMaterial = SymbolMaterial;
                Faces[f].GetComponent<MeshRenderer>().material.mainTexture = Symbols.First(sym => sym.name == "Symbol" + symbols[f]);
            }
            Faces[f].transform.Find(Faces[f].name).GetComponent<MeshRenderer>().sharedMaterial = Faces[f].GetComponent<MeshRenderer>().sharedMaterial;
        }
        CameraInfo.Set(Camera.transform);

        if (AllFacesCopy != null && !keepCopy)
        {
            Destroy(AllFacesCopy);
            AllFacesCopy = null;
        }
    }

    private void animateCamera(TransformInfo cameraPosition)
    {
        startOrQueueAnimation(animateTransform(Camera.transform, cameraPosition, .7f));
    }

    private IEnumerator animateTransform(Transform transform, TransformInfo newPos, float duration, Action cleanup = null)
    {
        return animateTransforms(new[] { transform }, new[] { newPos }, duration, cleanup);
    }

    private IEnumerator animateTransforms(Transform[] transforms, TransformInfo[] newPoses, float duration, Action cleanup = null)
    {
        var elapsed = 0f;
        var oldPoses = transforms.Select(TransformInfo.From).ToArray();
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            var t = Easing.InOutCubic(elapsed, 0, 1, duration);
            for (var i = 0; i < transforms.Length; i++)
            {
                transforms[i].localPosition = Vector3.Lerp(oldPoses[i].Position, newPoses[i].Position, t);
                transforms[i].localRotation = Quaternion.Slerp(oldPoses[i].Rotation, newPoses[i].Rotation, t);
                transforms[i].localScale = Vector3.Lerp(oldPoses[i].Scale, newPoses[i].Scale, t);
            }
        }

        if (cleanup != null)
            cleanup();

        if (queue.Count > 0)
            coroutine = StartCoroutine(queue.Dequeue());
        else
            coroutine = null;
    }

    private void animateRotation(int x, int y, int z, Rotation rotation, params Face[] faces)
    {
        startOrQueueAnimation(animate(x, y, z, rotation, faces));
    }

    private void startOrQueueAnimation(IEnumerator animation)
    {
        if (coroutine != null)
            queue.Enqueue(animation);
        else
            coroutine = StartCoroutine(animation);
    }

    private IEnumerator animate(int x, int y, int z, Rotation rotation, Face[] faces)
    {
        yield return null;
        var rotator = new GameObject();
        rotator.transform.parent = AllFaces.transform;
        rotator.transform.localPosition = new Vector3(x, y, z);
        rotator.transform.localRotation = Quaternion.identity;
        rotator.transform.localScale = new Vector3(1, 1, 1);

        foreach (var f in faces)
            Faces[(int) f].transform.parent = rotator.transform;

        var duration = .7f;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            var t = Easing.InOutCubic(elapsed, 0, 90, duration);
            rotator.transform.localEulerAngles = new Vector3(
                rotation == Rotation.X ? -t : rotation == Rotation.MX ? t : 0,
                rotation == Rotation.Y ? -t : rotation == Rotation.MY ? t : 0,
                rotation == Rotation.Z ? -t : rotation == Rotation.MZ ? t : 0);
        }

        foreach (var f in faces)
            Faces[(int) f].transform.parent = AllFaces.transform;

        Destroy(rotator);

        if (queue.Count > 0)
            coroutine = StartCoroutine(queue.Dequeue());
        else
            coroutine = null;
    }
}
