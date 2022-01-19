using System.ComponentModel;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="INotifyPropertyChanged"/> �C���^�t�F�[�X����������N���X�̋��ʃx�[�X�N���X�B
    /// </summary>
    public class NotificationObject : INotifyPropertyChanged
    {
        /// <summary>
        /// �v���p�e�B�ύX�C�x���g�n���h���̓o�^��B
        /// 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// �v���p�e�B��ύX���C�x���g�𔭍s����B
        /// </summary>
        /// <typeparam name="T">�v���p�e�B�̃f�[�^�^</typeparam>
        /// <param name="variable">�v���p�e�B���i�[����ϐ�</param>
        /// <param name="value">�ύX����v���p�e�B�̒l</param>
        protected void SetProperty<T>(ref T? variable, T value)
        {
            variable = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(variable)));
        }
    }
}
