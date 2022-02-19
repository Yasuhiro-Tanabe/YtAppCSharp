using System;

using YasT.Framework.WPF;

namespace DDLGenerator.ViewModels
{
    public class TabItemControlBase : NotificationObject
    {
        /// <summary>
        /// ファイル出力が開始されたときに発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationStarted;

        /// <summary>
        /// ファイル出力に成功したとき発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationCompleted;

        /// <summary>
        /// ファイル出力に失敗したときに発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationFailed;

        /// <summary>
        /// ファイル出力を開始する
        /// 
        /// 具体的には <see cref="FileGenerationStarted"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationStarted()
        {
            FileGenerationStarted?.Invoke(this, null);
        }

        /// <summary>
        /// ファイル出力を正常終了する
        /// 
        /// 具体的には <see cref="FileGenerationCompleted"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationCompleted()
        {
            FileGenerationCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// ファイル出力を異常終了する
        /// 
        /// 具体的には <see cref="FileGenerationFailed"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationFailed()
        {
            FileGenerationFailed?.Invoke(this, null);
        }
    }
}
