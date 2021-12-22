using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core.Crypto;

namespace Nethermind.State.SnapDb
{
    public class SortedAccounts : ISortedAccounts
    {
        private SortedSet<Keccak> _sortedSet = new();

        public void AddAccount(Keccak addressHash)
        {
            _sortedSet.Add(addressHash);
        }
    }
}
