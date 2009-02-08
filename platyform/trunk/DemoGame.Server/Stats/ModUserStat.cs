using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DemoGame.Extensions;
using Platyform.Extensions;

namespace DemoGame.Server
{
    class ModUserStat<T> : ModStat<T>, IUserStat, IUpdateableStat where T : IStatValueType, new()
    {
        readonly T _lastUpdatedValue = new T();
        readonly User _user;
        StatUpdateHandler _updateHandler;

        public ModUserStat(IUserStat baseStat, StatUpdateHandler updateHandler, ModStatHandler modHandler)
            : this(baseStat.User, updateHandler, modHandler, ModStat.GetModStat(baseStat.StatType))
        {
        }

        public ModUserStat(User user, StatUpdateHandler updateHandler, ModStatHandler modHandler, StatType statType)
            : base(statType, modHandler)
        {
            _user = user;
            _updateHandler = updateHandler;
        }

        #region IUpdateableStat Members

        public int LastUpdatedValue
        {
            get { return _lastUpdatedValue.GetValue(); }
        }

        public virtual bool NeedsUpdate
        {
            get { return _lastUpdatedValue.GetValue() != Value; }
        }

        public StatUpdateHandler UpdateHandler
        {
            get { return _updateHandler; }
            set { _updateHandler = value; }
        }

        public virtual void Update()
        {
            if (!NeedsUpdate || UpdateHandler == null)
                return;

            UpdateHandler(this);
            _lastUpdatedValue.SetValue(Value);
        }

        #endregion

        #region IUserStat Members

        public User User
        {
            get { return _user; }
        }

        #endregion
    }
}