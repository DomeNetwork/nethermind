using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core.Crypto;
using Nethermind.Serialization.Rlp;

namespace Nethermind.State.SnapSync
{
    public class SnapProvider
    {
        private readonly StateTree _tree;
        private SortedDictionary<Keccak, Keccak> _sortedAccountLookup = new();

        public SnapProvider(StateTree tree)
        {
            _tree = tree;
        }



        //public bool AddAccountRange(Keccak expectedRootHash, Keccak startingHash, AccountWithAddressHash[] accounts, byte[][] proofs)
        //{
        //    bool proved = ProveRange(expectedRootHash, startingHash, accounts, proofs);

        //    if(proved)
        //    {
        //        for (int i = 0; i < accounts.Length; i++)
        //        {
        //            AccountWithAddressHash account = accounts[i];

        //            Rlp accountRlp = _tree.Set(account.AddressHash, account.Account);
        //            _sortedLookup[account.AddressHash] = 
        //        }

        //        _tree.Commit()
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public void AddAccountHash(Keccak addressHash, Keccak accountHash)
        //{
        //    _sortedLookup[addressHash] = accountHash;
        //}

        private bool AddAccountRange(Keccak expectedRootHash, Keccak startingHash, AccountWithAddressHash[] accounts, byte[][] proofs)
        {

            return true;
        }
    }
}
