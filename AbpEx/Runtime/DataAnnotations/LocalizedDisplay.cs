﻿using System.ComponentModel;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedDisplay : DisplayNameAttribute
    {
        private readonly string _nameOrKey;
        public LocalizedDisplay(string nameOrKey)
        {
            _nameOrKey = nameOrKey;
        }

        public override string DisplayName
        {
            get
            {
                return LocalizedHelper.L(_nameOrKey);
            }
        }
    }
}
