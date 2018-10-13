﻿using System;
using Caliburn.Micro;
using H.NET.Storages;

namespace HomeCenter.NET.ViewModels
{
    public class SingleKeyViewModel : Screen
    {
        #region Properties

        public SingleKey Key { get; }
        public string Text { get => Key.Text; set => Key.Text = value; }

        #endregion

        #region Constructors

        public SingleKeyViewModel(SingleKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        #endregion
    }
}
