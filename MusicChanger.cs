using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace PronoesProMod
{
    public static class MusicChanger
    {
        public static void PlayBackgroundMusicForScene(AudioClip clip)
        {
            MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
            List<MusicCue.MusicChannelInfo> channelInfos = new List<MusicCue.MusicChannelInfo>();
            MusicCue.MusicChannelInfo channelInfo = new MusicCue.MusicChannelInfo();

            ReflectionHelper.SetField(channelInfo, "clip", clip);
            channelInfos.Add(channelInfo);
            channelInfos.Add(null);
            channelInfos.Add(null);
            channelInfos.Add(null);
            channelInfos.Add(null);
            channelInfos.Add(null);
            ReflectionHelper.SetField(musicCue, "channelInfos", channelInfos.ToArray());
            var objs = Resources.FindObjectsOfTypeAll<AudioMixer>();
            foreach (var x in objs)
            {
                if (x.name == "Music")
                {
                    var yoursnapshot = x.FindSnapshot("Main Only");
                    yoursnapshot.TransitionTo(1f);
                    GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0, 0, false);
                    return;
                }
            }
        }
    }
}
