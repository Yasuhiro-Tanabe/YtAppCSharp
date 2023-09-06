using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="INotifyPropertyChanged"/> �C���^�t�F�[�X����������N���X�̋��ʃx�[�X�N���X�B
    /// </summary>
    public class NotificationObject : INotifyPropertyChanged
    {
        /// <summary>
        /// �v���p�e�B�ύX�f���Q�[�g�F�T�u�N���X�̃v���p�e�B�Ƃ��āu�T�u�N���X�̃f�[�^�����o�̃v���p�e�B�v�����J�E�ύX����Ƃ��A
        /// ���̕ύX���\�b�h(�����_���Ȃ�)���L�q���邽�߂Ɏg�p����B
        /// </summary>
        public delegate void SetPropertyDelegate();

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
        /// <param name="name">(�ʏ�͏ȗ�)���̃��\�b�h���Ăяo�����v���p�e�B�̃v���p�e�B��</param>
        protected void SetProperty<T>(ref T? variable, T value, [CallerMemberName] string name = "")
        {
            if(variable == null) { throw new NullReferenceException(nameof(variable)); }
            variable = value;
            RaisePropertyChanged(name);
        }
        /// <summary>
        /// �v���p�e�B��ύX���C�x���g�𔭍s����B
        /// </summary>
        /// <param name="setProperty">�v���p�e�B�ύX�f���Q�[�g�B</param>
        /// <param name="name">(�ʏ�͏ȗ�)���̃��\�b�h���Ăяo�����v���p�e�B�̃v���p�e�B���B</param>
        protected void SetProperty(SetPropertyDelegate setProperty, [CallerMemberName] string name = "")
        {
            setProperty();
            RaisePropertyChanged(name);
        }

        /// <summary>
        /// �v���p�e�B�ύX�C�x���g�𔭍s����B
        /// </summary>
        /// <param name="name">�C�x���g���s�Ώۂ̃v���p�e�B��</param>
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
