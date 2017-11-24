using System;
using System.Collections.Generic;
using System.Text;

namespace UserSettingsShared
{
    public interface IUserSettingState
    {
        Guid Guid { get; set; }
        bool IsSet { get; set; }
        string Namespace { get; set; }
        string Setting { get; set; }
        string Value { get; set; }
    }
    public interface IUserSettingRepository
    {
        IUserSettingState CreateUserSettingState();
        void UpdateUserSettingState(IUserSettingState state);
        void DeleteUserSettingState(Guid guid);
        IUserSettingState GetUserSettingState(string ns, string setting);
    }

    public class UserSetting
    {
        private IUserSettingRepository _repo;
        private IUserSettingState _state;
        public UserSetting(IUserSettingRepository repo)
        {
            _repo = repo;
            if (_state == null) { _state = repo.CreateUserSettingState(); }
        }
        public UserSetting(IUserSettingRepository repo, IUserSettingState state) : this(repo)
        {
            _state = state;
        }
        public string Value { get { return _state.Value; } }
        public bool IsSet { get { return _state.IsSet; } }

        public void UpdateValue(object value)
        {
            string stringVal = value.ToString();
            _state.Value = stringVal;
            _repo.UpdateUserSettingState(_state);
        }
    }
    public class UserSettings
    {
        private IUserSettingRepository _repo;
        public UserSettings(IUserSettingRepository repo)
        {
            _repo = repo;
        }
        public UserSetting GetSetting(string ns, string setting)
        {
            var state = _repo.GetUserSettingState(ns, setting);
            if (state == null)
            {
                state = _repo.CreateUserSettingState();
                state.Namespace = ns;
                state.Setting = setting;
            }
            return new UserSetting(_repo, state);
        }

        public void SetSetting(string ns, string setting, string value)
        {
            var state = _repo.GetUserSettingState(ns, setting);
            if (state == null)
            {
                state = _repo.CreateUserSettingState();
                state.Namespace = ns;
                state.Setting = setting;
                state.IsSet = true;
            }
            state.Value = value;
        }
        public void ClearSetting(string ns, string setting)
        {
            var state = _repo.GetUserSettingState(ns, setting);
            state.IsSet = false;
        }
    }
}
