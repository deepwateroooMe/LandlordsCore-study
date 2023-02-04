using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ET {
	public class AccountSessionsComponent : IAwake, IDestroy {
        public Dictionary<long, long> AccountSessionsDictionary = new Dictionary<long, long>();

		void IAwake.Run(object o) => throw new NotImplementedException();
	}
}
