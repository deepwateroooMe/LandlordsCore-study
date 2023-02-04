using System;
using System.Collections.Generic;
using System.Text;

namespace ET {
    public class AccountCheckOutTimeComponent : Entity, IAwake<long>, IDestroy {
        public long Timer = 0;
        public long AccountId;
    }
}
