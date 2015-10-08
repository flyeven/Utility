using SpeechLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.SpeechTools
{
    /// <summary>
    /// 语音识别管理类
    /// </summary>
    public class SpeechRecognition
    {
        private static SpeechRecognition _Instance = null;                  //语音识别检索单例对象
        private ISpeechRecoGrammar isrgammar;                               //Speech语法对象
        private SpeechLib.SpSharedRecoContext ssrContex = null;             //共享解码器

        /// <summary>
        /// 传递字符串事件
        /// </summary>
        /// <param name="str"></param>
        public delegate void StringEvent(string str);

        /// <summary>
        /// 设置消息
        /// </summary>
        public event StringEvent SetMessage;

        /// <summary>
        /// 是否已经开始侦听
        /// </summary>
        public bool IsStart = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        private SpeechRecognition()
        {
        }

        /// <summary>
        /// 开始侦听
        /// </summary>
        public void BeginRec()
        {
            ssrContex = new SpSharedRecoContext();
            ssrContex.EventInterests = SpeechRecoEvents.SREAllEvents;//在"语音事件"中有说明
            isrgammar = ssrContex.CreateGrammar(0);
            //isrgammar.CmdLoadFromFile("D:\\SpeechGammar.xml", SpeechLoadOption.SLODynamic);//读入规则
            isrgammar.CmdLoadFromFile("", SpeechLoadOption.SLODynamic);//读入规则
            isrgammar.CmdSetRuleState(isrgammar.Rules.Item(0).Name, SpeechRuleState.SGDSActive);//激活规则 
            ssrContex.Recognition += new _ISpeechRecoContextEvents_RecognitionEventHandler(ContexRecognition);

            ssrContex.State = SpeechRecoContextState.SRCS_Enabled;
            isrgammar.DictationSetState(SpeechRuleState.SGDSActive);
            IsStart = true;
        }

        /// <summary>
        /// 初始化单例
        /// </summary>
        /// <returns></returns>
        public static SpeechRecognition Instance()
        {
            if (_Instance == null)
                _Instance = new SpeechRecognition();
            return _Instance;
        }

        /// <summary>
        /// 停止侦听
        /// </summary>
        public void CloseRec()
        {
            ssrContex.State = SpeechRecoContextState.SRCS_Disabled;
            isrgammar.DictationSetState(SpeechRuleState.SGDSInactive);
            IsStart = false;
        }

        /// <summary>
        /// 语音识别回调函数
        /// </summary>
        /// <param name="iIndex"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        private void ContexRecognition(int iIndex, object obj, SpeechLib.SpeechRecognitionType type, SpeechLib.ISpeechRecoResult result)
        {
            if (SetMessage != null)
            {
                SetMessage(result.PhraseInfo.GetText(0, -1, true));
            }
        }
    }
}
