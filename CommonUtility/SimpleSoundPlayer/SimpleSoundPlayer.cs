using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.CommonUtility.SimpleSoundPlayer
{
    /// <summary>
    /// 简易的声音播放器
    /// </summary>
    public class SimpleSoundPlayer
    {
        //---------Members-----------
        /// <summary>
        /// 简易声音播放器单例
        /// </summary>
        public static SimpleSoundPlayer Instance = new SimpleSoundPlayer();


        //----------Structs----------

        #region SoundPlayer与停止时间
        /// <summary>
        /// SoundPlayer与停止时间
        /// </summary>
        public class PlayTime
        {
            //播放器
            public SoundPlayer Player = null;

            //结束播放的时间点
            public DateTime EndTime = DateTime.MinValue;
        }

        #endregion


        //----------Control----------

        #region 播放指定路径的Wav文件

        /// <summary>
        /// 播放指定路径的Wav文件
        /// </summary>
        /// <param name="FilePath">Wav路径</param>
        /// <param name="PlayTime">持续播放时间(单位：秒)
        /// <para />
        /// -1：无限地循环播放; 0：只播放一次; 具体正整数：持续循环地播放指定时间
        /// </param>
        public static void PlayWavFile(string FilePath, int PlayTime)
        {
            //增强判断
            if (File.Exists(FilePath) == false)
                return;

            //加载Wav文件
            SoundPlayer Player = new SoundPlayer(FilePath);

            //根据参数执行不同的行为
            if (PlayTime < 0)
                Player.PlayLooping();
            else if (PlayTime == 0)
                Player.Play();
            else
            {
                lock (((ICollection)_playTimeList).SyncRoot)
                {
                    Player.PlayLooping();
                    _playTimeList.Add(new PlayTime
                    {
                        Player = Player,
                        EndTime = DateTime.Now.AddSeconds(PlayTime)
                    });
                }
            }

        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SimpleSoundPlayer()
        {
            _playThread = new Thread(SoundPlayEvent);
            _playThread.Name = "SoundsPlayer::SimpleSoundPlayer 检查并处理“指定了持续播放时间的Player”";
            _playThread.IsBackground = true;
            _playThread.Start();
        }
        #endregion

        #region 指定了持续播放时间的Player相关处理

        private readonly Thread _playThread = null;

        private readonly static List<PlayTime> _playTimeList = new List<PlayTime>();

        /// <summary>
        /// 检查并处理“指定了持续播放时间的Player”
        /// </summary>
        private static void SoundPlayEvent()
        {
            while (true)
            {
                try
                {
                    lock (((ICollection)_playTimeList).SyncRoot)
                    {
                        if (_playTimeList.Count > 0)
                        {
                            PlayTime[] relations = _playTimeList.ToArray();
                            foreach (PlayTime item in relations)
                            {
                                if (DateTime.Compare(DateTime.Now, item.EndTime) >= 0)
                                {
                                    item.Player.Stop();
                                    _playTimeList.Remove(item);
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
        }
        #endregion
    }
}
