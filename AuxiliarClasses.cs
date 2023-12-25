using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding.Utils;
using UnityEngine;

namespace PronoesProMod
{

    public class DialogPromptInteractableObject
    {
        public string objName;
        public string[] objDialog;
        public string[] dialogSounds;
        public string interactionPropt;

        public DialogPromptInteractableObject(string name, string[] dialog, string[] sounds, string prompt = "Interact")
        {
            objName = name;
            objDialog = dialog;
            interactionPropt = prompt;
            dialogSounds = sounds;
        }
    }


    public class tk2dAnimationDataForCreation
    {
        public string name;

        public tk2dSpriteAnimation source;
        public int[] frames;
        public string sourceClip;

        public tk2dAnimationDataForCreation(string animName, tk2dSpriteAnimation animSource, int[] frameNumbers, string clipSource = null)
        {
            name = animName;
            source = animSource;
            frames = frameNumbers;
            sourceClip = clipSource;
        }
    }

    public class tk2dFullAnimationData
    {
        public string name;

        public tk2dSpriteCollectionData collection;
        public tk2dAnimationDataForCreation[] animationData;

        public tk2dFullAnimationData(tk2dSpriteCollectionData data, tk2dAnimationDataForCreation[] animData)
        {
            collection = data;
            animationData = animData;
        }
    }

    public class tk2dFullAnimation
    {
        public string name;

        public tk2dFullAnimationData animation;
        public tk2dSpriteCollectionData colection;

        public tk2dFullAnimation(string _name, tk2dFullAnimationData _anim, tk2dSpriteCollectionData _collection)
        {
            name = _name;

            animation = _anim;
            colection = _collection;
        }
    }

    public static class tk2dAuxiliarHelper
    {

        public static Material CreateMaterialFromTexture(Texture tex, bool setdontdestroy = true)
        {
            Material mat = new Material(Shader.Find("tk2d/BlendVertexColor"));
            mat.mainTexture = tex;
            if (setdontdestroy)
            {
                UnityEngine.Object.DontDestroyOnLoad(mat);
                UnityEngine.Object.DontDestroyOnLoad(tex);
            }

            return mat;
        }

        public static tk2dSpriteCollectionData CreateTk2dSpriteCollection(Texture texture, string[] names, Rect[] rects, Vector2[] anchors, GameObject go)
        {
            if (texture != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(texture);
                var spriteCollection = CreateFromTexture(go, texture, new tk2dSpriteCollectionSize(), new Vector2((float)texture.width, (float)texture.height), names, rects, null, anchors, new bool[6]);
                string text = "SpriteFromTexture " + texture.name;
                spriteCollection.spriteCollectionName = text;
                spriteCollection.spriteDefinitions[0].material.name = text;
                spriteCollection.spriteDefinitions[0].material.hideFlags = (HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);

                UnityEngine.Object.DontDestroyOnLoad(spriteCollection);
                return spriteCollection;
            }

            throw new NullReferenceException();
        }

        public static tk2dSpriteCollectionData CreateFromTexture(GameObject parentObject, Texture texture, tk2dSpriteCollectionSize size, Vector2 textureDimensions, string[] names, Rect[] regions, Rect[] trimRects, Vector2[] anchors, bool[] rotated)
        {
            tk2dSpriteCollectionData tk2dSpriteCollectionData = parentObject.GetOrAddComponent<tk2dSpriteCollectionData>();
            tk2dSpriteCollectionData.material = CreateMaterialFromTexture(texture);
            tk2dSpriteCollectionData.materials = new Material[]
            {
                tk2dSpriteCollectionData.material
            };
            tk2dSpriteCollectionData.textures = new Texture[]
            {
                texture
            };
            float scale = 2f * size.OrthoSize / size.TargetHeight;
            Rect trimRect = new Rect(0f, 0f, 0f, 0f);
            tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                if (trimRects != null)
                {
                    trimRect = trimRects[i];
                }
                else
                {
                    trimRect.Set(0f, 0f, regions[i].width, regions[i].height);
                }

                tk2dSpriteCollectionData.spriteDefinitions[i] = CreateDefinitionForRegionInTexture(names[i], textureDimensions, scale, regions[i], trimRect, anchors[i], false);
            }

            foreach (tk2dSpriteDefinition tk2dSpriteDefinition in tk2dSpriteCollectionData.spriteDefinitions)
            {
                tk2dSpriteDefinition.material = tk2dSpriteCollectionData.material;
            }

            return tk2dSpriteCollectionData;
        }

        public static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(string name, Vector2 textureDimensions, float scale, Rect uvRegion, Rect trimRect, Vector2 anchor, bool rotated)
        {
            float height = uvRegion.height;
            float width = uvRegion.width;
            float x = textureDimensions.x;
            float y = textureDimensions.y;
            tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
            tk2dSpriteDefinition.flipped = ((!rotated) ? tk2dSpriteDefinition.FlipMode.None : tk2dSpriteDefinition.FlipMode.TPackerCW);
            tk2dSpriteDefinition.extractRegion = false;
            tk2dSpriteDefinition.name = name;
            tk2dSpriteDefinition.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
            Vector2 vector = new Vector2(0.001f, 0.001f);
            Vector2 vector2 = new Vector2((uvRegion.x + vector.x) / x, 1f - (uvRegion.y + uvRegion.height + vector.y) / y);
            Vector2 vector3 = new Vector2((uvRegion.x + uvRegion.width - vector.x) / x, 1f - (uvRegion.y - vector.y) / y);
            Vector2 a = new Vector2(trimRect.x - anchor.x, -trimRect.y + anchor.y);
            if (rotated)
            {
                a.y -= width;
            }

            a *= scale;
            Vector3 a2 = new Vector3(-anchor.x * scale, anchor.y * scale, 0f);
            Vector3 vector4 = a2 + new Vector3(trimRect.width * scale, -trimRect.height * scale, 0f);
            Vector3 a3 = new Vector3(0f, -height * scale, 0f);
            Vector3 vector5 = a3 + new Vector3(width * scale, height * scale, 0f);
            if (rotated)
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(-vector5.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-vector5.y + a.x, vector5.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, vector5.x + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector3.y),
                    new Vector2(vector3.x, vector2.y)
                };
            }
            else
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(a3.x + a.x, a3.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, a3.y + a.y, 0f),
                    new Vector3(a3.x + a.x, vector5.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, vector5.y + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector2.y),
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector3.x, vector3.y)
                };
            }

            tk2dSpriteDefinition.normals = new Vector3[0];
            tk2dSpriteDefinition.tangents = new Vector4[0];
            tk2dSpriteDefinition.indices = new int[]
            {
                0,
                3,
                1,
                2,
                3,
                0
            };
            Vector3 b = new Vector3(a2.x, vector4.y, 0f);
            Vector3 a4 = new Vector3(vector4.x, a2.y, 0f);
            tk2dSpriteDefinition.boundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.untrimmedBoundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.texelSize = new Vector2(scale, scale);
            return tk2dSpriteDefinition;
        }

        public static tk2dSpriteCollectionData CreateCollectionSimple(Texture2D tex, GameObject go, Vector2 baseAnchor, int frameCount = 1)
        {
            Vector2[] anchors = new Vector2[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                anchors[i] = baseAnchor;
            }
            return CreateCollections(tex, go, anchors, frameCount);
        }

        public static tk2dSpriteCollectionData CreateCollections(Texture2D tex, GameObject go, Vector2[] anchors, int frameCount = 1)
        {
            float width = (float)tex.width / frameCount;
            float height = (float)tex.height;
            string[] names = new string[frameCount];
            Rect[] rects = new Rect[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                names[i] = i.ToString();
                rects[i] = new Rect(width * (float)i, 0, width, height);
            }
            tk2dSpriteCollectionData frame = CreateFromTexture(go, tex, tk2dSpriteCollectionSize.PixelsPerMeter(66f), new Vector2(width * frameCount, height), names, rects, null, anchors, new bool[frameCount]);
            frame.hasPlatformData = false;

            return frame;
        }

        public static void AddAnimations(tk2dSpriteAnimation animation, tk2dSpriteCollectionData collection, tk2dAnimationDataForCreation[] animations)
        {
            tk2dSpriteAnimationClip[] clips = new tk2dSpriteAnimationClip[animations.Length];

            for (int i = 0; i < animations.Length; i++)
            {
                clips[i] = AddSingleAnimation(animations[i].source, collection, animations[i].name, animations[i].frames, animations[i].sourceClip);
            }

            for (int i = 0; i < clips.Length; i++)
            {
                animation.clips = animation.clips.Append(clips[i]).ToArray();
            }
        }

        public static tk2dSpriteAnimationClip[] MakeAnimations(tk2dSpriteCollectionData collection, tk2dAnimationDataForCreation[] animations)
        {
            tk2dSpriteAnimationClip[] clips = new tk2dSpriteAnimationClip[animations.Length];

            for (int i = 0; i < animations.Length; i++)
            {
                clips[i] = AddSingleAnimation(animations[i].source, collection, animations[i].name, animations[i].frames, animations[i].sourceClip);
            }
            return clips;
        }

        public static tk2dSpriteAnimationClip AddSingleAnimation(tk2dSpriteAnimation source, tk2dSpriteCollectionData collection, string clipName, int[] clipFrames, string sourceClip = null)
        {
            tk2dSpriteAnimationClip clip;
            if (source == null || sourceClip == null)
            {
                clip = new tk2dSpriteAnimationClip()
                {
                    name = clipName,
                    frames = new tk2dSpriteAnimationFrame[clipFrames.Length]
                };
                for (int i = 0; i < clip.frames.Length; i++)
                {
                    clip.frames[i] = new() { spriteCollection = collection, spriteId = clipFrames[i] };
                }
            }
            else
            {
                clip = new tk2dSpriteAnimationClip(source.GetClipByName(sourceClip))
                {
                    name = clipName,
                    frames = new tk2dSpriteAnimationFrame[clipFrames.Length]
                };
                for (int i = 0; i < clip.frames.Length; i++)
                {
                    clip.frames[i] = new() { spriteCollection = collection, spriteId = clipFrames[i] };
                }
            }
            return clip;
        }
    }
}


